using BaseApiArchitecture.Domain;
using CloudMicroServices.Domain.Models;
using CloudMicroServices.Domain.ViewModels;
using System.Threading.Tasks;

namespace CloudMicroServices.Authentication.Interfaces
{
	public interface IAutenticacao
	{
		Task<Result<UsuarioViewModel>> Autenticar(Usuario Usuario);
	}
}
