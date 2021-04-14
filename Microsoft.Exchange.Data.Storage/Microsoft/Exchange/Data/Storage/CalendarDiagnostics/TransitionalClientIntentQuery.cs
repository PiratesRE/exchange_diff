using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransitionalClientIntentQuery : ClientIntentQuery
	{
		public ICalendarItemStateDefinition InitialState { get; private set; }

		public TransitionalClientIntentQuery(GlobalObjectId globalObjectId, ICalendarItemStateDefinition initialState, ICalendarItemStateDefinition targetState) : base(globalObjectId, targetState)
		{
			Util.ThrowOnNullArgument(initialState, "initialState");
			if (!initialState.SchemaKey.Equals(targetState.SchemaKey))
			{
				throw new ArgumentException(string.Format("Cannot query client intent for a transition between heterogeneous states (Initial: {0}; Target: {1}).", initialState.SchemaKey, targetState.SchemaKey));
			}
			this.InitialState = initialState;
		}

		public override ClientIntentQuery.QueryResult Execute(MailboxSession session, CalendarVersionStoreGateway cvsGateway)
		{
			bool isChainComplete = false;
			CalendarItemState previousState = null;
			bool foundVersionInTargetState = false;
			VersionedId sourceVersionId = null;
			ClientIntentFlags accumulatedFlags = ClientIntentFlags.None;
			ClientIntentFlags intentToAccumulate = ClientIntentFlags.None;
			base.RunQuery(session, cvsGateway, delegate(PropertyBag propertyBag)
			{
				CalendarItemState calendarItemState = new CalendarItemState();
				if (!foundVersionInTargetState)
				{
					if (this.TargetState.Evaluate(calendarItemState, propertyBag, session))
					{
						foundVersionInTargetState = true;
						intentToAccumulate = this.GetClientIntentFromPropertyBag(propertyBag);
						accumulatedFlags = intentToAccumulate;
					}
				}
				else if (this.InitialState.Evaluate(calendarItemState, propertyBag, session))
				{
					isChainComplete = true;
					accumulatedFlags &= intentToAccumulate;
					sourceVersionId = this.GetIdFromPropertyBag(propertyBag);
				}
				else if (this.TargetState.Evaluate(calendarItemState, propertyBag, session))
				{
					intentToAccumulate = this.GetClientIntentFromPropertyBag(propertyBag);
					accumulatedFlags = intentToAccumulate;
				}
				else if (this.TargetState.Equals(calendarItemState, previousState))
				{
					intentToAccumulate = this.GetClientIntentFromPropertyBag(propertyBag);
				}
				else
				{
					accumulatedFlags &= intentToAccumulate;
					intentToAccumulate = this.GetClientIntentFromPropertyBag(propertyBag);
				}
				previousState = calendarItemState;
				return !isChainComplete;
			});
			return new ClientIntentQuery.QueryResult(isChainComplete ? new ClientIntentFlags?(accumulatedFlags) : null, sourceVersionId);
		}
	}
}
