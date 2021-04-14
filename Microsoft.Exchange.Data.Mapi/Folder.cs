using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public abstract class Folder : MapiObject
	{
		internal override MapiProp RawMapiEntry
		{
			get
			{
				MapiProp mapiFolder;
				using (MapiStore messageStore = this.GetMessageStore())
				{
					mapiFolder = this.GetMapiFolder(messageStore);
				}
				return mapiFolder;
			}
		}

		public new FolderId Identity
		{
			get
			{
				return (FolderId)base.MapiIdentity;
			}
			internal set
			{
				base.MapiIdentity = value;
			}
		}

		private static MapiFolderPath GetFolderPath(MapiEntryId folderEntryId, MapiFolder mapiFolder, MapiStore mapiStore)
		{
			MapiFolderPath result;
			try
			{
				if (null == folderEntryId)
				{
					if (mapiFolder == null)
					{
						throw new ArgumentNullException("mapiFolder");
					}
					PropValue prop = mapiFolder.GetProp(PropTag.EntryId);
					if (PropType.Error == prop.PropType)
					{
						return null;
					}
					folderEntryId = new MapiEntryId(prop.GetBytes());
				}
				if (mapiStore == null)
				{
					throw new ArgumentNullException("mapiStore");
				}
				MapiEntryId operand;
				using (MapiFolder rootFolder = mapiStore.GetRootFolder())
				{
					operand = new MapiEntryId(rootFolder.GetProp(PropTag.EntryId).GetBytes());
				}
				MapiEntryId operand2 = new MapiEntryId(mapiStore.GetIpmSubtreeFolderEntryId());
				MapiEntryId operand3 = new MapiEntryId(mapiStore.GetNonIpmSubtreeFolderEntryId());
				MapiFolderPath mapiFolderPath = null;
				MapiEntryId mapiEntryId = folderEntryId;
				MapiEntryId mapiEntryId2 = null;
				string parentFolderName = null;
				while (!(operand2 == mapiEntryId))
				{
					if (operand3 == mapiEntryId || operand == mapiEntryId)
					{
						mapiFolderPath = MapiFolderPath.GenerateFolderPath("NON_IPM_SUBTREE", mapiFolderPath, true);
					}
					else
					{
						using (MapiFolder mapiFolder2 = (MapiFolder)mapiStore.OpenEntry((byte[])mapiEntryId))
						{
							PropValue[] props = mapiFolder2.GetProps(new PropTag[]
							{
								PropTag.ParentEntryId,
								PropTag.DisplayName
							});
							if (PropType.Error == props[0].PropType || PropType.Error == props[1].PropType)
							{
								return null;
							}
							mapiEntryId2 = new MapiEntryId(props[0].GetBytes());
							parentFolderName = props[1].GetString();
						}
						if (!(mapiEntryId2 == mapiEntryId))
						{
							mapiFolderPath = MapiFolderPath.GenerateFolderPath(parentFolderName, mapiFolderPath, false);
							mapiEntryId = mapiEntryId2;
							continue;
						}
					}
					IL_19C:
					return mapiFolderPath;
				}
				mapiFolderPath = MapiFolderPath.GenerateFolderPath("IPM_SUBTREE", mapiFolderPath, true);
				goto IL_19C;
			}
			catch (MapiPermanentException)
			{
				result = null;
			}
			catch (MapiRetryableException)
			{
				result = null;
			}
			return result;
		}

		private static string GetFolderLegacyDistinguishedName(MapiEntryId abEntryId, MapiFolder folder)
		{
			string result;
			try
			{
				if (null == abEntryId)
				{
					if (folder == null)
					{
						throw new ArgumentNullException("folder");
					}
					PropValue prop = folder.GetProp(PropTag.AddressBookEntryId);
					if (PropType.Error == prop.PropType)
					{
						return null;
					}
					abEntryId = new MapiEntryId(prop.GetBytes());
				}
				result = MapiMessageStoreSession.GetLegacyDNFromAddressBookEntryId(abEntryId);
			}
			catch (MapiPermanentException)
			{
				result = null;
			}
			catch (MapiRetryableException)
			{
				result = null;
			}
			return result;
		}

		internal static MapiFolder RetrieveMapiFolder(MapiStore store, FolderId identity)
		{
			MapiFolder mapiFolder = null;
			MapiFolder result;
			try
			{
				FolderId folderId = null;
				result = Folder.RetrieveMapiFolder(store, identity, ref mapiFolder, null, out folderId);
			}
			finally
			{
				if (mapiFolder != null)
				{
					mapiFolder.Dispose();
				}
			}
			return result;
		}

		internal static MapiFolder RetrieveMapiFolder(MapiStore store, FolderId identity, ref MapiFolder parent, Folder.IdentityConstructor idCtor, out FolderId realId)
		{
			realId = null;
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			Folder.CheckRequirementsOnIdentityToContinue(identity);
			MapiFolder mapiFolder = null;
			bool flag = false;
			bool flag2 = false;
			MapiFolder result;
			try
			{
				byte[] array = null;
				MapiEntryId entryId = null;
				MapiFolderPath folderPath = null;
				string legacyDn = null;
				if (null == identity.MapiEntryId)
				{
					if (identity.LegacyDistinguishedName != null)
					{
						legacyDn = identity.LegacyDistinguishedName;
						array = store.GetFolderEntryId(identity.LegacyDistinguishedName);
					}
				}
				else
				{
					array = (byte[])identity.MapiEntryId;
				}
				if (array != null)
				{
					entryId = new MapiEntryId(array);
					mapiFolder = (MapiFolder)store.OpenEntry(array);
					if (parent == null)
					{
						parent = (MapiFolder)store.OpenEntry(mapiFolder.GetProp(PropTag.ParentEntryId).GetBytes());
						flag = true;
					}
				}
				else if (null != identity.MapiFolderPath)
				{
					folderPath = identity.MapiFolderPath;
					if (parent == null)
					{
						mapiFolder = Folder.GetFolderByPath(store, identity.MapiFolderPath, out parent);
						flag = true;
					}
					else
					{
						mapiFolder = Folder.GetFolderByPath(store, parent, identity.MapiFolderPath);
					}
				}
				if (idCtor != null)
				{
					realId = idCtor(entryId, folderPath, legacyDn);
				}
				flag2 = true;
				result = mapiFolder;
			}
			finally
			{
				if (!flag2)
				{
					if (mapiFolder != null)
					{
						mapiFolder.Dispose();
						mapiFolder = null;
					}
					if (flag && parent != null)
					{
						parent.Dispose();
						parent = null;
					}
				}
			}
			return result;
		}

		private static MapiFolder RetrieveParentMapiFolder(MapiStore store, FolderId identity, ref MapiFolder folder)
		{
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			Folder.CheckRequirementsOnIdentityToContinue(identity);
			MapiFolder mapiFolder = null;
			bool flag = false;
			bool flag2 = false;
			MapiFolder result;
			try
			{
				byte[] array = null;
				if (null == identity.MapiEntryId)
				{
					if (identity.LegacyDistinguishedName != null)
					{
						array = store.GetFolderEntryId(identity.LegacyDistinguishedName);
					}
				}
				else
				{
					array = (byte[])identity.MapiEntryId;
				}
				if (array != null)
				{
					if (folder == null)
					{
						folder = (MapiFolder)store.OpenEntry(array);
						flag = true;
					}
					mapiFolder = (MapiFolder)store.OpenEntry(folder.GetProp(PropTag.ParentEntryId).GetBytes());
				}
				else if (null != identity.MapiFolderPath)
				{
					MapiFolderPath parent = identity.MapiFolderPath.Parent;
					if (null == parent)
					{
						mapiFolder = store.GetRootFolder();
					}
					else
					{
						mapiFolder = Folder.GetFolderByPath(store, parent);
					}
				}
				flag2 = true;
				result = mapiFolder;
			}
			finally
			{
				if (!flag2)
				{
					if (flag && folder != null)
					{
						folder.Dispose();
						folder = null;
					}
					if (mapiFolder != null)
					{
						mapiFolder.Dispose();
						mapiFolder = null;
					}
				}
			}
			return result;
		}

		internal static MapiFolder GetFolderByPath(MapiStore mapiStore, MapiFolder parentFolder, MapiFolderPath folderPath)
		{
			if (null == folderPath)
			{
				throw new ArgumentNullException("folderPath");
			}
			if (folderPath.IsSubtreeRoot)
			{
				if (mapiStore == null)
				{
					throw new ArgumentNullException("mapiStore");
				}
				if (!folderPath.IsIpmPath)
				{
					return mapiStore.GetNonIpmSubtreeFolder();
				}
				return mapiStore.GetIpmSubtreeFolder();
			}
			else
			{
				if (parentFolder == null)
				{
					throw new ArgumentNullException("parentFolder");
				}
				return parentFolder.OpenSubFolderByName(folderPath[folderPath.Depth - 1]);
			}
		}

		internal static MapiFolder GetFolderByPath(MapiStore mapiStore, MapiFolderPath folderPath)
		{
			MapiFolder mapiFolder = null;
			MapiFolder folderByPath;
			try
			{
				folderByPath = Folder.GetFolderByPath(mapiStore, folderPath, out mapiFolder);
			}
			finally
			{
				if (mapiFolder != null)
				{
					mapiFolder.Dispose();
				}
			}
			return folderByPath;
		}

		internal static MapiFolder GetFolderByPath(MapiStore mapiStore, MapiFolderPath folderPath, out MapiFolder parent)
		{
			if (mapiStore == null)
			{
				throw new ArgumentNullException("mapiStore");
			}
			if (null == folderPath)
			{
				throw new ArgumentNullException("folderPath");
			}
			parent = null;
			MapiFolder mapiFolder = null;
			bool flag = false;
			MapiFolder result;
			try
			{
				mapiFolder = (folderPath.IsIpmPath ? mapiStore.GetIpmSubtreeFolder() : mapiStore.GetNonIpmSubtreeFolder());
				int num = 0;
				while (folderPath.Depth - 1 > num)
				{
					MapiFolder mapiFolder2;
					parent = (mapiFolder2 = mapiFolder);
					using (mapiFolder2)
					{
						mapiFolder = parent.OpenSubFolderByName(folderPath[num]);
					}
					num++;
				}
				if (folderPath.Depth - 1 == num)
				{
					if (parent != null)
					{
						parent.Dispose();
						parent = null;
					}
					parent = mapiFolder;
					mapiFolder = parent.OpenSubFolderByName(folderPath[num]);
				}
				if (parent == null)
				{
					parent = mapiStore.GetRootFolder();
				}
				flag = true;
				result = mapiFolder;
			}
			finally
			{
				if (!flag)
				{
					if (mapiFolder != null)
					{
						mapiFolder.Dispose();
						mapiFolder = null;
					}
					if (parent != null)
					{
						parent.Dispose();
						parent = null;
					}
				}
			}
			return result;
		}

		internal override void ReleaseUnmanagedResources()
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Folder>(this);
		}

		internal override void AdjustPropertyValuesToUpdate(List<PropValue> propertyValues)
		{
			base.AdjustPropertyValuesToUpdate(propertyValues);
			if (base.ObjectState == ObjectState.New)
			{
				propertyValues.RemoveAll((PropValue pv) => PropTag.DisplayName == pv.PropTag);
			}
		}

		internal override void Delete()
		{
			if (ObjectState.Deleted == base.ObjectState)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionObjectStateInvalid(base.ObjectState.ToString()));
			}
			if (base.ObjectState == ObjectState.New)
			{
				base.MarkAsDeleted();
				return;
			}
			Folder.CheckRequirementsToContinue(this.Identity, base.MapiSession);
			ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string>((long)this.GetHashCode(), "To Delete Folder '{0}' on server '{1}'.", this.Identity, base.MapiSession.ServerName);
			base.EnableDisposeTracking();
			using (MapiStore messageStore = this.GetMessageStore())
			{
				using (MapiFolder mapiFolder = this.GetMapiFolder(messageStore))
				{
					try
					{
						this.DetectDisallowedModification(messageStore);
						bool flag = false;
						this.TryTestHasSubFolders(mapiFolder, out flag);
						try
						{
							using (MapiFolder parentFolder = this.GetParentFolder(messageStore))
							{
								parentFolder.DeleteFolder((byte[])this.Identity.MapiEntryId);
							}
						}
						catch (MapiExceptionPartialCompletion innerException)
						{
							if (flag)
							{
								throw new MapiPartialCompletionException(Strings.ErrorRemovalPartialCompleted(this.Identity.ToString()), innerException);
							}
							throw;
						}
					}
					finally
					{
						base.Dispose();
					}
				}
			}
			base.ResetChangeTracking();
			base.MarkAsDeleted();
		}

		internal override void Read(bool keepUnmanagedResources)
		{
			if (base.ObjectState != ObjectState.New)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionObjectStateInvalid(base.ObjectState.ToString()));
			}
			Folder.CheckRequirementsToContinue(this.Identity, base.MapiSession);
			bool flag = false;
			ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string>((long)this.GetHashCode(), "To Read Folder '{0}' from server '{1}'.", this.Identity, base.MapiSession.ServerName);
			base.EnableDisposeTracking();
			using (MapiStore messageStore = this.GetMessageStore())
			{
				using (MapiFolder mapiFolder = this.GetMapiFolder(messageStore))
				{
					try
					{
						base.Instantiate(mapiFolder.GetProps(base.GetPropertyTagsToRead()));
						this.UpdateIdentity(this.UpdateIdentityFlagsForReading);
						flag = true;
					}
					finally
					{
						if (!keepUnmanagedResources || !flag)
						{
							base.Dispose();
						}
					}
				}
			}
			base.ResetChangeTrackingAndObjectState();
		}

		internal override void Save(bool keepUnmanagedResources)
		{
			if (ObjectState.Unchanged == base.ObjectState)
			{
				return;
			}
			if (ObjectState.Deleted == base.ObjectState)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionObjectStateInvalid(base.ObjectState.ToString()));
			}
			base.EnableDisposeTracking();
			using (MapiStore messageStore = this.GetMessageStore())
			{
				MapiFolder mapiFolder = null;
				MapiFolder parentFolder = null;
				bool isFolderRetrieved = false;
				try
				{
					MapiSession mapiSession = base.MapiSession;
					Folder.CheckRequirementsOnSessionToContinue(base.MapiSession);
					bool creating = false;
					if (base.ObjectState == ObjectState.New)
					{
						creating = true;
						string text = null;
						if (this.newFolderIdentity == null)
						{
							if (null == this.Identity)
							{
								throw new MapiInvalidOperationException(Strings.ExceptionIdentityNull);
							}
							if (null == this.Identity.MapiFolderPath)
							{
								throw new MapiInvalidOperationException(Strings.ExceptionIdentityInvalid);
							}
							MapiFolder mapiFolder2 = null;
							try
							{
								parentFolder = Folder.RetrieveParentMapiFolder(messageStore, this.Identity, ref mapiFolder2);
							}
							finally
							{
								if (mapiFolder2 != null)
								{
									mapiFolder2.Dispose();
								}
							}
							if (parentFolder != null && !parentFolder.AllowWarnings)
							{
								parentFolder.AllowWarnings = true;
							}
							text = this.Identity.MapiFolderPath.Name;
						}
						else
						{
							this.Identity = this.ConstructIdentity(null, MapiFolderPath.GenerateFolderPath(this.newFolderIdentity.ParentIdentity.MapiFolderPath, this.newFolderIdentity.FolderName, false), null);
							parentFolder = Folder.RetrieveMapiFolder(messageStore, this.newFolderIdentity.ParentIdentity);
							if (parentFolder != null && !parentFolder.AllowWarnings)
							{
								parentFolder.AllowWarnings = true;
							}
							text = this.newFolderIdentity.FolderName;
						}
						ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string>((long)this.GetHashCode(), "To Create Folder '{0}' on server '{1}'.", this.Identity, base.MapiSession.ServerName);
						try
						{
							mapiFolder = parentFolder.CreateFolder(text, string.Empty, false);
							if (mapiFolder != null && !mapiFolder.AllowWarnings)
							{
								mapiFolder.AllowWarnings = true;
							}
							isFolderRetrieved = true;
						}
						catch (MapiExceptionCollision innerException)
						{
							this.DetectFolderExistence(messageStore, parentFolder, text, innerException);
							throw;
						}
						this.UpdateIdentity(this.UpdateIdentityFlagsForCreating);
					}
					if (creating || ObjectState.Changed == base.ObjectState)
					{
						Folder.CheckRequirementsOnIdentityToContinue(this.Identity);
						this.DetectDisallowedModification(messageStore);
						if (ObjectState.Changed == base.ObjectState && base.IsChanged(MapiPropertyDefinitions.Name))
						{
							string name = (string)this[MapiPropertyDefinitions.Name];
							if (parentFolder == null)
							{
								MapiFolder mapiFolder3 = null;
								try
								{
									parentFolder = Folder.RetrieveParentMapiFolder(messageStore, this.Identity, ref mapiFolder3);
								}
								finally
								{
									if (mapiFolder3 != null)
									{
										mapiFolder3.Dispose();
									}
								}
							}
							this.DetectFolderExistence(messageStore, parentFolder, name, null);
						}
						PropProblem[] array = null;
						PropProblem[] array2 = null;
						FolderId folderId = null;
						if (mapiFolder != null)
						{
							mapiFolder.Dispose();
							mapiFolder = null;
						}
						mapiFolder = Folder.RetrieveMapiFolder(messageStore, this.Identity, ref parentFolder, new Folder.IdentityConstructor(this.ConstructIdentity), out folderId);
						ParameterlessReturnlessDelegate parameterlessReturnlessDelegate = delegate()
						{
							if (creating && isFolderRetrieved)
							{
								ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string>((long)this.GetHashCode(), "To Remove Folder '{0}' for rolling back creating on server '{1}'.", this.Identity, this.MapiSession.ServerName);
								try
								{
									parentFolder.DeleteFolder((byte[])this.Identity.MapiEntryId);
								}
								catch (MapiRetryableException ex)
								{
									ExTraceGlobals.FolderTracer.TraceError<FolderId, string, string>((long)this.GetHashCode(), "Removing Folder '{0}' for rolling back caughting an exception on server '{1}': {2}", this.Identity, this.MapiSession.ServerName, ex.Message);
								}
								catch (MapiPermanentException ex2)
								{
									ExTraceGlobals.FolderTracer.TraceError<FolderId, string, string>((long)this.GetHashCode(), "Removing Folder '{0}' for rolling back caughting an exception on server '{1}': {2}", this.Identity, this.MapiSession.ServerName, ex2.Message);
								}
							}
						};
						try
						{
							PropValue[] propertyValuesToUpdate = base.GetPropertyValuesToUpdate();
							if (0 < propertyValuesToUpdate.Length)
							{
								ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string>((long)this.GetHashCode(), "To Set PropValues against Folder '{0}' on server '{1}'.", this.Identity, base.MapiSession.ServerName);
								array = mapiFolder.SetProps(propertyValuesToUpdate);
							}
							PropTag[] propertyTagsToDelete = base.GetPropertyTagsToDelete();
							if (0 < propertyTagsToDelete.Length)
							{
								ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string>((long)this.GetHashCode(), "To Delete PropTags against Folder '{0}' on server '{1}'.", this.Identity, base.MapiSession.ServerName);
								array2 = mapiFolder.DeleteProps(propertyTagsToDelete);
							}
						}
						catch (DataValidationException)
						{
							parameterlessReturnlessDelegate();
							throw;
						}
						catch (MapiRetryableException)
						{
							parameterlessReturnlessDelegate();
							throw;
						}
						catch (MapiPermanentException)
						{
							parameterlessReturnlessDelegate();
							throw;
						}
						base.ResetChangeTrackingAndObjectState();
						StringBuilder stringBuilder = new StringBuilder();
						if (array != null && 0 < array.Length)
						{
							stringBuilder.AppendLine(Strings.ErrorSetPropsProblem(this.Identity.ToString(), array.Length.ToString()));
							foreach (PropProblem propProblem in array)
							{
								stringBuilder.Append('\t');
								stringBuilder.AppendLine(Strings.ErrorPropProblem(propProblem.PropTag.ToString(), propProblem.PropType.ToString(), propProblem.Scode.ToString()));
							}
							stringBuilder.AppendLine();
						}
						if (array2 != null && 0 < array2.Length)
						{
							stringBuilder.AppendLine(Strings.ErrorDeletePropsProblem(this.Identity.ToString(), array2.Length.ToString()));
							foreach (PropProblem propProblem2 in array2)
							{
								stringBuilder.Append('\t');
								stringBuilder.Append(Strings.ErrorPropProblem(propProblem2.PropTag.ToString(), propProblem2.PropType.ToString(), propProblem2.Scode.ToString()));
								stringBuilder.AppendLine();
							}
							stringBuilder.AppendLine();
						}
						if (0 < stringBuilder.Length)
						{
							string text2 = stringBuilder.ToString();
							ExTraceGlobals.FolderTracer.TraceDebug<FolderId, string, string>((long)this.GetHashCode(), "Setting Folder '{0}' on server '{1}' returns error '{2}'.", this.Identity, base.MapiSession.ServerName, text2);
							parameterlessReturnlessDelegate();
							throw new MapiInvalidOperationException(new LocalizedString(text2));
						}
					}
				}
				finally
				{
					if (mapiFolder != null)
					{
						mapiFolder.Dispose();
					}
					if (parentFolder != null)
					{
						parentFolder.Dispose();
					}
					base.Dispose();
				}
			}
		}

		internal override T[] Find<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int maximumResultsSize)
		{
			return new List<T>(this.FindPaged<T>(filter, root, scope, sort, 0, maximumResultsSize)).ToArray();
		}

		internal override IEnumerable<T> FindPaged<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int pageSize, int maximumResultsSize)
		{
			Folder.<FindPaged>d__1a<T> <FindPaged>d__1a = new Folder.<FindPaged>d__1a<T>(-2);
			<FindPaged>d__1a.<>4__this = this;
			<FindPaged>d__1a.<>3__filter = filter;
			<FindPaged>d__1a.<>3__root = root;
			<FindPaged>d__1a.<>3__scope = scope;
			<FindPaged>d__1a.<>3__sort = sort;
			<FindPaged>d__1a.<>3__pageSize = pageSize;
			<FindPaged>d__1a.<>3__maximumResultsSize = maximumResultsSize;
			return <FindPaged>d__1a;
		}

		private static void CheckRequirementsToContinue(MapiObjectId identity, MapiSession session)
		{
			Folder.CheckRequirementsOnIdentityToContinue(identity);
			Folder.CheckRequirementsOnSessionToContinue(session);
		}

		private static void CheckRequirementsOnIdentityToContinue(MapiObjectId identity)
		{
			if (null == identity)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionIdentityNull);
			}
			FolderId folderId = identity as FolderId;
			if (null == folderId)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionIdentityTypeInvalid);
			}
			if (null == folderId.MapiEntryId && null == folderId.MapiFolderPath && string.IsNullOrEmpty(folderId.LegacyDistinguishedName))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionIdentityInvalid);
			}
		}

		private static void CheckRequirementsOnSessionToContinue(MapiSession session)
		{
			if (session == null)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSessionNull);
			}
			if (!(session is MapiMessageStoreSession))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSessionInvalid);
			}
		}

		private void DetectDisallowedModification(MapiStore messageStore)
		{
			this.UpdateIdentity(MapiObject.UpdateIdentityFlags.EntryIdentity | MapiObject.UpdateIdentityFlags.SkipIfExists);
			MapiEntryId operand = new MapiEntryId(messageStore.GetIpmSubtreeFolderEntryId());
			MapiEntryId operand2 = new MapiEntryId(messageStore.GetNonIpmSubtreeFolderEntryId());
			MapiEntryId operand3 = new MapiEntryId(messageStore.GetEFormsRegistryFolderEntryId());
			if (this.Identity.MapiEntryId == operand)
			{
				throw new ModificationDisallowedException(Strings.ExceptionModifyIpmSubtree);
			}
			if (this.Identity.MapiEntryId == operand2)
			{
				throw new ModificationDisallowedException(Strings.ExceptionModifyNonIpmSubtree);
			}
			if (this.Identity.MapiEntryId == operand3)
			{
				this.UpdateIdentityFolderPath();
				throw new ModificationDisallowedException(Strings.ExceptionModifyFolder(this.Identity.MapiFolderPath.ToString()));
			}
		}

		private void DetectFolderExistence(MapiStore messageStore, MapiFolder parentFolder, string name, Exception innerException)
		{
			try
			{
				MapiFolderPath mapiFolderPath = null;
				using (MapiFolder mapiFolder = parentFolder.OpenSubFolderByName(name))
				{
					mapiFolderPath = Folder.GetFolderPath(null, mapiFolder, messageStore);
				}
				throw new FolderAlreadyExistsException(mapiFolderPath.ToString(), innerException);
			}
			catch (MapiPermanentException ex)
			{
				ExTraceGlobals.FolderTracer.TraceError<string, string>((long)this.GetHashCode(), "Detecting Folder existence '{0}' caught an exception: '{1}'", name, ex.Message);
			}
			catch (MapiRetryableException ex2)
			{
				ExTraceGlobals.FolderTracer.TraceError<string, string>((long)this.GetHashCode(), "Detecting Folder existence '{0}' caught an exception: '{1}'", name, ex2.Message);
			}
		}

		private bool TryTestHasSubFolders(MapiFolder mapiFolder, out bool hasSubFolders)
		{
			hasSubFolders = false;
			object obj = null;
			if (this.propertyBag.TryGetField(MapiPropertyDefinitions.HasSubFolders, ref obj) && obj != null)
			{
				hasSubFolders = (bool)obj;
				return true;
			}
			try
			{
				PropValue prop = mapiFolder.GetProp(PropTag.SubFolders);
				if (PropType.Error != prop.PropType)
				{
					hasSubFolders = prop.GetBoolean();
					return true;
				}
			}
			catch (MapiPermanentException ex)
			{
				ExTraceGlobals.FolderTracer.TraceError<FolderId, string>((long)this.GetHashCode(), "TryTestHasSubFolders for folder '{0}' caught an exception: '{1}'", this.Identity, ex.Message);
			}
			catch (MapiRetryableException ex2)
			{
				ExTraceGlobals.FolderTracer.TraceError<FolderId, string>((long)this.GetHashCode(), "TryTestHasSubFolders for folder '{0}' caught an exception: '{1}'", this.Identity, ex2.Message);
			}
			return false;
		}

		internal object OpenProperty(MapiStore messageStore, PropTag propTag, Guid interfaceId, int interfaceOptions, OpenPropertyFlags flags)
		{
			object result;
			using (MapiFolder mapiFolder = this.GetMapiFolder(messageStore))
			{
				if (mapiFolder == null)
				{
					throw new MapiInvalidOperationException(Strings.ExceptionRawMapiEntryNull);
				}
				result = mapiFolder.OpenProperty(propTag, interfaceId, interfaceOptions, flags);
			}
			return result;
		}

		internal sealed override MapiProp GetRawMapiEntry(out MapiStore store)
		{
			store = this.GetMessageStore();
			return this.GetMapiFolder(store);
		}

		internal void SetNewFolderIdentity(FolderId parentId, string folderName)
		{
			if (base.ObjectState != ObjectState.New)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionObjectStateInvalid(base.ObjectState.ToString()));
			}
			if (null == parentId)
			{
				throw new ArgumentNullException("parentId");
			}
			IList<ValidationError> list = MapiPropertyDefinitions.Name.ValidateProperty(folderName, null, false);
			if (list != null && 0 < list.Count)
			{
				throw new DataValidationException(list[0]);
			}
			this.newFolderIdentity = new Folder.NewFolderIdentity(parentId, folderName);
		}

		internal void UpdateIdentityEntryId()
		{
			this.UpdateIdentityEntryId(true);
		}

		internal void UpdateIdentityEntryId(bool force)
		{
			MapiObject.UpdateIdentityFlags flags = MapiObject.UpdateIdentityFlags.EntryIdentity;
			if (!force)
			{
				flags |= MapiObject.UpdateIdentityFlags.SkipIfExists;
			}
			base.MapiSession.InvokeWithWrappedException(delegate()
			{
				this.UpdateIdentity(flags);
			}, Strings.ErrorCannotUpdateIdentityEntryId(this.Identity.ToString()), null);
		}

		internal void UpdateIdentityFolderPath()
		{
			this.UpdateIdentityFolderPath(true);
		}

		internal void UpdateIdentityFolderPath(bool force)
		{
			MapiObject.UpdateIdentityFlags flags = MapiObject.UpdateIdentityFlags.FolderPath;
			if (!force)
			{
				flags |= MapiObject.UpdateIdentityFlags.SkipIfExists;
			}
			base.MapiSession.InvokeWithWrappedException(delegate()
			{
				this.UpdateIdentity(flags);
			}, Strings.ErrorCannotUpdateIdentityFolderPath(this.Identity.ToString()), null);
		}

		internal void UpdateIdentityLegacyDistinguishedName()
		{
			this.UpdateIdentityLegacyDistinguishedName(true);
		}

		internal void UpdateIdentityLegacyDistinguishedName(bool force)
		{
			MapiObject.UpdateIdentityFlags flags = MapiObject.UpdateIdentityFlags.LegacyDistinguishedName;
			if (!force)
			{
				flags |= MapiObject.UpdateIdentityFlags.SkipIfExists;
			}
			base.MapiSession.InvokeWithWrappedException(delegate()
			{
				this.UpdateIdentity(flags);
			}, Strings.ErrorCannotUpdateIdentityLegacyDistinguishedName(this.Identity.ToString()), null);
		}

		protected override MapiObject.RetrievePropertiesScope RetrievePropertiesScopeForFinding
		{
			get
			{
				return MapiObject.RetrievePropertiesScope.Instance;
			}
		}

		protected override void UpdateIdentity(MapiObject.UpdateIdentityFlags flags)
		{
			this.UpdateIdentity(flags, null);
		}

		protected void UpdateIdentity(MapiObject.UpdateIdentityFlags flags, MapiFolderPath parentPath)
		{
			bool flag = MapiObject.UpdateIdentityFlags.Nop == (MapiObject.UpdateIdentityFlags.Offline & flags);
			bool flag2 = MapiObject.UpdateIdentityFlags.Nop == (MapiObject.UpdateIdentityFlags.SkipIfExists & flags);
			flag &= (base.MapiSession != null && base.MapiSession.IsConnectionConfigurated && base.MapiSession is MapiMessageStoreSession);
			MapiEntryId entryId = this.Identity.MapiEntryId;
			MapiFolderPath folderPath = this.Identity.MapiFolderPath;
			string legacyDn = this.Identity.LegacyDistinguishedName;
			if (null == this.Identity.MapiEntryId || flag2)
			{
				MapiEntryId mapiEntryId = (MapiEntryId)this[MapiPropertyDefinitions.EntryId];
				if (null != mapiEntryId)
				{
					entryId = mapiEntryId;
				}
			}
			if ((null == this.Identity.MapiFolderPath || flag2) && null != parentPath)
			{
				string text = (string)this[MapiPropertyDefinitions.Name];
				if (!string.IsNullOrEmpty(text))
				{
					folderPath = MapiFolderPath.GenerateFolderPath(parentPath, text, false);
				}
			}
			this.Identity = this.ConstructIdentity(entryId, folderPath, legacyDn);
			if (flag)
			{
				if ((MapiObject.UpdateIdentityFlags.EntryIdentity & flags) != MapiObject.UpdateIdentityFlags.Nop && (null == this.Identity.MapiEntryId || flag2))
				{
					using (MapiStore messageStore = this.GetMessageStore())
					{
						using (MapiFolder mapiFolder = this.GetMapiFolder(messageStore))
						{
							entryId = MapiObject.GetEntryIdentity(mapiFolder);
						}
					}
				}
				if ((MapiObject.UpdateIdentityFlags.FolderPath & flags) != MapiObject.UpdateIdentityFlags.Nop && (null == this.Identity.MapiFolderPath || flag2))
				{
					using (MapiStore messageStore2 = this.GetMessageStore())
					{
						using (MapiFolder mapiFolder2 = this.GetMapiFolder(messageStore2))
						{
							if (null == parentPath)
							{
								folderPath = Folder.GetFolderPath(this.Identity.MapiEntryId, mapiFolder2, messageStore2);
							}
							else
							{
								string name = MapiObject.GetName(mapiFolder2);
								if (!string.IsNullOrEmpty(name))
								{
									folderPath = MapiFolderPath.GenerateFolderPath(parentPath, name, false);
								}
							}
						}
					}
				}
				if ((MapiObject.UpdateIdentityFlags.LegacyDistinguishedName & flags) != MapiObject.UpdateIdentityFlags.Nop && (string.IsNullOrEmpty(this.Identity.LegacyDistinguishedName) || flag2))
				{
					using (MapiStore messageStore3 = this.GetMessageStore())
					{
						using (MapiFolder mapiFolder3 = this.GetMapiFolder(messageStore3))
						{
							legacyDn = Folder.GetFolderLegacyDistinguishedName((MapiEntryId)this[MapiPropertyDefinitions.AddressBookEntryId], mapiFolder3);
						}
					}
				}
			}
			this.Identity = this.ConstructIdentity(entryId, folderPath, legacyDn);
		}

		protected abstract FolderId ConstructIdentity(MapiEntryId entryId, MapiFolderPath folderPath, string legacyDn);

		internal abstract MapiStore GetMessageStore();

		internal MapiFolder GetParentFolder(MapiStore messageStore)
		{
			MapiFolder mapiFolder = null;
			MapiFolder mapiFolder2 = null;
			FolderId identity = null;
			try
			{
				if (null == this.Identity.MapiFolderPath)
				{
					mapiFolder2 = Folder.RetrieveMapiFolder(messageStore, this.Identity, ref mapiFolder, new Folder.IdentityConstructor(this.ConstructIdentity), out identity);
					this.Identity = identity;
				}
				else
				{
					mapiFolder2 = this.GetMapiFolder(messageStore);
					mapiFolder = Folder.RetrieveParentMapiFolder(messageStore, this.Identity, ref mapiFolder2);
				}
				mapiFolder.AllowWarnings = true;
			}
			finally
			{
				if (mapiFolder2 != null)
				{
					mapiFolder2.Dispose();
				}
			}
			return mapiFolder;
		}

		internal MapiFolder GetMapiFolder(MapiStore messageStore)
		{
			MapiFolder mapiFolder = null;
			MapiFolder mapiFolder2 = null;
			FolderId identity = null;
			try
			{
				mapiFolder = Folder.RetrieveMapiFolder(messageStore, this.Identity, ref mapiFolder2, new Folder.IdentityConstructor(this.ConstructIdentity), out identity);
			}
			finally
			{
				if (mapiFolder2 != null)
				{
					mapiFolder2.Dispose();
				}
			}
			this.Identity = identity;
			mapiFolder.AllowWarnings = true;
			return mapiFolder;
		}

		public Folder()
		{
		}

		internal Folder(FolderId mapiObjectId, MapiSession mapiSession) : base(mapiObjectId, mapiSession)
		{
		}

		private Folder.NewFolderIdentity newFolderIdentity;

		internal delegate FolderId IdentityConstructor(MapiEntryId entryId, MapiFolderPath folderPath, string legacyDn);

		[Serializable]
		private struct FolderRawPresentation
		{
			public FolderRawPresentation(MapiFolderPath parent, PropValue[] properties)
			{
				this.Parent = parent;
				this.Properties = properties;
			}

			public MapiFolderPath Parent;

			public PropValue[] Properties;
		}

		[Serializable]
		private class NewFolderIdentity
		{
			public NewFolderIdentity(FolderId parent, string name)
			{
				this.ParentIdentity = parent;
				this.FolderName = name;
			}

			public FolderId ParentIdentity;

			public string FolderName;
		}
	}
}
