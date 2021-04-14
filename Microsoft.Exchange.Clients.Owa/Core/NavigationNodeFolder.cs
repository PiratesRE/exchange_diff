using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class NavigationNodeFolder : NavigationNode, ICloneable
	{
		private static List<KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>> CreateMappingFromFolderFlagToNodeFlag()
		{
			return new List<KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>>
			{
				new KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>(ExtendedFolderFlags.IsSharepointFolder, NavigationNodeFlags.SharepointFolder),
				new KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>(ExtendedFolderFlags.SharedIn, NavigationNodeFlags.SharedIn),
				new KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>(ExtendedFolderFlags.SharedOut, NavigationNodeFlags.SharedOut),
				new KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>(ExtendedFolderFlags.SharedViaExchange, NavigationNodeFlags.SharedOut),
				new KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>(ExtendedFolderFlags.PersonalShare, NavigationNodeFlags.PersonFolder),
				new KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>(ExtendedFolderFlags.ICalFolder, NavigationNodeFlags.IcalFolder)
			};
		}

		internal NavigationNodeFolder(MailboxSession session, bool isMyMailbox, object[] folderPropertyValues, Dictionary<PropertyDefinition, int> folderPropertyMap, string subject, Guid groupClassId, NavigationNodeGroupSection navigationNodeGroupSection, string groupName) : base(NavigationNodeType.NormalFolder, subject, navigationNodeGroupSection)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			Utilities.CheckAndThrowForRequiredProperty(folderPropertyMap, new PropertyDefinition[]
			{
				FolderSchema.Id,
				StoreObjectSchema.RecordKey,
				FolderSchema.ExtendedFolderFlags
			});
			StoreObjectId objectId = ((VersionedId)folderPropertyValues[folderPropertyMap[FolderSchema.Id]]).ObjectId;
			ExtendedFolderFlags flags = (ExtendedFolderFlags)0;
			object obj = folderPropertyValues[folderPropertyMap[FolderSchema.ExtendedFolderFlags]];
			if (!(obj is PropertyError))
			{
				flags = (ExtendedFolderFlags)obj;
			}
			this.Initialize(session, isMyMailbox, objectId, folderPropertyValues[folderPropertyMap[StoreObjectSchema.RecordKey]], flags, groupClassId, groupName);
			base.ClearDirty();
		}

		internal NavigationNodeFolder(Folder folder, bool isMyFolder, string subject, NavigationNodeType navigationNodeType, Guid groupClassId, NavigationNodeGroupSection navigationNodeGroupSection, string groupName) : base(navigationNodeType, subject, navigationNodeGroupSection)
		{
			if (navigationNodeType == NavigationNodeType.Header || navigationNodeType == NavigationNodeType.Undefined)
			{
				throw new ArgumentException("The type should not be header for folders.");
			}
			MailboxSession mailboxSession = folder.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new ArgumentException("The folder doesn't belong to a mailbox session.");
			}
			this.Initialize(mailboxSession, isMyFolder, folder.Id.ObjectId, folder.TryGetProperty(StoreObjectSchema.RecordKey), folder.GetValueOrDefault<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags), groupClassId, groupName);
			if (navigationNodeType == NavigationNodeType.SmartFolder)
			{
				object obj = folder.TryGetProperty(FolderSchema.OutlookSearchFolderClsId);
				if (obj is Guid)
				{
					this.AssociatedSearchFolderId = (Guid)obj;
				}
			}
			base.ClearDirty();
		}

		internal NavigationNodeFolder(UserContext userContext, MailboxSession session, bool includeArchive, object[] values, Dictionary<PropertyDefinition, int> propertyMap) : base(NavigationNodeFolder.navigationNodeFolderProperties, values, propertyMap)
		{
			string originalLegacyDN = this.GetLegacyDNFromStoreEntryId();
			if (base.IsFlagSet(NavigationNodeFlags.IsDefaultStore) || StringComparer.OrdinalIgnoreCase.Equals(session.MailboxOwner.LegacyDn, originalLegacyDN))
			{
				this.calculatedLegacyDN = originalLegacyDN;
				if (base.IsFlagSet(NavigationNodeFlags.TodoFolder))
				{
					this.calculatedFolderId = Utilities.TryGetDefaultFolderId(session, DefaultFolderType.ToDoSearch);
					return;
				}
				if (base.IsFlagSet(NavigationNodeFlags.RootFolder))
				{
					this.calculatedFolderId = Utilities.TryGetDefaultFolderId(session, DefaultFolderType.Root);
					return;
				}
			}
			else if (base.NavigationNodeType == NavigationNodeType.NormalFolder || base.NavigationNodeType == NavigationNodeType.SmartFolder)
			{
				this.isMailboxLegacyDNValid = false;
				if (userContext != null && userContext.ArchiveAccessed && includeArchive)
				{
					userContext.TryLoopArchiveMailboxes(delegate(MailboxSession archiveSession)
					{
						if (StringComparer.OrdinalIgnoreCase.Equals(archiveSession.MailboxOwnerLegacyDN, originalLegacyDN))
						{
							this.isMailboxLegacyDNValid = true;
							this.calculatedLegacyDN = originalLegacyDN;
						}
					});
				}
			}
		}

		private NavigationNodeFolder(string subject, NavigationNodeGroupSection navigationNodeGroupSection) : base(NavigationNodeType.SharedFolder, subject, navigationNodeGroupSection)
		{
		}

		internal static NavigationNodeFolder CreateGSNode(ExchangePrincipal exchangePrincipal, Guid groupClassId, string groupName, string subject, NavigationNodeGroupSection navigationNodeGroupSection)
		{
			NavigationNodeFolder navigationNodeFolder = new NavigationNodeFolder(subject, navigationNodeGroupSection);
			navigationNodeFolder.GSCalendarSharerAddressBookEntryId = AddressBookEntryId.MakeAddressBookEntryID(exchangePrincipal);
			UserContext userContext = OwaContext.Current.UserContext;
			navigationNodeFolder.GSCalendarShareeStoreEntryId = StoreEntryId.ToProviderStoreEntryId(userContext.ExchangePrincipal);
			try
			{
				using (OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(exchangePrincipal, userContext))
				{
					try
					{
						using (Folder folder = Folder.Bind(owaStoreObjectIdSessionHandle.Session as MailboxSession, DefaultFolderType.Calendar, new PropertyDefinition[]
						{
							StoreObjectSchema.EffectiveRights
						}))
						{
							if (CalendarUtilities.UserHasRightToLoad(folder))
							{
								navigationNodeFolder.NavigationNodeEntryId = folder.StoreObjectId.ProviderLevelItemId;
								navigationNodeFolder.NavigationNodeStoreEntryId = StoreEntryId.ToProviderStoreEntryId(exchangePrincipal);
							}
						}
					}
					catch (ObjectNotFoundException)
					{
					}
				}
			}
			catch (OwaSharedFromOlderVersionException)
			{
			}
			navigationNodeFolder.ClearDirty();
			return navigationNodeFolder;
		}

		private NavigationNodeFolder(MemoryPropertyBag propertyBag) : base(propertyBag)
		{
		}

		private void Initialize(MailboxSession session, bool isMyMailbox, StoreObjectId folderId, object recordKey, ExtendedFolderFlags flags, Guid groupClassId, string groupName)
		{
			StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(session, DefaultFolderType.Root);
			if (storeObjectId != null && storeObjectId.Equals(folderId))
			{
				throw new NotSupportedException("We don't support adding root folder to favorites.");
			}
			if (!session.IsRemote)
			{
				this.NavigationNodeStoreEntryId = StoreEntryId.ToProviderStoreEntryId(session.MailboxOwner);
			}
			this.NavigationNodeEntryId = folderId.ProviderLevelItemId;
			if (recordKey is byte[])
			{
				this.NavigationNodeRecordKey = (byte[])recordKey;
			}
			StoreObjectId storeObjectId2 = Utilities.TryGetDefaultFolderId(session, DefaultFolderType.ToDoSearch);
			if (storeObjectId2 != null && storeObjectId2.Equals(folderId))
			{
				base.NavigationNodeFlags |= NavigationNodeFlags.TodoFolder;
			}
			foreach (KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags> keyValuePair in NavigationNodeFolder.mappingFromFolderFlagToNodeFlag)
			{
				if (Utilities.IsFlagSet((int)flags, (int)keyValuePair.Key))
				{
					base.NavigationNodeFlags |= keyValuePair.Value;
				}
			}
			if (isMyMailbox)
			{
				base.NavigationNodeFlags |= NavigationNodeFlags.IsDefaultStore;
			}
			if (!base.IsFavorites)
			{
				this.NavigationNodeParentGroupClassId = groupClassId;
				this.NavigationNodeGroupName = groupName;
			}
		}

		private StoreObjectType GetExpectedStoreObjectType()
		{
			switch (base.NavigationNodeGroupSection)
			{
			case NavigationNodeGroupSection.First:
				if (this.IsFilteredView)
				{
					return StoreObjectType.SearchFolder;
				}
				if (!this.AssociatedSearchFolderId.Equals(Guid.Empty))
				{
					return StoreObjectType.OutlookSearchFolder;
				}
				break;
			case NavigationNodeGroupSection.Calendar:
				return StoreObjectType.CalendarFolder;
			case NavigationNodeGroupSection.Contacts:
				return StoreObjectType.ContactsFolder;
			case NavigationNodeGroupSection.Tasks:
				if (base.IsFlagSet(NavigationNodeFlags.TodoFolder))
				{
					return StoreObjectType.SearchFolder;
				}
				return StoreObjectType.TasksFolder;
			case NavigationNodeGroupSection.Notes:
				return StoreObjectType.NotesFolder;
			case NavigationNodeGroupSection.Journal:
				return StoreObjectType.JournalFolder;
			}
			return StoreObjectType.Folder;
		}

		internal void FixLegacyDNRelatedFlag(MailboxSession mailboxSession)
		{
			bool flag = string.Equals(mailboxSession.MailboxOwner.LegacyDn, this.GetLegacyDNFromStoreEntryId(), StringComparison.OrdinalIgnoreCase);
			if (base.IsFlagSet(NavigationNodeFlags.IsDefaultStore))
			{
				if (!flag)
				{
					this.NavigationNodeStoreEntryId = StoreEntryId.ToProviderStoreEntryId(mailboxSession.MailboxOwner);
					return;
				}
			}
			else if (flag)
			{
				base.NavigationNodeFlags |= NavigationNodeFlags.IsDefaultStore;
			}
		}

		private string GetLegacyDNFromStoreEntryId()
		{
			string text = null;
			byte[] valueOrDefault = this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.StoreEntryId);
			if (valueOrDefault != null)
			{
				text = StoreEntryId.TryParseStoreEntryIdMailboxDN(valueOrDefault);
				if (text == null)
				{
					text = StoreEntryId.TryParseMailboxLegacyDN(valueOrDefault);
				}
				if (string.IsNullOrEmpty(text) || !Utilities.IsValidLegacyDN(text))
				{
					text = string.Empty;
				}
			}
			return text ?? string.Empty;
		}

		internal string MailboxLegacyDN
		{
			get
			{
				if (this.calculatedLegacyDN != null)
				{
					return this.calculatedLegacyDN;
				}
				if (this.legacyDN == null)
				{
					if (this.IsGSCalendar)
					{
						Eidt eidt;
						string text;
						if (AddressBookEntryId.IsAddressBookEntryId(this.GSCalendarSharerAddressBookEntryId, out eidt, out text))
						{
							this.legacyDN = text;
						}
					}
					else
					{
						this.legacyDN = this.GetLegacyDNFromStoreEntryId();
					}
				}
				return this.legacyDN;
			}
		}

		private byte[] GSCalendarSharerAddressBookEntryId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.AddressBookEntryId);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.AddressBookEntryId, value);
			}
		}

		private byte[] GSCalendarShareeStoreEntryId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.AddressBookStoreEntryId);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.AddressBookStoreEntryId, value);
			}
		}

		internal bool IsFolderInSpecificMailboxSession(MailboxSession mailboxSession)
		{
			return string.Equals(mailboxSession.MailboxOwnerLegacyDN, this.MailboxLegacyDN, StringComparison.OrdinalIgnoreCase);
		}

		internal StoreObjectId FolderId
		{
			get
			{
				if (this.calculatedFolderId != null)
				{
					return this.calculatedFolderId;
				}
				if (!this.IsGSCalendar && this.folderId == null)
				{
					byte[] valueOrDefault = this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.NodeEntryId);
					if (valueOrDefault != null)
					{
						try
						{
							this.folderId = StoreObjectId.FromProviderSpecificId(valueOrDefault, this.GetExpectedStoreObjectType());
						}
						catch (CorruptDataException ex)
						{
							string message = string.Format(CultureInfo.InvariantCulture, "NavigationNodeFolder.FolderId property accessor exception {0} for folder entry id {1}", new object[]
							{
								ex,
								Convert.ToBase64String(valueOrDefault)
							});
							ExTraceGlobals.CoreCallTracer.TraceDebug(0L, message);
						}
					}
				}
				return this.folderId;
			}
		}

		private byte[] NavigationNodeEntryId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.NodeEntryId);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.NodeEntryId, value);
			}
		}

		private byte[] NavigationNodeRecordKey
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.NodeRecordKey);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.NodeRecordKey, value);
			}
		}

		private byte[] NavigationNodeStoreEntryId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.StoreEntryId);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.StoreEntryId, value);
			}
		}

		public void SetFilterParameter(StoreObjectId folderId, int flags, string[] categories, string from, string to)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (base.NavigationNodeType != NavigationNodeType.SmartFolder)
			{
				throw new InvalidOperationException("Only smart folder can be set filter parameters");
			}
			this.propertyBag.SetOrDeleteProperty(ViewStateProperties.FilterSourceFolder, folderId.ToBase64String());
			this.propertyBag.SetOrDeleteProperty(ViewStateProperties.FilteredViewFlags, flags);
			this.propertyBag.SetOrDeleteProperty(ViewStateProperties.FilteredViewFrom, from);
			this.propertyBag.SetOrDeleteProperty(ViewStateProperties.FilteredViewTo, to);
			this.propertyBag.SetOrDeleteProperty(ItemSchema.Categories, categories);
		}

		public void DeleteFilterParameter()
		{
			this.propertyBag.Delete(ViewStateProperties.FilterSourceFolder);
			this.propertyBag.Delete(ViewStateProperties.FilteredViewFlags);
			this.propertyBag.Delete(ViewStateProperties.FilteredViewFrom);
			this.propertyBag.Delete(ViewStateProperties.FilteredViewTo);
			this.propertyBag.Delete(ItemSchema.Categories);
		}

		public bool IsFilteredView
		{
			get
			{
				return base.NavigationNodeType == NavigationNodeType.SmartFolder && this.FilterSourceFolder != null;
			}
		}

		public StoreObjectId FilterSourceFolder
		{
			get
			{
				if (base.NavigationNodeType != NavigationNodeType.SmartFolder)
				{
					return null;
				}
				string valueOrDefault = this.propertyBag.GetValueOrDefault<string>(ViewStateProperties.FilterSourceFolder);
				if (string.IsNullOrEmpty(valueOrDefault))
				{
					return null;
				}
				StoreObjectId result;
				try
				{
					result = Utilities.CreateStoreObjectId(valueOrDefault);
				}
				catch (OwaInvalidIdFormatException)
				{
					result = null;
				}
				return result;
			}
		}

		public int FilterFlag
		{
			get
			{
				if (!this.IsFilteredView)
				{
					return 0;
				}
				return this.propertyBag.GetValueOrDefault<int>(ViewStateProperties.FilteredViewFlags);
			}
		}

		public string[] FitlerCategories
		{
			get
			{
				if (!this.IsFilteredView)
				{
					return null;
				}
				return this.propertyBag.GetValueOrDefault<string[]>(ItemSchema.Categories);
			}
		}

		public string FilterFrom
		{
			get
			{
				if (!this.IsFilteredView)
				{
					return null;
				}
				return this.propertyBag.GetValueOrDefault<string>(ViewStateProperties.FilteredViewFrom);
			}
		}

		public string FilterTo
		{
			get
			{
				if (!this.IsFilteredView)
				{
					return null;
				}
				return this.propertyBag.GetValueOrDefault<string>(ViewStateProperties.FilteredViewTo);
			}
		}

		public string NavigationNodeGroupName
		{
			get
			{
				this.ThrowIfInFavorites();
				return this.propertyBag.GetValueOrDefault<string>(NavigationNodeSchema.GroupName);
			}
			set
			{
				this.ThrowIfInFavorites();
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.GroupName, value);
			}
		}

		internal Guid NavigationNodeParentGroupClassId
		{
			get
			{
				this.ThrowIfInFavorites();
				return base.GuidGetter(NavigationNodeSchema.ParentGroupClassId);
			}
			set
			{
				this.ThrowIfInFavorites();
				base.GuidSetter(NavigationNodeSchema.ParentGroupClassId, value);
			}
		}

		public int NavigationNodeCalendarColor
		{
			get
			{
				if (base.NavigationNodeGroupSection != NavigationNodeGroupSection.Calendar)
				{
					throw new NotSupportedException("Only calendar item supports it.");
				}
				return this.propertyBag.GetValueOrDefault<int>(NavigationNodeSchema.CalendarColor, -1);
			}
			set
			{
				if (base.NavigationNodeGroupSection != NavigationNodeGroupSection.Calendar)
				{
					throw new NotSupportedException("Only calendar item supports it.");
				}
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.CalendarColor, value);
			}
		}

		internal Guid AssociatedSearchFolderId
		{
			get
			{
				this.ThrowIfGSCalendar();
				return base.GuidGetter(FolderSchema.AssociatedSearchFolderId);
			}
			private set
			{
				this.ThrowIfGSCalendar();
				base.GuidSetter(FolderSchema.AssociatedSearchFolderId, value);
			}
		}

		public object Clone()
		{
			return new NavigationNodeFolder(this.propertyBag)
			{
				isMailboxLegacyDNValid = this.isMailboxLegacyDNValid,
				calculatedFolderId = this.calculatedFolderId,
				calculatedLegacyDN = this.calculatedLegacyDN
			};
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				this.hashCode = new int?(this.MailboxLegacyDN.ToLowerInvariant().GetHashCode());
				if (base.IsFavorites)
				{
					if (this.AssociatedSearchFolderId.Equals(Guid.Empty))
					{
						if (this.FolderId != null)
						{
							this.hashCode ^= this.FolderId.GetHashCode();
						}
					}
					else
					{
						this.hashCode ^= this.AssociatedSearchFolderId.GetHashCode();
					}
				}
				else
				{
					if (this.FolderId != null)
					{
						this.hashCode ^= this.FolderId.GetHashCode();
					}
					this.hashCode ^= base.NavigationNodeType.GetHashCode();
				}
				this.hashCode ^= base.NavigationNodeGroupSection.GetHashCode();
			}
			return this.hashCode.Value;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NavigationNodeFolder))
			{
				return false;
			}
			NavigationNodeFolder navigationNodeFolder = obj as NavigationNodeFolder;
			if (!this.IsValid || !navigationNodeFolder.IsValid)
			{
				return false;
			}
			if (base.NavigationNodeGroupSection != navigationNodeFolder.NavigationNodeGroupSection)
			{
				return false;
			}
			if (!string.Equals(this.MailboxLegacyDN, navigationNodeFolder.MailboxLegacyDN, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (base.IsFavorites)
			{
				Guid associatedSearchFolderId = this.AssociatedSearchFolderId;
				Guid associatedSearchFolderId2 = navigationNodeFolder.AssociatedSearchFolderId;
				if (associatedSearchFolderId.Equals(Guid.Empty) && associatedSearchFolderId2.Equals(Guid.Empty))
				{
					return this.FolderId.Equals(navigationNodeFolder.FolderId);
				}
				return !associatedSearchFolderId.Equals(Guid.Empty) && !associatedSearchFolderId2.Equals(Guid.Empty) && associatedSearchFolderId.Equals(associatedSearchFolderId2);
			}
			else
			{
				if (this.IsGSCalendar && navigationNodeFolder.IsGSCalendar)
				{
					return this.NavigationNodeParentGroupClassId.Equals(navigationNodeFolder.NavigationNodeParentGroupClassId);
				}
				return this.FolderId != null && navigationNodeFolder.FolderId != null && this.FolderId.Equals(navigationNodeFolder.FolderId) && base.NavigationNodeType == navigationNodeFolder.NavigationNodeType;
			}
		}

		protected override void UpdateMessage(MessageItem message)
		{
			base.UpdateMessage(message);
			if (this.IsGSCalendar)
			{
				message[NavigationNodeSchema.AddressBookEntryId] = this.GSCalendarSharerAddressBookEntryId;
				message.SetOrDeleteProperty(NavigationNodeSchema.AddressBookStoreEntryId, this.GSCalendarShareeStoreEntryId);
			}
			message.SetOrDeleteProperty(NavigationNodeSchema.NodeEntryId, this.NavigationNodeEntryId);
			message.SetOrDeleteProperty(NavigationNodeSchema.StoreEntryId, this.NavigationNodeStoreEntryId);
			message.SetOrDeleteProperty(NavigationNodeSchema.CalendarTypeFromOlderExchange, this.propertyBag.TryGetProperty(NavigationNodeSchema.CalendarTypeFromOlderExchange));
			message.SetOrDeleteProperty(NavigationNodeSchema.NodeRecordKey, this.NavigationNodeRecordKey);
			if (base.IsFavorites)
			{
				Guid associatedSearchFolderId = this.AssociatedSearchFolderId;
				message.SetOrDeleteProperty(FolderSchema.AssociatedSearchFolderId, associatedSearchFolderId.Equals(Guid.Empty) ? null : associatedSearchFolderId.ToByteArray());
				if (this.IsFilteredView)
				{
					message.SetOrDeleteProperty(ViewStateProperties.FilterSourceFolder, this.FilterSourceFolder.ToBase64String());
					message.SetOrDeleteProperty(ViewStateProperties.FilteredViewFlags, this.FilterFlag);
					message.SetOrDeleteProperty(ItemSchema.Categories, this.FitlerCategories);
					message.SetOrDeleteProperty(ViewStateProperties.FilteredViewFrom, this.FilterFrom);
					message.SetOrDeleteProperty(ViewStateProperties.FilteredViewTo, this.FilterTo);
				}
				else
				{
					message.Delete(ViewStateProperties.FilterSourceFolder);
					message.Delete(ViewStateProperties.FilteredViewFlags);
					message.Delete(ItemSchema.Categories);
					message.Delete(ViewStateProperties.FilteredViewFrom);
					message.Delete(ViewStateProperties.FilteredViewTo);
				}
			}
			message.SetOrDeleteProperty(NavigationNodeSchema.ParentGroupClassId, base.IsFavorites ? null : this.NavigationNodeParentGroupClassId.ToByteArray());
			message.SetOrDeleteProperty(NavigationNodeSchema.GroupName, base.IsFavorites ? null : this.NavigationNodeGroupName);
			if (base.NavigationNodeGroupSection == NavigationNodeGroupSection.Calendar)
			{
				message[NavigationNodeSchema.CalendarColor] = this.NavigationNodeCalendarColor;
			}
		}

		internal bool IsValid
		{
			get
			{
				if (!this.isMailboxLegacyDNValid || string.IsNullOrEmpty(this.MailboxLegacyDN))
				{
					return false;
				}
				if (base.IsFavorites)
				{
					return !this.AssociatedSearchFolderId.Equals(Guid.Empty) || this.FolderId != null;
				}
				if (this.IsGSCalendar)
				{
					string gscalendarShareeLegacyDN = this.GetGSCalendarShareeLegacyDN();
					return !string.IsNullOrEmpty(this.MailboxLegacyDN) && (string.IsNullOrEmpty(gscalendarShareeLegacyDN) || string.Equals(OwaContext.Current.UserContext.ExchangePrincipal.LegacyDn, gscalendarShareeLegacyDN, StringComparison.Ordinal));
				}
				return this.FolderId != null;
			}
		}

		internal string GetFolderClass()
		{
			return NavigationNode.GetFolderClass(base.NavigationNodeGroupSection);
		}

		private void ThrowIfInFavorites()
		{
			if (base.IsFavorites)
			{
				throw new NotSupportedException("Favorite nodes don't support this property");
			}
		}

		private void ThrowIfGSCalendar()
		{
			if (this.IsGSCalendar)
			{
				throw new NotSupportedException("GS calendar nodes don't support this property/method");
			}
		}

		public bool IsGSCalendar
		{
			get
			{
				return null != this.GSCalendarSharerAddressBookEntryId;
			}
		}

		public void UpgradeToGSCalendar()
		{
			if (!this.IsPrimarySharedCalendar)
			{
				throw new InvalidOperationException("Only Primary shared calendar folder node can be upgraded to GS calendar.");
			}
			if (this.IsGSCalendar)
			{
				return;
			}
			UserContext userContext = OwaContext.Current.UserContext;
			ExchangePrincipal exchangePrincipal;
			if (userContext.DelegateSessionManager.TryGetExchangePrincipal(this.MailboxLegacyDN, out exchangePrincipal))
			{
				this.GSCalendarSharerAddressBookEntryId = AddressBookEntryId.MakeAddressBookEntryID(exchangePrincipal);
				base.Save(userContext.MailboxSession);
			}
		}

		public bool IsPrimarySharedCalendar
		{
			get
			{
				return this.IsE14PrimarySharedCalendar || this.OlderExchangeSharedCalendarType == NavigationNodeFolder.OlderExchangeCalendarType.Primary;
			}
		}

		public bool IsE14PrimarySharedCalendar
		{
			get
			{
				if (base.NavigationNodeType != NavigationNodeType.SharedFolder || base.NavigationNodeGroupSection != NavigationNodeGroupSection.Calendar)
				{
					return false;
				}
				if (this.isE14PrimarySharedCalendar == null)
				{
					this.isE14PrimarySharedCalendar = new bool?(false);
					UserContext userContext = OwaContext.Current.UserContext;
					ExchangePrincipal exchangePrincipal;
					if (userContext.DelegateSessionManager.TryGetExchangePrincipal(this.MailboxLegacyDN, out exchangePrincipal))
					{
						try
						{
							StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(userContext, exchangePrincipal, DefaultFolderType.Calendar);
							if (storeObjectId != null)
							{
								this.isE14PrimarySharedCalendar = new bool?(storeObjectId.Equals(this.FolderId));
							}
						}
						catch (OwaSharedFromOlderVersionException)
						{
						}
					}
				}
				return this.isE14PrimarySharedCalendar.Value;
			}
		}

		public NavigationNodeFolder.OlderExchangeCalendarType OlderExchangeSharedCalendarType
		{
			get
			{
				object obj = this.propertyBag.TryGetProperty(NavigationNodeSchema.CalendarTypeFromOlderExchange);
				if (obj != null && obj is int)
				{
					return (NavigationNodeFolder.OlderExchangeCalendarType)obj;
				}
				return NavigationNodeFolder.OlderExchangeCalendarType.Unknown;
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.CalendarTypeFromOlderExchange, (int)value);
			}
		}

		private string GetGSCalendarShareeLegacyDN()
		{
			return StoreEntryId.TryParseMailboxLegacyDN(this.GSCalendarShareeStoreEntryId);
		}

		private static readonly List<KeyValuePair<ExtendedFolderFlags, NavigationNodeFlags>> mappingFromFolderFlagToNodeFlag = NavigationNodeFolder.CreateMappingFromFolderFlagToNodeFlag();

		private static PropertyDefinition[] navigationNodeFolderProperties = new PropertyDefinition[]
		{
			NavigationNodeSchema.NodeEntryId,
			NavigationNodeSchema.NodeRecordKey,
			NavigationNodeSchema.StoreEntryId,
			NavigationNodeSchema.ParentGroupClassId,
			NavigationNodeSchema.GroupName,
			NavigationNodeSchema.CalendarColor,
			NavigationNodeSchema.AddressBookEntryId,
			NavigationNodeSchema.AddressBookStoreEntryId,
			NavigationNodeSchema.CalendarTypeFromOlderExchange,
			FolderSchema.AssociatedSearchFolderId,
			ItemSchema.Categories,
			ViewStateProperties.FilteredViewFrom,
			ViewStateProperties.FilteredViewTo,
			ViewStateProperties.FilteredViewFlags,
			ViewStateProperties.FilterSourceFolder
		};

		private string legacyDN;

		private string calculatedLegacyDN;

		private StoreObjectId folderId;

		private StoreObjectId calculatedFolderId;

		private int? hashCode = null;

		private bool isMailboxLegacyDNValid = true;

		private bool? isE14PrimarySharedCalendar;

		public enum OlderExchangeCalendarType
		{
			Unknown,
			Primary,
			Secondary,
			NotOlderVersion
		}
	}
}
