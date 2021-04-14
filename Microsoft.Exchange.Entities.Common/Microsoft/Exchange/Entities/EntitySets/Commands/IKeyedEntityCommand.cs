using System;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public interface IKeyedEntityCommand<TEntitySet, out TResult> : IEntityCommand<TEntitySet, TResult>
	{
		string EntityKey { get; set; }
	}
}
