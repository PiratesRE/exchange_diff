using System;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IItemReference<out TEntity> : IEntityReference<TEntity> where TEntity : class, IEntity
	{
		IAttachments Attachments { get; }
	}
}
