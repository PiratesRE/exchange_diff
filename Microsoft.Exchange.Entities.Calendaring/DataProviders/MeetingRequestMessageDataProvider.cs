using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class MeetingRequestMessageDataProvider : StorageItemDataProvider<IMailboxSession, MeetingRequestMessage, IMeetingRequest>
	{
		public MeetingRequestMessageDataProvider(IStorageEntitySetScope<IMailboxSession> parentScope) : base(parentScope, null, ExTraceGlobals.MeetingRequestMessageDataProviderTracer)
		{
		}

		protected override IStorageTranslator<IMeetingRequest, MeetingRequestMessage> Translator
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual StoreId GetCorrelatedItemId(IMeetingRequest meetingRequest)
		{
			StoreObjectId defaultFolderId = base.Scope.StoreSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			StoreId result;
			using (CalendarFolder calendarFolder = (CalendarFolder)base.XsoFactory.BindToCalendarFolder(base.Session, defaultFolderId))
			{
				IEnumerable<VersionedId> enumerable;
				result = meetingRequest.FetchCorrelatedItemId(calendarFolder, false, out enumerable);
			}
			return result;
		}

		internal void SaveMeetingRequest(IMeetingRequest meetingRequest, CommandContext commandContext)
		{
			SaveMode saveMode = base.GetSaveMode(null, commandContext);
			meetingRequest.Save(saveMode);
		}

		protected internal override IMeetingRequest BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToMeetingRequestMessage(base.Session, id);
		}

		protected override IMeetingRequest CreateNewStoreObject()
		{
			throw new NotImplementedException();
		}
	}
}
