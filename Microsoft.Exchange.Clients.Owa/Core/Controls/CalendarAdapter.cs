using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class CalendarAdapter : CalendarAdapterBase
	{
		public CalendarAdapter(UserContext userContext, StoreObjectId storeObjectId) : this(userContext, OwaStoreObjectId.CreateFromSessionFolderId(userContext, userContext.MailboxSession, storeObjectId))
		{
		}

		public CalendarAdapter(UserContext userContext, OwaStoreObjectId folderId)
		{
			this.ownFolder = true;
			base..ctor();
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			switch (folderId.OwaStoreObjectIdType)
			{
			case OwaStoreObjectIdType.MailBoxObject:
			case OwaStoreObjectIdType.PublicStoreFolder:
			case OwaStoreObjectIdType.ArchiveMailboxObject:
				goto IL_A8;
			case OwaStoreObjectIdType.OtherUserMailboxObject:
			case OwaStoreObjectIdType.GSCalendar:
				if (folderId.MailboxOwnerLegacyDN == null)
				{
					ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "MailboxOwnerLegacyDN cannot be null");
					throw new ArgumentException("MailboxOwnerLegacyDN cannot be null");
				}
				goto IL_A8;
			}
			ExTraceGlobals.CalendarCallTracer.TraceDebug<OwaStoreObjectIdType>((long)this.GetHashCode(), "Does not support this type of OwaStoreObjectId: {0}", folderId.OwaStoreObjectIdType);
			throw new ArgumentException("Does not support this type of OwaStoreObjectId");
			IL_A8:
			this.UserContext = userContext;
			this.FolderId = folderId;
			this.ownFolder = true;
		}

		public CalendarAdapter(UserContext userContext, CalendarFolder folder)
		{
			this.ownFolder = true;
			base..ctor();
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.UserContext = userContext;
			this.folder = folder;
			this.FolderId = OwaStoreObjectId.CreateFromStoreObject(folder);
			this.ownFolder = false;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.folder != null && this.ownFolder)
			{
				this.folder.Dispose();
				this.folder = null;
			}
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, bool addOwaConditionAdvisor, bool throwIfFolderNotFound)
		{
			if (queryProperties == null || queryProperties.Length == 0)
			{
				throw new ArgumentNullException("queryProperties");
			}
			base.DateRanges = CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(days);
			if (this.folder == null)
			{
				try
				{
					this.folder = this.OpenFolder(throwIfFolderNotFound);
				}
				catch (OwaSharedFromOlderVersionException)
				{
				}
				catch (OwaLoadSharedCalendarFailedException)
				{
					return;
				}
			}
			this.GetDataAndUpdateCommonViewIfNecessary(false);
			if (this.folder != null && CalendarUtilities.UserHasRightToLoad(this.folder))
			{
				base.DataSource = new CalendarDataSource(this.UserContext, this.folder, base.DateRanges, queryProperties);
				if (addOwaConditionAdvisor)
				{
					this.AddOwaConditionAdvisorIfNecessary(this.folder);
				}
			}
			else if (this.IsGSCalendar || (this.isFromOlderVersion && this.olderExchangeCalendarTypeInNode != NavigationNodeFolder.OlderExchangeCalendarType.Secondary))
			{
				base.DataSource = new AvailabilityDataSource(this.UserContext, this.SmtpAddress, (!this.IsGSCalendar && this.olderExchangeCalendarTypeInNode == NavigationNodeFolder.OlderExchangeCalendarType.Unknown) ? this.FolderId.StoreObjectId : null, base.DateRanges, true);
			}
			this.dataLoaded = true;
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, bool addOwaConditionAdvisor, ref CalendarViewType viewType, out int viewWidth, out ReadingPanePosition readingPanePosition)
		{
			this.LoadData(queryProperties, days, addOwaConditionAdvisor, 0, 24, ref viewType, out viewWidth, out readingPanePosition);
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, bool addOwaConditionAdvisor, int startHour, int endHour, ref CalendarViewType viewType, out int viewWidth, out ReadingPanePosition readingPanePosition)
		{
			if (queryProperties == null)
			{
				throw new ArgumentNullException("queryProperties");
			}
			if (queryProperties.Length == 0)
			{
				throw new ArgumentException("Length of queryProperties cannot be 0");
			}
			viewWidth = 0;
			readingPanePosition = ReadingPanePosition.Min;
			if (this.folder == null)
			{
				try
				{
					this.folder = this.OpenFolder(false);
				}
				catch (OwaSharedFromOlderVersionException)
				{
				}
				catch (OwaLoadSharedCalendarFailedException)
				{
					return;
				}
			}
			this.GetDataAndUpdateCommonViewIfNecessary(true);
			if (this.folder != null && CalendarUtilities.UserHasRightToLoad(this.folder))
			{
				this.LoadFolderViewStates(this.folder, ref days, ref viewType, out viewWidth, out readingPanePosition);
				base.DateRanges = CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(days, startHour, endHour);
				base.DataSource = new CalendarDataSource(this.UserContext, this.folder, base.DateRanges, queryProperties);
				if (addOwaConditionAdvisor)
				{
					this.AddOwaConditionAdvisorIfNecessary(this.folder);
				}
			}
			else if (this.IsGSCalendar || (this.isFromOlderVersion && this.olderExchangeCalendarTypeInNode != NavigationNodeFolder.OlderExchangeCalendarType.Secondary))
			{
				this.LoadFolderViewStates(null, ref days, ref viewType, out viewWidth, out readingPanePosition);
				base.DateRanges = CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(days, startHour, endHour);
				base.DataSource = new AvailabilityDataSource(this.UserContext, this.SmtpAddress, (!this.IsGSCalendar && this.olderExchangeCalendarTypeInNode == NavigationNodeFolder.OlderExchangeCalendarType.Unknown) ? this.FolderId.StoreObjectId : null, base.DateRanges, false);
			}
			this.dataLoaded = true;
		}

		private static void InternalGetFolderViewStates(UserContext userContext, CalendarFolder folder, ref ExDateTime[] days, ref CalendarViewType viewType, out int viewWidth, out ReadingPanePosition readingPanePosition)
		{
			FolderViewStates folderViewStates = userContext.GetFolderViewStates(folder);
			CalendarUtilities.GetCalendarViewParamsFromViewStates(folderViewStates, out viewWidth, ref viewType, out readingPanePosition);
			days = CalendarUtilities.GetViewDays(userContext, days, viewType, OwaStoreObjectId.CreateFromStoreObject(folder), folderViewStates);
		}

		private void GetDataAndUpdateCommonViewIfNecessary(bool needGetColor)
		{
			NavigationNodeCollection navigationNodeCollection = null;
			NavigationNodeFolder[] array = null;
			if (Utilities.IsWebPartDelegateAccessRequest(OwaContext.Current) || (!needGetColor && this.FolderId.Equals(this.UserContext.CalendarFolderOwaId)) || !this.TryGetNodeFoldersFromNavigationTree(out array, out navigationNodeCollection))
			{
				base.CalendarTitle = ((this.folder != null) ? this.folder.DisplayName : string.Empty);
				this.CalendarColor = -2;
				return;
			}
			this.CalendarColor = CalendarColorManager.GetCalendarFolderColor(this.UserContext, navigationNodeCollection, array);
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromNavigationNodeFolder(this.UserContext, array[0]);
			if (owaStoreObjectId.IsArchive)
			{
				base.CalendarTitle = string.Format(LocalizedStrings.GetNonEncoded(-83764036), array[0].Subject, Utilities.GetMailboxOwnerDisplayName((MailboxSession)owaStoreObjectId.GetSession(this.UserContext)));
			}
			else
			{
				base.CalendarTitle = array[0].Subject;
			}
			foreach (NavigationNodeFolder navigationNodeFolder in array)
			{
				if (!navigationNodeFolder.IsGSCalendar && navigationNodeFolder.IsPrimarySharedCalendar)
				{
					navigationNodeFolder.UpgradeToGSCalendar();
				}
				if (this.olderExchangeCalendarTypeInNode == NavigationNodeFolder.OlderExchangeCalendarType.Unknown)
				{
					this.olderExchangeCalendarTypeInNode = navigationNodeFolder.OlderExchangeSharedCalendarType;
				}
			}
		}

		public void SaveCalendarTypeFromOlderExchangeAsNeeded()
		{
			if (this.isFromOlderVersion)
			{
				NavigationNodeCollection navigationNodeCollection = null;
				NavigationNodeFolder[] array = null;
				if (this.TryGetNodeFoldersFromNavigationTree(out array, out navigationNodeCollection))
				{
					foreach (NavigationNodeFolder navigationNodeFolder in array)
					{
						if (this.olderExchangeCalendarTypeInNode == NavigationNodeFolder.OlderExchangeCalendarType.Unknown)
						{
							navigationNodeFolder.OlderExchangeSharedCalendarType = this.OlderExchangeSharedCalendarType;
						}
					}
					navigationNodeCollection.Save(this.UserContext.MailboxSession);
				}
			}
		}

		public static void KeepMapiNotification(UserContext userContext, OwaStoreObjectId folderId)
		{
			using (CalendarAdapter calendarAdapter = new CalendarAdapter(userContext, folderId))
			{
				calendarAdapter.KeepMapiNotification();
			}
		}

		private void KeepMapiNotification()
		{
			try
			{
				this.folder = this.OpenFolder(false);
			}
			catch (OwaSharedFromOlderVersionException)
			{
				return;
			}
			catch (OwaLoadSharedCalendarFailedException)
			{
				return;
			}
			if (this.folder != null)
			{
				this.AddOwaConditionAdvisorIfNecessary(this.folder);
				if (this.PromotedFolderId != null && this.PromotedFolderId.IsOtherMailbox)
				{
					MailboxSession session = this.folder.Session as MailboxSession;
					this.UserContext.MapiNotificationManager.RenewDelegateHandler(session);
				}
			}
		}

		public bool IsGSCalendar
		{
			get
			{
				return this.FolderId.IsGSCalendar;
			}
		}

		public bool IsSecondaryCalendarFromOldExchange
		{
			get
			{
				return this.OlderExchangeSharedCalendarType == NavigationNodeFolder.OlderExchangeCalendarType.Secondary;
			}
		}

		public NavigationNodeFolder.OlderExchangeCalendarType OlderExchangeSharedCalendarType
		{
			get
			{
				if (!this.dataLoaded)
				{
					return NavigationNodeFolder.OlderExchangeCalendarType.Unknown;
				}
				if (!this.isFromOlderVersion)
				{
					return NavigationNodeFolder.OlderExchangeCalendarType.NotOlderVersion;
				}
				if (this.olderExchangeCalendarTypeInNode != NavigationNodeFolder.OlderExchangeCalendarType.Unknown)
				{
					return this.olderExchangeCalendarTypeInNode;
				}
				switch (((AvailabilityDataSource)base.DataSource).AssociatedCalendarType)
				{
				case AvailabilityDataSource.CalendarType.Primary:
					return NavigationNodeFolder.OlderExchangeCalendarType.Primary;
				case AvailabilityDataSource.CalendarType.Secondary:
					return NavigationNodeFolder.OlderExchangeCalendarType.Secondary;
				}
				return NavigationNodeFolder.OlderExchangeCalendarType.Unknown;
			}
		}

		public CalendarFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public UserContext UserContext { get; private set; }

		public OwaStoreObjectId FolderId { get; private set; }

		public OwaStoreObjectId PromotedFolderId { get; private set; }

		public int CalendarColor { get; protected set; }

		public bool IsPublic
		{
			get
			{
				return this.FolderId.IsPublic;
			}
		}

		public string SmtpAddress
		{
			get
			{
				if (this.FolderId.IsPublic)
				{
					return null;
				}
				if (this.exchangePrincipal != null)
				{
					return this.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
				}
				return this.UserContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		public string LegacyDN
		{
			get
			{
				return this.FolderId.MailboxOwnerLegacyDN;
			}
		}

		public override string CalendarOwnerDisplayName
		{
			get
			{
				if (base.DataSource.SharedType == SharedType.CrossOrg || base.DataSource.SharedType == SharedType.WebCalendar)
				{
					throw new NotSupportedException("CalendarOwnerDisplayName is not support for external shared calendar.");
				}
				if (this.FolderId.IsOtherMailbox || this.FolderId.IsGSCalendar)
				{
					return this.exchangePrincipal.MailboxInfo.DisplayName;
				}
				return this.UserContext.MailboxSession.DisplayName;
			}
		}

		public bool IsInArchiveMailbox
		{
			get
			{
				return this.folder != null && Utilities.IsInArchiveMailbox(this.folder);
			}
		}

		public bool IsPublishedOut
		{
			get
			{
				return this.folder != null && Utilities.IsPublishedOutFolder(this.folder);
			}
		}

		public bool IsExternalSharedInFolder
		{
			get
			{
				return this.folder != null && Utilities.IsExternalSharedInFolder(this.folder);
			}
		}

		public override string IdentityString
		{
			get
			{
				return this.FolderId.ToBase64String();
			}
		}

		public ExDateTime LastAttemptTime
		{
			get
			{
				if (!this.IsExternalSharedInFolder)
				{
					return ExDateTime.MinValue;
				}
				return this.folder.LastAttemptedSyncTime;
			}
		}

		public ExDateTime LastSuccessSyncTime
		{
			get
			{
				if (!this.IsExternalSharedInFolder)
				{
					return ExDateTime.MinValue;
				}
				return this.folder.LastSuccessfulSyncTime;
			}
		}

		public string WebCalendarUrl
		{
			get
			{
				if (this.webCalendarUrl == null && base.DataSource.SharedType == SharedType.WebCalendar)
				{
					this.webCalendarUrl = CalendarUtilities.GetWebCalendarUrl((MailboxSession)this.folder.Session, this.FolderId.StoreObjectId);
				}
				return this.webCalendarUrl;
			}
		}

		private bool TryGetNodeFoldersFromNavigationTree(out NavigationNodeFolder[] navigationNodeFolders, out NavigationNodeCollection navigationNodeCollection)
		{
			navigationNodeFolders = null;
			navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(this.UserContext, this.UserContext.MailboxSession, NavigationNodeGroupSection.Calendar);
			if (navigationNodeCollection != null)
			{
				try
				{
					if (this.IsGSCalendar)
					{
						navigationNodeFolders = navigationNodeCollection.FindGSCalendarsByLegacyDN(this.FolderId.MailboxOwnerLegacyDN);
					}
					else
					{
						navigationNodeFolders = navigationNodeCollection.FindFoldersById(this.FolderId.StoreObjectId);
					}
					if (navigationNodeFolders != null && navigationNodeFolders.Length > 0)
					{
						return true;
					}
				}
				catch (StoragePermanentException ex)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "CalendarColorManager.GetCalendarFolderColor. Unable to find tree node related to the given calendar. Exception: {0}.", new object[]
					{
						ex.Message
					});
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, message);
				}
				catch (StorageTransientException ex2)
				{
					string message2 = string.Format(CultureInfo.InvariantCulture, "CalendarColorManager.GetCalendarFolderColor. Unable to find tree node related to the given calendar. Exception: {0}.", new object[]
					{
						ex2.Message
					});
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, message2);
				}
				return false;
			}
			return false;
		}

		private CalendarFolder OpenFolder(bool throwIfFolderNotFound)
		{
			CalendarFolder calendarFolder = null;
			this.isFromOlderVersion = false;
			if (this.exchangePrincipal == null && (this.FolderId.IsOtherMailbox || this.FolderId.IsGSCalendar) && !this.UserContext.DelegateSessionManager.TryGetExchangePrincipal(this.FolderId.MailboxOwnerLegacyDN, out this.exchangePrincipal))
			{
				throw new OwaLoadSharedCalendarFailedException("cannot get exchange principal");
			}
			try
			{
				if (this.IsGSCalendar)
				{
					StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(this.UserContext, this.exchangePrincipal, DefaultFolderType.Calendar);
					if (storeObjectId != null)
					{
						this.PromotedFolderId = OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(storeObjectId, this.LegacyDN);
					}
				}
				else
				{
					this.PromotedFolderId = this.FolderId;
				}
				if (this.PromotedFolderId != null)
				{
					try
					{
						calendarFolder = Utilities.GetFolderForContent<CalendarFolder>(this.UserContext, this.PromotedFolderId, CalendarUtilities.FolderViewProperties);
						if (!this.IsGSCalendar)
						{
							this.FolderId = OwaStoreObjectId.CreateFromSessionFolderId(this.FolderId.OwaStoreObjectIdType, this.FolderId.MailboxOwnerLegacyDN, calendarFolder.StoreObjectId);
						}
					}
					catch (ObjectNotFoundException)
					{
						if (throwIfFolderNotFound)
						{
							throw;
						}
					}
					catch (WrongObjectTypeException innerException)
					{
						throw new OwaInvalidRequestException("The folder is not a calendar folder", innerException, this);
					}
				}
			}
			catch (OwaSharedFromOlderVersionException)
			{
				this.isFromOlderVersion = true;
				throw;
			}
			return calendarFolder;
		}

		private void AddOwaConditionAdvisorIfNecessary(CalendarFolder folder)
		{
			Utilities.AddOwaConditionAdvisorIfNecessary(this.UserContext, folder, EventObjectType.None, EventType.None);
		}

		private void LoadFolderViewStates(CalendarFolder advicedFolder, ref ExDateTime[] days, ref CalendarViewType viewType, out int viewWidth, out ReadingPanePosition readingPanePosition)
		{
			OwaStoreObjectId owaStoreObjectId = null;
			if (advicedFolder != null)
			{
				owaStoreObjectId = OwaStoreObjectId.CreateFromStoreObject(advicedFolder);
			}
			if (owaStoreObjectId != null && (owaStoreObjectId.Equals(this.UserContext.CalendarFolderOwaId) || owaStoreObjectId.IsPublic))
			{
				CalendarAdapter.InternalGetFolderViewStates(this.UserContext, advicedFolder, ref days, ref viewType, out viewWidth, out readingPanePosition);
				return;
			}
			using (CalendarFolder folderForContent = Utilities.GetFolderForContent<CalendarFolder>(this.UserContext, this.UserContext.CalendarFolderOwaId, CalendarUtilities.FolderViewProperties))
			{
				CalendarAdapter.InternalGetFolderViewStates(this.UserContext, folderForContent, ref days, ref viewType, out viewWidth, out readingPanePosition);
			}
		}

		private ExchangePrincipal exchangePrincipal;

		private CalendarFolder folder;

		private bool isFromOlderVersion;

		private NavigationNodeFolder.OlderExchangeCalendarType olderExchangeCalendarTypeInNode;

		private bool ownFolder;

		private bool dataLoaded;

		private string webCalendarUrl;
	}
}
