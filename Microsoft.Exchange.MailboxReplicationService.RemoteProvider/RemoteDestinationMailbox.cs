using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteDestinationMailbox : RemoteMailbox, IDestinationMailbox, IMailbox, IDisposable
	{
		public RemoteDestinationMailbox(string serverName, string remoteOrgName, NetworkCredential remoteCred, ProxyControlFlags proxyControlFlags, IEnumerable<MRSProxyCapabilities> requiredCapabilities, bool useHttps, LocalMailboxFlags flags) : base(serverName, remoteOrgName, remoteCred, proxyControlFlags, requiredCapabilities, useHttps, flags)
		{
		}

		void IDestinationMailbox.CreateFolder(FolderRec sourceFolder, CreateFolderFlags createFolderFlags, out byte[] newFolderId)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.CreateFolder(\"{0}\")", new object[]
			{
				sourceFolder.FolderName
			});
			base.VerifyMailboxConnection();
			if (!base.ServerVersion[8])
			{
				if (sourceFolder.EntryId == null)
				{
					throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationMailbox_CreateFolder");
				}
				base.MrsProxy.IDestinationMailbox_CreateFolder(base.Handle, sourceFolder, createFolderFlags.HasFlag(CreateFolderFlags.FailIfExists));
				newFolderId = sourceFolder.EntryId;
				return;
			}
			else
			{
				if (!base.ServerVersion[50])
				{
					base.MrsProxy.IDestinationMailbox_CreateFolder2(base.Handle, sourceFolder, createFolderFlags.HasFlag(CreateFolderFlags.FailIfExists), out newFolderId);
					return;
				}
				if (createFolderFlags.HasFlag(CreateFolderFlags.CreatePublicFolderDumpster) && !base.ServerVersion[49])
				{
					throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationMailbox_CreateFolder with CanStoreCreatePFDumpster");
				}
				base.MrsProxy.IDestinationMailbox_CreateFolder3(base.Handle, sourceFolder, (int)createFolderFlags, out newFolderId);
				return;
			}
		}

		CreateMailboxResult IDestinationMailbox.CreateMailbox(byte[] mailboxData, MailboxSignatureFlags sourceSignatureFlags)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.CreateMailbox", new object[0]);
			base.VerifyMailboxConnection();
			if (!base.ServerVersion[24])
			{
				return base.MrsProxy.IDestinationMailbox_CreateMailbox(base.Handle, mailboxData);
			}
			return base.MrsProxy.IDestinationMailbox_CreateMailbox2(base.Handle, mailboxData, (int)sourceSignatureFlags);
		}

		void IDestinationMailbox.ProcessMailboxSignature(byte[] mailboxData)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.ProcessMailboxSignature", new object[0]);
			base.MrsProxy.IDestinationMailbox_ProcessMailboxSignature(base.Handle, mailboxData);
		}

		void IDestinationMailbox.DeleteFolder(FolderRec folderRec)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.DeleteFolder(\"{0}\")", new object[]
			{
				folderRec.FolderName
			});
			base.VerifyMailboxConnection();
			base.MrsProxy.IDestinationMailbox_DeleteFolder(base.Handle, folderRec);
		}

		IDestinationFolder IDestinationMailbox.GetFolder(byte[] entryId)
		{
			base.VerifyMailboxConnection();
			long num = base.MrsProxy.IDestinationMailbox_GetFolder(base.Handle, entryId);
			if (num == 0L)
			{
				return null;
			}
			return new RemoteDestinationFolder(base.MrsProxy, num, entryId, this);
		}

		IFxProxy IDestinationMailbox.GetFxProxy()
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.GetFxProxy", new object[0]);
			base.VerifyMailboxConnection();
			byte[] data;
			long handle = base.MrsProxy.IDestinationMailbox_GetFxProxy(base.Handle, out data);
			IDataMessage getDataResponseMsg = FxProxyGetObjectDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyGetObjectDataResponse, data, base.MrsProxyClient.UseCompression);
			BufferedTransmitter destination = new BufferedTransmitter(new RemoteDataImport(base.MrsProxy, handle, getDataResponseMsg), base.ExportBufferSizeKB, true, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression);
			AsynchronousTransmitter destination2 = new AsynchronousTransmitter(destination, true);
			return new FxProxyTransmitter(destination2, true);
		}

		IFxProxyPool IDestinationMailbox.GetFxProxyPool(ICollection<byte[]> folderIds)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.GetFxProxyPool", new object[0]);
			base.VerifyMailboxConnection();
			List<byte[]> list = new List<byte[]>(folderIds);
			byte[] data;
			long handle = base.MrsProxy.IDestinationMailbox_GetFxProxyPool(base.Handle, list.ToArray(), out data);
			IDataMessage getDataResponseMsg = FxProxyPoolGetFolderDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyPoolGetFolderDataResponse, data, base.MrsProxyClient.UseCompression);
			BufferedTransmitter destination = new BufferedTransmitter(new RemoteDataImport(base.MrsProxy, handle, getDataResponseMsg), base.ExportBufferSizeKB, true, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression);
			AsynchronousTransmitter destination2 = new AsynchronousTransmitter(destination, true);
			return new FxProxyPoolTransmitter(destination2, true, base.ServerVersion);
		}

		bool IDestinationMailbox.MailboxExists()
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.MailboxExists", new object[0]);
			base.VerifyMailboxConnection();
			return base.MrsProxy.IDestinationMailbox_MailboxExists(base.Handle);
		}

		void IDestinationMailbox.MoveFolder(byte[] folderId, byte[] oldParentId, byte[] newParentId)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.MoveFolder", new object[0]);
			base.VerifyMailboxConnection();
			base.MrsProxy.IDestinationMailbox_MoveFolder(base.Handle, folderId, oldParentId, newParentId);
		}

		PropProblemData[] IDestinationMailbox.SetProps(PropValueData[] pva)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.SetProps", new object[0]);
			base.VerifyMailboxConnection();
			return base.MrsProxy.IDestinationMailbox_SetProps(base.Handle, pva);
		}

		void IDestinationMailbox.SetMailboxSecurityDescriptor(RawSecurityDescriptor sd)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.SetMailboxSecurityDescriptor", new object[0]);
			base.VerifyMailboxConnection();
			byte[] array;
			if (sd != null)
			{
				array = new byte[sd.BinaryLength];
				sd.GetBinaryForm(array, 0);
			}
			else
			{
				array = null;
			}
			base.MrsProxy.IDestinationMailbox_SetMailboxSecurityDescriptor(base.Handle, array);
		}

		void IDestinationMailbox.SetUserSecurityDescriptor(RawSecurityDescriptor sd)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.SetUserSecurityDescriptor", new object[0]);
			base.VerifyMailboxConnection();
			byte[] array;
			if (sd != null)
			{
				array = new byte[sd.BinaryLength];
				sd.GetBinaryForm(array, 0);
			}
			else
			{
				array = null;
			}
			base.MrsProxy.IDestinationMailbox_SetUserSecurityDescriptor(base.Handle, array);
		}

		void IDestinationMailbox.PreFinalSyncDataProcessing(int? sourceMailboxVersion)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.PreFinalSyncDataProcessing({0})", new object[]
			{
				sourceMailboxVersion
			});
			if (!base.ServerVersion[30])
			{
				MrsTracer.ProxyClient.Debug("PreFinalSyncDataProcessing: Downlevel server does not support CopyToWithFlags call, assuming success", new object[0]);
				return;
			}
			base.VerifyMailboxConnection();
			base.MrsProxy.IDestinationMailbox_PreFinalSyncDataProcessing(base.Handle, sourceMailboxVersion);
		}

		ConstraintCheckResultType IDestinationMailbox.CheckDataGuarantee(DateTime commitTimestamp, out LocalizedString failureReason)
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.CheckDataGuarantee", new object[0]);
			if (!base.ServerVersion[10])
			{
				MrsTracer.ProxyClient.Debug("Downlevel server does not support CheckDataGuarantee call, assuming success", new object[0]);
				failureReason = LocalizedString.Empty;
				return ConstraintCheckResultType.Satisfied;
			}
			base.VerifyMailboxConnection();
			byte[] bytes;
			int result = base.MrsProxy.IDestinationMailbox_CheckDataGuarantee(base.Handle, commitTimestamp, out bytes);
			failureReason = CommonUtils.ByteDeserialize(bytes);
			return (ConstraintCheckResultType)result;
		}

		void IDestinationMailbox.ForceLogRoll()
		{
			MrsTracer.ProxyClient.Function("RemoteDestinationMailbox.ForceLogRoll", new object[0]);
			if (!base.ServerVersion[14])
			{
				MrsTracer.ProxyClient.Debug("Downlevel server does not support ForceLogRoll call, skipping.", new object[0]);
				return;
			}
			base.VerifyMailboxConnection();
			base.MrsProxy.IDestinationMailbox_ForceLogRoll(base.Handle);
		}

		List<ReplayAction> IDestinationMailbox.GetActions(string replaySyncState, int maxNumberOfActions)
		{
			MrsTracer.Provider.Function("RemoteDestinationMailbox.GetActions", new object[0]);
			if (!((IMailbox)this).IsMailboxCapabilitySupported(MailboxCapabilities.PagedGetActions))
			{
				MrsTracer.ProxyClient.Debug("Downlevel server does not support GetActions call.", new object[0]);
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationMailbox_GetActions");
			}
			base.VerifyMailboxConnection();
			return base.MrsProxy.IDestinationMailbox_GetActions(base.Handle, replaySyncState, maxNumberOfActions);
		}

		void IDestinationMailbox.SetMailboxSettings(ItemPropertiesBase item)
		{
			MrsTracer.Provider.Function("RemoteDestinationMailbox.SetMailboxSettings", new object[0]);
			if (!base.ServerVersion[59])
			{
				MrsTracer.ProxyClient.Debug("Downlevel server does not support SetMailboxSettings call", new object[0]);
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationMailbox_SetMailboxSettings");
			}
			base.VerifyMailboxConnection();
			base.MrsProxy.IDestinationMailbox_SetMailboxSettings(base.Handle, item);
		}
	}
}
