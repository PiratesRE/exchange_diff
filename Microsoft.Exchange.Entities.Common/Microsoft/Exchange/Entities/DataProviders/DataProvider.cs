using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.DataProviders
{
	internal abstract class DataProvider<TEntity, TId>
	{
		protected DataProvider(ITracer trace)
		{
			this.Trace = trace;
		}

		private protected ITracer Trace { protected get; private set; }

		public abstract TEntity Create(TEntity entity);

		public abstract void Delete(TId id, DeleteItemFlags flags);

		public abstract TEntity Read(TId id);

		public abstract TEntity Update(TEntity entity, CommandContext commandContext);

		public virtual void Validate(TEntity entity, bool isNew)
		{
		}
	}
}
