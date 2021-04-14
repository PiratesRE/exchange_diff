using System;
using System.ComponentModel;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.EntitySets
{
	[ImmutableObject(true)]
	public sealed class EntityCommandFactory<TScope, TEntity, TCreate, TDelete, TFind, TRead, TUpdate> : IEntityCommandFactory<TScope, TEntity> where TEntity : IEntity where TCreate : ICreateEntityCommand<TScope, TEntity>, new() where TDelete : IDeleteEntityCommand<TScope>, new() where TFind : IFindEntitiesCommand<TScope, TEntity>, new() where TRead : IReadEntityCommand<TScope, TEntity>, new() where TUpdate : IUpdateEntityCommand<TScope, TEntity>, new()
	{
		private EntityCommandFactory()
		{
		}

		public ICreateEntityCommand<TScope, TEntity> CreateCreateCommand(TEntity entity, TScope scope)
		{
			TCreate tcreate = (default(TCreate) == null) ? Activator.CreateInstance<TCreate>() : default(TCreate);
			tcreate.Entity = entity;
			tcreate.Scope = scope;
			return tcreate;
		}

		public IDeleteEntityCommand<TScope> CreateDeleteCommand(string key, TScope scope)
		{
			TDelete tdelete = (default(TDelete) == null) ? Activator.CreateInstance<TDelete>() : default(TDelete);
			tdelete.EntityKey = key;
			tdelete.Scope = scope;
			return tdelete;
		}

		public IFindEntitiesCommand<TScope, TEntity> CreateFindCommand(IEntityQueryOptions queryOptions, TScope scope)
		{
			TFind tfind = (default(TFind) == null) ? Activator.CreateInstance<TFind>() : default(TFind);
			tfind.QueryOptions = queryOptions;
			tfind.Scope = scope;
			return tfind;
		}

		public IReadEntityCommand<TScope, TEntity> CreateReadCommand(string key, TScope scope)
		{
			TRead tread = (default(TRead) == null) ? Activator.CreateInstance<TRead>() : default(TRead);
			tread.EntityKey = key;
			tread.Scope = scope;
			return tread;
		}

		public IUpdateEntityCommand<TScope, TEntity> CreateUpdateCommand(string key, TEntity entity, TScope scope)
		{
			TUpdate tupdate = (default(TUpdate) == null) ? Activator.CreateInstance<TUpdate>() : default(TUpdate);
			tupdate.EntityKey = key;
			tupdate.Entity = entity;
			tupdate.Scope = scope;
			return tupdate;
		}

		public static readonly IEntityCommandFactory<TScope, TEntity> Instance = new EntityCommandFactory<TScope, TEntity, TCreate, TDelete, TFind, TRead, TUpdate>();
	}
}
