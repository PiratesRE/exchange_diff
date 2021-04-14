using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MapiDestinationMailbox : MapiMailbox, IDestinationMailbox, IMailbox, IDisposable
	{
		public MapiDestinationMailbox(LocalMailboxFlags flags) : base(flags)
		{
		}

		bool IDestinationMailbox.MailboxExists()
		{
			return base.MapiStore != null;
		}

		CreateMailboxResult IDestinationMailbox.CreateMailbox(byte[] mailboxData, MailboxSignatureFlags sourceSignatureFlags)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.CreateMailbox", new object[0]);
			base.CheckDisposed();
			base.ResolveMDB(true);
			CreateMailboxResult createMailboxResult = base.CreateMailbox(mailboxData, sourceSignatureFlags);
			if (createMailboxResult != CreateMailboxResult.Success)
			{
				return createMailboxResult;
			}
			base.MapiStore = this.ConnectToTargetMailbox(true, base.ServerDN, base.ServerFqdn, MailboxConnectFlags.None);
			this.AfterConnect();
			return CreateMailboxResult.Success;
		}

		void IDestinationMailbox.ProcessMailboxSignature(byte[] mailboxData)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.ProcessMailbox", new object[0]);
			base.CheckDisposed();
			using (base.RHTracker.Start())
			{
				base.ProcessMailboxSignature(mailboxData);
			}
		}

		IDestinationFolder IDestinationMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			IDestinationFolder folder;
			using (base.RHTracker.Start())
			{
				folder = base.GetFolder<MapiDestinationFolder>(entryId);
			}
			return folder;
		}

		IFxProxy IDestinationMailbox.GetFxProxy()
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.GetFxProxy", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			IFxProxy result;
			using (base.RHTracker.Start())
			{
				FxProxyBudgetWrapper proxy = new FxProxyBudgetWrapper(base.MapiStore.GetFxProxyCollector(), true, new Func<IDisposable>(base.RHTracker.Start), new Action<uint>(base.RHTracker.Charge));
				FxProxyWrapper fxProxyWrapper = new FxProxyWrapper(proxy, null);
				result = fxProxyWrapper;
			}
			return result;
		}

		IFxProxyPool IDestinationMailbox.GetFxProxyPool(ICollection<byte[]> folderIds)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.GetFxProxyPool", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			IFxProxyPool result;
			using (base.RHTracker.Start())
			{
				MapiFxProxyPool proxy = new MapiFxProxyPool(this, folderIds);
				IFxProxyPool proxy2 = new FxProxyPoolBudgetWrapper(proxy, true, new Func<IDisposable>(base.RHTracker.Start), new Action<uint>(base.RHTracker.Charge));
				IFxProxyPool fxProxyPool = new FxProxyPoolWrapper(proxy2, null);
				result = fxProxyPool;
			}
			return result;
		}

		PropProblemData[] IDestinationMailbox.SetProps(PropValueData[] pvda)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.SetProps", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			PropValue[] native = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(pvda);
			PropProblemData[] data;
			using (base.RHTracker.Start())
			{
				data = DataConverter<PropProblemConverter, PropProblem, PropProblemData>.GetData(base.MapiStore.SetProps(native));
			}
			return data;
		}

		void IDestinationMailbox.CreateFolder(FolderRec sourceFolder, CreateFolderFlags createFolderFlags, out byte[] newFolderId)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.CreateFolder(\"{0}\")", new object[]
			{
				sourceFolder.FolderName
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			if (createFolderFlags.HasFlag(CreateFolderFlags.InternalAccess))
			{
				throw new InternalAccessFolderCreationIsNotSupportedException();
			}
			FolderRec folderRec = null;
			newFolderId = null;
			using (base.RHTracker.Start())
			{
				if (!createFolderFlags.HasFlag(CreateFolderFlags.FailIfExists) && sourceFolder.EntryId != null)
				{
					OpenEntryFlags flags = OpenEntryFlags.DontThrowIfEntryIsMissing;
					using (MapiFolder mapiFolder = (MapiFolder)base.MapiStore.OpenEntry(sourceFolder.EntryId, flags))
					{
						if (mapiFolder != null)
						{
							folderRec = FolderRec.Create(mapiFolder, null);
							newFolderId = folderRec.EntryId;
						}
					}
				}
				if (newFolderId == null)
				{
					OpenEntryFlags flags2 = OpenEntryFlags.DeferredErrors | OpenEntryFlags.Modify;
					using (MapiFolder mapiFolder2 = (MapiFolder)base.MapiStore.OpenEntry(sourceFolder.ParentId, flags2))
					{
						using (MapiFolder mapiFolder3 = mapiFolder2.CreateFolder(sourceFolder.FolderName, null, false, sourceFolder.FolderType == FolderType.Search, sourceFolder.EntryId))
						{
							newFolderId = mapiFolder3.GetProp(PropTag.EntryId).GetBytes();
						}
						goto IL_160;
					}
				}
				if (!CommonUtils.IsSameEntryId(folderRec.ParentId, sourceFolder.ParentId))
				{
					MrsTracer.Provider.Debug("Existing folder is under the wrong parent. Moving it.", new object[0]);
					((IDestinationMailbox)this).MoveFolder(folderRec.EntryId, folderRec.ParentId, sourceFolder.ParentId);
				}
				IL_160:
				PropTag[] promotedProperties = sourceFolder.GetPromotedProperties();
				if ((promotedProperties != null && promotedProperties.Length > 0) || (sourceFolder.Restrictions != null && sourceFolder.Restrictions.Length > 0) || (sourceFolder.Views != null && sourceFolder.Views.Length > 0) || (sourceFolder.ICSViews != null && sourceFolder.ICSViews.Length > 0))
				{
					using (MapiDestinationFolder folder = base.GetFolder<MapiDestinationFolder>(newFolderId))
					{
						ICSViewData[] icsViews = null;
						if (this.ServerVersion >= Server.E15MinVersion)
						{
							icsViews = sourceFolder.ICSViews;
						}
						folder.SetExtendedProps(promotedProperties, sourceFolder.Restrictions, sourceFolder.Views, icsViews);
					}
				}
			}
		}

		void IDestinationMailbox.MoveFolder(byte[] folderId, byte[] oldParentId, byte[] newParentId)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.MoveFolder", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			OpenEntryFlags flags = OpenEntryFlags.DeferredErrors | OpenEntryFlags.Modify;
			using (base.RHTracker.Start())
			{
				using (MapiFolder mapiFolder = (MapiFolder)base.MapiStore.OpenEntry(oldParentId, flags))
				{
					using (MapiFolder mapiFolder2 = (MapiFolder)base.MapiStore.OpenEntry(newParentId, flags))
					{
						mapiFolder.AllowWarnings = true;
						mapiFolder.CopyFolder(CopyFolderFlags.FolderMove, mapiFolder2, folderId, null);
					}
				}
			}
		}

		void IDestinationMailbox.DeleteFolder(FolderRec folderRec)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.DeleteFolder(\"{0}\")", new object[]
			{
				folderRec.FolderName
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			OpenEntryFlags flags = OpenEntryFlags.DeferredErrors | OpenEntryFlags.Modify;
			using (base.RHTracker.Start())
			{
				using (MapiFolder mapiFolder = (MapiFolder)base.MapiStore.OpenEntry(folderRec.ParentId, flags))
				{
					mapiFolder.AllowWarnings = true;
					mapiFolder.DeleteFolder(folderRec.EntryId);
				}
			}
		}

		void IDestinationMailbox.SetMailboxSecurityDescriptor(RawSecurityDescriptor sd)
		{
			if (base.IsPureMAPI)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			base.SetMailboxSecurityDescriptor(sd);
		}

		void IDestinationMailbox.SetUserSecurityDescriptor(RawSecurityDescriptor sd)
		{
			if (base.IsPureMAPI)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			base.SetUserSecurityDescriptor(sd);
		}

		void IDestinationMailbox.PreFinalSyncDataProcessing(int? sourceMailboxVersion)
		{
			MrsTracer.Provider.Debug("PreFinalSyncDataProcessing({0} is skipped for mapi destination.", new object[]
			{
				sourceMailboxVersion
			});
		}

		ConstraintCheckResultType IDestinationMailbox.CheckDataGuarantee(DateTime commitTimestamp, out LocalizedString failureReason)
		{
			if (this.ServerVersion < Server.E14MinVersion || base.Credential != null)
			{
				MrsTracer.Provider.Debug("CheckDataGuarantee API does not work for pre-E14 or remote servers. Will assume it is satisfied", new object[0]);
				failureReason = LocalizedString.Empty;
				return ConstraintCheckResultType.Satisfied;
			}
			return CommonUtils.DumpsterStatus.CheckReplicationFlushed(base.MdbGuid, commitTimestamp, out failureReason);
		}

		void IDestinationMailbox.ForceLogRoll()
		{
			if (this.ServerVersion < Server.E14SP1MinVersion)
			{
				MrsTracer.Provider.Debug("ForceNewLog API does not work for pre-R5, skipping.", new object[0]);
				return;
			}
			using (ExRpcAdmin rpcAdmin = base.GetRpcAdmin())
			{
				try
				{
					using (base.RHTracker.Start())
					{
						rpcAdmin.ForceNewLog(base.MdbGuid);
					}
				}
				catch (MapiExceptionVersion)
				{
				}
			}
		}

		List<ReplayAction> IDestinationMailbox.GetActions(string replaySyncState, int maxNumberOfActions)
		{
			throw new NotImplementedException();
		}

		void IDestinationMailbox.SetMailboxSettings(ItemPropertiesBase item)
		{
			throw new NotImplementedException();
		}

		protected override Exception GetMailboxInTransitException(Exception innerException)
		{
			MrsTracer.Provider.Error("Destination mailbox is being moved into.", new object[0]);
			return new DestMailboxAlreadyBeingMovedTransientException(innerException);
		}

		protected override OpenEntryFlags GetFolderOpenEntryFlags()
		{
			return OpenEntryFlags.Modify | OpenEntryFlags.DontThrowIfEntryIsMissing;
		}

		protected override MapiStore CreateStoreConnection(string serverLegDN, string serverFqdn, MailboxConnectFlags mailboxConnectFlags)
		{
			if (base.IsPureMAPI || base.UseHomeMDB || base.IsPublicFolderMove || (mailboxConnectFlags & MailboxConnectFlags.NonMrsLogon) != MailboxConnectFlags.None)
			{
				return base.CreateStoreConnection(serverLegDN, serverFqdn, mailboxConnectFlags);
			}
			return this.ConnectToTargetMailbox(false, serverLegDN, serverFqdn, mailboxConnectFlags);
		}

		protected override void DeleteMailboxInternal(int flags)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.DeleteMailbox({0})", new object[]
			{
				flags
			});
			base.CheckDisposed();
			base.DeleteMailboxInternal(flags);
			if (base.IsTitanium && base.MbxType == MailboxType.DestMailboxCrossOrg)
			{
				base.CleanupAdUserAfterDeleteMailbox();
			}
		}

		private MapiStore ConnectToTargetMailbox(bool mailboxMustExist, string serverLegDN, string serverFqdn, MailboxConnectFlags mailboxConnectFlags)
		{
			MrsTracer.Provider.Function("MapiDestinationMailbox.ConnectToTargetMailbox", new object[0]);
			base.CheckDisposed();
			MapiStore mapiStore = null;
			Guid guidMdb = Guid.Empty;
			string text = null;
			bool flag = false;
			OpenStoreFlag openStoreFlag = OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership | OpenStoreFlag.OverrideHomeMdb | OpenStoreFlag.FailIfNoMailbox | OpenStoreFlag.NoLocalization | OpenStoreFlag.MailboxGuid | OpenStoreFlag.DisconnectedMailbox;
			if (base.MbxType == MailboxType.DestMailboxCrossOrg)
			{
				openStoreFlag |= OpenStoreFlag.XForestMove;
			}
			ConnectFlag connectFlag = ConnectFlag.UseAdminPrivilege;
			if (base.Credential == null)
			{
				connectFlag |= ConnectFlag.UseRpcContextPool;
			}
			if (!mailboxMustExist)
			{
				bool flag2;
				for (;;)
				{
					flag2 = false;
					MrsTracer.Provider.Debug("Checking if destination mailbox exists...", new object[0]);
					using (ExRpcAdmin rpcAdmin = base.GetRpcAdmin())
					{
						try
						{
							using (base.RHTracker.Start())
							{
								flag2 = MapiUtils.IsMailboxInDatabase(rpcAdmin, base.MdbGuid, base.MailboxGuid);
							}
						}
						catch (MapiExceptionMdbOffline)
						{
							if (!flag)
							{
								MrsTracer.Provider.Debug("GetMailboxTableInfo returned MdbOffline, forcing AM rediscovery", new object[0]);
								base.ResolveMDB(true);
								serverLegDN = base.ServerDN;
								serverFqdn = base.ServerFqdn;
								flag = true;
								continue;
							}
							throw;
						}
					}
					break;
				}
				if (!flag2)
				{
					MrsTracer.Provider.Debug("Mailbox {0} does not exist in database {1}", new object[]
					{
						base.MailboxGuid,
						base.MdbGuid
					});
					return null;
				}
				MrsTracer.Provider.Debug("Mailbox {0} exists in database {1}", new object[]
				{
					base.MailboxGuid,
					base.MdbGuid
				});
			}
			string userName;
			string domainName;
			string password;
			base.GetCreds(out userName, out domainName, out password);
			if (base.IsTitanium)
			{
				if (base.MbxType == MailboxType.DestMailboxCrossOrg && mailboxMustExist)
				{
					base.RunADRecipientOperation(false, delegate(IRecipientSession adSession)
					{
						ADUser aduser = adSession.Read(base.MailboxId) as ADUser;
						if (aduser == null)
						{
							throw new RecipientNotFoundPermanentException(base.MailboxGuid);
						}
						if (aduser.Database == null || !aduser.Database.Equals(base.MdbId))
						{
							MrsTracer.Provider.Debug("Stamping homeMDB on the destination Ti user", new object[0]);
							aduser.Database = base.MdbId;
							adSession.Save(aduser);
							using (ExRpcAdmin rpcAdmin2 = base.GetRpcAdmin())
							{
								rpcAdmin2.PurgeCachedMailboxObject(base.MailboxGuid);
							}
						}
					});
				}
			}
			else
			{
				guidMdb = base.MdbGuid;
				text = guidMdb.ToString();
				if (mailboxMustExist && this.ServerVersion < Server.E14MinVersion)
				{
					MrsTracer.Provider.Debug("E12 stores do not support open-by-mdb-guid functionality. Will use random mdb guid to connect to a newly created mailbox.", new object[0]);
					guidMdb = Guid.NewGuid();
					text = "(random)";
				}
			}
			TimeSpan timeSpan;
			TimeSpan callTimeout;
			if (base.Credential != null)
			{
				timeSpan = base.TestIntegration.RemoteMailboxConnectionTimeout;
				callTimeout = base.TestIntegration.RemoteMailboxCallTimeout;
			}
			else
			{
				timeSpan = base.TestIntegration.LocalMailboxConnectionTimeout;
				callTimeout = base.TestIntegration.LocalMailboxCallTimeout;
			}
			string applicationId = "Client=MSExchangeMigration";
			if ((mailboxConnectFlags & MailboxConnectFlags.PublicFolderHierarchyReplication) != MailboxConnectFlags.None)
			{
				applicationId = "Client=PublicFolderSystem;Action=PublicFolderHierarchyReplication";
			}
			for (;;)
			{
				string text2 = (!string.IsNullOrEmpty(serverFqdn)) ? serverFqdn : serverLegDN;
				try
				{
					using (base.RHTracker.Start())
					{
						ConnectFlag connectFlag2 = connectFlag;
						if (base.IsTitanium)
						{
							connectFlag2 |= ConnectFlag.AllowLegacyStore;
							MrsTracer.Provider.Debug("Opening Ti MapiStore: serverDN='{0}', mailboxDN='{1}', connectFlags=[{2}], openStoreFlags=[{3}], timeout={4}", new object[]
							{
								text2,
								base.MailboxDN,
								connectFlag2,
								openStoreFlag,
								timeSpan
							});
							mapiStore = MapiStore.OpenMailbox(text2, Server.GetSystemAttendantLegacyDN(serverLegDN), base.MailboxDN, userName, domainName, password, null, connectFlag2, openStoreFlag, null, null, applicationId, timeSpan, callTimeout);
						}
						else
						{
							if (base.IsExchange2007)
							{
								connectFlag2 |= ConnectFlag.AllowLegacyStore;
							}
							MrsTracer.Provider.Debug("Opening MapiStore: serverDN='{0}', mailbox='{1}', mailboxGuid={2}, dbGuid={3}, connectFlags=[{4}], openStoreFlags=[{5}], timeout={6}", new object[]
							{
								text2,
								base.TraceMailboxId,
								base.MailboxGuid.ToString(),
								text,
								connectFlag2,
								openStoreFlag,
								timeSpan
							});
							mapiStore = MapiStore.OpenMailbox(text2, Server.GetSystemAttendantLegacyDN(serverLegDN), base.MailboxGuid, guidMdb, userName, domainName, password, connectFlag2, openStoreFlag, null, null, applicationId, timeSpan, callTimeout, null);
						}
						MapiUtils.StartMapiDeadSessionChecking(mapiStore, base.TraceMailboxId);
					}
				}
				catch (MapiExceptionNotFound originalException)
				{
					base.VerifyMdbIsOnline(originalException);
					if (mailboxMustExist)
					{
						throw;
					}
				}
				catch (MapiExceptionWrongServer originalException2)
				{
					if (base.IsTitanium)
					{
						base.VerifyMdbIsOnline(originalException2);
						if (mailboxMustExist)
						{
							throw;
						}
					}
					else
					{
						if (!flag)
						{
							MrsTracer.Provider.Debug("OpenMailbox returned WrongServer, forcing AM rediscovery", new object[0]);
							base.ResolveMDB(true);
							serverLegDN = base.ServerDN;
							serverFqdn = base.ServerFqdn;
							flag = true;
							continue;
						}
						throw;
					}
				}
				catch (MapiExceptionLogonFailed originalException3)
				{
					if (!base.IsTitanium && !flag)
					{
						MrsTracer.Provider.Debug("OpenMailbox returned LogonFailed, forcing AM rediscovery", new object[0]);
						base.ResolveMDB(true);
						serverLegDN = base.ServerDN;
						serverFqdn = base.ServerFqdn;
						flag = true;
						continue;
					}
					base.VerifyMdbIsOnline(originalException3);
					if (mailboxMustExist)
					{
						throw;
					}
				}
				catch (MapiExceptionMailboxInTransit innerException)
				{
					throw this.GetMailboxInTransitException(innerException);
				}
				break;
			}
			return mapiStore;
		}
	}
}
