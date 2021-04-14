using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ItemDeletionClientIntentQuery : ClientIntentQuery
	{
		public ItemDeletionClientIntentQuery(GlobalObjectId globalObjectId, ICalendarItemStateDefinition targetState) : base(globalObjectId, targetState)
		{
		}

		public override ClientIntentQuery.QueryResult Execute(MailboxSession session, CalendarVersionStoreGateway cvsGateway)
		{
			ClientIntentFlags targetVersionClientIntent = ClientIntentFlags.None;
			bool foundVersionInTargetState = false;
			VersionedId sourceVersionId = null;
			base.RunQuery(session, cvsGateway, delegate(PropertyBag propertyBag)
			{
				CalendarItemState state = new CalendarItemState();
				if (this.TargetState.Evaluate(state, propertyBag, session))
				{
					targetVersionClientIntent = this.GetClientIntentFromPropertyBag(propertyBag);
					sourceVersionId = this.GetIdFromPropertyBag(propertyBag);
					foundVersionInTargetState = true;
				}
				return !foundVersionInTargetState;
			});
			return new ClientIntentQuery.QueryResult(foundVersionInTargetState ? new ClientIntentFlags?(targetVersionClientIntent) : null, sourceVersionId);
		}
	}
}
