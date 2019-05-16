using BaseApiArchitecture.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudMicroServices.Domain.Models
{	
	public class Usuario : BaseEntity
	{
		public string Login { get; set; }
		public string Senha { get; set; }
		public string NomeCompleto { get; set; }
		//public Departamento Departamento { get; set; }
		//public ICollection<PermissaoUsuario> PermissoesUsuario { get; set; }
	}
}
