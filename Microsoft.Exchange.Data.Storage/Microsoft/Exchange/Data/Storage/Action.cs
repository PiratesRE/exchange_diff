using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Action : ActionBase
	{
		protected Action(ActionType actionType, Rule rule) : base(actionType, rule)
		{
		}
	}
}
