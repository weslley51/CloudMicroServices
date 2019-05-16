using BaseApiArchitecture.Domain;
using CloudMicroServices.Authentication.Interfaces;
using CloudMicroServices.Domain.Models;
using CloudMicroServices.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudMicroServices.Authentication.Controllers
{
	public class AutenticarController : Controller
	{
		private IAutenticacao Autenticacao { get; set; }

		public AutenticarController(IAutenticacao Autenticacao)
		{
			this.Autenticacao = Autenticacao;
		}

		[AllowAnonymous]
		[Route("Autenticar")]
		public async Task<Result<UsuarioViewModel>> Autenticar([FromBody]Usuario Usuario)
		{
			return (await Autenticacao.Autenticar(Usuario));
		}
	}
}
