using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FolderCreatePropertyBag : FolderPropertyBag
	{
		internal FolderCreatePropertyBag(StoreSession storeSession, StoreObjectId parentFolderId, bool isSearchFolder, CreateMode createMode, bool isSecure, ICollection<PropertyDefinition> properties) : base(storeSession, null, properties)
		{
			Util.ThrowOnNullArgument(storeSession, "storeSession");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			EnumValidator.AssertValid<CreateMode>(createMode);
			this.parentFolderId = parentFolderId;
			this.isSearchFolder = isSearchFolder;
			this.createMode = createMode;
			this.isSecure = isSecure;
		}

		public bool IsSearchFolder
		{
			get
			{
				return this.isSearchFolder;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderCreatePropertyBag>(this);
		}

		internal override FolderSaveResult SaveFolderPropertyBag(bool needVersionCheck)
		{
			Origin origin = this.Context.CoreState.Origin;
			if (origin == Origin.New)
			{
				this.CreateMapiFolder();
			}
			FolderSaveResult result;
			using (this.GetRestrictedOperationToken(origin))
			{
				result = base.SaveFolderPropertyBag(needVersionCheck);
			}
			return result;
		}

		protected override void LazyCreateMapiPropertyBag()
		{
			if (this.Context.CoreObject != null)
			{
				if (this.Context.CoreState.Origin == Origin.New)
				{
					return;
				}
				base.LazyCreateMapiPropertyBag();
			}
		}

		private void CreateMapiFolder()
		{
			string valueOrDefault = base.GetValueOrDefault<string>(FolderSchema.DisplayName);
			string valueOrDefault2 = base.GetValueOrDefault<string>(FolderSchema.Description, string.Empty);
			byte[] array = base.Session.IsMoveUser ? base.GetValueOrDefault<byte[]>(StoreObjectSchema.EntryId, null) : null;
			MapiProp mapiProp = null;
			MapiPropertyBag mapiPropertyBag = null;
			bool flag = false;
			try
			{
				using (Folder folder = Folder.Bind(base.Session, this.parentFolderId, new PropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights
				}))
				{
					if (folder is SearchFolder)
					{
						throw new InvalidParentFolderException(ServerStrings.ExCannotCreateSubfolderUnderSearchFolder);
					}
					bool flag2 = (this.createMode & CreateMode.OpenIfExists) == CreateMode.OpenIfExists;
					bool flag3 = (this.createMode & CreateMode.InstantSearch) == CreateMode.InstantSearch;
					bool flag4 = (this.createMode & CreateMode.OptimizedConversationSearch) == CreateMode.OptimizedConversationSearch;
					bool flag5 = (this.createMode & CreateMode.CreatePublicFolderDumpster) == CreateMode.CreatePublicFolderDumpster;
					if (flag2 && flag3)
					{
						throw new ArgumentException("Cannot use both openIfExists and instantSearch folder creation flags");
					}
					if (this.isSecure && (flag3 || flag4 || flag5))
					{
						throw new ArgumentException("Cannot use isSecure in conjunction with instantSearch, optimizedConversationSearch or createPublicFolderDumpster");
					}
					StoreSession session = base.Session;
					bool flag6 = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag6 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						try
						{
							this.OnBeforeCreateFolder(folder);
							if (this.isSecure)
							{
								mapiProp = folder.MapiFolder.CreateSecureFolder(valueOrDefault, valueOrDefault2, flag2, array);
							}
							else
							{
								mapiProp = folder.MapiFolder.CreateFolder(valueOrDefault, valueOrDefault2, flag2, this.isSearchFolder, flag3, flag4, flag5, array);
							}
						}
						catch (MapiExceptionNotEnoughMemory mapiExceptionNotEnoughMemory)
						{
							ExTraceGlobals.StorageTracer.TraceError<MapiExceptionNotEnoughMemory>((long)this.GetHashCode(), "CreateFolderPropertyBag::CreateMapiPropertyBag. Failed to create MapiFolder due to MapiException {0}.", mapiExceptionNotEnoughMemory);
							bool flag7 = !string.IsNullOrEmpty(valueOrDefault2);
							string errorDescription = mapiExceptionNotEnoughMemory.ToString();
							PropertyError[] array2 = new PropertyError[flag7 ? 2 : 1];
							array2[0] = new PropertyError(FolderSchema.DisplayName, PropertyErrorCode.NotEnoughMemory, errorDescription);
							if (flag7)
							{
								array2[1] = new PropertyError(FolderSchema.Description, PropertyErrorCode.NotEnoughMemory, errorDescription);
							}
							throw PropertyError.ToException(array2);
						}
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateFolder(valueOrDefault), ex, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CreateFolderPropertyBag::CreateMapiPropertyBag.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateFolder(valueOrDefault), ex2, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CreateFolderPropertyBag::CreateMapiPropertyBag.", new object[0]),
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
								if (flag6)
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
				}
				this.Context.CoreState.Origin = Origin.Existing;
				base.MemoryPropertyBag.ClearChangeInfo(FolderSchema.DisplayName);
				if (array != null)
				{
					base.MemoryPropertyBag.ClearChangeInfo(StoreObjectSchema.EntryId);
				}
				if (this.createMode == CreateMode.CreateNew)
				{
					base.MemoryPropertyBag.ClearChangeInfo(FolderSchema.Description);
				}
				mapiPropertyBag = new MapiPropertyBag(base.Session, mapiProp);
				base.MapiPropertyBag = mapiPropertyBag;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(mapiPropertyBag);
					Util.DisposeIfPresent(mapiProp);
				}
			}
		}

		private void OnBeforeCreateFolder(Folder parentFolder)
		{
			PublicFolderSession publicFolderSession = base.Session as PublicFolderSession;
			if (publicFolderSession == null)
			{
				return;
			}
			if (this.isSearchFolder)
			{
				base.SetProperty(CoreFolderSchema.ReplicaList, new string[]
				{
					publicFolderSession.MailboxGuid.ToString()
				});
				return;
			}
			PublicFolderContentMailboxInfo contentMailboxInfo = CoreFolder.GetContentMailboxInfo(base.GetValueOrDefault<string[]>(CoreFolderSchema.ReplicaList, Array<string>.Empty));
			PublicFolderContentMailboxInfo contentMailboxInfo2 = parentFolder.GetContentMailboxInfo();
			Guid guid = contentMailboxInfo.IsValid ? contentMailboxInfo.MailboxGuid : (contentMailboxInfo2.IsValid ? contentMailboxInfo2.MailboxGuid : publicFolderSession.MailboxGuid);
			base.SetProperty(CoreFolderSchema.ReplicaList, new string[]
			{
				guid.ToString()
			});
		}

		private DisposableFrame GetRestrictedOperationToken(Origin origin)
		{
			if (origin == Origin.New && base.Session.IsPublicFolderSession)
			{
				return (base.Session as PublicFolderSession).GetRestrictedOperationToken();
			}
			return null;
		}

		private readonly StoreObjectId parentFolderId;

		private readonly bool isSearchFolder;

		private readonly CreateMode createMode;

		private readonly bool isSecure;
	}
}
