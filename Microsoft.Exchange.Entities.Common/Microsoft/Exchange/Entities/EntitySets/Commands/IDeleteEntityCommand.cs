using System;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public interface IDeleteEntityCommand<TScope> : IKeyedEntityCommand<TScope, VoidResult>, IEntityCommand<TScope, VoidResult>
	{
	}
}
