using BaseApiArchitecture.Domain;
using BaseApiArchitecture.Interfaces;
using CloudMicroServices.Domain.Models;
using CloudMicroServices.Domain.ViewModels;
using System.Threading.Tasks;

namespace GerenciarAcesso.Interfaces.Bussiness
{
	public interface IGerenciarUsuario : IBaseOperations<Usuario>
	{
		Task<Result<UsuarioViewModel>> Autenticar(Usuario Usuario);
		Task<Result<UsuarioViewModel>> AlterarSenha(AlterarSenhaViewModel Usuario);
		Task<Result<UsuarioViewModel>> ResetarSenha(Usuario Usuario);
	}
}
