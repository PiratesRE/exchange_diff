using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IReplayProvider
	{
		void MarkAsRead(IReadOnlyCollection<MarkAsReadAction> actions);

		void MarkAsUnRead(IReadOnlyCollection<MarkAsUnReadAction> actions);

		IReadOnlyCollection<MoveActionResult> Move(IReadOnlyCollection<MoveAction> actions);

		void Send(SendAction action);

		void Delete(IReadOnlyCollection<DeleteAction> actions);

		void Flag(IReadOnlyCollection<FlagAction> actions);

		void FlagClear(IReadOnlyCollection<FlagClearAction> actions);

		void FlagComplete(IReadOnlyCollection<FlagCompleteAction> actions);

		IReadOnlyCollection<CreateCalendarEventActionResult> CreateCalendarEvent(IReadOnlyCollection<CreateCalendarEventAction> actions);

		void UpdateCalendarEvent(IReadOnlyCollection<UpdateCalendarEventAction> actions);
	}
}
