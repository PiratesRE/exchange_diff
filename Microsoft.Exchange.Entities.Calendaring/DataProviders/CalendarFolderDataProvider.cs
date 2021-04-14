using System;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class CalendarFolderDataProvider : StorageFolderDataProvider<IStoreSession, Calendar, ICalendarFolder>
	{
		public CalendarFolderDataProvider(IStorageEntitySetScope<IStoreSession> parentScope, StoreId parentFolderId) : base(parentScope, ExTraceGlobals.CalendarDataProviderTracer)
		{
			this.ParentFolderId = parentFolderId;
		}

		public StoreId ParentFolderId { get; private set; }

		protected override IStorageTranslator<ICalendarFolder, Calendar> Translator
		{
			get
			{
				return CalendarTranslator.Instance;
			}
		}

		public override void Delete(StoreId id, DeleteItemFlags flags)
		{
			try
			{
				base.Delete(id, flags);
			}
			catch (CannotMoveDefaultFolderException innerException)
			{
				throw new CannotDeleteDefaultCalendarException(innerException);
			}
		}

		protected internal override ICalendarFolder BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToCalendarFolder(base.Session, id);
		}

		protected override ICalendarFolder CreateNewStoreObject()
		{
			return base.XsoFactory.CreateCalendarFolder(base.Session, this.ParentFolderId);
		}

		protected override void SaveAndCheckForConflicts(ICalendarFolder storeObject, SaveMode saveMode)
		{
			string displayName = storeObject.DisplayName;
			try
			{
				FolderSaveResult folderSaveResult = storeObject.Save(saveMode);
				switch (folderSaveResult.OperationResult)
				{
				case OperationResult.Failed:
					throw new CalendarFolderUpdateFailedException(folderSaveResult.Exception);
				case OperationResult.PartiallySucceeded:
					if (folderSaveResult.PropertyErrors.Length == 1 && folderSaveResult.PropertyErrors[0].PropertyDefinition == FolderSchema.DisplayName && folderSaveResult.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.FolderNameConflict)
					{
						throw new CalendarNameAlreadyInUseException(displayName, folderSaveResult.Exception);
					}
					throw new CalendarFolderUpdateFailedException(folderSaveResult.Exception);
				}
			}
			catch (ObjectValidationException ex)
			{
				IMailboxSession mailboxSession = base.Session as IMailboxSession;
				bool flag = mailboxSession != null && mailboxSession.IsDefaultFolderType(storeObject.Id) == DefaultFolderType.Calendar;
				bool flag2;
				if (flag)
				{
					flag2 = (ex.Errors.FirstOrDefault((StoreObjectValidationError x) => x.PropertyDefinition == FolderSchema.DisplayName) != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					throw new CannotRenameDefaultCalendarException(ex);
				}
				throw;
			}
			catch (ObjectExistedException innerException)
			{
				throw new CalendarNameAlreadyInUseException(displayName, innerException);
			}
		}
	}
}
