using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class MoveCalendarCommand : ServiceCommand<CalendarActionResponse>
	{
		public MoveCalendarCommand(CallContext callContext, FolderId calendarToMove, string parentGroupId, FolderId calendarBefore) : base(callContext)
		{
			this.calendarToMove = calendarToMove;
			this.parentGroupId = parentGroupId;
			this.calendarBefore = calendarBefore;
		}

		protected override CalendarActionResponse InternalExecute()
		{
			if (!this.VerifyFolderId(this.calendarToMove, "calendarToMove") || (this.calendarBefore != null && !this.VerifyFolderId(this.calendarBefore, "calendarBefore")))
			{
				return new CalendarActionResponse(CalendarActionError.CalendarActionInvalidItemId);
			}
			StoreObjectId asStoreObjectId = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.calendarToMove).GetAsStoreObjectId();
			StoreObjectId storeObjectId = (this.calendarBefore == null) ? null : base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.calendarBefore).GetAsStoreObjectId();
			return new MoveCalendar(base.MailboxIdentityMailboxSession, asStoreObjectId, this.parentGroupId, storeObjectId).Execute();
		}

		private bool VerifyFolderId(FolderId idToVerify, string folderIdName)
		{
			if (idToVerify == null || string.IsNullOrEmpty(idToVerify.Id) || string.IsNullOrEmpty(idToVerify.ChangeKey))
			{
				ExTraceGlobals.SetCalendarColorCallTracer.TraceError<string, string, string>((long)this.GetHashCode(), "Invalid calendar folderid ({0}) supplied. FolderId.Id: {1}, FolderId.ChangeKey: {2}", folderIdName, (idToVerify == null || idToVerify.Id == null) ? "is null" : idToVerify.Id, (idToVerify == null || idToVerify.ChangeKey == null) ? "is null" : idToVerify.ChangeKey);
				return false;
			}
			return true;
		}

		private readonly FolderId calendarToMove;

		private readonly string parentGroupId;

		private readonly FolderId calendarBefore;
	}
}
