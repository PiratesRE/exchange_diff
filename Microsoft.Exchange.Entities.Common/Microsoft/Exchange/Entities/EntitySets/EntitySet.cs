using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.EntitySets.Linq;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal abstract class EntitySet<TEntitySet, TEntity, TCommandFactory> : IEntitySet<TEntity> where TEntitySet : class where TEntity : class, IEntity where TCommandFactory : IEntityCommandFactory<TEntitySet, TEntity>
	{
		protected EntitySet(TCommandFactory commandFactory)
		{
			this.CommandFactory = commandFactory;
		}

		private protected TCommandFactory CommandFactory { protected get; private set; }

		public IQueryable<TEntity> AsQueryable(CommandContext context = null)
		{
			return new EntitySetQueryProvider<TEntity>(this, context);
		}

		public TEntity Create(TEntity entity, CommandContext context = null)
		{
			TCommandFactory commandFactory = this.CommandFactory;
			ICreateEntityCommand<TEntitySet, TEntity> createEntityCommand = commandFactory.CreateCreateCommand(entity, this as TEntitySet);
			return createEntityCommand.Execute(context);
		}

		public void Delete(string key, CommandContext context = null)
		{
			TCommandFactory commandFactory = this.CommandFactory;
			IDeleteEntityCommand<TEntitySet> deleteEntityCommand = commandFactory.CreateDeleteCommand(key, this as TEntitySet);
			deleteEntityCommand.Execute(context);
		}

		public virtual int EstimateTotalCount(IEntityQueryOptions queryOptions, CommandContext context = null)
		{
			throw new UnsupportedEstimatedTotalCountException();
		}

		public IEnumerable<TEntity> Find(IEntityQueryOptions queryOptions = null, CommandContext context = null)
		{
			TCommandFactory commandFactory = this.CommandFactory;
			IFindEntitiesCommand<TEntitySet, TEntity> findEntitiesCommand = commandFactory.CreateFindCommand(queryOptions, this as TEntitySet);
			return findEntitiesCommand.Execute(context);
		}

		public TEntity Read(string key, CommandContext context = null)
		{
			TCommandFactory commandFactory = this.CommandFactory;
			IReadEntityCommand<TEntitySet, TEntity> readEntityCommand = commandFactory.CreateReadCommand(key, this as TEntitySet);
			return readEntityCommand.Execute(context);
		}

		public TEntity Update(string key, TEntity entity, CommandContext context = null)
		{
			TCommandFactory commandFactory = this.CommandFactory;
			IUpdateEntityCommand<TEntitySet, TEntity> updateEntityCommand = commandFactory.CreateUpdateCommand(key, entity, this as TEntitySet);
			return updateEntityCommand.Execute(context);
		}
	}
}
