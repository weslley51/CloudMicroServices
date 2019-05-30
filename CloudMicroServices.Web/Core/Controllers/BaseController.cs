using System.Collections.Generic;
using System.Threading.Tasks;
using BaseApiArchitecture.Interfaces;
using BaseApiArchitecture.Domain;
using BaseApiArchitecture.Utils;
using System.Linq;
using System.Web.Http;
using CloudMicroServices.Domain.Models;

namespace CloudMicroProcessmentCommands.Core.Controllers
{
	[Route("api/[controller]")]
	public class BaseController<T> : ApiController, IController<T> where T : DomainEntity, new()
	{
		protected IProcessmentCommand ProcessmentCommand { get; set; }

		public BaseController(IProcessmentCommand ProcessmentCommand)
		{
			this.ProcessmentCommand = ProcessmentCommand;
		}

		// GET api/values
		[HttpGet]
		public virtual async Task<Result<IEnumerable<BaseEntity>>> Get()
		{
			//var Filters = GetFilter();
			//Result<IEnumerable<T>> Result;
			//
			//if (Filters == null || Filters.IsAllNull())
			//	Result = await ProcessmentCommand.GetAll(1, 10);
			//else
			//	Result = await ProcessmentCommand.GetWithFilter(Filters);
			//
			//return Result.ConvertData(Result.Data.Cast<BaseEntity>());
			return null;
		}

		// GET api/values/5
		[HttpGet, Route("/{Id}")]
		public virtual async Task<Result<BaseEntity>> Get([FromUri]int Id)
		{
			//var Result = await ProcessmentCommand.GetById(new T { Id = Id });
			//return Result.ConvertData(Result.Data as BaseEntity);
			return null;
		}
		
		// GET api/values/1/10
		[HttpGet, Route("/{Page}/{Quantity}")]
		public virtual async Task<Result<IEnumerable<BaseEntity>>> Get([FromUri]int Page = 1, [FromUri]int Quantity = 10)
		{
			//var Result = await ProcessmentCommand.GetAll(Page, Quantity);
			//return Result.ConvertData(Result.Data.Cast<BaseEntity>());
			return null;
		}
		
		// POST api/values
		[HttpPost]
		public virtual async Task<IEnumerable<Result<BaseEntity>>> Post([FromBody]params T[] Entities)
		{
			return null;
			//return ConvertResult(await ProcessmentCommand.Save(true, Entities));
		}
		
		// PUT api/values/
		[HttpPut]
		public virtual async Task<IEnumerable<Result<BaseEntity>>> Put([FromBody]params T[] Entities)
		{
			return null;
			//return ConvertResult(await ProcessmentCommand.Save(true, Entities));
		}
		
		// DELETE api/values/5
		[HttpDelete]
		public virtual async Task<Result<IEnumerable<BaseEntity>>> Delete([FromBody]params T[] Entities)
		{
			//var Result = await ProcessmentCommand.Delete(true, Entities);
			//return Result.ConvertData(Result.Data.Cast<BaseEntity>());
			return null;
		}
		
		protected IEnumerable<Result<BaseEntity>> ConvertResult(IEnumerable<Result<T>> Result)
		{
			var Converted = new List<Result<BaseEntity>>();
			Result.ToList().ForEach(x => Converted.Add(x.ConvertData(x.Data as BaseEntity)));
			return Converted;
		}

		protected virtual IFilter<T> GetFilter()
		{
			return null;
		}
	}
}
