using System;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public interface IEntityCommand<TScope, out TResult>
	{
		Guid Id { get; }

		TScope Scope { get; set; }

		TResult Execute(CommandContext context);
	}
}
