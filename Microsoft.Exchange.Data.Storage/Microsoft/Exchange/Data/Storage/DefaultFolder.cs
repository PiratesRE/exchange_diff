using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DefaultFolder
	{
		internal DefaultFolder(DefaultFolderContext context, DefaultFolderInfo defaultFolderInfo, MailboxSessionSharableDataManager sharableDataManager, DefaultFolderType defaultFolderType, bool deferInitialize) : this(context, defaultFolderInfo, context.Session.InternalPreferedCulture, sharableDataManager, defaultFolderType, deferInitialize, false)
		{
		}

		internal DefaultFolder(DefaultFolderContext context, DefaultFolderInfo defaultFolderInfo, CultureInfo cultureInfo, MailboxSessionSharableDataManager sharableDataManager, DefaultFolderType defaultFolderType, bool deferInitialize, bool forceInitialize)
		{
			bool flag = (defaultFolderInfo.Behavior & DefaultFolderBehavior.AlwaysDeferInitialization) == DefaultFolderBehavior.AlwaysDeferInitialization;
			this.sharableDataManager = sharableDataManager;
			this.defaultFolderType = defaultFolderType;
			this.defaultFolderInfo = defaultFolderInfo;
			this.context = context;
			this.cultureInfo = cultureInfo;
			DefaultFolderData defaultFolderData = this.GetDefaultFolderData();
			DefaultFolderData defaultFolderData2 = defaultFolderData ?? new DefaultFolderData(false);
			if (!this.context.DeferFolderIdInit && !defaultFolderData2.IdInitialized)
			{
				defaultFolderData2 = this.InitializeFolderIdPrivate();
			}
			if (forceInitialize || (!flag && !deferInitialize))
			{
				this.InitializeDefaultFolderIfNeeded(ref defaultFolderData2);
				if (this.GetDefaultFolderData().FolderId == null)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolderInfo>((long)this.GetHashCode(), "DefaultFolder::Ctor. Cannot find the Id of the DefaultFolder. DefaultFolder = {0}.", this.defaultFolderInfo);
				}
			}
			if (defaultFolderData != defaultFolderData2)
			{
				this.SetDefaultFolderData(defaultFolderData2);
			}
		}

		internal StoreObjectId FolderId
		{
			get
			{
				return this.GetDefaultFolderData().FolderId;
			}
		}

		internal bool IsLocalized
		{
			get
			{
				bool result;
				try
				{
					if (this.defaultFolderInfo.Localizable != DefaultFolderLocalization.CanLocalize)
					{
						result = true;
					}
					else
					{
						DefaultFolderData defaultFolderData = this.GetDefaultFolderData();
						if (defaultFolderData.FolderId != null)
						{
							using (Folder folder = Folder.Bind(this.Session, defaultFolderData.FolderId))
							{
								string localizedName = this.defaultFolderInfo.LocalizableDisplayName.ToString(this.context.Session.PreferedCulture);
								if (!string.IsNullOrEmpty(folder.DisplayName) && DefaultFolder.IsMatchingNameWithSuffix(folder.DisplayName, localizedName))
								{
									return true;
								}
								return false;
							}
						}
						result = true;
					}
				}
				catch (ObjectNotFoundException)
				{
					result = true;
				}
				return result;
			}
		}

		private static bool IsMatchingNameWithSuffix(string currentName, string localizedName)
		{
			int num;
			return currentName.StartsWith(localizedName, StringComparison.OrdinalIgnoreCase) && (localizedName.Length >= currentName.Length || (int.TryParse(currentName.Remove(0, localizedName.Length), out num) && num >= StorageLimits.Instance.DefaultFolderMinimumSuffix && num <= StorageLimits.Instance.DefaultFolderMaximumSuffix));
		}

		internal virtual bool TryGetFolderId(out StoreObjectId folderId)
		{
			DefaultFolderData defaultFolderData = this.GetDefaultFolderData();
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<bool, bool, DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::TryGetFolderId. Trying to get folder ID. HasInitialized = {0}, ShouldRefresh = {1}, DefaultFolder = {2}.", defaultFolderData.HasInitialized, this.CheckShouldRefreshFolderId(defaultFolderData), this);
			this.InitializeDefaultFolderIfNeeded(ref defaultFolderData);
			this.SetDefaultFolderData(defaultFolderData);
			folderId = defaultFolderData.FolderId;
			return defaultFolderData.FolderId != null;
		}

		private void InitializeDefaultFolderIfNeeded(ref DefaultFolderData defaultFolderData)
		{
			if ((!defaultFolderData.HasInitialized || this.CheckShouldRefreshFolderId(defaultFolderData)) && !this.Initialize(ref defaultFolderData))
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceWarning<string, DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::InitializeDefaultFolderIfNeeded. The default folder cannot be initialized. defaultFolderId is null. Session = {0}, DefaultFolder = {1}.", this.context.Session.DisplayName, this);
			}
		}

		private bool ShouldRefreshFolderId
		{
			get
			{
				return this.CheckShouldRefreshFolderId(this.GetDefaultFolderData());
			}
		}

		internal bool CanLocalize
		{
			get
			{
				return this.defaultFolderInfo.Localizable == DefaultFolderLocalization.CanLocalize;
			}
		}

		internal StoreObjectType StoreObjectType
		{
			get
			{
				return this.defaultFolderInfo.StoreObjectType;
			}
		}

		internal bool IsIdInitialized
		{
			get
			{
				return this.GetDefaultFolderData().IdInitialized;
			}
		}

		private bool Initialize(ref DefaultFolderData data)
		{
			bool flag = true;
			Exception ex = null;
			if (!data.IdInitialized)
			{
				DefaultFolderData defaultFolderData = this.InitializeFolderIdPrivate();
				if (defaultFolderData != null)
				{
					data = defaultFolderData;
				}
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Initialize. The default folder is about to be initialized. defaultFolder = {0}.", this);
			if (this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.None)
			{
				if (!data.HasInitialized)
				{
					data = new DefaultFolderData(true);
				}
				return true;
			}
			try
			{
				if (DefaultFolder.TestInjectInitFailure != null)
				{
					DefaultFolder.TestInjectInitFailure(this);
				}
				bool flag2 = true;
				if (data.FolderId != null && !this.Validate(data))
				{
					flag2 = false;
					data = new DefaultFolderData(null, data.IdInitialized, data.HasInitialized);
				}
				if (data.FolderId == null && (this.IsOwnerSession || (!flag2 && this.context.Session.Capabilities.MustHideDefaultFolders)) && (this.defaultFolderInfo.Behavior & DefaultFolderBehavior.CreateIfMissing) == DefaultFolderBehavior.CreateIfMissing)
				{
					flag = this.CreateInternal(ref data);
				}
				if (flag && data.FolderId != null && this.HideUnderNonIpmSubtree)
				{
					using (Folder folder = Folder.Bind(this.context.Session, data.FolderId))
					{
						if (!this.IsParentFolderConfigurationFolder(folder))
						{
							ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Initialize. defaultFolder={0} is not under the non-IPM subtree(aka. Configuration folder). Move it to the non-IPM subtree.", this);
							AggregateOperationResult aggregateOperationResult = this.context.Session.Move(this.context[DefaultFolderType.Configuration], new StoreId[]
							{
								data.FolderId
							});
							if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
							{
								LocalizedException exception = aggregateOperationResult.GroupOperationResults[0].Exception;
								ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, LocalizedException>((long)this.GetHashCode(), "DefaultFolder::Initialize. Moving defaultFolder={0} to the non-IPM subtree(aka. Configuration folder) fails with exception={1}.", this, exception);
								if (exception.ErrorCode == -2146233088 && exception is ObjectExistedException)
								{
									folder.PropertyBag.Reload();
									if (this.IsParentFolderConfigurationFolder(folder))
									{
										ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Initialize. After reloading, defaultFolder={0} is under the non-IPM subtree(aka. Configuration folder).", this);
									}
									else
									{
										data = new DefaultFolderData(null, data.IdInitialized, data.HasInitialized);
										flag = false;
										ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Initialize. After reloading, defaultFolder={0} is still NOT under the non-IPM subtree(aka. Configuration folder).", this);
									}
								}
								else
								{
									data = new DefaultFolderData(null, data.IdInitialized, data.HasInitialized);
									flag = false;
									ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder, LocalizedException>((long)this.GetHashCode(), "DefaultFolder::Initialize. Unable to move defaultFolder={0} to the Configuration node. Exception={1}.", this, exception);
								}
							}
						}
					}
				}
				if (flag && data.FolderId != null && this.context.Session.MailboxOwner.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox && this.IsHiddenTeamMailboxFolder)
				{
					using (Folder folder2 = Folder.Bind(this.context.Session, data.FolderId))
					{
						folder2.PropertyBag[FolderSchema.IsHidden] = true;
						folder2.Save();
					}
				}
				if (this.CheckShouldRefreshFolderId(data))
				{
					data = this.InitializeFolderIdPrivate();
					if (data.FolderId != null)
					{
						if (this.Validate(data))
						{
							flag = true;
						}
						else
						{
							data = new DefaultFolderData(null, data.IdInitialized, data.HasInitialized);
						}
					}
				}
			}
			catch (CorruptDataException ex2)
			{
				ex = ex2;
			}
			this.SetDefaultFolderData(data);
			if (ex != null)
			{
				this.defaultFolderInfo.CorruptDataRecoveryStrategy.Recover(this, ex, ref data);
			}
			if (!data.HasInitialized)
			{
				data = new DefaultFolderData(data.FolderId, data.IdInitialized, true);
				this.SetDefaultFolderData(data);
			}
			return flag;
		}

		private bool IsParentFolderConfigurationFolder(Folder folder)
		{
			return folder.ParentId.Equals(this.context[DefaultFolderType.Configuration]);
		}

		private bool Validate(DefaultFolderData data)
		{
			bool flag = true;
			if (this.context.Session.Capabilities.MustHideDefaultFolders || (this.context.Session.LogonType != LogonType.SystemService && this.context.Session.LogonType != LogonType.Transport))
			{
				flag = this.defaultFolderInfo.FolderValidationStrategy.EnsureIsValid(this.context, data.FolderId, this.context.FolderDataDictionary);
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, bool>((long)this.GetHashCode(), "DefaultFolder::EnsureIsValid. default folder = {0}, isValid = {1}", this, flag);
			return flag;
		}

		internal void Refresh()
		{
			DefaultFolderData defaultFolderData = new DefaultFolderData(false);
			this.Initialize(ref defaultFolderData);
			this.SetDefaultFolderData(defaultFolderData);
		}

		internal void Create()
		{
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Create. The default folder is about to be created. defaultFolder = {0}.", this);
			DefaultFolderData defaultFolderData = this.GetDefaultFolderData();
			this.CreateInternal(ref defaultFolderData);
			this.SetDefaultFolderData(defaultFolderData);
		}

		internal virtual void RemoveForRecover()
		{
			DefaultFolderData defaultFolderData = this.GetDefaultFolderData();
			try
			{
				this.RemoveForRecover(ref defaultFolderData);
			}
			finally
			{
				this.SetDefaultFolderData(defaultFolderData);
			}
		}

		internal virtual void RemoveForRecover(ref DefaultFolderData data)
		{
			if (data.FolderId != null)
			{
				if (this.defaultFolderInfo.StoreObjectType == StoreObjectType.SearchFolder)
				{
					AggregateOperationResult aggregateOperationResult = this.defaultFolderInfo.FolderCreator.Delete(this.context, DeleteItemFlags.HardDelete, data.FolderId);
					if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
					{
						ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Create. The default folder cannot be deleted. defaultFolder = {0}.", this);
					}
				}
				data = new DefaultFolderData(null, data.IdInitialized, data.HasInitialized);
				this.defaultFolderInfo.EntryIdStrategy.UnsetEntryId(this.context);
			}
		}

		internal virtual void Delete(DeleteItemFlags deleteItemFlags)
		{
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Delete. The default folder is about to be deleted. defaultFolder = {0}.", this);
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteItemFlags, "deleteItemFlags");
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(this.defaultFolderInfo.DefaultFolderType, DefaultFolder.DefaultFoldersThatCanBeDeleted);
			StoreObjectId storeObjectId;
			if (!this.TryGetFolderId(out storeObjectId))
			{
				throw new InvalidOperationException(string.Format("Cannot delete the default folder {0}, because it does not exist.", this.defaultFolderInfo.DefaultFolderType));
			}
			this.defaultFolderInfo.EntryIdStrategy.UnsetEntryId(this.context);
			DefaultFolderData defaultFolderData = this.GetDefaultFolderData();
			AggregateOperationResult aggregateOperationResult = this.defaultFolderInfo.FolderCreator.Delete(this.context, deleteItemFlags, defaultFolderData.FolderId);
			if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
			{
				throw new AggregateOperationFailedException(ServerStrings.ExFailedToDeleteDefaultFolder, aggregateOperationResult);
			}
			this.defaultFolderInfo.EntryIdStrategy.UnsetEntryId(this.context);
			this.SetDefaultFolderData(new DefaultFolderData(null));
		}

		internal bool Localize(out PropertyError error)
		{
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Localize. The default folder is about to be localized. defaultFolder = {0}.", this);
			error = null;
			return this.defaultFolderInfo.Localizable == DefaultFolderLocalization.CanLocalize && this.InternalLocalize(this.GetDefaultFolderData(), out error);
		}

		internal bool Rename(string displayName, out PropertyError error)
		{
			if (this.defaultFolderInfo.DefaultFolderType != DefaultFolderType.ElcRoot)
			{
				throw new NotSupportedException(string.Format("We do not support renaming the default folder. defaultFolder = {0}.", this.defaultFolderInfo.DefaultFolderType));
			}
			error = DefaultFolderCreator.UpdateElcRootFolderName(this.context, displayName);
			return error != null;
		}

		public override string ToString()
		{
			return this.defaultFolderInfo.ToString();
		}

		private bool IsOwnerSession
		{
			get
			{
				return this.context.Session.LogonType == LogonType.Owner;
			}
		}

		internal MailboxSession Session
		{
			get
			{
				return this.context.Session;
			}
		}

		internal void InitializeFolderId()
		{
			this.SetDefaultFolderData(this.InitializeFolderIdPrivate());
		}

		internal DefaultFolderType GetDefaultFolderType()
		{
			return this.defaultFolderInfo.DefaultFolderType;
		}

		private DefaultFolderData GetDefaultFolderData()
		{
			return this.sharableDataManager.GetDefaultFolder(this.defaultFolderType);
		}

		private void SetDefaultFolderData(DefaultFolderData data)
		{
			this.sharableDataManager.SetDefaultFolder(this.defaultFolderType, data);
		}

		private DefaultFolderData InitializeFolderIdPrivate()
		{
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::InitializeFolderId. The default folder ID is about to be initialized. defaultFolder = {0}.", this);
			byte[] array = this.defaultFolderInfo.EntryIdStrategy.GetEntryId(this.context);
			if (!IdConverter.IsFolderId(array))
			{
				array = null;
				ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::InitializeFolderId. Invalid entry id found. defaultFolder = {0}.", this);
			}
			DefaultFolderData result;
			if (array != null)
			{
				result = new DefaultFolderData(StoreObjectId.FromProviderSpecificId(array, this.StoreObjectType), true, false);
			}
			else
			{
				result = new DefaultFolderData(null, true, false);
			}
			return result;
		}

		private bool CheckShouldRefreshFolderId(DefaultFolderData defaultFolderData)
		{
			return defaultFolderData.FolderId == null && (this.defaultFolderInfo.Behavior & DefaultFolderBehavior.RefreshIfMissing) == DefaultFolderBehavior.RefreshIfMissing;
		}

		internal bool CreateInternal(ref DefaultFolderData data)
		{
			if ((this.defaultFolderInfo.Behavior & DefaultFolderBehavior.CanCreate) != DefaultFolderBehavior.CanCreate)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::Create. Cannot create default folder because DefaultFolderBehavior.CanCreate is not set. defaultFolder = {0}.", this);
				throw new NotSupportedException(string.Format("The defaultFolder cannot be created. defaultFolder = {0}.", this));
			}
			if (data.FolderId != null)
			{
				throw new InvalidOperationException(string.Format("Cannot create default folder {0}, because it already exists.", this.defaultFolderInfo.DefaultFolderType));
			}
			for (int i = 0; i <= StorageLimits.Instance.DefaultFolderMaximumSuffix; i++)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, int>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. About to create the default folder. defaultFolder = {0}. Iteration = {1}.", this, i);
				Folder folder = null;
				bool flag = false;
				this.context.Session.Mailbox.Load(DefaultFolder.localeIdPropertyDefinition);
				string text = this.defaultFolderInfo.LocalizableDisplayName.ToString(this.cultureInfo);
				StoreObjectId storeObjectId = null;
				if (this.HideUnderNonIpmSubtree)
				{
					storeObjectId = this.context[DefaultFolderType.Configuration];
				}
				else
				{
					storeObjectId = this.context[this.defaultFolderInfo.FolderCreator.DefaultContainer];
				}
				if (i > 0)
				{
					text += i;
				}
				bool flag2 = false;
				try
				{
					folder = this.defaultFolderInfo.FolderCreator.Create(this.context, text, storeObjectId, out flag2);
				}
				catch (ObjectNotFoundException arg)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, string, ObjectNotFoundException>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. The user may have no permission to the container folder. defaultFolder = {0}, session = {1}, exception = {2}.", this, this.context.Session.DisplayName, arg);
					return false;
				}
				catch (AccessDeniedException arg2)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, string, AccessDeniedException>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. The user may have no permission to the container folder. defaultFolder = {0}, session = {1}, exception = {2}.", this, this.context.Session.DisplayName, arg2);
					return false;
				}
				catch (ObjectExistedException arg3)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, string, ObjectExistedException>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. The folder already exists. defaultFolder = {0}, session = {1}, exception = {2}.", this, this.context.Session.DisplayName, arg3);
					if (folder != null && folder.Id != null)
					{
						folder.Dispose();
						folder = null;
						data = new DefaultFolderData(data.HasInitialized);
					}
					folder = DefaultFolderCreator.BindToSubfolderByName(this.context.Session, storeObjectId, text, new PropertyDefinition[0]);
					flag2 = false;
				}
				if (folder != null)
				{
					if (!flag2)
					{
						data = new DefaultFolderData(folder.StoreObjectId, data.IdInitialized, data.HasInitialized);
					}
					StoreObjectId objectId = folder.Id.ObjectId;
					try
					{
						if (flag2 || this.defaultFolderInfo.FolderValidationStrategy.EnsureIsValid(this.context, folder))
						{
							ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder, bool>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. We created or hijacked a new folder for the missing default folder. defaultFolder = {0}, hasCreatedNew = {1}.", this, flag2);
							if (flag2)
							{
								this.SetProperties(folder);
								folder.Load(null);
							}
							flag = true;
						}
					}
					catch (DefaultFolderPropertyValidationException)
					{
						this.SetProperties(folder);
						folder.Load(null);
						flag = true;
					}
					finally
					{
						folder.Dispose();
					}
					if (flag)
					{
						this.defaultFolderInfo.EntryIdStrategy.SetEntryId(this.context, objectId.ProviderLevelItemId);
						data = new DefaultFolderData(StoreObjectId.FromProviderSpecificId(objectId.ProviderLevelItemId, this.StoreObjectType), data.IdInitialized, data.HasInitialized);
						return true;
					}
					ExTraceGlobals.DefaultFoldersTracer.TraceWarning<DefaultFolder, string, int>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. We failed to create a default folder. We will try with a new name. defaultFolder = {0}, displayName = {1}, iteration = {2}.", this, text, i);
				}
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceError<string, DefaultFolderType>((long)this.GetHashCode(), "DefaultFolder::CreateInternal. Failed to open or create default folder. Folder = {0}, defaultFolderType = {1}.", this.defaultFolderInfo.LocalizableDisplayName.ToString(this.cultureInfo), this.defaultFolderInfo.DefaultFolderType);
			throw new DefaultFolderNameClashException(ServerStrings.ExUnableToOpenOrCreateDefaultFolder(this.defaultFolderInfo.LocalizableDisplayName.ToString(this.cultureInfo)));
		}

		internal void SetProperties(Folder folder)
		{
			this.defaultFolderInfo.FolderValidationStrategy.SetProperties(this.context, folder);
			FolderSaveResult folderSaveResult = folder.Save();
			if (folderSaveResult.OperationResult == OperationResult.Succeeded)
			{
				return;
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder, OperationResult>((long)this.GetHashCode(), "DefaultFolder::SetProperties. We failed to stamp default folder. defaultFolder = {0}, result = {1}.", this, folderSaveResult.OperationResult);
			LocalizedException ex = folderSaveResult.ToException(ServerStrings.ExCannotCreateFolder(folderSaveResult));
			if (ex.InnerException != null && ex.InnerException is ObjectNotFoundException)
			{
				throw new CannotCompleteOperationException(ex.LocalizedString, ex.InnerException);
			}
			throw ex;
		}

		private bool InternalLocalize(DefaultFolderData data, out PropertyError error)
		{
			error = null;
			if (data.FolderId == null)
			{
				return false;
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::InternalLocalize. We are trying to localize the default folder. defaultFolder = {0}.", this);
			string text = this.defaultFolderInfo.LocalizableDisplayName.ToString(this.cultureInfo);
			try
			{
				using (MapiPropertyBag mapiPropertyBag = MapiPropertyBag.CreateMapiPropertyBag(this.context.Session, data.FolderId))
				{
					object[] properties = mapiPropertyBag.GetProperties(DefaultFolder.displayNameNativePropertyDefinition);
					if (properties.Length <= 0)
					{
						return false;
					}
					string text2 = properties[0] as string;
					if (text2 == null || string.Compare(text, text2, StringComparison.OrdinalIgnoreCase) != 0)
					{
						PropertyDefinition[] propertyDefinitions;
						if (this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.ElcRoot)
						{
							propertyDefinitions = DefaultFolder.elcFolderLocalizedNamePropertyDefinition;
						}
						else
						{
							propertyDefinitions = DefaultFolder.displayNamePropertyDefinition;
						}
						PropertyError[] array = mapiPropertyBag.SetProperties(propertyDefinitions, new object[]
						{
							text
						});
						if (array != null && array.Length > 0)
						{
							if (array[0].PropertyErrorCode == PropertyErrorCode.FolderNameConflict)
							{
								ExTraceGlobals.DefaultFoldersTracer.TraceError<string, DefaultFolderType>((long)this.GetHashCode(), "DefaultFolder::InternalLocalize. Failed to localize default folder. Folder = {0}, defaultFolderType = {1}.", this.defaultFolderInfo.LocalizableDisplayName.ToString(this.cultureInfo), this.defaultFolderInfo.DefaultFolderType);
								throw new DefaultFolderLocalizationException(new DefaultFolderNameClashException(this.defaultFolderInfo.LocalizableDisplayName));
							}
							error = array[0];
							ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder, PropertyError>((long)this.GetHashCode(), "DefaultFolder::InternalLocalize. We failed to localize default folder due to error. defaultFolder = {0}, error = {1}.", this, error);
						}
						mapiPropertyBag.SaveChanges(false);
						return true;
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceError<DefaultFolder>((long)this.GetHashCode(), "DefaultFolder::InternalLocalize. The default folder was missing. Localization aborted. defaultFolder = {0}.", this);
			}
			return false;
		}

		private bool HideUnderNonIpmSubtree
		{
			get
			{
				return this.context.Session.Capabilities.MustHideDefaultFolders && (this.defaultFolderInfo.Behavior & DefaultFolderBehavior.CanHideFolderFromOutlook) == DefaultFolderBehavior.CanHideFolderFromOutlook;
			}
		}

		private bool IsHiddenTeamMailboxFolder
		{
			get
			{
				return this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.RssSubscription || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Notes || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Tasks || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Contacts || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.SentItems || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Calendar || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Journal || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Drafts || this.defaultFolderInfo.DefaultFolderType == DefaultFolderType.Outbox;
			}
		}

		private static readonly DefaultFolderType[] DefaultFoldersThatCanBeDeleted = new DefaultFolderType[]
		{
			DefaultFolderType.ElcRoot,
			DefaultFolderType.UMVoicemail,
			DefaultFolderType.UMFax,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.Drafts,
			DefaultFolderType.Tasks,
			DefaultFolderType.RecipientCache,
			DefaultFolderType.QuickContacts,
			DefaultFolderType.ImContactList,
			DefaultFolderType.OrganizationalContacts,
			DefaultFolderType.PushNotificationRoot,
			DefaultFolderType.BirthdayCalendar,
			DefaultFolderType.SnackyApps
		};

		private readonly DefaultFolderInfo defaultFolderInfo;

		private readonly DefaultFolderContext context;

		private readonly CultureInfo cultureInfo;

		private readonly MailboxSessionSharableDataManager sharableDataManager;

		private readonly DefaultFolderType defaultFolderType;

		private static readonly PropertyDefinition[] elcFolderLocalizedNamePropertyDefinition = new PropertyDefinition[]
		{
			InternalSchema.ElcFolderLocalizedName
		};

		private static readonly PropertyDefinition[] displayNamePropertyDefinition = new PropertyDefinition[]
		{
			InternalSchema.DisplayName
		};

		private static readonly NativeStorePropertyDefinition[] displayNameNativePropertyDefinition = new NativeStorePropertyDefinition[]
		{
			InternalSchema.DisplayName
		};

		private static readonly PropertyDefinition[] localeIdPropertyDefinition = new PropertyDefinition[]
		{
			InternalSchema.LocaleId
		};

		internal static DefaultFolder.InitFailureDelegate TestInjectInitFailure = null;

		internal delegate void InitFailureDelegate(DefaultFolder folder);
	}
}
