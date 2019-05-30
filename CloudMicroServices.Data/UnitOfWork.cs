using BaseApiArchitecture.Interfaces;
using System;
using System.Threading.Tasks;

namespace CloudMicroServices.Data
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private PortalContext Context;

		public UnitOfWork(PortalContext dbContext)
		{
			Context = dbContext;
		}

		public async Task Commit()
		{
			await Context.SaveChangesAsync();
		}

		public void Close()
		{
			Dispose();
		}

		public string GetContextId()
		{
			return Context.GetHashCode().ToString();
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}
