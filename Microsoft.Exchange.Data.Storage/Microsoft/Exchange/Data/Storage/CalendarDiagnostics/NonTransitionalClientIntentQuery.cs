using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NonTransitionalClientIntentQuery : ClientIntentQuery
	{
		public NonTransitionalClientIntentQuery(GlobalObjectId globalObjectId, ICalendarItemStateDefinition targetState) : base(globalObjectId, targetState)
		{
		}

		public override ClientIntentQuery.QueryResult Execute(MailboxSession session, CalendarVersionStoreGateway cvsGateway)
		{
			ClientIntentFlags targetVersionClientIntent = ClientIntentFlags.None;
			bool foundShiftToTargetState = false;
			bool foundVersionInTargetState = false;
			VersionedId sourceVersionId = null;
			base.RunQuery(session, cvsGateway, delegate(PropertyBag propertyBag)
			{
				CalendarItemState state = new CalendarItemState();
				if (this.TargetState.Evaluate(state, propertyBag, session))
				{
					foundVersionInTargetState = true;
					targetVersionClientIntent = this.GetClientIntentFromPropertyBag(propertyBag);
				}
				else if (foundVersionInTargetState)
				{
					foundShiftToTargetState = true;
					sourceVersionId = this.GetIdFromPropertyBag(propertyBag);
				}
				return !foundShiftToTargetState;
			});
			return new ClientIntentQuery.QueryResult(foundShiftToTargetState ? new ClientIntentFlags?(targetVersionClientIntent) : null, sourceVersionId);
		}
	}
}
