using System;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IEntityReference<out TEntity> where TEntity : class, IEntity
	{
		string GetKey();

		TEntity Read(CommandContext context = null);
	}
}
