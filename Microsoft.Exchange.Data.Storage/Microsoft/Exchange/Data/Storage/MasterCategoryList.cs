using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MasterCategoryList : ICollection<Category>, IEnumerable<Category>, IEnumerable
	{
		public MasterCategoryList(MailboxSession session) : this()
		{
			Util.ThrowOnNullArgument(session, "session");
			this.session = session;
		}

		private MasterCategoryList(MemoryPropertyBag propertyBagToAssume)
		{
			this.propertyBag = propertyBagToAssume;
		}

		private MasterCategoryList() : this(new MemoryPropertyBag())
		{
			this.propertyBag.SetAllPropertiesLoaded();
		}

		private MasterCategoryList(MasterCategoryList copyFrom) : this(new MemoryPropertyBag(copyFrom.propertyBag))
		{
			foreach (Category category in copyFrom)
			{
				this.Add(category.Clone());
			}
		}

		public string DefaultCategoryName
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(MasterCategoryListSchema.DefaultCategory);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(MasterCategoryListSchema.DefaultCategory, value);
			}
		}

		public bool LoadedWithProblems
		{
			get
			{
				return this.loadedWithProblems;
			}
		}

		public int Count
		{
			get
			{
				return this.categories.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal bool IsLoaded
		{
			get
			{
				return this.configurationItemId != null;
			}
		}

		public Category this[string categoryName]
		{
			get
			{
				Category result;
				if (!this.categories.TryGetValue(categoryName, out result))
				{
					return null;
				}
				return result;
			}
		}

		internal Category this[Guid categoryGuid]
		{
			get
			{
				Category result;
				if (!this.categoriesByGuid.TryGetValue(categoryGuid, out result))
				{
					return null;
				}
				return result;
			}
		}

		public static IComparer<Category> CreateUsageBasedComparer(OutlookModule module)
		{
			EnumValidator.ThrowIfInvalid<OutlookModule>(module);
			return new MasterCategoryList.UsageBasedCategoryComparer(module);
		}

		public bool Contains(string categoryName)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			return this.categories.ContainsKey(categoryName);
		}

		public bool HasQuickFlagsMigrationStarted()
		{
			throw new NotImplementedException();
		}

		public bool Remove(string categoryName)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			Category category = this[categoryName];
			if (category != null)
			{
				Guid guid = category.Guid;
				category.Abandon();
				this.isListModified = true;
				return this.categories.Remove(categoryName) && this.categoriesByGuid.Remove(guid);
			}
			return false;
		}

		public void Save()
		{
			this.Save(SaveMode.ResolveConflicts);
		}

		public void Save(SaveMode saveMode)
		{
			EnumValidator.ThrowIfInvalid<SaveMode>(saveMode);
			if (!this.IsLoaded)
			{
				throw new InvalidOperationException("The Master Category List is not loaded and thus cannot be saved");
			}
			this.FlushCategoryUsageLog();
			if (!this.IsDirty())
			{
				return;
			}
			this.propertyBag[MasterCategoryListSchema.LastSavedTime] = ExDateTime.GetNow(ExTimeZone.UtcTimeZone);
			using (UserConfiguration folderConfiguration = this.session.UserConfigurationManager.GetFolderConfiguration("CategoryList", UserConfigurationTypes.XML, this.session.GetDefaultFolderId(DefaultFolderType.Calendar)))
			{
				MasterCategoryList masterCategoryList;
				using (Stream xmlStream = folderConfiguration.GetXmlStream())
				{
					masterCategoryList = ((!this.configurationItemId.Equals(folderConfiguration.VersionedId)) ? this.MergeCopies(saveMode, folderConfiguration, xmlStream) : this);
					xmlStream.Position = 0L;
					using (Stream stream = new MemoryStream((int)xmlStream.Length))
					{
						masterCategoryList.SaveToStream(saveMode, stream, xmlStream);
						if (stream.Length > 524288L)
						{
							throw new StoragePermanentException(ServerStrings.ExMclIsTooBig(stream.Length, 524288L));
						}
						stream.Position = 0L;
						xmlStream.Position = 0L;
						xmlStream.SetLength(0L);
						Util.StreamHandler.CopyStreamData(stream, xmlStream);
					}
				}
				folderConfiguration.Save();
				this.MovePersistentContent(masterCategoryList);
				this.originalMcl = this.Clone();
				this.configurationItemId = folderConfiguration.VersionedId;
				this.loadedWithProblems = false;
				this.propertyBag.ClearChangeInfo();
				this.isListModified = false;
			}
		}

		public Category[] ToArray()
		{
			return Util.CollectionToArray<Category>(this.categories.Values);
		}

		public void Add(Category item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (this.FindMatch(item) == null)
			{
				item.AssignMasterCategoryList(this);
				this.categoriesByGuid.Add(item.Guid, item);
				this.categories.Add(item.Name, item);
				this.isListModified = true;
				return;
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Category \"{0}\" is already in the list", new object[]
			{
				item.Name
			}), "item");
		}

		public void Clear()
		{
			foreach (Category category in this.categories.Values)
			{
				category.Abandon();
			}
			this.categories.Clear();
			this.categoriesByGuid.Clear();
			this.DefaultCategoryName = null;
			this.isListModified = true;
		}

		bool ICollection<Category>.Contains(Category item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return this.Contains(item.Name);
		}

		public void CopyTo(Category[] array, int arrayIndex)
		{
			this.categories.Values.CopyTo(array, arrayIndex);
		}

		bool ICollection<Category>.Remove(Category item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return this.Remove(item.Name);
		}

		public IEnumerator<Category> GetEnumerator()
		{
			return this.categories.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.categories.Values.GetEnumerator();
		}

		internal static IEnumerable<PropValue> ResolveProperties(MemoryPropertyBag client, MemoryPropertyBag server, MemoryPropertyBag original, AcrProfile profile)
		{
			ConflictResolutionResult resolutionResult = profile.ResolveConflicts(MasterCategoryList.GetPropValuesToResolve(client, server, original, profile));
			if (resolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new Exception();
			}
			foreach (PropertyConflict conflict in resolutionResult.PropertyConflicts)
			{
				yield return new PropValue(InternalSchema.ToStorePropertyDefinition(conflict.PropertyDefinition), conflict.ResolvedValue);
			}
			yield break;
		}

		internal static void Delete(MailboxSession mailboxSession)
		{
			mailboxSession.UserConfigurationManager.DeleteFolderConfigurations(mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar), new string[]
			{
				"CategoryList"
			});
		}

		internal void CategoryWasUsed(StoreId itemId, string itemClass, string categoryName)
		{
			if (this.IsLoaded && !this.Contains(categoryName))
			{
				return;
			}
			if (this.categoryUsageLogSize >= 500)
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "Category usage log size has reached its maximum of {0} entries. Cannot add this: category=\"{1}\", class=\"{2}\", id=\"{3}\"", new object[]
				{
					500,
					categoryName,
					itemClass,
					itemId
				});
				return;
			}
			StoreObjectId parentFolderId;
			if (itemId != null)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(itemId);
				parentFolderId = this.session.GetParentFolderId(storeObjectId);
			}
			else
			{
				parentFolderId = MasterCategoryList.invalidId;
			}
			MasterCategoryList.PerFolderCategoryUsageLog perFolderCategoryUsageLog;
			if (!this.categoryUsageLog.TryGetValue(parentFolderId, out perFolderCategoryUsageLog))
			{
				perFolderCategoryUsageLog = new MasterCategoryList.PerFolderCategoryUsageLog();
				this.categoryUsageLog.Add(parentFolderId, perFolderCategoryUsageLog);
			}
			MasterCategoryList.CategoryUsageRecord key = new MasterCategoryList.CategoryUsageRecord(categoryName, MasterCategoryList.GetModuleForObjectClass(itemClass));
			perFolderCategoryUsageLog.CategoryUsageRecords[key] = ExDateTime.GetNow(ExTimeZone.UtcTimeZone);
			this.categoryUsageLogSize++;
		}

		internal Category FindMatch(Category categoryToMatch)
		{
			return this[categoryToMatch.Guid] ?? this[categoryToMatch.Name];
		}

		internal void Load()
		{
			ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "MasterCategoryList::Load - NOT forcing reload.");
			this.Load(false);
		}

		internal void Load(bool forceReload)
		{
			if (this.IsLoaded && !forceReload)
			{
				return;
			}
			IReadableUserConfiguration readableUserConfiguration = null;
			this.loadedWithProblems = false;
			try
			{
				try
				{
					readableUserConfiguration = this.session.UserConfigurationManager.GetReadOnlyFolderConfiguration("CategoryList", UserConfigurationTypes.XML, this.session.GetDefaultFolderId(DefaultFolderType.Calendar));
					ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "MasterCategoryList::Load - newly loaded configurationItem");
					if (this.configurationItemId != null && this.configurationItemId.Equals(readableUserConfiguration.VersionedId))
					{
						ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "MasterCategoryList::Load - returning without reloading since this.configurationItemId.Equals(configurationItem.VersionedId.");
						return;
					}
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "MasterCategoryList::Load - ObjectNotFoundException: creating and saving a new MCL");
					try
					{
						readableUserConfiguration = this.CreateMclConfiguration();
					}
					catch (ObjectExistedException ex)
					{
						ExTraceGlobals.StorageTracer.TraceWarning<string, string>((long)this.GetHashCode(), "MclConfiguration already created. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
						readableUserConfiguration = this.session.UserConfigurationManager.GetFolderConfiguration("CategoryList", UserConfigurationTypes.XML, this.session.GetDefaultFolderId(DefaultFolderType.Calendar));
					}
				}
				using (Stream xmlStream = readableUserConfiguration.GetXmlStream())
				{
					this.Load(xmlStream);
					this.originalMcl = this.Clone();
					this.configurationItemId = readableUserConfiguration.VersionedId;
				}
				this.isListModified = false;
			}
			finally
			{
				if (readableUserConfiguration != null)
				{
					readableUserConfiguration.Dispose();
				}
			}
		}

		internal void SetProperties(IEnumerable<PropValue> propValues)
		{
			this.propertyBag.Clear();
			foreach (PropValue propValue in propValues)
			{
				((IDirectPropertyBag)this.propertyBag).SetValue(propValue.Property, propValue.Value);
			}
			this.propertyBag.SetAllPropertiesLoaded();
			this.propertyBag.ClearChangeInfo();
		}

		internal object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			return this.propertyBag.TryGetProperty(propertyDefinition);
		}

		internal bool IsDirty()
		{
			if (this.isListModified || this.propertyBag.IsDirty)
			{
				return true;
			}
			foreach (Category category in this.categories.Values)
			{
				if (category.CategoryPropertyBag.IsDirty)
				{
					return true;
				}
			}
			return false;
		}

		private static Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> GetPropValuesToResolve(MemoryPropertyBag client, MemoryPropertyBag server, MemoryPropertyBag original, AcrProfile profile)
		{
			Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> dictionary = new Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve>();
			HashSet<PropertyDefinition> propertiesNeededForResolution = profile.GetPropertiesNeededForResolution(Util.CompositeEnumerator<PropertyDefinition>(new IEnumerable<PropertyDefinition>[]
			{
				client.Keys,
				server.Keys
			}));
			foreach (PropertyDefinition propertyDefinition in propertiesNeededForResolution)
			{
				dictionary.Add(propertyDefinition, new AcrPropertyProfile.ValuesToResolve(client.TryGetProperty(propertyDefinition), server.TryGetProperty(propertyDefinition), (original != null) ? original.TryGetProperty(propertyDefinition) : null));
			}
			return dictionary;
		}

		private static OutlookModule GetModuleForObjectClass(string objectClass)
		{
			if (objectClass == null)
			{
				return OutlookModule.None;
			}
			if (ObjectClass.IsCalendarFolder(objectClass) || ObjectClass.IsCalendarItem(objectClass))
			{
				return OutlookModule.Calendar;
			}
			if (ObjectClass.IsContactsFolder(objectClass) || ObjectClass.IsContact(objectClass) || ObjectClass.IsDistributionList(objectClass))
			{
				return OutlookModule.Contacts;
			}
			if (ObjectClass.IsJournalFolder(objectClass) || ObjectClass.IsJournalItem(objectClass))
			{
				return OutlookModule.Journal;
			}
			if (ObjectClass.IsNotesFolder(objectClass) || ObjectClass.IsNotesItem(objectClass))
			{
				return OutlookModule.Notes;
			}
			if (ObjectClass.IsTaskFolder(objectClass) || ObjectClass.IsTask(objectClass))
			{
				return OutlookModule.Tasks;
			}
			if (ObjectClass.IsMessageFolder(objectClass) || ObjectClass.IsMessage(objectClass, false) || ObjectClass.IsMeetingMessage(objectClass) || ObjectClass.IsTaskRequest(objectClass) || ObjectClass.IsReport(objectClass))
			{
				return OutlookModule.Mail;
			}
			return OutlookModule.None;
		}

		private static MasterCategoryList Resolve(MasterCategoryList client, MasterCategoryList server, MasterCategoryList original)
		{
			MasterCategoryList masterCategoryList = new MasterCategoryList();
			masterCategoryList.SetProperties(MasterCategoryList.ResolveProperties(client.propertyBag, server.propertyBag, original.propertyBag, AcrProfile.MasterCategoryListProfile));
			HashSet<Category> hashSet = new HashSet<Category>(server.Count);
			Util.AddRange<Category, Category>(hashSet, server);
			foreach (Category category in client)
			{
				Category category2 = server.FindMatch(category);
				Category original2 = original.FindMatch(category);
				Category category3 = Category.Resolve(category, category2, original2);
				if (category3 != null && masterCategoryList.FindMatch(category3) == null)
				{
					masterCategoryList.Add(category3);
				}
				if (category2 != null)
				{
					hashSet.Remove(category2);
				}
			}
			foreach (Category category4 in hashSet)
			{
				Category original3 = original.FindMatch(category4);
				Category category5 = Category.Resolve(null, category4, original3);
				if (category5 != null && masterCategoryList.FindMatch(category5) == null)
				{
					masterCategoryList.Add(category5);
				}
			}
			return masterCategoryList;
		}

		private MasterCategoryList Clone()
		{
			return new MasterCategoryList(this);
		}

		private IReadableUserConfiguration CreateMclConfiguration()
		{
			IReadableUserConfiguration result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				UserConfiguration userConfiguration = this.session.UserConfigurationManager.CreateFolderConfiguration("CategoryList", UserConfigurationTypes.XML, this.session.GetDefaultFolderId(DefaultFolderType.Calendar));
				disposeGuard.Add<UserConfiguration>(userConfiguration);
				userConfiguration.Save();
				disposeGuard.Success();
				result = userConfiguration;
			}
			return result;
		}

		private void Load(Stream xmlStream)
		{
			this.Clear();
			if (xmlStream.Length == 0L)
			{
				return;
			}
			using (Stream stream = new BoundedStream(xmlStream, false, 0L, 524288L))
			{
				using (XmlReader xmlReader = XmlReader.Create(stream))
				{
					MasterCategoryListSerializer masterCategoryListSerializer = new MasterCategoryListSerializer(xmlReader);
					try
					{
						masterCategoryListSerializer.Deserialize(this);
						this.loadedWithProblems = masterCategoryListSerializer.HasFaults;
					}
					catch (CorruptDataException)
					{
						this.loadedWithProblems = true;
						if (xmlStream.Length > 524288L)
						{
							ExTraceGlobals.StorageTracer.TraceWarning<long, long>((long)this.GetHashCode(), "The size of MCL XML ({0}) has exceeded the limit of {1}. The loaded data is truncated.", xmlStream.Length, 524288L);
						}
					}
				}
			}
		}

		private void MovePersistentContent(MasterCategoryList sourceMcl)
		{
			if (this == sourceMcl)
			{
				List<Category> list = new List<Category>(this.Count);
				foreach (Category category in this)
				{
					list.Add(category.Clone());
				}
				this.Clear();
				Util.AddRange<Category, Category>(this, list);
				return;
			}
			this.Clear();
			foreach (Category category2 in sourceMcl.categories.Values)
			{
				category2.Detach();
				this.Add(category2);
			}
			sourceMcl.categories.Clear();
			this.propertyBag = sourceMcl.propertyBag;
			sourceMcl.propertyBag = new MemoryPropertyBag();
		}

		private void FlushCategoryUsageLog()
		{
			foreach (KeyValuePair<StoreObjectId, MasterCategoryList.PerFolderCategoryUsageLog> keyValuePair in this.categoryUsageLog)
			{
				if (keyValuePair.Key != MasterCategoryList.invalidId && keyValuePair.Value.OutlookModule == null)
				{
					try
					{
						using (Folder folder = Folder.Bind(this.session, keyValuePair.Key))
						{
							keyValuePair.Value.OutlookModule = new OutlookModule?(MasterCategoryList.GetModuleForObjectClass(folder.ClassName));
						}
					}
					catch (ObjectNotFoundException)
					{
						keyValuePair.Value.OutlookModule = new OutlookModule?(OutlookModule.None);
					}
				}
			}
			foreach (KeyValuePair<StoreObjectId, MasterCategoryList.PerFolderCategoryUsageLog> keyValuePair2 in this.categoryUsageLog)
			{
				foreach (KeyValuePair<MasterCategoryList.CategoryUsageRecord, ExDateTime> keyValuePair3 in keyValuePair2.Value.CategoryUsageRecords)
				{
					Category category = this[keyValuePair3.Key.CategoryName];
					if (category != null)
					{
						category.UpdateLastTimeUsed(keyValuePair3.Value, (keyValuePair2.Value.OutlookModule != null && keyValuePair2.Value.OutlookModule != OutlookModule.None) ? keyValuePair2.Value.OutlookModule : new OutlookModule?(keyValuePair3.Key.ModuleForItem));
					}
				}
				keyValuePair2.Value.CategoryUsageRecords.Clear();
			}
		}

		private MasterCategoryList MergeCopies(SaveMode saveMode, UserConfiguration mclConfigurationItem, Stream mclXmlStream)
		{
			MasterCategoryList masterCategoryList = new MasterCategoryList();
			switch (saveMode)
			{
			case SaveMode.ResolveConflicts:
				try
				{
					using (Stream stream = new BoundedStream(mclXmlStream, false, 0L, 524288L))
					{
						using (XmlReader xmlReader = XmlReader.Create(stream))
						{
							new MasterCategoryListSerializer(xmlReader).Deserialize(masterCategoryList);
						}
					}
				}
				catch (CorruptDataException ex)
				{
					Exception ex2 = new SaveConflictException(ServerStrings.ExMclCannotBeResolved, ex);
					ExTraceGlobals.StorageTracer.TraceDebug<SaveMode, string>((long)this.GetHashCode(), "Failed to load the Server copy of MCL for conflict resolution (SaveMode={0}): {1}", saveMode, ex.Message);
					throw ex2;
				}
				return MasterCategoryList.Resolve(this, masterCategoryList, this.originalMcl);
			case SaveMode.FailOnAnyConflict:
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(mclConfigurationItem.Id), null);
			case SaveMode.NoConflictResolution:
				return this;
			default:
				throw new ArgumentOutOfRangeException("saveMode");
			}
		}

		private void SaveToStream(SaveMode saveMode, Stream destination, Stream serverCopy)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = false;
			if (saveMode != SaveMode.NoConflictResolution && serverCopy.Length != 0L)
			{
				try
				{
					using (Stream stream = new BoundedStream(serverCopy, false, 0L, 524288L))
					{
						using (StreamReader streamReader = new StreamReader(stream, true))
						{
							using (XmlReader xmlReader = XmlReader.Create(streamReader))
							{
								xmlWriterSettings.Encoding = (streamReader.CurrentEncoding ?? Encoding.UTF8);
								using (XmlWriter xmlTextWriter = Util.GetXmlTextWriter(destination, xmlWriterSettings))
								{
									new MasterCategoryListSerializer(xmlReader).SerializeUsingSource(this, xmlTextWriter);
									return;
								}
							}
						}
					}
				}
				catch (CorruptDataException inner)
				{
					if (saveMode == SaveMode.FailOnAnyConflict)
					{
						throw new SaveConflictException(ServerStrings.ExMclCannotBeResolved, inner);
					}
				}
			}
			destination.Position = 0L;
			destination.SetLength(0L);
			using (XmlWriter xmlWriter = XmlWriter.Create(destination, xmlWriterSettings))
			{
				MasterCategoryListSerializer.Serialize(this, xmlWriter);
			}
		}

		internal const string MclConfigurationName = "CategoryList";

		private const int MaxCategoryUsageLogSize = 500;

		private const long MaxMclXmlSize = 524288L;

		private static readonly StoreObjectId invalidId = StoreObjectId.FromProviderSpecificId(Array<byte>.Empty, StoreObjectType.Unknown);

		private readonly Dictionary<string, Category> categories = new Dictionary<string, Category>(Category.NameComparer);

		private readonly Dictionary<Guid, Category> categoriesByGuid = new Dictionary<Guid, Category>();

		private readonly Dictionary<StoreObjectId, MasterCategoryList.PerFolderCategoryUsageLog> categoryUsageLog = new Dictionary<StoreObjectId, MasterCategoryList.PerFolderCategoryUsageLog>();

		private readonly MailboxSession session;

		private VersionedId configurationItemId;

		private bool loadedWithProblems;

		private MasterCategoryList originalMcl;

		private MemoryPropertyBag propertyBag;

		private int categoryUsageLogSize;

		private bool isListModified;

		private struct CategoryUsageRecord : IEquatable<MasterCategoryList.CategoryUsageRecord>
		{
			public CategoryUsageRecord(string categoryName, OutlookModule moduleForItem)
			{
				this.CategoryName = categoryName;
				this.ModuleForItem = moduleForItem;
			}

			public override bool Equals(object obj)
			{
				return obj != null && this.Equals((MasterCategoryList.CategoryUsageRecord)obj);
			}

			public override int GetHashCode()
			{
				return this.CategoryName.GetHashCode() ^ (int)this.ModuleForItem;
			}

			public bool Equals(MasterCategoryList.CategoryUsageRecord other)
			{
				return this.ModuleForItem == other.ModuleForItem && this.CategoryName.Equals(other.CategoryName);
			}

			public readonly string CategoryName;

			public readonly OutlookModule ModuleForItem;
		}

		private sealed class UsageBasedCategoryComparer : IComparer<Category>
		{
			public UsageBasedCategoryComparer(OutlookModule module)
			{
				this.module = module;
			}

			public int Compare(Category x, Category y)
			{
				int num = -x.LastTimeUsed[this.module].CompareTo(y.LastTimeUsed[this.module]);
				if (num == 0)
				{
					num = Category.NameComparer.Compare(x.Name, y.Name);
				}
				return num;
			}

			private readonly OutlookModule module;
		}

		private sealed class PerFolderCategoryUsageLog
		{
			public Dictionary<MasterCategoryList.CategoryUsageRecord, ExDateTime> CategoryUsageRecords
			{
				get
				{
					return this.categoryUsageRecords;
				}
			}

			public OutlookModule? OutlookModule
			{
				get
				{
					return this.outlookModule;
				}
				set
				{
					this.outlookModule = value;
				}
			}

			private Dictionary<MasterCategoryList.CategoryUsageRecord, ExDateTime> categoryUsageRecords = new Dictionary<MasterCategoryList.CategoryUsageRecord, ExDateTime>();

			private OutlookModule? outlookModule;
		}
	}
}
