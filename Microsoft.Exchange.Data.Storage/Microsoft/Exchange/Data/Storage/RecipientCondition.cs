using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RecipientCondition : Condition
	{
		protected RecipientCondition(ConditionType conditionType, Rule rule, IList<Participant> participants) : base(conditionType, rule)
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

		protected static void CheckParticipants(Rule rule, IList<Participant> participants)
		{
			if (participants.Count == 0)
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentException("No participants");
				}, "No participants");
			}
			using (IEnumerator<Participant> enumerator = participants.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Participant participant = enumerator.Current;
					if (participant.RoutingType == "MAPIPDL")
					{
						rule.ThrowValidateException(delegate
						{
							throw new ArgumentException("MAPIPDL participant:" + participant.DisplayName);
						}, "MAPIPDL participant:" + participant.DisplayName);
					}
					if (participant.ValidationStatus != ParticipantValidationStatus.NoError)
					{
						rule.ThrowValidateException(delegate
						{
							throw new ArgumentException("Invalid participant:" + participant.DisplayName);
						}, "Invalid participant:" + participant.DisplayName);
					}
				}
			}
		}

		private readonly IList<Participant> participants;
	}
}
