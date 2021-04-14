using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class IntAction : ActionBase
	{
		protected IntAction(ActionType actionType, int number, Rule rule) : base(actionType, rule)
		{
			this.number = number;
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		private readonly int number;
	}
}
