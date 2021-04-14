using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class PeopleIKnowMarkAllAsReadCommand : MarkAllItemsAsRead
	{
		public PeopleIKnowMarkAllAsReadCommand(CallContext callContext, MarkAllItemsAsReadRequest request) : base(callContext, request)
		{
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			this.fromFilter = base.Request.FromFilter;
			ServiceCommandBase.ThrowIfNullOrEmpty(this.fromFilter, "fromFilter", "PeopleIKnowMarkAllAsReadCommand::PreExecuteCommand");
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.folderIds[0]);
			StoreId defaultFolderId = idAndSession.Session.GetDefaultFolderId(DefaultFolderType.FromFavoriteSenders);
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "PeopleIKnowMarkAllAsReadCommand.Execute calling PeopleIKnowMarkAllAsRead with fromFilter: {0}", this.fromFilter);
			new PeopleIKnowMarkAllAsRead(mailboxSession, defaultFolderId, this.fromFilter, this.supressReadReceipts, this.tracer).Execute();
			this.objectsChanged++;
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private string fromFilter;
	}
}
