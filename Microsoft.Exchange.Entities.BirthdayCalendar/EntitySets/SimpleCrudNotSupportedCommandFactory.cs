using System;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal class SimpleCrudNotSupportedCommandFactory<TScope, TEntity> : IEntityCommandFactory<TScope, TEntity> where TEntity : IEntity
	{
		[Obsolete("Use specific create command instead")]
		public ICreateEntityCommand<TScope, TEntity> CreateCreateCommand(TEntity entity, TScope scope)
		{
			throw SimpleCrudNotSupportedCommandFactory<TScope, TEntity>.CreateGenericCrudCommandException();
		}

		[Obsolete("Use specific delete command instead")]
		public IDeleteEntityCommand<TScope> CreateDeleteCommand(string key, TScope scope)
		{
			throw SimpleCrudNotSupportedCommandFactory<TScope, TEntity>.CreateGenericCrudCommandException();
		}

		[Obsolete("Use specific find command instead")]
		public IFindEntitiesCommand<TScope, TEntity> CreateFindCommand(IEntityQueryOptions queryOptions, TScope scope)
		{
			throw SimpleCrudNotSupportedCommandFactory<TScope, TEntity>.CreateGenericCrudCommandException();
		}

		[Obsolete("Use specific read command instead")]
		public IReadEntityCommand<TScope, TEntity> CreateReadCommand(string key, TScope scope)
		{
			throw SimpleCrudNotSupportedCommandFactory<TScope, TEntity>.CreateGenericCrudCommandException();
		}

		[Obsolete("Use specific update command instead")]
		public IUpdateEntityCommand<TScope, TEntity> CreateUpdateCommand(string key, TEntity entity, TScope scope)
		{
			throw SimpleCrudNotSupportedCommandFactory<TScope, TEntity>.CreateGenericCrudCommandException();
		}

		private static Exception CreateGenericCrudCommandException()
		{
			throw new NotSupportedException("Generic CRUDF commands are not supported");
		}
	}
}
