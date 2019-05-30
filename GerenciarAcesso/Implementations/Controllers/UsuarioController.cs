using BaseApiArchitecture.Domain;
using BaseApiArchitecture.Implementations;
using BaseApiArchitecture.Interfaces;
using BaseApiArchitecture.Utils;
using CloudMicroServices.Core.Controllers;
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
using System.Web.Http;

namespace GerenciarAcesso.Implementations.Controllers
{
	public class UsuarioController : BaseController<Usuario>
	{
		private IGerenciarUsuario GerenciarUsuario { get; set; }

		public UsuarioController(IGerenciarUsuario GerenciarUsuario) : base(GerenciarUsuario)
		{
			this.GerenciarUsuario = GerenciarUsuario;
		}

		[AllowAnonymous]
		[HttpPost, Route("Autenticar")]
		public async Task<Result<UsuarioViewModel>> Autenticar([FromBody]params Usuario[] Usuarios)
		{
			return (await GerenciarUsuario.Autenticar(Usuarios[0]));
		}

		[HttpPost, Route("AlterarSenha")]
		public async Task<Result<UsuarioViewModel>> AlterarSenha([FromBody]AlterarSenhaViewModel Usuario)
		{
			return (await GerenciarUsuario.AlterarSenha(Usuario));
		}

		[HttpPost, Route("ResetarSenha")]
		public async Task<Result<UsuarioViewModel>> ResetarSenha([FromBody]Usuario Usuario)
		{
			return (await GerenciarUsuario.ResetarSenha(Usuario));
		}
	}
}
