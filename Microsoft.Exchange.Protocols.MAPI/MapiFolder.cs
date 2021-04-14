using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiFolder : MapiPropBagBase
	{
		private MapiFolder() : base(MapiObjectType.Folder)
		{
		}

		public ExchangeId Fid
		{
			get
			{
				base.ThrowIfNotValid(null);
				return this.fid.ConvertNullToZero();
			}
		}

		protected override PropertyBag StorePropertyBag
		{
			get
			{
				base.ThrowIfNotValid(null);
				return this.storeFolder;
			}
		}

		internal Folder StoreFolder
		{
			get
			{
				base.ThrowIfNotValid(null);
				return this.storeFolder;
			}
		}

		internal AccessCheckState AccessCheckState
		{
			get
			{
				return this.accessCheckState;
			}
		}

		public bool IsInstantSearch
		{
			get
			{
				return this.isInstantSearch;
			}
		}

		public bool IsOptimizedConversationSearch
		{
			get
			{
				return this.isOptimizedConversationSearch;
			}
		}

		internal static void Initialize()
		{
			if (MapiFolder.deleteInstantSearchActionSlot == -1)
			{
				MapiFolder.deleteInstantSearchActionSlot = LazyMailboxActionList.AllocateSlot();
			}
		}

		public static MapiFolder OpenFolder(MapiContext context, MapiLogon logon, ExchangeId fid)
		{
			if (fid.IsNullOrZero)
			{
				return null;
			}
			if (!logon.UnifiedLogon)
			{
				FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, logon.MapiMailbox.StoreMailbox, logon.FidC.FidRoot.ToExchangeShortId(), FolderInformationType.Basic);
				if (folderHierarchy.Find(context, fid.ToExchangeShortId()) == null)
				{
					return null;
				}
			}
			bool flag = false;
			MapiFolder mapiFolder = new MapiFolder();
			try
			{
				ErrorCode first = mapiFolder.Configure(context, logon, fid);
				if (first == ErrorCode.NoError)
				{
					mapiFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem, AccessCheckOperation.FolderOpen, (LID)46439U);
					flag = true;
				}
			}
			finally
			{
				if (!flag)
				{
					mapiFolder.Dispose();
					mapiFolder = null;
				}
			}
			return mapiFolder;
		}

		public static ErrorCode CreateFolder(MapiContext context, MapiLogon logon, ref ExchangeId fid, bool skipWriteableHierarchyCheck, FolderConfigureFlags flags, MapiFolder parentFolder, bool createAssociatedDumpsterFolder, out MapiFolder newFolder)
		{
			return MapiFolder.CreateFolder(context, logon, ref fid, skipWriteableHierarchyCheck, flags, parentFolder, null, createAssociatedDumpsterFolder, out newFolder);
		}

		private static ErrorCode CreateFolder(MapiContext context, MapiLogon logon, ref ExchangeId fid, bool skipWriteableHierarchyCheck, FolderConfigureFlags flags, MapiFolder parentFolder, MapiFolder sourceFolder, bool createAssociatedDumpsterFolder, out MapiFolder newFolder)
		{
			newFolder = null;
			if ((flags & FolderConfigureFlags.InternalAccess) != FolderConfigureFlags.None && !context.HasInternalAccessRights)
			{
				throw new StoreException((LID)64716U, ErrorCodeValue.NoAccess, "InternalAccess privileges are required.");
			}
			if (parentFolder != null)
			{
				parentFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.AppendMsg, AccessCheckOperation.FolderCreate, (LID)62823U);
				if (parentFolder.IsSearchFolder())
				{
					throw new StoreException((LID)60892U, ErrorCodeValue.SearchFolder, "Search folder cannot have children");
				}
			}
			else if (!context.HasMailboxFullRights)
			{
				if (ExTraceGlobals.AccessCheckTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.AccessCheckTracer.TraceError(0L, "Trying to create root folder without full rights");
				}
				throw new ExExceptionAccessDenied((LID)44792U, "Trying to create root folder without full rights.");
			}
			if (!fid.IsNullOrZero)
			{
				using (MapiFolder mapiFolder = MapiFolder.OpenFolder(context, logon, fid))
				{
					if (mapiFolder != null)
					{
						if (ExTraceGlobals.AccessCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.AccessCheckTracer.TraceError<ExchangeId>(0L, "Folder {0} already exists in folders table", fid);
						}
						throw new ExExceptionAccessDenied((LID)52096U, "Trying to create a folder which already exists.");
					}
				}
			}
			bool flag = FolderConfigureFlags.None != (flags & FolderConfigureFlags.CreateSearchFolder);
			if (flag && logon.MapiMailbox.IsPublicFolderMailbox && context.ClientType != ClientType.PublicFolderSystem && context.ClientType != ClientType.Migration)
			{
				throw new StoreException((LID)58976U, ErrorCodeValue.NotSupported, "Creating search folder on public folder is not supported");
			}
			if (!skipWriteableHierarchyCheck)
			{
				switch (logon.MapiMailbox.SharedState.MailboxType)
				{
				case MailboxInfo.MailboxType.PublicFolderPrimary:
					if (context.ClientType == ClientType.PublicFolderSystem)
					{
						throw new ExExceptionAccessDenied((LID)39264U, "Unexpected client is trying to create a folder in a public folder primary mailbox.");
					}
					break;
				case MailboxInfo.MailboxType.PublicFolderSecondary:
					if (context.ClientType != ClientType.Migration && context.ClientType != ClientType.PublicFolderSystem)
					{
						throw new ExExceptionAccessDenied((LID)58720U, "Unexpected client is trying to create a folder in a public folder secondary mailbox.");
					}
					break;
				}
			}
			bool flag2 = false;
			ErrorCode first = ErrorCode.NoError;
			MapiFolder mapiFolder2 = new MapiFolder();
			try
			{
				first = mapiFolder2.ConfigureNew(context, logon, ref fid, flags, parentFolder, sourceFolder);
				if (first == ErrorCode.NoError && !flag && createAssociatedDumpsterFolder)
				{
					using (context.GrantMailboxFullRights())
					{
						using (MapiFolder mapiFolder3 = MapiFolder.OpenFolder(context, logon, logon.FidC.FidPublicFolderDumpsterRoot))
						{
							if (mapiFolder3 == null)
							{
								throw new StoreException((LID)41408U, ErrorCodeValue.NotFound);
							}
							MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder3, mapiFolder2, createAssociatedDumpsterFolder);
						}
					}
				}
				flag2 = true;
			}
			finally
			{
				if (!flag2 || first != ErrorCode.NoError)
				{
					mapiFolder2.Dispose();
					mapiFolder2 = null;
				}
			}
			newFolder = mapiFolder2;
			return first.Propagate((LID)38201U);
		}

		internal static bool ManageAssociatedDumpsterFolder(MapiLogon logon, bool createPublicFolderDumpster)
		{
			return logon.MapiMailbox.SharedState.MailboxType == MailboxInfo.MailboxType.PublicFolderPrimary && (!logon.IsMoveUser || createPublicFolderDumpster);
		}

		internal static void CreateAssociatedDumpsterFolder(MapiContext context, MapiLogon logon, MapiFolder dumpsterRootFolder, MapiFolder folder)
		{
			MapiFolder.CreateAssociatedDumpsterFolder(context, logon, dumpsterRootFolder, folder, false);
		}

		internal static void CreateAssociatedDumpsterFolder(MapiContext context, MapiLogon logon, MapiFolder dumpsterRootFolder, MapiFolder folder, bool createPublicFolderDumpster)
		{
			MapiFolder mapiFolder = null;
			try
			{
				if (!MapiFolder.IsCurrentDumpsterHolderValid(context, dumpsterRootFolder, out mapiFolder))
				{
					mapiFolder = MapiFolder.RecomputePublicFolderDumpsterHolder(context, dumpsterRootFolder);
				}
				UnlimitedItems effectivePublicFolderDumpsterHierarchyChildrenQuota = MapiFolder.GetEffectivePublicFolderDumpsterHierarchyChildrenQuota(dumpsterRootFolder.StoreFolder.Mailbox);
				if ((long)mapiFolder.GetChildFolderCount(context) >= effectivePublicFolderDumpsterHierarchyChildrenQuota.Value)
				{
					MapiFolder.AdvancePublicFolderDumpsterHolder(context, effectivePublicFolderDumpsterHierarchyChildrenQuota, dumpsterRootFolder, ref mapiFolder);
				}
				folder.InternalCreateAssociatedDumpsterFolder(context, mapiFolder);
			}
			finally
			{
				if (mapiFolder != null)
				{
					mapiFolder.Dispose();
				}
			}
		}

		private static MapiFolder RecomputePublicFolderDumpsterHolder(MapiContext context, MapiFolder dumpsterRootFolder)
		{
			MapiFolder mapiFolder = null;
			MapiFolder result;
			using (MapiFolder dumpsterExtendPublicFolder = MapiFolder.GetDumpsterExtendPublicFolder(context, dumpsterRootFolder))
			{
				using (MapiFolder mapiFolder2 = MapiFolder.CreateReservedSubFolderForPublicFolderDumpsterStructure(context, dumpsterExtendPublicFolder, true, (LID)51644U))
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						mapiFolder = MapiFolder.CreateReservedSubFolderForPublicFolderDumpsterStructure(context, mapiFolder2, true, (LID)55740U);
						disposeGuard.Add<MapiFolder>(mapiFolder);
						dumpsterRootFolder.SetCurrentPublicFolderDumpsterHolder(context, mapiFolder.Fid);
						disposeGuard.Success();
					}
					result = mapiFolder;
				}
			}
			return result;
		}

		internal static void AdvancePublicFolderDumpsterHolder(MapiContext context, UnlimitedItems quotaToUse, MapiFolder dumpsterRootFolder, ref MapiFolder currentDumpsterHolder)
		{
			ErrorCode noError = ErrorCode.NoError;
			MapiFolder mapiFolder = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				using (MapiFolder parentFolder = currentDumpsterHolder.GetParentFolder(context))
				{
					if (parentFolder == null)
					{
						throw new StoreException((LID)33212U, ErrorCodeValue.NotFound);
					}
					if ((long)parentFolder.GetChildFolderCount(context) < quotaToUse.Value)
					{
						mapiFolder = MapiFolder.CreateReservedSubFolderForPublicFolderDumpsterStructure(context, parentFolder, false, (LID)39356U);
						disposeGuard.Add<MapiFolder>(mapiFolder);
					}
					else
					{
						using (MapiFolder parentFolder2 = parentFolder.GetParentFolder(context))
						{
							if (parentFolder2 == null)
							{
								throw new StoreException((LID)59836U, ErrorCodeValue.NotFound);
							}
							if ((long)parentFolder2.GetChildFolderCount(context) < quotaToUse.Value)
							{
								using (MapiFolder mapiFolder2 = MapiFolder.CreateReservedSubFolderForPublicFolderDumpsterStructure(context, parentFolder2, false, (LID)45500U))
								{
									mapiFolder = MapiFolder.CreateReservedSubFolderForPublicFolderDumpsterStructure(context, mapiFolder2, false, (LID)61884U);
									disposeGuard.Add<MapiFolder>(mapiFolder);
									goto IL_EB;
								}
								goto IL_D1;
								IL_EB:
								goto IL_F7;
							}
							IL_D1:
							throw new StoreException((LID)41404U, ErrorCodeValue.PublicFolderDumpstersLimitExceeded, "The maximum number of dumpster public folders allowed has been reached.");
						}
					}
					IL_F7:;
				}
				dumpsterRootFolder.SetCurrentPublicFolderDumpsterHolder(context, mapiFolder.Fid);
				disposeGuard.Success();
			}
			if (currentDumpsterHolder != null)
			{
				currentDumpsterHolder.Dispose();
			}
			currentDumpsterHolder = mapiFolder;
		}

		private static MapiFolder GetDumpsterExtendPublicFolder(MapiContext context, MapiFolder parentFolder)
		{
			ExchangeId exchangeId = Folder.FindFolderIdByName(context, parentFolder.Fid, "DUMPSTER_EXTEND", parentFolder.Logon.StoreMailbox);
			MapiFolder result;
			if (exchangeId.IsValid)
			{
				result = MapiFolder.OpenFolder(context, parentFolder.Logon, exchangeId);
			}
			else
			{
				result = parentFolder.CreateFolderHelper(context, "DUMPSTER_EXTEND", (LID)49596U, "Could not create dumpster extend public folder.");
			}
			return result;
		}

		private static MapiFolder CreateReservedSubFolderForPublicFolderDumpsterStructure(MapiContext context, MapiFolder parentFolder, bool openIfExists, LID lid)
		{
			int childFolderCount = parentFolder.GetChildFolderCount(context);
			if (openIfExists && childFolderCount > 0)
			{
				ExchangeId exchangeId = Folder.FindFolderIdByName(context, parentFolder.Fid, "RESERVED_" + childFolderCount.ToString(), parentFolder.Logon.StoreMailbox);
				if (exchangeId.IsValid)
				{
					return MapiFolder.OpenFolder(context, parentFolder.Logon, exchangeId);
				}
			}
			return parentFolder.CreateFolderHelper(context, "RESERVED_" + (childFolderCount + 1).ToString(), lid, "Could not created the reserved sub folder for public folder dumpster structure");
		}

		internal static bool IsCurrentDumpsterHolderValid(MapiContext context, MapiFolder dumpsterRootFolder, out MapiFolder currentDumpsterHolder)
		{
			currentDumpsterHolder = null;
			object value = dumpsterRootFolder.InternalGetOneProp(context, PropTag.Folder.CurrentIPMWasteBasketContainerEntryId).Value;
			return value != LegacyHelper.BoxedErrorCodeNotFound && MapiFolder.IsValidFolderEntryId(context, dumpsterRootFolder.Logon, dumpsterRootFolder.StoreFolder.ReplidGuidMap, value as byte[], out currentDumpsterHolder);
		}

		internal static UnlimitedItems GetEffectivePublicFolderDumpsterHierarchyChildrenQuota(Mailbox mailbox)
		{
			UnlimitedItems unlimitedItems = mailbox.QuotaInfo.FolderHierarchyChildrenCountReceiveQuota;
			if (unlimitedItems.IsUnlimited || unlimitedItems > ConfigurationSchema.MaxChildCountForDumpsterHierarchyPublicFolder.Value)
			{
				unlimitedItems = ConfigurationSchema.MaxChildCountForDumpsterHierarchyPublicFolder.Value;
			}
			return unlimitedItems;
		}

		private static bool IsValidFolderEntryId(MapiContext context, MapiLogon logon, IReplidGuidMap replidGuidMap, byte[] folderIdBytes, out MapiFolder folder)
		{
			folder = null;
			if (folderIdBytes != null && folderIdBytes.Length == 22)
			{
				ExchangeId exchangeId = ExchangeId.CreateFrom22ByteArray(context, replidGuidMap, folderIdBytes);
				if (exchangeId.IsValid)
				{
					folder = MapiFolder.OpenFolder(context, logon, exchangeId);
					if (folder != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool HasFolderAccessRights(MapiContext context, ExchangeShortId folderId, bool isSearchFolder, AccessCheckState accessCheckState, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, AccessCheckOperation operation, LID lid)
		{
			bool flag;
			if (isSearchFolder && context.LockedMailboxState.MailboxTypeDetail != MailboxInfo.MailboxTypeDetail.GroupMailbox)
			{
				flag = context.HasMailboxFullRights;
				if (!flag)
				{
					DiagnosticContext.TraceDword((LID)62343U, (uint)operation);
					DiagnosticContext.TraceDword(lid, (uint)operation);
				}
			}
			else
			{
				flag = accessCheckState.CheckFolderRights(context, requestedRights, allRights);
				if (!flag)
				{
					DiagnosticContext.TraceDword(lid, (uint)operation);
					DiagnosticContext.TraceDword((LID)58216U, (uint)accessCheckState.GetFolderRights(context));
				}
			}
			if (ExTraceGlobals.AccessCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug(0L, "MapiFolder({0}) folder access check: Operation={1}, Requested rights={2}, Allowed={3}", new object[]
				{
					folderId,
					operation,
					requestedRights,
					flag
				});
			}
			return flag;
		}

		internal static void CheckFolderRights(MapiContext context, ExchangeShortId folderId, bool isSearchFolder, bool isInternalAccess, AccessCheckState accessCheckState, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, AccessCheckOperation operation, LID lid)
		{
			if (isInternalAccess && !context.HasInternalAccessRights)
			{
				DiagnosticContext.TraceLocation((LID)46924U);
				throw new ExExceptionAccessDenied(lid, "Insufficient rights for InternalAccess folder.");
			}
			if (!MapiFolder.HasFolderAccessRights(context, folderId, isSearchFolder, accessCheckState, requestedRights, allRights, operation, lid))
			{
				throw new ExExceptionAccessDenied(lid, "Insufficient rights.");
			}
		}

		internal static void CheckMessageRights(MapiContext context, ExchangeShortId folderId, AccessCheckState accessCheckState, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, TopMessage topMessage, bool isNewMessage, AccessCheckOperation operation, LID lid)
		{
			if (isNewMessage)
			{
				return;
			}
			bool flag = accessCheckState.CheckMessageRights(context, requestedRights, allRights, new AccessCheckState.CreatorSecurityIdentifierAccessor(topMessage));
			if (ExTraceGlobals.AccessCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug(0L, "MapiFolder({0}) message access check: Operation={1}, Requested rights={2}, Allowed={3}", new object[]
				{
					folderId,
					operation,
					requestedRights,
					flag
				});
			}
			if (!flag)
			{
				DiagnosticContext.TraceDword(lid, (uint)operation);
				DiagnosticContext.TraceDword((LID)54120U, (uint)accessCheckState.GetFolderRights(context));
				DiagnosticContext.TraceDword((LID)41832U, (uint)accessCheckState.GetMessageRights(context, null));
				throw new ExExceptionAccessDenied(lid, "Insufficient rights.");
			}
		}

		internal static bool IsFolderSecurityRelatedProperty(uint propertyTag)
		{
			return propertyTag == 1715011587U || propertyTag == 267649027U;
		}

		internal static object CalculateSecurityRelatedProperty(MapiContext context, StorePropTag propertyTag, AccessCheckState accessCheckState)
		{
			FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights = accessCheckState.GetContextRights(context, false);
			if (exchangeSecurityDescriptorFolderRights != FolderSecurity.ExchangeSecurityDescriptorFolderRights.AllFolder)
			{
				exchangeSecurityDescriptorFolderRights |= accessCheckState.GetFolderRights(context);
			}
			FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights2 = accessCheckState.GetContextRights(context, true);
			if (exchangeSecurityDescriptorFolderRights2 != FolderSecurity.ExchangeSecurityDescriptorFolderRights.AllMessage)
			{
				exchangeSecurityDescriptorFolderRights2 |= accessCheckState.GetMessageRights(context, null);
			}
			FolderSecurity.ExchangeFolderRights exchangeFolderRights = FolderSecurity.FolderRightsFromSecurityDescriptorRights(exchangeSecurityDescriptorFolderRights, FolderSecurity.AceTarget.Folder) | FolderSecurity.FolderRightsFromSecurityDescriptorRights(exchangeSecurityDescriptorFolderRights2, FolderSecurity.AceTarget.Message);
			if (propertyTag == PropTag.Folder.Rights)
			{
				return (int)exchangeFolderRights;
			}
			if (propertyTag == PropTag.Folder.Access)
			{
				MapiAccess mapiAccess = MapiAccess.None;
				if ((exchangeFolderRights & FolderSecurity.ExchangeFolderRights.ReadAny) != FolderSecurity.ExchangeFolderRights.None)
				{
					mapiAccess |= MapiAccess.Read;
				}
				if ((exchangeFolderRights & FolderSecurity.ExchangeFolderRights.Owner) != FolderSecurity.ExchangeFolderRights.None)
				{
					mapiAccess |= (MapiAccess.Modify | MapiAccess.Delete);
				}
				if ((exchangeFolderRights & FolderSecurity.ExchangeFolderRights.CreateSubfolder) != FolderSecurity.ExchangeFolderRights.None)
				{
					mapiAccess |= MapiAccess.CreateHierarchy;
				}
				if ((exchangeFolderRights & FolderSecurity.ExchangeFolderRights.Create) != FolderSecurity.ExchangeFolderRights.None)
				{
					mapiAccess |= MapiAccess.CreateContent;
					if ((exchangeFolderRights & FolderSecurity.ExchangeFolderRights.Owner) != FolderSecurity.ExchangeFolderRights.None)
					{
						mapiAccess |= MapiAccess.CreateAssociated;
					}
				}
				return (int)mapiAccess;
			}
			return null;
		}

		public static void QueueInstantSearchDeletion(Mailbox mailbox, ExchangeId searchFolderId)
		{
			LazyMailboxActionList.AppendMailboxAction(MapiFolder.deleteInstantSearchActionSlot, mailbox.SharedState, delegate(Context context, Mailbox storeMailbox)
			{
				MapiFolder.DeleteInstantSearch(context, storeMailbox, searchFolderId);
			});
		}

		private static void DeleteInstantSearch(Context context, Mailbox mailbox, ExchangeId searchFolderId)
		{
			SearchFolder searchFolder = Folder.OpenFolder(context, mailbox, searchFolderId) as SearchFolder;
			if (searchFolder != null && searchFolder.GetAgeOutTimeout(context) == 0 && !searchFolder.IsSearchEvaluationInProgress(context))
			{
				searchFolder.Delete(context);
			}
		}

		public bool CheckAlive(MapiContext context)
		{
			return base.IsValid && this.StoreFolder != null && this.StoreFolder.CheckAlive(context);
		}

		protected override List<Property> InternalGetAllProperties(MapiContext context, GetPropListFlags flags, bool loadValues, Predicate<StorePropTag> propertyFilter)
		{
			List<Property> list = base.InternalGetAllProperties(context, flags, loadValues, propertyFilter);
			this.ReloadPropertyIntoPropertyList(context, list, loadValues, true, PropTag.Folder.SourceEntryId);
			this.ReloadPropertyIntoPropertyList(context, list, loadValues, true, PropTag.Folder.ReplicaList);
			if (this.Session.InternalClientType != ClientType.User && this.Session.InternalClientType != ClientType.RpcHttp && this.Session.InternalClientType != ClientType.MoMT)
			{
				this.ReloadPropertyIntoPropertyList(context, list, loadValues, false, PropTag.Folder.SentMailEntryId);
			}
			if (flags == GetPropListFlags.None)
			{
				this.ReloadPropertyIntoPropertyList(context, list, loadValues, false, PropTag.Folder.HasRules);
				this.ReloadPropertyIntoPropertyList(context, list, loadValues, false, PropTag.Folder.ResolveMethod);
				this.ReloadPropertyIntoPropertyList(context, list, loadValues, false, PropTag.Folder.PredecessorChangeList);
				this.ReloadPropertyIntoPropertyList(context, list, loadValues, false, PropTag.Folder.ICSChangeKey);
			}
			if (base.Logon.IsMoveUser && !this.FolderHasRealFreeBusyNTSD(context))
			{
				Properties.RemoveFrom(list, PropTag.Folder.FreeBusyNTSD);
			}
			if (!base.Logon.StoreMailbox.SharedState.SupportsPerUserFeatures)
			{
				Properties.RemoveFrom(list, PropTag.Folder.SearchGUID);
			}
			return list;
		}

		internal bool IsSearchFolder()
		{
			return this.storeFolder is SearchFolder;
		}

		public ExchangeId GetParentFid(MapiContext context)
		{
			base.ThrowIfNotValid(null);
			return this.storeFolder.GetParentFolderId(context);
		}

		public MapiFolder GetParentFolder(MapiContext context)
		{
			ExchangeId parentFid = this.GetParentFid(context);
			if (parentFid.IsNullOrZero)
			{
				return null;
			}
			MapiFolder mapiFolder = MapiFolder.OpenFolder(context, base.Logon, parentFid);
			if (mapiFolder == null)
			{
				throw new StoreException((LID)40892U, ErrorCodeValue.ObjectDeleted);
			}
			return mapiFolder;
		}

		public string GetDisplayName(MapiContext context)
		{
			base.ThrowIfNotValid(null);
			return this.storeFolder.GetName(context);
		}

		public string GetFolderPathName(MapiContext context, char pathSeparator)
		{
			base.ThrowIfNotValid(null);
			return Folder.GetFolderPathName(context, this.storeFolder.Mailbox, this.Fid, pathSeparator);
		}

		public int GetChildFolderCount(MapiContext context)
		{
			base.ThrowIfNotValid(null);
			return (int)this.InternalGetOneProp(context, PropTag.Folder.FolderChildCount).Value;
		}

		public ExchangeId GetAssociatedFolderId(Context context)
		{
			byte[] array = (byte[])this.storeFolder.GetPropertyValue(context, PropTag.Folder.IPMWastebasketEntryId);
			if (array != null && array.Length == 22)
			{
				return ExchangeId.CreateFrom22ByteArray(context, this.storeFolder.ReplidGuidMap, array);
			}
			return ExchangeId.Null;
		}

		public void SetAssociatedFolderId(Context context, ExchangeId value, bool markAsDumpsterFolder)
		{
			this.storeFolder.SetProperty(context, PropTag.Folder.IPMWastebasketEntryId, value.To22ByteArray());
			if (markAsDumpsterFolder)
			{
				FolderAdminFlags folderAdminFlags = (FolderAdminFlags)0;
				object propertyValue = this.storeFolder.GetPropertyValue(context, PropTag.Folder.FolderAdminFlags);
				if (propertyValue != null)
				{
					folderAdminFlags = (FolderAdminFlags)((int)propertyValue);
				}
				this.storeFolder.SetProperty(context, PropTag.Folder.FolderAdminFlags, (int)(folderAdminFlags | FolderAdminFlags.DumpsterFolder));
			}
		}

		public ErrorCode MoveMessageTo(MapiContext context, MapiFolder destination, ExchangeId mid, Properties propertyOverrides, out ExchangeId outputMid, out ExchangeId outputCn)
		{
			base.ThrowIfNotValid(null);
			destination.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody, AccessCheckOperation.FolderMoveMessageDestination, (LID)42343U);
			this.GhostedFolderCheck(context, (LID)60825U);
			destination.GhostedFolderCheck(context, (LID)51545U);
			ErrorCode first = MapiMailboxShape.PerformMailboxShapeQuotaCheck(context, destination.Logon, destination.StoreFolder, MapiMailboxShape.Operation.MoveMessage, false);
			if (first != ErrorCode.NoError)
			{
				outputMid = ExchangeId.Null;
				outputCn = ExchangeId.Null;
				return first.Propagate((LID)48192U);
			}
			foreach (Property property in propertyOverrides)
			{
				first = MapiMessage.CheckPropertyOperationAllowed(context, base.Logon, false, MapiPropBagBase.PropOperation.SetProps, property.Tag, property.Value);
				if (first != ErrorCode.NoError)
				{
					outputMid = ExchangeId.Null;
					outputCn = ExchangeId.Null;
					return first.Propagate((LID)41264U);
				}
			}
			using (TopMessage topMessage = TopMessage.OpenMessage(context, this.storeFolder.Mailbox, this.storeFolder.GetId(context), mid))
			{
				if (topMessage == null)
				{
					outputMid = ExchangeId.Null;
					outputCn = ExchangeId.Null;
					return ErrorCode.CreateNotFound((LID)65048U);
				}
				this.CheckMessageRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty | FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete, topMessage, AccessCheckOperation.FolderMoveMessageSource, (LID)38247U);
				if (PropertyBagHelpers.TestPropertyFlags(context, topMessage, PropTag.Message.MessageFlagsActual, 4, 4))
				{
					outputMid = ExchangeId.Null;
					outputCn = ExchangeId.Null;
					return ErrorCode.CreateNoDeleteSubmitMessage((LID)40472U);
				}
				if (!this.CheckLocalDirectoryMessageAndHasAdminRight(context, mid))
				{
					outputMid = ExchangeId.Null;
					outputCn = ExchangeId.Null;
					return ErrorCode.CreateNoAccess((LID)47824U);
				}
				topMessage.Move(context, destination.StoreFolder);
				if (destination.StoreFolder.IsDumpsterMarkedFolder(context))
				{
					MapiMessage.SendNotReadReportIfNeeded(context, base.Logon, topMessage, false);
				}
				foreach (Property property2 in propertyOverrides)
				{
					first = MapiMessage.InternalSetOneProp(context, base.Logon, topMessage, false, property2.Tag, property2.Value);
					if (first != ErrorCode.NoError)
					{
						outputMid = ExchangeId.Null;
						outputCn = ExchangeId.Null;
						return first.Propagate((LID)53552U);
					}
				}
				SaveMessageChangesFlags saveMessageChangesFlags = SaveMessageChangesFlags.None;
				if (!base.Logon.MapiMailbox.IsPublicFolderMailbox)
				{
					saveMessageChangesFlags |= SaveMessageChangesFlags.SkipFolderQuotaCheck;
				}
				topMessage.SaveChanges(context, saveMessageChangesFlags);
				outputMid = topMessage.GetId(context);
				outputCn = topMessage.GetLcnCurrent(context);
			}
			return ErrorCode.NoError;
		}

		public ErrorCode CopyMessageTo(MapiContext context, MapiFolder destination, ExchangeId mid, Properties propertyOverrides, out ExchangeId outputMid, out ExchangeId outputCn)
		{
			base.ThrowIfNotValid(null);
			destination.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody, AccessCheckOperation.FolderCopyMessageDestination, (LID)34151U);
			this.GhostedFolderCheck(context, (LID)32708U);
			destination.GhostedFolderCheck(context, (LID)30324U);
			ErrorCode first = MapiMailboxShape.PerformMailboxShapeQuotaCheck(context, destination.Logon, destination.StoreFolder, MapiMailboxShape.Operation.CopyMessage, false);
			if (first != ErrorCode.NoError)
			{
				outputMid = ExchangeId.Null;
				outputCn = ExchangeId.Null;
				return first.Propagate((LID)64576U);
			}
			foreach (Property property in propertyOverrides)
			{
				first = MapiMessage.CheckPropertyOperationAllowed(context, base.Logon, false, MapiPropBagBase.PropOperation.SetProps, property.Tag, property.Value);
				if (first != ErrorCode.NoError)
				{
					outputMid = ExchangeId.Null;
					outputCn = ExchangeId.Null;
					return first.Propagate((LID)61744U);
				}
			}
			TopMessage topMessage = TopMessage.CopyMessage(context, this.storeFolder, mid, destination.StoreFolder);
			if (topMessage == null)
			{
				outputMid = ExchangeId.Null;
				outputCn = ExchangeId.Null;
				return ErrorCode.CreateNotFound((LID)56856U);
			}
			bool flag = false;
			try
			{
				MapiFolder.CheckMessageRights(context, this.Fid.ToExchangeShortId(), this.accessCheckState, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, true, topMessage, false, AccessCheckOperation.FolderCopyMessageSource, (LID)58727U);
				foreach (Property property2 in propertyOverrides)
				{
					first = MapiMessage.InternalSetOneProp(context, base.Logon, topMessage, false, property2.Tag, property2.Value);
					if (first != ErrorCode.NoError)
					{
						outputMid = ExchangeId.Null;
						outputCn = ExchangeId.Null;
						return first.Propagate((LID)37168U);
					}
				}
				if (PropertyBagHelpers.TestPropertyFlags(context, topMessage, PropTag.Message.MessageFlagsActual, 4, 4))
				{
					topMessage.AdjustUncomputedMessageFlags(context, MessageFlags.Unsent, MessageFlags.Submit);
				}
				SaveMessageChangesFlags saveMessageChangesFlags = SaveMessageChangesFlags.None;
				if (!base.Logon.MapiMailbox.IsPublicFolderMailbox)
				{
					saveMessageChangesFlags |= SaveMessageChangesFlags.SkipFolderQuotaCheck;
				}
				topMessage.SaveChanges(context, saveMessageChangesFlags);
				flag = true;
				outputMid = topMessage.GetId(context);
				outputCn = topMessage.GetLcnCurrent(context);
			}
			finally
			{
				if (!flag)
				{
					topMessage.Delete(context);
				}
				topMessage.Dispose();
			}
			return ErrorCode.NoError;
		}

		public ErrorCode DeleteMessage(MapiContext context, bool sendNRN, bool deleteSubmitted, ExchangeId mid)
		{
			base.ThrowIfNotValid(null);
			if (this.deleteMessageTestHook != null)
			{
				return this.deleteMessageTestHook();
			}
			this.GhostedFolderCheck(context, (LID)63129U);
			using (TopMessage topMessage = TopMessage.OpenMessage(context, this.storeFolder.Mailbox, this.storeFolder.GetId(context), mid))
			{
				if (topMessage == null)
				{
					return ErrorCode.CreateNotFound((LID)44568U);
				}
				this.CheckMessageRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete, topMessage, AccessCheckOperation.FolderDeleteMessage, (LID)47463U);
				if (!this.CheckLocalDirectoryMessageAndHasAdminRight(context, mid))
				{
					return ErrorCode.CreateNoAccess((LID)51408U);
				}
				if (!deleteSubmitted && PropertyBagHelpers.TestPropertyFlags(context, topMessage, PropTag.Message.MessageFlagsActual, 4, 4))
				{
					return ErrorCode.CreateNoDeleteSubmitMessage((LID)60952U);
				}
				if (sendNRN)
				{
					MapiMessage.SendNotReadReportIfNeeded(context, base.Logon, topMessage, false);
				}
				topMessage.Delete(context);
			}
			return ErrorCode.NoError;
		}

		internal ErrorCode SetMessageReadFlag(MapiContext context, SetReadFlagFlags flags, ExchangeId mid, out bool changed, out ExchangeId outputReadCn)
		{
			base.ThrowIfNotValid(null);
			this.GhostedFolderCheck(context, (LID)39705U);
			return MapiMessage.SetReadFlag(context, base.Logon, this.storeFolder, this.accessCheckState, mid, flags, out changed, out outputReadCn).Propagate((LID)29696U);
		}

		public int GetMessageStatus(MapiContext context, ExchangeId mid)
		{
			base.ThrowIfNotValid(null);
			this.GhostedFolderCheck(context, (LID)32704U);
			int result;
			using (MapiMessage mapiMessage = new MapiMessage())
			{
				ErrorCode errorCode = mapiMessage.ConfigureMessage(context, base.Logon, this.Fid, mid, MessageConfigureFlags.RequestReadOnly, base.Logon.Session.CodePage);
				if (errorCode != ErrorCode.NoError)
				{
					throw new StoreException((LID)61176U, errorCode);
				}
				this.CheckMessageRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, mapiMessage.StoreMessage as TopMessage, AccessCheckOperation.FolderGetMessageStatus, (LID)59239U);
				int messageStatus = mapiMessage.GetMessageStatus(context);
				result = messageStatus;
			}
			return result;
		}

		public ErrorCode Move(MapiContext context, MapiFolder destinationFolder, string newFolderName, bool bypassNameCollisionCheck)
		{
			base.ThrowIfNotValid(null);
			this.CheckRecursiveRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.FolderMoveSource, (LID)43367U);
			destinationFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody, AccessCheckOperation.FolderMoveDestination, (LID)59751U);
			if (destinationFolder.IsSearchFolder())
			{
				return ErrorCode.CreateSearchFolder((LID)44508U);
			}
			if (!bypassNameCollisionCheck && !base.Logon.AllowsDuplicateFolderNames)
			{
				string displayName = newFolderName;
				if (string.IsNullOrEmpty(newFolderName))
				{
					displayName = this.storeFolder.GetName(context);
				}
				if (Folder.FindFolderIdByName(context, destinationFolder.Fid, displayName, base.Logon.StoreMailbox).IsValid)
				{
					DiagnosticContext.TraceLong((LID)37209U, (ulong)DateTime.UtcNow.Ticks);
					DiagnosticContext.TraceDword((LID)32680U, (uint)base.Logon.InTransitStatus);
					return ErrorCode.CreateCollision((LID)52760U);
				}
			}
			if (!string.IsNullOrEmpty(newFolderName))
			{
				this.storeFolder.SetName(context, newFolderName);
			}
			ErrorCode first = MapiMailboxShape.PerformMailboxShapeQuotaCheck(context, base.Logon, destinationFolder.StoreFolder, MapiMailboxShape.Operation.MoveFolder, false);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)40000U);
			}
			this.storeFolder.Move(context, destinationFolder.StoreFolder);
			return ErrorCode.NoError;
		}

		public ErrorCode Copy(MapiContext context, MapiFolder destinationFolder, string newFolderName, out MapiFolder result)
		{
			base.ThrowIfNotValid(null);
			base.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.FolderShallowCopySource, (LID)35175U);
			destinationFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.AppendMsg, AccessCheckOperation.FolderShallowCopyDestination, (LID)51559U);
			result = null;
			ExchangeId exchangeId = Folder.FindFolderIdByName(context, destinationFolder.Fid, newFolderName, base.Logon.StoreMailbox);
			if (exchangeId.IsValid && !base.Logon.AllowsDuplicateFolderNames)
			{
				DiagnosticContext.TraceLong((LID)30560U, (ulong)DateTime.UtcNow.Ticks);
				DiagnosticContext.TraceDword((LID)31952U, (uint)base.Logon.InTransitStatus);
				return ErrorCode.CreateCollision((LID)46616U);
			}
			FolderConfigureFlags folderConfigureFlags = FolderConfigureFlags.None;
			if (this.IsSearchFolder())
			{
				folderConfigureFlags |= FolderConfigureFlags.CreateSearchFolder;
			}
			List<MapiPropertyProblem> list = null;
			exchangeId = ExchangeId.Zero;
			MapiFolder mapiFolder;
			ErrorCode first = MapiFolder.CreateFolder(context, base.Logon, ref exchangeId, false, folderConfigureFlags, destinationFolder, this, MapiFolder.ManageAssociatedDumpsterFolder(base.Logon, false), out mapiFolder);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)50780U);
			}
			ErrorCode noError;
			try
			{
				List<StorePropTag> propTagsToCopy = base.InternalGetPropList(context, GetPropListFlags.None);
				this.CopyToRemoveNoAccessProps(context, mapiFolder, propTagsToCopy);
				if (!string.IsNullOrEmpty(newFolderName))
				{
					mapiFolder.InternalSetOnePropShouldNotFail(context, PropTag.Folder.DisplayName, newFolderName);
					StorePropTag[] propTagsToExclude = new StorePropTag[]
					{
						PropTag.Folder.DisplayName
					};
					MapiPropBagBase.CopyToRemoveExcludeProps(propTagsToExclude, propTagsToCopy);
				}
				base.CopyToCopyPropertiesToDestination(context, propTagsToCopy, mapiFolder, ref list);
				if (this.IsSearchFolder())
				{
					SetSearchCriteriaFlags setSearchCriteriaFlags = SetSearchCriteriaFlags.Restart;
					SearchFolder searchFolder = (SearchFolder)this.storeFolder;
					byte[] legacyRestriction;
					IList<ExchangeId> list2;
					SearchState searchState;
					searchFolder.GetSearchCriteria(context, GetSearchCriteriaFlags.Unicode | GetSearchCriteriaFlags.Restriction | GetSearchCriteriaFlags.FolderIds, out legacyRestriction, out list2, out searchState);
					if (list2.Count > 0)
					{
						if ((searchState & SearchState.Recursive) != SearchState.None)
						{
							setSearchCriteriaFlags |= SetSearchCriteriaFlags.Recursive;
						}
						if ((searchState & SearchState.Static) != SearchState.None)
						{
							setSearchCriteriaFlags |= SetSearchCriteriaFlags.Static;
						}
						if ((searchState & (SearchState.CiTotally | SearchState.CiWithTwirResidual)) != SearchState.None)
						{
							setSearchCriteriaFlags |= SetSearchCriteriaFlags.ContentIndexed;
						}
						else if ((searchState & SearchState.TwirTotally) != SearchState.None)
						{
							setSearchCriteriaFlags |= SetSearchCriteriaFlags.NonContentIndexed;
						}
						if ((searchState & SearchState.StatisticsOnly) != SearchState.None)
						{
							setSearchCriteriaFlags &= ~SetSearchCriteriaFlags.NonContentIndexed;
							setSearchCriteriaFlags |= (SetSearchCriteriaFlags.ContentIndexed | SetSearchCriteriaFlags.Static | SetSearchCriteriaFlags.StatisticsOnly);
						}
						ErrorCode first2 = mapiFolder.SetSearchCriteria(context, legacyRestriction, list2, setSearchCriteriaFlags);
						if (first2 != ErrorCode.NoError)
						{
							return first2.Propagate((LID)30256U);
						}
					}
				}
				result = mapiFolder;
				mapiFolder = null;
				noError = ErrorCode.NoError;
			}
			finally
			{
				if (mapiFolder != null)
				{
					mapiFolder.Dispose();
				}
			}
			return noError;
		}

		internal void Save(MapiContext context)
		{
			this.storeFolder.Save(context);
		}

		internal ErrorCode Delete(MapiContext context)
		{
			base.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete, AccessCheckOperation.FolderDelete, (LID)45415U);
			if (this.storeFolder.GetIsSpecialFolder(context))
			{
				return ErrorCode.CreateFolderSetReceive((LID)63000U);
			}
			if (this.storeFolder.GetIsReceiveFolder(context))
			{
				return ErrorCode.CreateFolderSetReceive((LID)38424U);
			}
			if (this.storeFolder.GetFolderCount(context) != 0L)
			{
				return ErrorCode.CreateHasFolders((LID)54808U);
			}
			SearchFolder searchFolder = this.storeFolder as SearchFolder;
			if (searchFolder == null)
			{
				if (this.storeFolder.GetMessageCount(context) != 0L || this.storeFolder.GetHiddenItemCount(context) != 0L)
				{
					return ErrorCode.CreateHasMessages((LID)42520U);
				}
			}
			else if (searchFolder.IsSearchEvaluationInProgress(context))
			{
				return ErrorCode.CreateSearchEvaluationInProgress((LID)58904U);
			}
			if (MapiFolder.ManageAssociatedDumpsterFolder(base.Logon, false))
			{
				if (this.storeFolder.IsDumpsterMarkedFolder(context))
				{
					return ErrorCode.CreateFolderSetReceive((LID)62016U);
				}
				if (this.GetAssociatedFolderId(context) != ExchangeId.Null)
				{
					if (this.associatedStoreFolder == null)
					{
						return ErrorCode.CreateNotFound((LID)53696U);
					}
					if (this.associatedStoreFolder.GetFolderCount(context) != 0L)
					{
						return ErrorCode.CreateHasFolders((LID)37440U);
					}
					if (this.associatedStoreFolder.GetMessageCount(context) != 0L || this.associatedStoreFolder.GetHiddenItemCount(context) != 0L)
					{
						return ErrorCode.CreateHasMessages((LID)53824U);
					}
					this.associatedStoreFolder.Delete(context);
				}
			}
			this.storeFolder.Delete(context);
			return ErrorCode.NoError;
		}

		internal ErrorCode SetSearchCriteria(MapiContext context, byte[] legacyRestriction, IList<ExchangeId> foldersToSearch, SetSearchCriteriaFlags searchCriteriaFlags)
		{
			ErrorCode result = ErrorCode.NoError;
			base.ThrowIfNotValid(null);
			base.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.FolderSetSearchCriteria, (LID)61799U);
			if (!this.IsSearchFolder())
			{
				result = ErrorCode.CreateNotSearchFolder((LID)34328U);
			}
			else
			{
				SearchFolder searchFolder = (SearchFolder)this.storeFolder;
				SpecialFolders specialFolderNumber = this.storeFolder.GetSpecialFolderNumber(context);
				if (specialFolderNumber == SpecialFolders.SpoolerQueue)
				{
					byte[] array;
					IList<ExchangeId> list;
					SearchState searchState;
					searchFolder.GetSearchCriteria(context, GetSearchCriteriaFlags.None, out array, out list, out searchState);
					if (searchState != SearchState.None)
					{
						return ErrorCode.CreateNotSupported((LID)64060U);
					}
				}
				searchFolder.SetSearchCriteria(context, legacyRestriction, foldersToSearch, searchCriteriaFlags, this.IsInstantSearch, this.IsOptimizedConversationSearch, false);
			}
			return result;
		}

		internal ErrorCode GetSearchCriteria(MapiContext context, GetSearchCriteriaFlags flags, out byte[] restriction, out IList<ExchangeId> foldersToSearch, out SearchState searchState)
		{
			ErrorCode result = ErrorCode.NoError;
			base.ThrowIfNotValid(null);
			restriction = null;
			foldersToSearch = null;
			searchState = SearchState.None;
			GetSearchCriteriaFlags getSearchCriteriaFlags = flags;
			IList<ExchangeId> list = null;
			base.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.FolderGetSearchCriteria, (LID)37223U);
			if (!this.IsSearchFolder())
			{
				result = ErrorCode.CreateNotSearchFolder((LID)50712U);
			}
			else
			{
				SpecialFolders specialFolderNumber = this.storeFolder.GetSpecialFolderNumber(context);
				if (specialFolderNumber == SpecialFolders.SpoolerQueue)
				{
					result = ErrorCode.CreateNotInitialized((LID)61712U);
				}
				else
				{
					SearchFolder searchFolder = (SearchFolder)this.storeFolder;
					bool flag = false;
					if ((byte)(flags & GetSearchCriteriaFlags.FolderIds) != 0)
					{
						flag = true;
					}
					else if ((byte)(flags & GetSearchCriteriaFlags.Restriction) != 0)
					{
						flag = true;
						getSearchCriteriaFlags |= GetSearchCriteriaFlags.FolderIds;
					}
					searchFolder.GetSearchCriteria(context, getSearchCriteriaFlags, out restriction, out list, out searchState);
					if ((byte)(flags & GetSearchCriteriaFlags.FolderIds) != 0)
					{
						foldersToSearch = list;
					}
					if (flag && list.Count == 0)
					{
						result = ErrorCode.CreateNotInitialized((LID)47640U);
					}
				}
			}
			return result;
		}

		public override void CopyTo(MapiContext context, MapiPropBagBase destination, IList<StorePropTag> propTagsExclude, CopyToFlags flags, ref List<MapiPropertyProblem> propProblems)
		{
			base.ThrowIfNotValid(null);
			MapiFolder mapiFolder = (MapiFolder)destination;
			if (mapiFolder == null)
			{
				throw base.CreateCopyPropsNotSupportedException((LID)36600U, destination);
			}
			if (mapiFolder.Fid == this.Fid)
			{
				throw new ExExceptionAccessDenied((LID)52984U, "Cannot CopyTo to itself");
			}
			if (mapiFolder.IsSearchFolder())
			{
				throw new StoreException((LID)46840U, ErrorCodeValue.NotSupported);
			}
			base.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.FolderCopyPropsSource, (LID)53607U);
			mapiFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.FolderCopyPropsDestination, (LID)41319U);
			base.CopyTo(context, destination, propTagsExclude, flags, ref propProblems);
		}

		public override void CopyProps(MapiContext context, MapiPropBagBase destination, IList<StorePropTag> propTags, bool replaceIfExists, ref List<MapiPropertyProblem> propProblems)
		{
			base.ThrowIfNotValid(null);
			MapiFolder mapiFolder = destination as MapiFolder;
			if (mapiFolder == null)
			{
				throw base.CreateCopyPropsNotSupportedException((LID)38648U, destination);
			}
			if (mapiFolder.Fid == this.Fid)
			{
				throw new ExExceptionAccessDenied((LID)63224U, "Cannot CopyProps to itself");
			}
			if (mapiFolder.IsSearchFolder())
			{
				throw new StoreException((LID)55032U, ErrorCodeValue.NotSupported);
			}
			base.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.FolderCopyPropsSource, (LID)57703U);
			mapiFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.FolderCopyPropsDestination, (LID)33127U);
			List<StorePropTag> list = new List<StorePropTag>(propTags);
			this.CopyToRemoveNoAccessProps(context, destination, list);
			if (MapiPropBagBase.copyToTestHook.Value != null)
			{
				MapiPropBagBase.copyToTestHook.Value(list);
				return;
			}
			if (!replaceIfExists)
			{
				MapiPropBagBase.CopyToRemovePreexistingDestinationProperties(context, destination, list);
			}
			if (list.Count != 0)
			{
				base.CopyToCopyPropertiesToDestination(context, list, destination, ref propProblems);
			}
		}

		public static void GhostedFolderCheck(MapiContext context, MapiLogon mapiLogon, ExchangeId folderId, LID lid)
		{
			using (MapiFolder mapiFolder = MapiFolder.OpenFolder(context, mapiLogon, folderId))
			{
				if (mapiFolder != null)
				{
					mapiFolder.GhostedFolderCheck(context, lid);
				}
			}
		}

		public void GhostedFolderCheck(MapiContext context, LID lid)
		{
			if (this.IsGhosted(context, lid))
			{
				throw new StoreException(lid, ErrorCode.CreateNoReplicaHere((LID)32712U));
			}
		}

		public bool IsGhosted(MapiContext context, LID lid)
		{
			if (!base.Logon.MapiMailbox.IsPublicFolderMailbox)
			{
				return false;
			}
			if (base.Logon.IsMoveUser)
			{
				return false;
			}
			if (!base.Logon.IsUserLogon(context))
			{
				return false;
			}
			Guid[] replicaList = this.storeFolder.GetReplicaList(context);
			if (replicaList == null || replicaList.Length == 0)
			{
				throw new StoreException(lid, ErrorCode.CreateNoReplicaAvailable((LID)51161U));
			}
			for (int i = 0; i < replicaList.Length; i++)
			{
				if (replicaList[i] == base.Logon.MapiMailbox.SharedState.MailboxGuid)
				{
					return false;
				}
			}
			return true;
		}

		public void SetMessageStatus(MapiContext context, ExchangeId mid, int status, int mask, out int oldStatus)
		{
			this.GhostedFolderCheck(context, (LID)61145U);
			int flagsToClear = mask & ~status;
			int flagsToSet = mask & status;
			using (TopMessage topMessage = TopMessage.OpenMessage(context, this.storeFolder.Mailbox, this.storeFolder.GetId(context), mid))
			{
				if (topMessage == null)
				{
					throw new ExExceptionNotFound((LID)60000U, "Message does not exist");
				}
				this.CheckMessageRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, topMessage, AccessCheckOperation.FolderSetMessageStatus, (LID)34663U);
				this.storeFolder.SetMessageStatus(context, topMessage, flagsToSet, flagsToClear, out oldStatus);
			}
		}

		public void SetMessageFlags(MapiContext context, ExchangeId mid, int flags, int mask)
		{
			if ((mask & -9) != 0)
			{
				throw new ExExceptionInvalidParameter((LID)59128U, "Only message flag that can be changed is UNSENT");
			}
			this.GhostedFolderCheck(context, (LID)51193U);
			using (TopMessage topMessage = TopMessage.OpenMessage(context, this.storeFolder.Mailbox, this.storeFolder.GetId(context), mid))
			{
				if (topMessage == null)
				{
					throw new ExExceptionNotFound((LID)46880U, "Message does not exist");
				}
				this.CheckMessageRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, topMessage, AccessCheckOperation.MessageSetMessageFlags, (LID)63264U);
				bool unsent = (mask & flags) != 0;
				this.storeFolder.SetUnsent(context, topMessage, unsent);
			}
		}

		internal ErrorCode MergeMidsetDeleted(MapiContext context, LongTermIdRange[] idRanges)
		{
			if (idRanges.Length == 0)
			{
				return ErrorCode.CreateInvalidParameter((LID)31152U);
			}
			IdSet idSet = new IdSet();
			foreach (LongTermIdRange idRange in idRanges)
			{
				if (!idRange.IsValid())
				{
					return ErrorCode.CreateInvalidParameter((LID)30356U);
				}
				idSet.Insert(idRange);
			}
			try
			{
				this.StoreFolder.AddToMidsetDeletedWithSanitize(context, idSet);
			}
			catch (StoreException ex)
			{
				context.OnExceptionCatch(ex);
				if (ex.Error != ErrorCodeValue.ObjectDeleted)
				{
					throw;
				}
				return ErrorCode.NoError;
			}
			return ErrorCode.NoError;
		}

		protected override Property InternalGetOneProp(MapiContext context, StorePropTag propTag)
		{
			uint propTag2 = propTag.PropTag;
			if (propTag2 != 242942210U)
			{
				if (propTag2 != 1721237762U)
				{
					if (propTag2 == 1734082818U)
					{
						IdSet midsetDeleted = this.storeFolder.GetMidsetDeleted(context);
						return new Property(PropTag.Folder.MidsetDeletedExport, midsetDeleted.Serialize());
					}
				}
				else
				{
					Guid[] replicaList = this.StoreFolder.GetReplicaList(context);
					if (replicaList != null)
					{
						byte[] value = MapiFolder.ConvertGuidArrayToWireReplicaListBlob(replicaList);
						return new Property(propTag, value);
					}
				}
				return base.InternalGetOneProp(context, propTag);
			}
			if (this.IsSearchFolder())
			{
				DiagnosticContext.TraceLocation((LID)50648U);
				return new Property(propTag.ConvertToError(), ErrorCodeValue.NotSupported);
			}
			if (!MapiFolder.HasFolderAccessRights(context, this.Fid.ToExchangeShortId(), false, this.accessCheckState, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, true, AccessCheckOperation.PropertyGet, (LID)33707U))
			{
				return new Property(propTag.ConvertToError(), ErrorCodeValue.NoAccess);
			}
			uint num = 65536U;
			byte[] array = base.Logon.StoreMailbox.GetLocalRepids(context, num).To22ByteArray();
			byte[] array2 = new byte[4 + array.Length];
			ParseSerialize.SerializeInt32((int)num, array2, 0);
			Buffer.BlockCopy(array, 0, array2, 4, array.Length);
			return new Property(propTag, array2);
		}

		public static byte[] ConvertGuidArrayToWireReplicaListBlob(Guid[] replicaGuids)
		{
			byte[] array = null;
			if (replicaGuids != null)
			{
				array = new byte[37 * replicaGuids.Length];
				for (int i = 0; i < replicaGuids.Length; i++)
				{
					ParseSerialize.SerializeAsciiString(replicaGuids[i].ToString(), array, i * 37);
				}
			}
			return array;
		}

		public override void SetProps(MapiContext context, Properties properties, ref List<MapiPropertyProblem> allProblems)
		{
			base.ThrowIfNotValid(null);
			int i = 0;
			while (i < properties.Count)
			{
				if (properties[i].Tag == PropTag.Folder.SetPropsCondition)
				{
					byte[] array = (byte[])properties[i].Value;
					int num = 0;
					Restriction restriction = Restriction.Deserialize(context, array, ref num, array.Length, base.Logon.StoreMailbox, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);
					if (!(restriction is RestrictionProperty) || ((RestrictionProperty)restriction).Operator != RelationOperator.Equal || ((RestrictionProperty)restriction).PropertyTag != PropTag.Folder.ChangeKey || !(((RestrictionProperty)restriction).Value is byte[]) || ((byte[])((RestrictionProperty)restriction).Value).Length != 22)
					{
						throw new ExExceptionTooComplex((LID)56056U, "SetPropsCondition criteria is too complex");
					}
					byte[] x = (byte[])this.storeFolder.GetPropertyValue(context, PropTag.Folder.ChangeKey);
					if (!ValueHelper.ValuesEqual(x, (byte[])((RestrictionProperty)restriction).Value))
					{
						throw new ExExceptionConditionViolation((LID)43768U, "SetProps condition violation");
					}
					Properties properties2 = new Properties(properties);
					properties2.Remove(PropTag.Folder.SetPropsCondition);
					properties = properties2;
					break;
				}
				else
				{
					i++;
				}
			}
			base.SetProps(context, properties, ref allProblems);
			if (base.Logon.IsMoveUser && !base.Logon.IsForPublicFolderMove)
			{
				this.storeFolder.TrackFolderChange = false;
			}
		}

		protected override ErrorCode InternalSetOneProp(MapiContext context, StorePropTag propTag, object value)
		{
			base.ThrowIfNotValid(null);
			ushort propId = propTag.PropId;
			if (propId <= 26265)
			{
				if (propId <= 3706)
				{
					if (propId == 3647)
					{
						MapiFolder.ValidateAclTableAndSecurityDescriptorProperty(context, value);
						goto IL_65B;
					}
					if (propId != 3706)
					{
						goto IL_65B;
					}
					if (propTag.PropType != PropertyType.Binary)
					{
						return ErrorCode.CreateInvalidParameter((LID)55975U);
					}
					try
					{
						using (Reader reader = Reader.CreateBufferReader((byte[])value))
						{
							uint num = reader.ReadUInt32();
							if ((ulong)num != (ulong)((reader.Length - reader.Position) / 48L))
							{
								return ErrorCode.CreateInvalidParameter((LID)30216U);
							}
							LongTermIdRange[] array = new LongTermIdRange[num];
							while (num != 0U)
							{
								num -= 1U;
								array[(int)((UIntPtr)num)] = LongTermIdRange.Parse(reader);
							}
							return this.MergeMidsetDeleted(context, array).Propagate((LID)30772U);
						}
					}
					catch (BufferParseException exception)
					{
						context.OnExceptionCatch(exception);
						return ErrorCode.CreateInvalidParameter((LID)30896U);
					}
				}
				else if (propId != 12289)
				{
					if (propId != 26083)
					{
						switch (propId)
						{
						case 26264:
						{
							if (propTag.PropType != PropertyType.Binary)
							{
								return ErrorCode.CreateInvalidParameter((LID)63047U);
							}
							byte[] array2 = value as byte[];
							if (array2 == null || array2.Length == 0 || array2.Length % 37 != 0)
							{
								return ErrorCode.CreateInvalidParameter((LID)32248U);
							}
							Guid[] array3 = new Guid[array2.Length / 37];
							bool flag = this.StoreFolder.HasLocalReplica(context);
							bool flag2 = false;
							for (int i = 0; i < array3.Length; i++)
							{
								string input = ParseSerialize.ParseAsciiString(array2, i * 37, 36);
								if (!Guid.TryParse(input, out array3[i]))
								{
									return ErrorCode.CreateInvalidParameter((LID)41177U);
								}
								if (array3[i] == base.Logon.MapiMailbox.SharedState.MailboxGuid)
								{
									flag2 = true;
								}
							}
							this.StoreFolder.SetReplicaList(context, array3);
							if (base.Logon.MapiMailbox.SharedState.MailboxType == MailboxInfo.MailboxType.PublicFolderPrimary && this.associatedStoreFolder != null && !this.storeFolder.IsDumpsterMarkedFolder(context))
							{
								ErrorCode errorCode = this.associatedStoreFolder.SetReplicaList(context, array3);
								if (errorCode != ErrorCode.NoError)
								{
									throw new StoreException((LID)36480U, errorCode, "Unable to set replica list on the associated dumpster folder");
								}
							}
							if (flag && !flag2)
							{
								PerUser.DeleteAllResidentEntriesForFolder(context, this.StoreFolder);
								this.StoreFolder.ResetMidsetDeleted(context);
							}
							return ErrorCode.NoError;
						}
						case 26265:
							goto IL_512;
						default:
							goto IL_65B;
						}
					}
				}
				else
				{
					if (propTag.PropType != PropertyType.Unicode)
					{
						return ErrorCode.CreateInvalidParameter((LID)65095U);
					}
					Folder parentFolder = this.StoreFolder.GetParentFolder(context);
					if (parentFolder != null && string.IsNullOrEmpty(value as string))
					{
						return ErrorCode.CreateBadFolderName((LID)52328U);
					}
					if (parentFolder == null || base.Logon.AllowsDuplicateFolderNames)
					{
						goto IL_65B;
					}
					ExchangeId id = Folder.FindFolderIdByName(context, this.GetParentFid(context), (string)value, base.Logon.StoreMailbox);
					if (id.IsValid && id != this.Fid)
					{
						DiagnosticContext.TraceLong((LID)32392U, (ulong)DateTime.UtcNow.Ticks);
						DiagnosticContext.TraceDword((LID)56793U, (uint)base.Logon.InTransitStatus);
						return ErrorCode.CreateCollision((LID)50200U);
					}
					goto IL_65B;
				}
			}
			else if (propId <= 26413)
			{
				if (propId == 26308)
				{
					goto IL_512;
				}
				switch (propId)
				{
				case 26401:
				case 26402:
					goto IL_512;
				default:
				{
					if (propId != 26413)
					{
						goto IL_65B;
					}
					if (propTag.PropType != PropertyType.Int32)
					{
						return ErrorCode.CreateInvalidParameter((LID)39591U);
					}
					FolderAdminFlags folderAdminFlags = (FolderAdminFlags)0;
					if (value != null)
					{
						folderAdminFlags = (FolderAdminFlags)((int)value);
					}
					FolderAdminFlags folderAdminFlags2 = (FolderAdminFlags)0;
					object propertyValue = this.storeFolder.GetPropertyValue(context, PropTag.Folder.FolderAdminFlags);
					if (propertyValue != null)
					{
						folderAdminFlags2 = (FolderAdminFlags)((int)propertyValue);
					}
					if (!this.VerifyFolderAdminFlags(context, folderAdminFlags2, folderAdminFlags))
					{
						DiagnosticContext.TraceDword((LID)48912U, (uint)folderAdminFlags2);
						DiagnosticContext.TraceDword((LID)63248U, (uint)folderAdminFlags);
						return ErrorCode.CreateNoAccess((LID)63036U);
					}
					if (((folderAdminFlags2 ^ folderAdminFlags) & FolderAdminFlags.DumpsterFolder) != (FolderAdminFlags)0)
					{
						this.StoreFolder.SetDumpsterMarkedFolder(context, (folderAdminFlags & FolderAdminFlags.DumpsterFolder) != (FolderAdminFlags)0);
						goto IL_65B;
					}
					goto IL_65B;
				}
				}
			}
			else if (propId != 26449)
			{
				switch (propId)
				{
				case 26458:
					break;
				case 26459:
					goto IL_65B;
				case 26460:
				{
					if (propTag.PropType != PropertyType.Binary)
					{
						return ErrorCode.CreateInvalidParameter((LID)53376U);
					}
					IdSet idSet;
					if (value == null)
					{
						idSet = new IdSet();
					}
					else
					{
						byte[] array4 = value as byte[];
						if (array4 == null)
						{
							return ErrorCode.CreateInvalidParameter((LID)43616U);
						}
						idSet = IdSet.Parse(context, array4);
					}
					this.storeFolder.SetMidsetDeleted(context, idSet, false);
					return ErrorCode.NoError;
				}
				default:
					switch (propId)
					{
					case 26489:
						if (propTag.PropType != PropertyType.Int32)
						{
							return ErrorCode.CreateInvalidParameter((LID)34887U);
						}
						return ErrorCode.CreateInvalidParameter((LID)35705U);
					case 26490:
						goto IL_65B;
					case 26491:
						goto IL_512;
					default:
						goto IL_65B;
					}
					break;
				}
			}
			else
			{
				int num2 = (int)value;
				int num3 = (int)this.storeFolder.GetPropertyValue(context, PropTag.Folder.ArticleNumNext);
				if (num2 <= num3)
				{
					return ErrorCode.NoError;
				}
				goto IL_65B;
			}
			if (propTag.PropType != PropertyType.Binary)
			{
				return ErrorCode.CreateInvalidParameter((LID)57415U);
			}
			if (base.Logon.IsMoveUser)
			{
				base.InternalDeleteOnePropShouldNotFail(context, propTag);
				goto IL_65B;
			}
			goto IL_65B;
			IL_512:
			if (propTag.PropType != PropertyType.Int32)
			{
				return ErrorCode.CreateInvalidParameter((LID)57177U);
			}
			if (base.Logon.MapiMailbox.SharedState.MailboxType == MailboxInfo.MailboxType.PublicFolderPrimary && this.associatedStoreFolder != null && !this.storeFolder.IsDumpsterMarkedFolder(context))
			{
				ErrorCode errorCode2 = this.associatedStoreFolder.SetProperty(context, propTag, value);
				if (errorCode2 != ErrorCode.NoError)
				{
					throw new StoreException((LID)61056U, errorCode2, "Unable to set property on the associated dumpster folder");
				}
			}
			IL_65B:
			return base.InternalSetOneProp(context, propTag, value);
		}

		internal bool VerifyFolderAdminFlags(Context context, FolderAdminFlags oldFlags, FolderAdminFlags newFlags)
		{
			if ((oldFlags & FolderAdminFlags.DumpsterFolder) != (FolderAdminFlags)0 && (newFlags & FolderAdminFlags.DumpsterFolder) == (FolderAdminFlags)0 && (newFlags & FolderAdminFlags.DumpsterFolder) == (FolderAdminFlags)0)
			{
				DiagnosticContext.TraceLocation((LID)49424U);
				return false;
			}
			if ((newFlags & FolderAdminFlags.ProvisionedFolder) != (FolderAdminFlags)0)
			{
				if ((newFlags & FolderAdminFlags.DumpsterFolder) != (FolderAdminFlags)0)
				{
					DiagnosticContext.TraceLocation((LID)33040U);
					return false;
				}
				if (((newFlags & FolderAdminFlags.HasQuota) != (FolderAdminFlags)0 || (newFlags & FolderAdminFlags.TrackFolderSize) != (FolderAdminFlags)0) && (oldFlags & FolderAdminFlags.HasQuota) == (FolderAdminFlags)0 && (oldFlags & FolderAdminFlags.TrackFolderSize) == (FolderAdminFlags)0 && (this.storeFolder.GetMessageCount(context) != 0L || this.storeFolder.GetHiddenItemCount(context) != 0L || this.storeFolder.GetFolderCount(context) != 0L))
				{
					DiagnosticContext.TraceLocation((LID)29812U);
					return false;
				}
				return true;
			}
			else
			{
				if ((newFlags & (FolderAdminFlags.ProtectedFolder | FolderAdminFlags.HasQuota | FolderAdminFlags.RootFolder | FolderAdminFlags.TrackFolderSize)) != (FolderAdminFlags)0)
				{
					DiagnosticContext.TraceLocation((LID)40720U);
					return false;
				}
				if ((newFlags & (FolderAdminFlags.DisplayComment | FolderAdminFlags.DumpsterFolder)) != (FolderAdminFlags)0)
				{
					return true;
				}
				if ((oldFlags & FolderAdminFlags.DumpsterFolder) == (FolderAdminFlags)0)
				{
					return true;
				}
				DiagnosticContext.TraceLocation((LID)65296U);
				return false;
			}
		}

		protected override ErrorCode InternalDeleteOneProp(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			if (propTag == PropTag.Folder.DisplayName)
			{
				return ErrorCode.CreateBadFolderName((LID)46184U);
			}
			return base.InternalDeleteOneProp(context, propTag);
		}

		protected override ErrorCode CheckPropertyOperationAllowed(MapiContext context, MapiPropBagBase.PropOperation operation, StorePropTag propTag, object value)
		{
			switch (operation)
			{
			case MapiPropBagBase.PropOperation.SetProps:
			case MapiPropBagBase.PropOperation.DeleteProps:
			{
				uint propTag2 = propTag.PropTag;
				if (propTag2 > 26491U)
				{
					if (propTag2 != 1071710466U)
					{
						if (propTag2 != 1071841538U)
						{
							if (propTag2 != 1734344707U)
							{
								break;
							}
						}
						else
						{
							if (context.ClientType == ClientType.EventBasedAssistants)
							{
								return ErrorCode.NoError;
							}
							break;
						}
					}
					return ErrorCode.CreateNotSupported((LID)58255U, propTag.PropTag);
				}
				if (propTag2 != 3647U)
				{
					switch (propTag2)
					{
					case 26401U:
					case 26402U:
						break;
					default:
						switch (propTag2)
						{
						case 26489U:
						case 26491U:
							break;
						case 26490U:
							goto IL_D4;
						default:
							goto IL_D4;
						}
						break;
					}
					if (!base.Logon.StoreMailbox.IsPublicFolderMailbox)
					{
						return ErrorCode.CreateNoAccess((LID)46064U);
					}
				}
				else if (operation == MapiPropBagBase.PropOperation.DeleteProps)
				{
					return ErrorCode.CreateNoAccess((LID)36656U);
				}
				break;
			}
			}
			IL_D4:
			return base.CheckPropertyOperationAllowed(context, operation, propTag, value);
		}

		internal override void CheckRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, AccessCheckOperation operation, LID lid)
		{
			if ((operation == AccessCheckOperation.PropertyGet || operation == AccessCheckOperation.FolderOpen) && this.Fid == base.Logon.FidC.FidRoot)
			{
				return;
			}
			MapiFolder.CheckFolderRights(context, this.Fid.ToExchangeShortId(), this.IsSearchFolder(), this.StoreFolder.IsInternalAccess(context), this.accessCheckState, requestedRights, allRights, operation, lid);
		}

		internal void CheckRecursiveRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedFolderRights, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedMessageRights, AccessCheckOperation operation, LID lid)
		{
			FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, base.Logon.MapiMailbox.StoreMailbox, this.Fid.ToExchangeShortId(), FolderInformationType.Extended);
			Queue<IFolderInformation> queue = new Queue<IFolderInformation>(folderHierarchy.HierarchyRoots);
			while (queue.Count != 0)
			{
				IFolderInformation folderInformation = queue.Dequeue();
				AccessCheckState accessCheckState = new AccessCheckState(context, folderInformation.SecurityDescriptor);
				if (requestedFolderRights != FolderSecurity.ExchangeSecurityDescriptorFolderRights.None)
				{
					MapiFolder.CheckFolderRights(context, folderInformation.Fid, folderInformation.IsSearchFolder, folderInformation.IsInternalAccess, accessCheckState, requestedFolderRights, true, operation, lid);
				}
				if (requestedMessageRights != FolderSecurity.ExchangeSecurityDescriptorFolderRights.None)
				{
					MapiFolder.CheckMessageRights(context, folderInformation.Fid, accessCheckState, requestedMessageRights, true, null, false, operation, lid);
				}
				foreach (IFolderInformation item in folderHierarchy.GetChildren(context, folderInformation))
				{
					queue.Enqueue(item);
				}
			}
		}

		internal void CheckMessageRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, TopMessage topMessage, AccessCheckOperation operation, LID lid)
		{
			MapiFolder.CheckMessageRights(context, this.Fid.ToExchangeShortId(), this.accessCheckState, requestedRights, allRights, topMessage, topMessage != null && topMessage.IsNew, operation, lid);
		}

		internal void CheckMessageRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, TopMessage topMessage, AccessCheckOperation operation, LID lid)
		{
			this.CheckMessageRights(context, requestedRights, true, topMessage, operation, lid);
		}

		protected override bool TryGetPropertyImp(MapiContext context, ushort propId, out StorePropTag actualPropTag, out object propValue)
		{
			if (propId == 16353 || propId == 26464)
			{
				DiagnosticContext.TraceLocation((LID)48271U);
				actualPropTag = StorePropTag.CreateWithoutInfo(propId, PropertyType.Unspecified, base.PropTagBaseObjectType);
				propValue = null;
				return false;
			}
			if (propId == 4084)
			{
				actualPropTag = PropTag.Folder.Access;
				propValue = MapiFolder.CalculateSecurityRelatedProperty(context, PropTag.Folder.Access, this.accessCheckState);
				return propValue != null;
			}
			if (propId == 26169)
			{
				actualPropTag = PropTag.Folder.Rights;
				propValue = MapiFolder.CalculateSecurityRelatedProperty(context, PropTag.Folder.Rights, this.accessCheckState);
				return propValue != null;
			}
			if (propId == 26293)
			{
				actualPropTag = PropTag.Folder.FolderPathName;
				propValue = this.GetFolderPathName(context, '￾');
				return true;
			}
			bool flag = base.TryGetPropertyImp(context, propId, out actualPropTag, out propValue);
			if (!flag && propId == 16359)
			{
				flag = true;
				actualPropTag = PropTag.Folder.ResolveMethod;
				propValue = 0;
			}
			return flag;
		}

		protected override object GetPropertyValueImp(MapiContext context, StorePropTag propTag)
		{
			if (propTag.PropTag == 1071710466U || propTag.PropTag == 1734344707U)
			{
				return null;
			}
			if (MapiFolder.IsFolderSecurityRelatedProperty(propTag.PropTag))
			{
				return MapiFolder.CalculateSecurityRelatedProperty(context, propTag, this.accessCheckState);
			}
			if (propTag.PropTag == 1723138079U)
			{
				return this.GetFolderPathName(context, (propTag.ExternalType == PropertyType.String8) ? '\\' : '￾');
			}
			object obj = base.GetPropertyValueImp(context, propTag);
			if (propTag.PropTag == 1072103427U && obj == null)
			{
				obj = 0;
			}
			if (base.Logon.IsMoveUser && obj != null && propTag.PropTag == 251658498U && !this.FolderHasRealFreeBusyNTSD(context))
			{
				obj = null;
			}
			return obj;
		}

		private static byte[] CreateAclTableAndSecurityDescriptorBlob(MapiContext context, MapiLogon logon, MapiFolder parentFolder)
		{
			if (parentFolder == null)
			{
				if (logon.MapiMailbox.IsPublicFolderMailbox)
				{
					return FolderSecurity.AclTableAndSecurityDescriptorProperty.GetDefaultBlobForPublicFolders();
				}
				if (logon.MapiMailbox.IsGroupMailbox)
				{
					return FolderSecurity.AclTableAndSecurityDescriptorProperty.GetDefaultBlobForGroupMailboxRootFolder(logon.MapiMailbox.MailboxGuid);
				}
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.GetDefaultBlobForRootFolder();
			}
			else
			{
				Property property = parentFolder.InternalGetOneProp(context, PropTag.Folder.AclTableAndSecurityDescriptor);
				if (property.IsError)
				{
					throw new ExExceptionNotFound((LID)39672U, "Parent folder doesn't contain AclTableAndSecurityDescriptor property");
				}
				if (context.HasMailboxFullRights)
				{
					return FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateForChildFolder((byte[])property.Value);
				}
				AddressInfo loggedOnUserAddressInfo = logon.LoggedOnUserAddressInfo;
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateForChildFolder((byte[])property.Value, context.SecurityContext.UserSid, AddressBookEID.MakeAddressBookEntryID(loggedOnUserAddressInfo.LegacyExchangeDN, false), (loggedOnUserAddressInfo.DisplayName != null) ? loggedOnUserAddressInfo.DisplayName : string.Empty, logon.MapiMailbox.IsPublicFolderMailbox ? (FolderSecurity.ExchangeFolderRights.ReadAny | FolderSecurity.ExchangeFolderRights.Create | FolderSecurity.ExchangeFolderRights.EditOwned | FolderSecurity.ExchangeFolderRights.DeleteOwned | FolderSecurity.ExchangeFolderRights.EditAny | FolderSecurity.ExchangeFolderRights.DeleteAny | FolderSecurity.ExchangeFolderRights.CreateSubfolder | FolderSecurity.ExchangeFolderRights.Owner | FolderSecurity.ExchangeFolderRights.Contact | FolderSecurity.ExchangeFolderRights.Visible | FolderSecurity.ExchangeFolderRights.FreeBusySimple | FolderSecurity.ExchangeFolderRights.FreeBusyDetailed) : FolderSecurity.ExchangeFolderRights.AllFolderRights);
			}
		}

		private static void ValidateAclTableAndSecurityDescriptorProperty(MapiContext context, object propertyValue)
		{
			if (propertyValue == null)
			{
				throw new StoreException((LID)46896U, ErrorCodeValue.InvalidParameter, "Null property value");
			}
			byte[] array = propertyValue as byte[];
			if (array == null)
			{
				throw new StoreException((LID)53040U, ErrorCodeValue.InvalidParameter, "Type mismatch");
			}
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, array);
			if (aclTableAndSecurityDescriptorProperty.SerializedAclTable.Array != null && aclTableAndSecurityDescriptorProperty.SerializedAclTable.Count != 0)
			{
				AclTableHelper.ParseAclTable(context, aclTableAndSecurityDescriptorProperty.SerializedAclTable);
			}
			if (aclTableAndSecurityDescriptorProperty.SecurityDescriptor != null)
			{
				SecurityHelper.CreateRawSecurityDescriptor(aclTableAndSecurityDescriptorProperty.SecurityDescriptor);
			}
			if (aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null)
			{
				SecurityHelper.CreateRawSecurityDescriptor(aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor);
			}
		}

		private void ReloadPropertyIntoPropertyList(MapiContext context, List<Property> props, bool loadValue, bool replaceExisting, StorePropTag propTag)
		{
			int num = -1;
			Property property;
			if (loadValue)
			{
				if (replaceExisting)
				{
					num = props.FindIndex((Property p) => p.Tag == propTag);
				}
				property = this.InternalGetOneProp(context, propTag);
			}
			else
			{
				property = new Property(propTag, null);
			}
			if (num == -1)
			{
				props.Add(property);
				return;
			}
			props[num] = property;
		}

		private bool FolderHasRealFreeBusyNTSD(MapiContext context)
		{
			byte[] buffer = (byte[])base.GetPropertyValueImp(context, PropTag.Folder.AclTableAndSecurityDescriptor);
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, buffer);
			return aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null;
		}

		private bool CheckLocalDirectoryMessageAndHasAdminRight(MapiContext context, ExchangeId mid)
		{
			StorePropTag storePropTag;
			object obj;
			if (this.Fid == base.Logon.FidC.FidRoot && base.Logon.StoreMailbox.TryGetProperty(context, 13334, out storePropTag, out obj))
			{
				byte[] bytes = (byte[])obj;
				ExchangeId id = ExchangeId.CreateFrom8ByteArray(context, base.Logon.StoreMailbox.ReplidGuidMap, bytes, 0);
				ExchangeId id2 = ExchangeId.CreateFrom8ByteArray(context, base.Logon.StoreMailbox.ReplidGuidMap, bytes, 8);
				if (this.Fid == id && mid == id2 && !base.Logon.SystemRights && !base.Logon.AdminRights)
				{
					return false;
				}
			}
			return true;
		}

		private ErrorCode Configure(MapiContext context, MapiLogon logon, ExchangeId fid)
		{
			this.storeFolder = Folder.OpenFolder(context, logon.StoreMailbox, fid);
			if (this.storeFolder == null)
			{
				return ErrorCode.CreateNotFound((LID)39448U);
			}
			Folder folder = this.storeFolder;
			ExchangeId exchangeId = logon.MapiMailbox.IsPublicFolderMailbox ? this.GetAssociatedFolderId(context) : ExchangeId.Null;
			if (exchangeId != ExchangeId.Null)
			{
				this.associatedStoreFolder = Folder.OpenFolder(context, logon.StoreMailbox, exchangeId);
				if (this.associatedStoreFolder != null && this.storeFolder.IsDumpsterMarkedFolder(context))
				{
					folder = this.associatedStoreFolder;
				}
			}
			this.accessCheckState = new AccessCheckState(context, folder);
			base.Logon = logon;
			this.fid = fid;
			base.ParentObject = logon;
			base.IsValid = true;
			return ErrorCode.NoError;
		}

		private ErrorCode ConfigureNew(MapiContext context, MapiLogon logon, ref ExchangeId fid, FolderConfigureFlags flags, MapiFolder parentFolder, MapiFolder sourceFolder)
		{
			base.Logon = logon;
			Folder folder = null;
			if (parentFolder != null)
			{
				folder = parentFolder.StoreFolder;
			}
			if (fid.IsNullOrZero)
			{
				if ((flags & FolderConfigureFlags.CreateSearchFolder) != FolderConfigureFlags.None)
				{
					fid = logon.StoreMailbox.GetNextObjectId(context);
				}
				else
				{
					fid = logon.StoreMailbox.GetNextFolderId(context);
				}
			}
			bool isPublicFolderMailbox = base.Logon.MapiMailbox.IsPublicFolderMailbox;
			bool flag = !isPublicFolderMailbox && logon.StoreMailbox.SharedState.MailboxTypeDetail != MailboxInfo.MailboxTypeDetail.TeamMailbox;
			byte[] array = null;
			if (isPublicFolderMailbox)
			{
				array = new byte[37];
				ParseSerialize.SerializeAsciiString(base.Logon.MailboxGuid.ToString(), array, 0);
			}
			if (parentFolder == null)
			{
				this.storeFolder = Folder.CreateFolder(context, logon.StoreMailbox, fid);
			}
			else
			{
				ErrorCode first = MapiMailboxShape.PerformMailboxShapeQuotaCheck(context, logon, folder, (sourceFolder == null) ? MapiMailboxShape.Operation.CreateFolder : MapiMailboxShape.Operation.CopyFolder, false);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)56384U);
				}
				if ((flags & FolderConfigureFlags.InternalAccess) != FolderConfigureFlags.None)
				{
					if (sourceFolder != null)
					{
						return ErrorCode.CreateNotSupported((LID)42828U);
					}
					if (folder.IsIpmFolder(context))
					{
						return ErrorCode.CreateNotSupported((LID)34636U);
					}
				}
				else
				{
					if (folder.IsInternalAccess(context))
					{
						return ErrorCode.CreateNotSupported((LID)41164U);
					}
					if (sourceFolder != null && sourceFolder.StoreFolder != null && sourceFolder.StoreFolder.IsInternalAccess(context))
					{
						return ErrorCode.CreateNotSupported((LID)57548U);
					}
				}
				if ((flags & FolderConfigureFlags.CreateSearchFolder) != FolderConfigureFlags.None)
				{
					SearchFolder sourceFolder2 = null;
					if (sourceFolder != null)
					{
						if (!sourceFolder.IsSearchFolder())
						{
							throw new StoreException((LID)34552U, ErrorCodeValue.NotSupported);
						}
						if (sourceFolder.IsInstantSearch)
						{
							throw new StoreException((LID)39408U, ErrorCodeValue.NotSupported);
						}
						sourceFolder2 = (SearchFolder)sourceFolder.StoreFolder;
					}
					else if ((flags & FolderConfigureFlags.InstantSearch) != FolderConfigureFlags.None)
					{
						this.isInstantSearch = true;
						if ((flags & FolderConfigureFlags.OptimizedConversationSearch) != FolderConfigureFlags.None)
						{
							this.isOptimizedConversationSearch = true;
						}
					}
					this.storeFolder = SearchFolder.CreateSearchFolder(context, folder, fid, this.isInstantSearch, sourceFolder2);
				}
				else
				{
					Folder sourceFolder3 = null;
					if (sourceFolder != null)
					{
						if (sourceFolder.IsSearchFolder())
						{
							throw new StoreException((LID)50936U, ErrorCodeValue.NotSupported);
						}
						sourceFolder3 = sourceFolder.StoreFolder;
					}
					this.storeFolder = Folder.CreateFolder(context, folder, fid, sourceFolder3, (flags & FolderConfigureFlags.InternalAccess) != FolderConfigureFlags.None);
				}
			}
			this.accessCheckState = new AccessCheckState(context, this.storeFolder);
			this.fid = fid;
			if (this.isInstantSearch)
			{
				this.storeFolder.AddDoNotDeleteReference(this);
			}
			base.ParentObject = base.Logon;
			base.IsValid = true;
			DateTime utcNow = logon.StoreMailbox.UtcNow;
			byte[] mapiAclTableAndSecurityDescriptorBlob = MapiFolder.CreateAclTableAndSecurityDescriptorBlob(context, base.Logon, parentFolder);
			Properties defaultPropertiesForNewFolder = this.GetDefaultPropertiesForNewFolder(utcNow, mapiAclTableAndSecurityDescriptorBlob);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.Catalog, parentFolder, null);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.CISearchEnabled, parentFolder, null);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.MaxCachedViews, parentFolder, null);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.MaxIndices, parentFolder, null);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.AttrHidden, parentFolder, false);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.AttrSystem, parentFolder, false);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.AttrReadOnly, parentFolder, false);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.GWFolder, parentFolder, null);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.DefaultViewEntryId, parentFolder, null);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.SecureOrigination, parentFolder, false);
			this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.ReplicaList, parentFolder, array);
			if (!base.Logon.IsMoveDestination && !base.Logon.IsPublicFolderSystem)
			{
				this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.RetentionAgeLimit, parentFolder, null);
				this.AddPropFromParent(context, defaultPropertiesForNewFolder, PropTag.Folder.DisablePerUserRead, parentFolder, flag);
			}
			if ((flags & FolderConfigureFlags.CreateIpmFolder) == FolderConfigureFlags.CreateIpmFolder)
			{
				defaultPropertiesForNewFolder.Add(PropTag.Folder.IPMFolder, true);
			}
			if ((flags & FolderConfigureFlags.CreateLastWriterWinsFolder) == FolderConfigureFlags.CreateLastWriterWinsFolder)
			{
				defaultPropertiesForNewFolder.Add(PropTag.Folder.ResolveMethod, 1);
			}
			base.InternalSetPropsShouldNotFail(context, defaultPropertiesForNewFolder);
			return ErrorCode.NoError;
		}

		private void AddPropFromParent(MapiContext context, Properties propsToSet, StorePropTag propTag, MapiFolder parentFolder, object defaultValue)
		{
			object obj = defaultValue;
			if (parentFolder != null)
			{
				Property property = parentFolder.InternalGetOneProp(context, propTag);
				if (property.IsError)
				{
					obj = defaultValue;
				}
				else
				{
					obj = property.Value;
				}
			}
			if (obj != null)
			{
				propsToSet.Add(propTag, obj);
			}
		}

		internal void SetCurrentPublicFolderDumpsterHolder(MapiContext context, ExchangeId currentDumpsterHolderId)
		{
			base.InternalSetPropsShouldNotFail(context, new Properties(MapiFolder.PublicFolderDumpsterHolderPropTagArray, new object[]
			{
				currentDumpsterHolderId.To22ByteArray()
			}));
		}

		private void InternalCreateAssociatedDumpsterFolder(MapiContext context, MapiFolder currentDumpsterHolder)
		{
			using (MapiFolder mapiFolder = currentDumpsterHolder.CreateFolderHelper(context, Guid.NewGuid().ToString(), (LID)41536U, "Unable to create the associated dumpster folder"))
			{
				mapiFolder.SetAssociatedFolderId(context, this.Fid, true);
				this.SetAssociatedFolderId(context, mapiFolder.Fid, false);
				this.associatedStoreFolder = Folder.OpenFolder(context, base.Logon.StoreMailbox, mapiFolder.Fid);
			}
		}

		private MapiFolder CreateFolderHelper(MapiContext context, string displayName, LID lid, string errorMessage)
		{
			MapiFolder mapiFolder = null;
			ExchangeId zero = ExchangeId.Zero;
			ErrorCode errorCode = MapiFolder.CreateFolder(context, base.Logon, ref zero, false, FolderConfigureFlags.None, this, null, false, out mapiFolder);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException(lid, errorCode, errorMessage);
			}
			StorePropTag[] tags = new StorePropTag[]
			{
				PropTag.Folder.DisplayName,
				PropTag.Folder.Comment
			};
			object[] values = new object[]
			{
				displayName,
				string.Empty
			};
			mapiFolder.InternalSetPropsShouldNotFail(context, new Properties(tags, values));
			return mapiFolder;
		}

		public override void OnRelease(MapiContext context)
		{
			if (this.IsInstantSearch && this.storeFolder != null)
			{
				this.storeFolder.RemoveDoNotDeleteReference(this);
				ExchangeId searchFolderId = this.fid;
				MapiFolder.QueueInstantSearchDeletion(this.storeFolder.Mailbox, searchFolderId);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiFolder>(this);
		}

		private Properties GetDefaultPropertiesForNewFolder(DateTime dateTime, byte[] mapiAclTableAndSecurityDescriptorBlob)
		{
			return new Properties(20)
			{
				{
					PropTag.Folder.NTSDModificationTime,
					dateTime
				},
				{
					PropTag.Folder.CreationTime,
					dateTime
				},
				{
					PropTag.Folder.LocalCommitTime,
					dateTime
				},
				{
					PropTag.Folder.HierRev,
					dateTime
				},
				{
					PropTag.Folder.DesignInProgress,
					false
				},
				{
					PropTag.Folder.AclTableAndSecurityDescriptor,
					mapiAclTableAndSecurityDescriptorBlob
				}
			};
		}

		internal const int ReasonableBulkOperationBatchSize = 100;

		internal const int NumberOfIdsForReserveRangeOfIDs = 65536;

		internal const string DumpsterRootPublicFolderName = "DUMPSTER_ROOT";

		internal const string DumpsterExtendPublicFolderName = "DUMPSTER_EXTEND";

		internal const string PublicFolderDumpsterReservedPrefix = "RESERVED_";

		internal static StorePropTag[] PublicFolderDumpsterHolderPropTagArray = new StorePropTag[]
		{
			PropTag.Folder.CurrentIPMWasteBasketContainerEntryId
		};

		private static int deleteInstantSearchActionSlot = -1;

		private ExchangeId fid;

		private Folder storeFolder;

		private Folder associatedStoreFolder;

		private bool isInstantSearch;

		private bool isOptimizedConversationSearch;

		private Func<ErrorCode> deleteMessageTestHook;

		private AccessCheckState accessCheckState;
	}
}
