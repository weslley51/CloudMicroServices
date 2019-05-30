using BaseApiArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudMicroServices.Data
{
	public abstract class EntityTypeConfiguration<T> where T : BaseEntity
	{
		public abstract void Map(EntityTypeBuilder<T> Builder);
	}

	public static class ModelBuilderExtensions
	{
		public static void AddConfiguration<T>(this ModelBuilder ModelBuilder, EntityTypeConfiguration<T> Configuration) where T : BaseEntity
		{
			Configuration.Map(ModelBuilder.Entity<T>());
		}
	}
}
