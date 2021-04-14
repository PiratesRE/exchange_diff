using System;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public abstract class CreateEntityCommand<TContext, TEntity> : EntityCommand<TContext, TEntity>, ICreateEntityCommand<TContext, TEntity>, IEntityCommand<TContext, TEntity> where TEntity : IEntity
	{
		public TEntity Entity { get; set; }

		protected override void UpdateCustomLoggingData()
		{
			base.UpdateCustomLoggingData();
			this.SetCustomLoggingData("InputEntity", this.Entity);
		}
	}
}
