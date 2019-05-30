using AutoMapper;
using BaseApiArchitecture.Domain;
using BaseApiArchitecture.Implementations;
using BaseApiArchitecture.Interfaces;
using BaseApiArchitecture.Utils;
using CloudMicroServices.Domain.Models;
using CloudMicroServices.Domain.ViewModels;
using CloudMicroServices.Web.Core.Configurations;
using GerenciarAcesso.Interfaces.Bussiness;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GerenciarAcesso.Implementations.Bussiness
{
	public class GerenciarUsuario : IGerenciarUsuario
	{
		private readonly IMapper Mapper;
		private readonly AppSettings AppSettings;
		private readonly ClaimsPrincipal Principal;

		public GerenciarUsuario(IOptions<AppSettings> AppSettings, IMapper Mapper, ClaimsPrincipal Principal, IRepository<Usuario> Repository, IProcessStrategy ProcessStrategy, IUnitOfWork UnitOfWork,
										ILogService LogService) 
		{
			this.Mapper = Mapper;
			this.Principal = Principal;
			this.AppSettings = AppSettings.Value;
		}

		public async Task<Result<UsuarioViewModel>> AlterarSenha(AlterarSenhaViewModel Usuario)
		{
			return await DatabaseProcessStrategy.Process(async () =>
			{
				var Id = Convert.ToInt32(Principal.GetClaim("Id"));
				var UsuarioBanco = await Repository.GetById(Id);

				if (Usuario.NovaSenha != Usuario.ConfirmacaoNovaSenha)
					throw new ValidationException(Usuario, "Senhas informadas não são iguais!");

				ValidarInformacoesLogin(UsuarioBanco, new Usuario { Senha = Usuario.Senha });

				UsuarioBanco.Senha = Criptography.GetHash(Usuario.NovaSenha, UsuarioBanco.Salt);
				return Mapper.Map<UsuarioViewModel>(await Repository.Update(UsuarioBanco));
			});
		}

		public async Task<Result<UsuarioViewModel>> Autenticar(Usuario Usuario)
		{
			return await FunctionProcessStrategy.Process(async () =>
			{
                //var UsuarioBanco = (await Repository.GetWithFilter(x => x.Login.ToLower() == Usuario.Login.ToLower()))?.FirstOrDefault();
                var UsuarioBanco = new Usuario { Login = "gti", Salt = "79WTLkIbL7kcf0IrxCJGxva4PBIxK2oSp06HiMjlZzA=", Senha = "I53LDJcVAV+yJZL56f5G8xYvSJdrmu59SExGgP1HIvg=" };
                ValidarInformacoesLogin(UsuarioBanco, Usuario);


				var Jwt = new JwtSecurityTokenHandler();
				var Token = Jwt.CreateToken(CreateToken(UsuarioBanco));
				var ViewModel = Mapper.Map<UsuarioViewModel>(UsuarioBanco);

				ViewModel.Token = Jwt.WriteToken(Token);

				return ViewModel;
			});
		}

		public async Task<Result<UsuarioViewModel>> ResetarSenha(Usuario Usuario)
		{
			return await DatabaseProcessStrategy.Process(async () =>
			{
				var Id = Convert.ToInt32(Principal.GetClaim("Id"));
				var UsuarioJwt = await Repository.GetById(Id);
				var UsuarioBanco = await Repository.GetById(Usuario.Id);

				if (UsuarioJwt.Id != Usuario.Id && Usuario.PermissoesUsuario.Any(x => x.PermissaoId == 1))
					throw new ValidationException(Usuario, "Usuário não possuí permissão para realizar a operação solicitada!");
								
				UsuarioBanco.AlterarSenha = true;
				UsuarioBanco.Senha = Criptography.GetHash($"PortalChamados{DateTime.Now.Year}", UsuarioBanco.Salt);
				return Mapper.Map<UsuarioViewModel>(await Repository.Update(UsuarioBanco));
			});
		}

		private void ValidarInformacoesLogin(Usuario UsuarioBanco, Usuario Usuario)
		{
			if (UsuarioBanco == null)
				throw new ValidationException(Usuario, "Usuário inexistente no sistema!");

			if (Criptography.GetHash(Usuario.Senha, UsuarioBanco.Salt) != UsuarioBanco.Senha)
				throw new ValidationException(Usuario, "Senha inválida!");
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
