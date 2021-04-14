using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteSourceFolder : RemoteFolder, ISourceFolder, IFolder, IDisposable
	{
		public RemoteSourceFolder(IMailboxReplicationProxyService mrsProxy, long handle, byte[] folderId, RemoteSourceMailbox mailbox) : base(mrsProxy, handle, folderId, mailbox)
		{
		}

		public new RemoteSourceMailbox Mailbox
		{
			get
			{
				return (RemoteSourceMailbox)base.Mailbox;
			}
		}

		void ISourceFolder.CopyTo(IFxProxy destFxProxy, CopyPropertiesFlags flags, PropTag[] excludeTags)
		{
			MrsTracer.ProxyClient.Function("ISourceFolder.CopyTo({0}): {1}", new object[]
			{
				flags,
				base.FolderName
			});
			byte[] objectData = destFxProxy.GetObjectData();
			DataExportBatch dataExportBatch;
			if (base.ServerVersion[30])
			{
				dataExportBatch = base.MrsProxy.ISourceFolder_CopyTo(base.Handle, (int)flags, DataConverter<PropTagConverter, PropTag, int>.GetData(excludeTags), objectData);
			}
			else
			{
				dataExportBatch = base.MrsProxy.ISourceFolder_Export2(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(excludeTags), objectData);
			}
			long dataExportHandle = dataExportBatch.DataExportHandle;
			using (FxProxyReceiver fxProxyReceiver = new FxProxyReceiver(destFxProxy, false))
			{
				using (BufferedReceiver bufferedReceiver = new BufferedReceiver(fxProxyReceiver, false, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression))
				{
					RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportHandle, bufferedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
				}
			}
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			MrsTracer.ProxyClient.Function("ISourceFolder.ExportMessages({0}): {1}", new object[]
			{
				flags,
				base.FolderName
			});
			byte[] objectData = destFolderProxy.GetObjectData();
			DataExportBatch dataExportBatch = base.MrsProxy.ISourceFolder_ExportMessages(base.Handle, (int)flags, entryIds, objectData);
			long dataExportHandle = dataExportBatch.DataExportHandle;
			using (FxProxyReceiver fxProxyReceiver = new FxProxyReceiver(destFolderProxy, false))
			{
				using (BufferedReceiver bufferedReceiver = new BufferedReceiver(fxProxyReceiver, false, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression))
				{
					RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportHandle, bufferedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
				}
			}
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			MrsTracer.ProxyClient.Function("ISourceFolder.EnumerateChanges({0}, {1}): {2}", new object[]
			{
				flags,
				maxChanges,
				base.FolderName
			});
			if (((IMailbox)this.Mailbox).IsMailboxCapabilitySupported(MailboxCapabilities.PagedEnumerateChanges))
			{
				return base.MrsProxy.ISourceFolder_EnumerateChanges2(base.Handle, (int)flags, maxChanges);
			}
			if (maxChanges != 0)
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "ISourceFolder_EnumerateChanges2");
			}
			return base.MrsProxy.ISourceFolder_EnumerateChanges(base.Handle, flags.HasFlag(EnumerateContentChangesFlags.Catchup));
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			MrsTracer.ProxyClient.Function("ISourceFolder.EnumerateMessagesPaged({0}): {1}", new object[]
			{
				maxPageSize,
				base.FolderName
			});
			return base.MrsProxy.ISourceFolder_EnumerateMessagesPaged(base.Handle, maxPageSize);
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			MrsTracer.ProxyClient.Function("ISourceFolder.GetEstimatedItemCount(): {0}", new object[]
			{
				base.FolderName
			});
			return base.MrsProxy.ISourceFolder_GetEstimatedItemCount(base.Handle);
		}
	}
}
