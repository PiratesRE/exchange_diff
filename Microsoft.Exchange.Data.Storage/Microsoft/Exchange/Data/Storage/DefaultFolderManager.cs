using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DefaultFolderManager
	{
		private DefaultFolderManager(MailboxSession session)
		{
			this.session = session;
			this.defaultFolders = new DefaultFolder[DefaultFolderInfo.DefaultFolderTypeCount];
		}

		internal MailboxSession Session
		{
			get
			{
				return this.session;
			}
		}

		internal DefaultFolder[] DefaultFolders
		{
			get
			{
				return this.defaultFolders;
			}
		}

		internal static DefaultFolderManager Create(MailboxSession session)
		{
			return DefaultFolderManager.Create(session, OpenMailboxSessionFlags.None, MailboxSession.AllDefaultFolders);
		}

		internal static DefaultFolderManager Create(MailboxSession session, OpenMailboxSessionFlags openFlags, IList<DefaultFolderType> foldersToInit)
		{
			DefaultFolderManager defaultFolderManager = new DefaultFolderManager(session);
			DefaultFolderContext defaultFolderContext = new DefaultFolderContext(session, defaultFolderManager.defaultFolders);
			defaultFolderManager.context = defaultFolderContext;
			if ((openFlags & OpenMailboxSessionFlags.DeferDefaultFolderIdInitialization) == OpenMailboxSessionFlags.DeferDefaultFolderIdInitialization)
			{
				defaultFolderContext.DeferFolderIdInit = true;
			}
			if ((openFlags & OpenMailboxSessionFlags.IgnoreForcedFolderInit) == OpenMailboxSessionFlags.IgnoreForcedFolderInit)
			{
				defaultFolderContext.IgnoreForcedFolderInit = true;
			}
			if (defaultFolderContext.Session.SharedDataManager.DefaultFoldersInitialized)
			{
				defaultFolderManager.CacheDefaultFoldersFromSharedDataManager(defaultFolderContext);
				defaultFolderContext.DoneDefaultFolderInitialization();
				return defaultFolderManager;
			}
			try
			{
				if ((openFlags & OpenMailboxSessionFlags.SuppressFolderIdPrefetch) == OpenMailboxSessionFlags.None)
				{
					defaultFolderContext.FolderDataDictionary = defaultFolderManager.ReadFolderData();
				}
				else if (session.Capabilities.CanCreateDefaultFolders)
				{
					string inboxDisplayName = null;
					CultureInfo stampedCulture = session.Capabilities.CanHaveCulture ? session.PreferedCulture : CultureInfo.InvariantCulture;
					if (session.LogonType != LogonType.Delegated)
					{
						object thisObject = null;
						bool flag = false;
						byte[] inboxFolderEntryId;
						try
						{
							if (session != null)
							{
								session.BeginMapiCall();
								session.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							inboxFolderEntryId = session.Mailbox.MapiStore.GetInboxFolderEntryId();
						}
						catch (MapiPermanentException ex)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenFolder, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("DefaultFolderManager::GetInboxId. Hit exception when adding ``free'' default folders.", new object[0]),
								ex
							});
						}
						catch (MapiRetryableException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenFolder, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("DefaultFolderManager::GetInboxId. Hit exception when adding ``free'' default folders.", new object[0]),
								ex2
							});
						}
						finally
						{
							try
							{
								if (session != null)
								{
									session.EndMapiCall();
									if (flag)
									{
										session.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
						if (IdConverter.IsFolderId(inboxFolderEntryId))
						{
							StoreObjectId inboxId = StoreObjectId.FromProviderSpecificId(inboxFolderEntryId);
							DefaultFolderManager.TryGetDefaultFolderCulture(session, inboxId, out stampedCulture, out inboxDisplayName);
						}
					}
					defaultFolderContext.Session.SharedDataManager.DefaultFoldersCulture = defaultFolderManager.GetBestCulture(stampedCulture, inboxDisplayName, session);
				}
				else
				{
					defaultFolderContext.Session.SharedDataManager.DefaultFoldersCulture = CultureInfo.InvariantCulture;
				}
				defaultFolderManager.CacheDefaultFolders(defaultFolderContext, foldersToInit);
				defaultFolderManager.ValidateDefaultFolderSet(defaultFolderContext);
			}
			finally
			{
				defaultFolderContext.DoneDefaultFolderInitialization();
			}
			return defaultFolderManager;
		}

		private void ValidateDefaultFolderSet(DefaultFolderContext context)
		{
			this.VerifyFromFavoriteSendersFolderEntryId(context);
		}

		private void VerifyFromFavoriteSendersFolderEntryId(DefaultFolderContext context)
		{
			CultureInfo defaultFoldersCulture = context.Session.SharedDataManager.DefaultFoldersCulture;
			DefaultFolder defaultFolder = this.DefaultFolders[54];
			if (defaultFolder == null || defaultFolder.FolderId == null)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug((long)this.GetHashCode(), "DefaultFolderManager::VerifyFromFavoriteSendersFolderEntryId. MyContacts folder information or the folder id is null. Returning.");
				return;
			}
			DefaultFolder defaultFolder2 = this.DefaultFolders[63];
			if (defaultFolder2 == null || defaultFolder2.FolderId == null)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug((long)this.GetHashCode(), "DefaultFolderManager::VerifyFromFavoriteSendersFolderEntryId. FromFavoriteSenders folder or the folder id is null. Returning.");
				return;
			}
			if (!defaultFolder.FolderId.Equals(defaultFolder2.FolderId))
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug((long)this.GetHashCode(), "DefaultFolderManager::VerifyFromFavoriteSendersFolderEntryId. FromFavoriteSenders folder id doesn't match MyContacts folder id. Returning.");
				return;
			}
			string localizableDisplayName = DefaultFolderManager.GetLocalizableDisplayName(DefaultFolderType.MyContacts, defaultFoldersCulture);
			string localizableDisplayName2 = DefaultFolderManager.GetLocalizableDisplayName(DefaultFolderType.FromFavoriteSenders, defaultFoldersCulture);
			if (localizableDisplayName.Equals(localizableDisplayName2, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string, CultureInfo>((long)this.GetHashCode(), "DefaultFolderManager::VerifyFromFavoriteSendersFolderEntryId. FromFavoriteSenders display name matches MyContacts display name '{0}' in culture {1}. Returning.", localizableDisplayName, defaultFoldersCulture);
				return;
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug((long)this.GetHashCode(), "DefaultFolderManager::VerifyFromFavoriteSendersFolderEntryId. Deleting and recreating FromFavoriteSenders and MyContacts folders to fix folder id conflict.");
			defaultFolder.RemoveForRecover();
			defaultFolder2.RemoveForRecover();
			DefaultFolderType[] foldersToInit = new DefaultFolderType[]
			{
				DefaultFolderType.MyContacts,
				DefaultFolderType.FromFavoriteSenders
			};
			this.CacheDefaultFolders(context, foldersToInit);
		}

		internal static string GetLocalizableDisplayName(DefaultFolderType defaultFolderType, CultureInfo cultureInfo)
		{
			Util.ThrowOnNullArgument(cultureInfo, "cultureInfo");
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType);
			return DefaultFolderInfo.Instance[(int)defaultFolderType].LocalizableDisplayName.ToString(cultureInfo);
		}

		internal static bool TryGetDefaultFolderCulture(MailboxSession session, StoreObjectId inboxId, out CultureInfo defaultFolderCulture, out string inboxDisplayName)
		{
			defaultFolderCulture = null;
			inboxDisplayName = null;
			if (session.LogonType == LogonType.Delegated)
			{
				return false;
			}
			try
			{
				using (Folder folder = Folder.Bind(session, inboxId, new PropertyDefinition[]
				{
					FolderSchema.DefaultFoldersLocaleId
				}))
				{
					inboxDisplayName = folder.DisplayName;
					int? num = folder.TryGetProperty(FolderSchema.DefaultFoldersLocaleId) as int?;
					if (num != null)
					{
						defaultFolderCulture = LocaleMap.GetCultureFromLcid(num.Value);
						return true;
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceError(0L, "DefaultFolderManager::TryGetDefaultFolderCulture. The Inbox is inaccessible.");
			}
			return false;
		}

		private void StampDefaultFolderCulture()
		{
			CultureInfo defaultFoldersCulture = this.session.SharedDataManager.DefaultFoldersCulture;
			StoreObjectId folderId;
			if (this.defaultFolders[5].TryGetFolderId(out folderId))
			{
				using (Folder folder = Folder.Bind(this.session, folderId, new PropertyDefinition[]
				{
					FolderSchema.DefaultFoldersLocaleId
				}))
				{
					folder.SetProperties(new StorePropertyDefinition[]
					{
						FolderSchema.DefaultFoldersLocaleId
					}, new object[]
					{
						LocaleMap.GetLcidFromCulture(defaultFoldersCulture)
					});
					folder.Save();
				}
			}
		}

		internal DefaultFolder GetDefaultFolder(DefaultFolderType defaultFolderType)
		{
			return this.defaultFolders[(int)defaultFolderType];
		}

		internal DefaultFolderType IsDefaultFolderType(StoreId folderId)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(folderId);
			for (int i = 0; i < DefaultFolderInfo.DefaultFolderTypeCount; i++)
			{
				StoreObjectId folderId2 = this.defaultFolders[i].FolderId;
				if (!this.defaultFolders[i].IsIdInitialized)
				{
					this.defaultFolders[i].InitializeFolderId();
					folderId2 = this.defaultFolders[i].FolderId;
				}
				if (folderId2 != null && storeObjectId.Equals(folderId2))
				{
					return (DefaultFolderType)i;
				}
			}
			return DefaultFolderType.None;
		}

		internal StoreObjectId GetSystemFolderId()
		{
			return this.GetDefaultFolderId(DefaultFolderType.System);
		}

		internal StoreObjectId GetDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			StoreObjectId result;
			if (!this.defaultFolders[(int)defaultFolderType].TryGetFolderId(out result))
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolderType, LogonType>((long)this.GetHashCode(), "DefaultFolderManager::GetDefaultFolderId. We have no permission to get to the defaultFolder. defaultFolder = {0}, logonType = {1}.", defaultFolderType, this.Session.LogonType);
			}
			return result;
		}

		internal StoreObjectId CreateDefaultSystemFolder()
		{
			DefaultFolder defaultFolder = this.GetDefaultFolder(DefaultFolderType.System);
			defaultFolder.Create();
			StoreObjectId result;
			if (!defaultFolder.TryGetFolderId(out result))
			{
				throw new AccessDeniedException(ServerStrings.DefaultFolderAccessDenied(defaultFolder.ToString()));
			}
			return result;
		}

		internal StoreObjectId CreateDefaultFolder(DefaultFolderType defaultFolderType)
		{
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, DefaultFolderManager.validFolderTypesForCreateDefaultFolder);
			DefaultFolder defaultFolder = this.GetDefaultFolder(defaultFolderType);
			defaultFolder.Create();
			StoreObjectId result;
			if (!defaultFolder.TryGetFolderId(out result))
			{
				throw new AccessDeniedException(ServerStrings.DefaultFolderAccessDenied(defaultFolder.ToString()));
			}
			return result;
		}

		internal bool TryFixDefaultFolderId(DefaultFolderType defaultFolderType, out StoreObjectId id)
		{
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType);
			DefaultFolder defaultFolder = this.GetDefaultFolder(defaultFolderType);
			try
			{
				defaultFolder.RemoveForRecover();
				defaultFolder.Create();
			}
			catch (ObjectNotFoundException)
			{
				id = null;
				return false;
			}
			catch (NotSupportedException)
			{
				id = null;
				return false;
			}
			return defaultFolder.TryGetFolderId(out id);
		}

		internal StoreObjectId RefreshDefaultFolder(DefaultFolderType defaultFolderType)
		{
			DefaultFolder defaultFolder = this.GetDefaultFolder(defaultFolderType);
			defaultFolder.Refresh();
			StoreObjectId result;
			defaultFolder.TryGetFolderId(out result);
			return result;
		}

		internal void DeleteDefaultFolder(DefaultFolderType defaultFolderType, DeleteItemFlags deleteItemFlags)
		{
			this.GetDefaultFolder(defaultFolderType).Delete(deleteItemFlags);
		}

		internal bool VerifyLocalization()
		{
			foreach (DefaultFolder defaultFolder in this.defaultFolders)
			{
				if (!defaultFolder.IsLocalized)
				{
					return false;
				}
			}
			return true;
		}

		internal OperationResult Localize(out Exception[] problems)
		{
			if (this.session.Capabilities.CanHaveCulture && this.session.PreferedCulture != this.session.SharedDataManager.DefaultFoldersCulture)
			{
				DefaultFolderContext defaultFolderContext = new DefaultFolderContext(this.session, this.defaultFolders);
				defaultFolderContext.Session.SharedDataManager.DefaultFoldersCulture = this.session.PreferedCulture;
				defaultFolderContext.DeferFolderIdInit = this.context.DeferFolderIdInit;
				defaultFolderContext.IgnoreForcedFolderInit = this.context.IgnoreForcedFolderInit;
				this.CacheDefaultFoldersFromSharedDataManager(defaultFolderContext);
				defaultFolderContext.DoneDefaultFolderInitialization();
				this.StampDefaultFolderCulture();
				this.context = defaultFolderContext;
			}
			return this.InternalLocalize(out problems);
		}

		private OperationResult InternalLocalize(out Exception[] problems)
		{
			int num = 0;
			List<Exception> list = new List<Exception>();
			foreach (DefaultFolder defaultFolder in this.defaultFolders)
			{
				try
				{
					PropertyError propertyError;
					if (defaultFolder.Localize(out propertyError) && propertyError != null)
					{
						list.Add(PropertyError.ToException(new PropertyError[]
						{
							propertyError
						}));
						num++;
					}
				}
				catch (StoragePermanentException item)
				{
					list.Add(item);
				}
				catch (StorageTransientException item2)
				{
					list.Add(item2);
				}
			}
			OperationResult result = OperationResult.Succeeded;
			if (list.Count > 0 && num > 0)
			{
				result = OperationResult.Failed;
			}
			else if (list.Count > 0)
			{
				result = OperationResult.PartiallySucceeded;
			}
			problems = list.ToArray();
			return result;
		}

		private Dictionary<string, DefaultFolderManager.FolderData> ReadFolderData()
		{
			MapiFolder mapiFolder = null;
			StoreSession storeSession = this.Session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiFolder = this.session.Mailbox.MapiStore.GetNonIpmSubtreeFolder();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotReadFolderData, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("DefaultFolderManager::ReadFolderData.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotReadFolderData, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("DefaultFolderManager::ReadFolderData.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			Dictionary<string, DefaultFolderManager.FolderData> result;
			using (mapiFolder)
			{
				HierarchyTableFlags flags = HierarchyTableFlags.ConvenientDepth;
				MapiTable mapiTable = null;
				StoreSession storeSession2 = this.Session;
				bool flag2 = false;
				byte[] inboxFolderEntryId;
				try
				{
					if (storeSession2 != null)
					{
						storeSession2.BeginMapiCall();
						storeSession2.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiTable = mapiFolder.GetHierarchyTable(flags);
					inboxFolderEntryId = this.Session.Mailbox.MapiStore.GetInboxFolderEntryId();
				}
				catch (MapiPermanentException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotReadFolderData, ex3, storeSession2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("DefaultFolderManager::ReadFolderData.", new object[0]),
						ex3
					});
				}
				catch (MapiRetryableException ex4)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotReadFolderData, ex4, storeSession2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("DefaultFolderManager::ReadFolderData.", new object[0]),
						ex4
					});
				}
				finally
				{
					try
					{
						if (storeSession2 != null)
						{
							storeSession2.EndMapiCall();
							if (flag2)
							{
								storeSession2.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				using (mapiTable)
				{
					PropTag[] columns = new PropTag[]
					{
						(PropTag)InternalSchema.EntryId.PropertyTag,
						(PropTag)InternalSchema.DisplayName.PropertyTag,
						(PropTag)InternalSchema.ContainerClass.PropertyTag,
						(PropTag)InternalSchema.ParentEntryId.PropertyTag,
						(PropTag)InternalSchema.MapiFolderType.PropertyTag,
						(PropTag)InternalSchema.AdminFolderFlags.PropertyTag,
						(PropTag)InternalSchema.IsHidden.PropertyTag,
						(PropTag)InternalSchema.SystemFolderFlags.PropertyTag,
						(PropTag)InternalSchema.ExtendedFolderFlagsInternal.PropertyTag,
						(PropTag)InternalSchema.DefaultFoldersLocaleId.PropertyTag
					};
					PropValue[][] array = null;
					StoreSession storeSession3 = this.Session;
					bool flag3 = false;
					try
					{
						if (storeSession3 != null)
						{
							storeSession3.BeginMapiCall();
							storeSession3.BeginServerHealthCall();
							flag3 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						mapiTable.SetColumns(columns);
						array = mapiTable.QueryRows(StorageLimits.Instance.DefaultFolderDataCacheMaxRowCount);
					}
					catch (MapiPermanentException ex5)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotReadFolderData, ex5, storeSession3, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("DefaultFolderManager::ReadFolderData.", new object[0]),
							ex5
						});
					}
					catch (MapiRetryableException ex6)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotReadFolderData, ex6, storeSession3, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("DefaultFolderManager::ReadFolderData.", new object[0]),
							ex6
						});
					}
					finally
					{
						try
						{
							if (storeSession3 != null)
							{
								storeSession3.EndMapiCall();
								if (flag3)
								{
									storeSession3.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
					if (array.Length == StorageLimits.Instance.DefaultFolderDataCacheMaxRowCount)
					{
						ExTraceGlobals.DefaultFoldersTracer.TraceDebug<int>((long)this.GetHashCode(), "DefaultFolderManager::ReadFolderData. There are too many folders under IpmSubtree. Some of default folders may have been ignored. Query Rows = {0}.", array.Length);
					}
					string inboxDisplayName = null;
					CultureInfo stampedCulture = null;
					Dictionary<string, DefaultFolderManager.FolderData> dictionary = new Dictionary<string, DefaultFolderManager.FolderData>();
					foreach (PropValue[] array3 in array)
					{
						byte[] array4 = array3[0].IsError() ? null : array3[0].GetBytes();
						string text = array3[1].IsError() ? null : array3[1].GetString();
						string containerClass = array3[2].IsError() ? null : array3[2].GetString();
						byte[] parentEntryId = array3[3].IsError() ? null : array3[3].GetBytes();
						FolderType? arg = array3[4].IsError() ? null : new FolderType?((FolderType)array3[4].GetInt());
						ELCFolderFlags? elcFolderFlags = array3[5].IsError() ? null : new ELCFolderFlags?((ELCFolderFlags)array3[5].GetInt());
						bool isHidden = !array3[6].IsError() && array3[6].GetBoolean();
						SystemFolderFlags? systemFolderFlags = array3[7].IsError() ? null : new SystemFolderFlags?((SystemFolderFlags)array3[7].GetInt());
						byte[] extendedFolderFlags = array3[8].IsError() ? null : array3[8].GetBytes();
						if (array4 != null && text != null && arg != null)
						{
							string key = Convert.ToBase64String(array4);
							if (dictionary.ContainsKey(key))
							{
								ExTraceGlobals.DefaultFoldersTracer.TraceError<string, string, FolderType?>((long)this.GetHashCode(), "DefaultFolderManager::ReadFolderData. Store returns duplicate entries from hierarch query. Mailbox = {0}. displayName = {1}, folderType = {2}.", this.Session.DisplayName, text, arg);
							}
							else
							{
								dictionary.Add(key, new DefaultFolderManager.FolderData(text, arg.Value, containerClass, parentEntryId, elcFolderFlags, isHidden, systemFolderFlags, extendedFolderFlags));
								if (array4.SequenceEqual(inboxFolderEntryId))
								{
									int? num = new int?(array3[9].GetInt());
									if (num != null && num.Value >= 0)
									{
										stampedCulture = LocaleMap.GetCultureFromLcid(num.Value);
									}
								}
							}
						}
					}
					this.session.SharedDataManager.DefaultFoldersCulture = this.GetBestCulture(stampedCulture, inboxDisplayName, this.session);
					result = dictionary;
				}
			}
			return result;
		}

		private void CacheDefaultFolders(DefaultFolderContext context, IList<DefaultFolderType> foldersToInit)
		{
			CultureInfo defaultFoldersCulture = context.Session.SharedDataManager.DefaultFoldersCulture;
			foreach (DefaultFolderType defaultFolderType in DefaultFolderManager.defaultFolderInitializationOrder)
			{
				bool deferInitialize = !Util.Contains(foldersToInit, defaultFolderType);
				bool forceInitialize = !context.IgnoreForcedFolderInit && MailboxSession.DefaultFoldersToForceInit != null && MailboxSession.DefaultFoldersToForceInit.Contains(defaultFolderType);
				this.defaultFolders[(int)defaultFolderType] = new DefaultFolder(context, DefaultFolderInfo.Instance[(int)defaultFolderType], defaultFoldersCulture, this.session.SharedDataManager, defaultFolderType, deferInitialize, forceInitialize);
			}
		}

		private void CacheDefaultFoldersFromSharedDataManager(DefaultFolderContext context)
		{
			CultureInfo defaultFoldersCulture = context.Session.SharedDataManager.DefaultFoldersCulture;
			foreach (DefaultFolderType defaultFolderType in DefaultFolderManager.defaultFolderInitializationOrder)
			{
				this.defaultFolders[(int)defaultFolderType] = new DefaultFolder(context, DefaultFolderInfo.Instance[(int)defaultFolderType], defaultFoldersCulture, this.session.SharedDataManager, defaultFolderType, false, false);
			}
		}

		internal CultureInfo GetBestCulture(CultureInfo stampedCulture, string inboxDisplayName, MailboxSession session)
		{
			List<CultureInfo> list = new List<CultureInfo>();
			if (stampedCulture != null)
			{
				list.Add(stampedCulture);
			}
			if (inboxDisplayName != null)
			{
				CultureInfo cultureInfo = session.MailboxOwner.PreferredCultures.FirstOrDefault<CultureInfo>();
				if (cultureInfo != null)
				{
					string localizableDisplayName = DefaultFolderManager.GetLocalizableDisplayName(DefaultFolderType.Inbox, cultureInfo);
					if (string.Equals(localizableDisplayName, inboxDisplayName, StringComparison.OrdinalIgnoreCase))
					{
						list.Add(cultureInfo);
					}
				}
			}
			list.Add(session.InternalPreferedCulture);
			return Util.CultureSelector.GetPreferedCulture(list.ToArray());
		}

		internal static readonly DefaultFolderType[] defaultFolderInitializationOrder = new DefaultFolderType[]
		{
			DefaultFolderType.None,
			DefaultFolderType.Root,
			DefaultFolderType.Configuration,
			DefaultFolderType.Inbox,
			DefaultFolderType.Outbox,
			DefaultFolderType.SentItems,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.SearchFolders,
			DefaultFolderType.CommonViews,
			DefaultFolderType.DeferredActionFolder,
			DefaultFolderType.LegacySpoolerQueue,
			DefaultFolderType.LegacySchedule,
			DefaultFolderType.LegacyShortcuts,
			DefaultFolderType.LegacyViews,
			DefaultFolderType.Calendar,
			DefaultFolderType.Contacts,
			DefaultFolderType.Drafts,
			DefaultFolderType.Tasks,
			DefaultFolderType.Journal,
			DefaultFolderType.Notes,
			DefaultFolderType.CommunicatorHistory,
			DefaultFolderType.ElcRoot,
			DefaultFolderType.SyncRoot,
			DefaultFolderType.Reminders,
			DefaultFolderType.UMVoicemail,
			DefaultFolderType.UMFax,
			DefaultFolderType.Sharing,
			DefaultFolderType.FreeBusyData,
			DefaultFolderType.AllItems,
			DefaultFolderType.RecoverableItemsRoot,
			DefaultFolderType.RecoverableItemsDeletions,
			DefaultFolderType.RecoverableItemsVersions,
			DefaultFolderType.RecoverableItemsPurges,
			DefaultFolderType.RecoverableItemsDiscoveryHolds,
			DefaultFolderType.RecoverableItemsMigratedMessages,
			DefaultFolderType.CalendarLogging,
			DefaultFolderType.Audits,
			DefaultFolderType.System,
			DefaultFolderType.CalendarVersionStore,
			DefaultFolderType.AdminAuditLogs,
			DefaultFolderType.Location,
			DefaultFolderType.AllContacts,
			DefaultFolderType.PeopleConnect,
			DefaultFolderType.LegacyArchiveJournals,
			DefaultFolderType.DocumentSyncIssues,
			DefaultFolderType.MailboxAssociation,
			DefaultFolderType.ToDoSearch,
			DefaultFolderType.RssSubscription,
			DefaultFolderType.Conflicts,
			DefaultFolderType.SyncIssues,
			DefaultFolderType.LocalFailures,
			DefaultFolderType.ServerFailures,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.ConversationActions,
			DefaultFolderType.RecipientCache,
			DefaultFolderType.QuickContacts,
			DefaultFolderType.ImContactList,
			DefaultFolderType.OrganizationalContacts,
			DefaultFolderType.PushNotificationRoot,
			DefaultFolderType.GroupNotifications,
			DefaultFolderType.MyContacts,
			DefaultFolderType.MyContactsExtended,
			DefaultFolderType.Favorites,
			DefaultFolderType.FromFavoriteSenders,
			DefaultFolderType.OutlookService,
			DefaultFolderType.GalContacts,
			DefaultFolderType.UserActivity,
			DefaultFolderType.WorkingSet,
			DefaultFolderType.Clutter,
			DefaultFolderType.ParkedMessages,
			DefaultFolderType.UnifiedInbox,
			DefaultFolderType.TemporarySaves,
			DefaultFolderType.BirthdayCalendar,
			DefaultFolderType.FromPeople,
			DefaultFolderType.SnackyApps,
			DefaultFolderType.SmsAndChatsSync
		};

		private readonly MailboxSession session;

		private static DefaultFolderType[] validFolderTypesForCreateDefaultFolder = new DefaultFolderType[]
		{
			DefaultFolderType.ElcRoot,
			DefaultFolderType.SyncRoot,
			DefaultFolderType.UMVoicemail,
			DefaultFolderType.UMFax,
			DefaultFolderType.AllItems,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.Drafts,
			DefaultFolderType.Tasks,
			DefaultFolderType.ConversationActions,
			DefaultFolderType.FreeBusyData,
			DefaultFolderType.RecoverableItemsRoot,
			DefaultFolderType.RecoverableItemsDeletions,
			DefaultFolderType.RecoverableItemsVersions,
			DefaultFolderType.RecoverableItemsPurges,
			DefaultFolderType.RecoverableItemsDiscoveryHolds,
			DefaultFolderType.RecoverableItemsMigratedMessages,
			DefaultFolderType.CalendarLogging,
			DefaultFolderType.CalendarVersionStore,
			DefaultFolderType.AdminAuditLogs,
			DefaultFolderType.Audits,
			DefaultFolderType.RecipientCache,
			DefaultFolderType.QuickContacts,
			DefaultFolderType.ImContactList,
			DefaultFolderType.CommunicatorHistory,
			DefaultFolderType.LegacyArchiveJournals,
			DefaultFolderType.OrganizationalContacts,
			DefaultFolderType.ToDoSearch,
			DefaultFolderType.DocumentSyncIssues,
			DefaultFolderType.MyContacts,
			DefaultFolderType.MyContactsExtended,
			DefaultFolderType.AllContacts,
			DefaultFolderType.PushNotificationRoot,
			DefaultFolderType.Contacts,
			DefaultFolderType.GroupNotifications,
			DefaultFolderType.MailboxAssociation,
			DefaultFolderType.OutlookService,
			DefaultFolderType.MailboxAssociation,
			DefaultFolderType.Location,
			DefaultFolderType.Sharing,
			DefaultFolderType.PeopleConnect,
			DefaultFolderType.Favorites,
			DefaultFolderType.FromFavoriteSenders,
			DefaultFolderType.Reminders,
			DefaultFolderType.GalContacts,
			DefaultFolderType.UserActivity,
			DefaultFolderType.WorkingSet,
			DefaultFolderType.Clutter,
			DefaultFolderType.ParkedMessages,
			DefaultFolderType.UnifiedInbox,
			DefaultFolderType.TemporarySaves,
			DefaultFolderType.BirthdayCalendar,
			DefaultFolderType.FromPeople,
			DefaultFolderType.SnackyApps,
			DefaultFolderType.SmsAndChatsSync
		};

		private DefaultFolder[] defaultFolders;

		private DefaultFolderContext context;

		internal class FolderData
		{
			internal FolderData(string displayName, FolderType folderType, string containerClass, byte[] parentEntryId, ELCFolderFlags? elcFolderFlags, bool isHidden, SystemFolderFlags? systemFolderFlags, byte[] extendedFolderFlags)
			{
				object[] array = new object[]
				{
					displayName,
					DefaultFolderManager.FolderData.BoxedFolderType(folderType),
					DefaultFolderManager.FolderData.InternedContainerClass(containerClass),
					parentEntryId,
					elcFolderFlags,
					isHidden ? BoxedConstants.True : BoxedConstants.False,
					systemFolderFlags,
					extendedFolderFlags
				};
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null)
					{
						array[i] = DefaultFolderManager.FolderData.folderDataProperties[i].GetNotFoundError();
					}
				}
				this.PropertyBag = new ReadonlyMemoryPropertyBag(DefaultFolderManager.FolderData.folderDataProperties, array);
			}

			private static object BoxedFolderType(FolderType type)
			{
				switch (type)
				{
				case FolderType.Root:
					return DefaultFolderManager.FolderData.FolderTypeRoot;
				case FolderType.Generic:
					return DefaultFolderManager.FolderData.FolderTypeGeneric;
				case FolderType.Search:
					return DefaultFolderManager.FolderData.FolderTypeSearch;
				default:
					return type;
				}
			}

			private static object InternedContainerClass(string containerClass)
			{
				switch (containerClass)
				{
				case "IPF.Note":
					return "IPF.Note";
				case "IPF.Appointment":
					return "IPF.Appointment";
				case "IPF.Contact":
					return "IPF.Contact";
				case "IPF.Task":
					return "IPF.Task";
				case "IPF.Journal":
					return "IPF.Journal";
				case "IPF.StickyNote":
					return "IPF.StickyNote";
				case "IPF.Configuration":
					return "IPF.Configuration";
				case "IPF":
					return "IPF";
				case "IPF.Contact.MOC.QuickContacts":
					return "IPF.Contact.MOC.QuickContacts";
				case "IPF.Contact.RecipientCache":
					return "IPF.Contact.RecipientCache";
				case "IPF.Note.OutlookHomepage":
					return "IPF.Note.OutlookHomepage";
				case "IPF.Contact.MOC.ImContactList":
					return "IPF.Contact.MOC.ImContactList";
				case "IPF.Note.Microsoft.Conversation":
					return "IPF.Note.Microsoft.Conversation";
				case "Outlook.Reminder":
					return "Outlook.Reminder";
				case "IPF.Note.SocialConnector.FeedItems":
					return "IPF.Note.SocialConnector.FeedItems";
				}
				return containerClass;
			}

			private static readonly object FolderTypeRoot = FolderType.Root;

			private static readonly object FolderTypeGeneric = FolderType.Generic;

			private static readonly object FolderTypeSearch = FolderType.Search;

			private static readonly StorePropertyDefinition[] folderDataProperties = new StorePropertyDefinition[]
			{
				InternalSchema.DisplayName,
				InternalSchema.MapiFolderType,
				InternalSchema.ContainerClass,
				InternalSchema.ParentEntryId,
				InternalSchema.AdminFolderFlags,
				InternalSchema.IsHidden,
				InternalSchema.SystemFolderFlags,
				InternalSchema.ExtendedFolderFlagsInternal
			};

			internal readonly ReadonlyMemoryPropertyBag PropertyBag;
		}
	}
}
