using BaseApiArchitecture.Domain;
using CloudMicroServices.Domain.Models;

namespace CloudMicroServices.Domain.ViewModels
{
	public class UsuarioViewModel : BaseEntity
	{
		public int Id { get; set; }
		public string Login { get; set; }
		public string Token { get; set; }
		public string NomeCompleto { get; set; }
		//public IEnumerable<Permissao> Permissoes { get; set; }
	}
}
