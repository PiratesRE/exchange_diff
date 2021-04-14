using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RecipientAction : ActionBase
	{
		protected RecipientAction(ActionType actionType, IList<Participant> participants, Rule rule) : base(actionType, rule)
		{
			this.participants = participants;
		}

		public IList<Participant> Participants
		{
			get
			{
				return this.participants;
			}
		}

		private readonly IList<Participant> participants;
	}
}
