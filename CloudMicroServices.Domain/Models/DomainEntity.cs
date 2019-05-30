using BaseApiArchitecture.Domain;

namespace CloudMicroServices.Domain.Models
{
	public abstract class DomainEntity : BaseEntity
	{
		public int Id { get; set; }
	}
}
