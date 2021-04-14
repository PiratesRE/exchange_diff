using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class IdAction : ActionBase
	{
		protected IdAction(ActionType actionType, StoreObjectId id, Rule rule) : base(actionType, rule)
		{
			this.id = id;
		}

		public StoreObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly StoreObjectId id;
	}
}
