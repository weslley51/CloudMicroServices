using BaseApiArchitecture.Domain;
using BaseApiArchitecture.Interfaces;
using CloudMicroServices.Authentication.Interfaces;
using CloudMicroServices.Domain.Models;
using CloudMicroServices.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMicroServices.Authentication.BussinessServices
{
	public class Autenticacao : IAutenticacao
	{
		private readonly IMapper Mapper;
		private readonly AppSettings AppSettings;
		private readonly IRepository<Usuario> Repository;
		private readonly IProcessStrategy ProcessStrategy;

		public Autenticacao(IRepository<Usuario> Repository, IProcessStrategy ProcessStrategy, IOptions<AppSettings> AppSettings, IMapper Mapper)
		{
			this.Mapper = Mapper;
			this.Repository = Repository;
			this.ProcessStrategy = ProcessStrategy;
			this.AppSettings = AppSettings.Value;
		}

		public async Task<Result<UsuarioViewModel>> Autenticar(Usuario Usuario)
		{
			return await ProcessStrategy.Process(async () =>
			{
				var Retorno = (await Repository.GetWithFilter(x => x.Login == Usuario.Login && x.Senha == Usuario.Senha))?.FirstOrDefault();

				if (Retorno == null)
				{
					if ((await Repository.GetWithFilter(x => x.Login == Usuario.Login))?.FirstOrDefault() == null)
						throw new ValidationException(Usuario, "Usuário inexistente no sistema!");
					else
						throw new ValidationException(Usuario, "Senha inválida!");
				}

				var ViewModel = Mapper.Map<UsuarioViewModel>(Retorno);

				var Jwt = new JwtSecurityTokenHandler();
				var TokenDescriptor = CreateToken(Retorno);
				var Token = Jwt.CreateToken(TokenDescriptor);
				ViewModel.Token = Jwt.WriteToken(Token);

				return ViewModel;
			});
		}

		private SecurityTokenDescriptor CreateToken(Usuario Usuario)
		{
			var Key = Encoding.ASCII.GetBytes(AppSettings.Secret);

			return new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim("Id", Usuario.Id.ToString()),
					new Claim(ClaimTypes.Name, Usuario.Login)
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
			};
		}
	}
}
