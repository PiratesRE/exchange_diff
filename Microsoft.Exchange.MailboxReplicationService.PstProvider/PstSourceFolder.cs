using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PstSourceFolder : PstFolder, ISourceFolder, IFolder, IDisposable
	{
		void ISourceFolder.CopyTo(IFxProxy fxFolderProxy, CopyPropertiesFlags flags, PropTag[] propTagsToExclude)
		{
			MrsTracer.Provider.Function("PstSourceFolder.ISourceFolder.CopyTo", new object[0]);
			bool exportCompleted = false;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				FxCollectorSerializer fxCollectorSerializer = new FxCollectorSerializer(fxFolderProxy);
				fxCollectorSerializer.Config(0, 1);
				using (FastTransferDownloadContext fastTransferDownloadContext = FastTransferDownloadContext.CreateForDownload(FastTransferSendOption.Unicode | FastTransferSendOption.UseCpId | FastTransferSendOption.ForceUnicode, 1U, Encoding.ASCII, NullResourceTracker.Instance, this.GetPropertyFilterFactory(PstMailbox.MoMTPtaFromPta(propTagsToExclude)), false))
				{
					IFastTransferProcessor<FastTransferDownloadContext> fastTransferObject = FastTransferFolderCopyTo.CreateDownloadStateMachine(this.Folder, FastTransferFolderContentBase.IncludeSubObject.None);
					fastTransferDownloadContext.PushInitial(fastTransferObject);
					FxUtils.TransferFxBuffers(fastTransferDownloadContext, fxCollectorSerializer);
				}
				exportCompleted = true;
				fxFolderProxy.Flush();
			}, delegate(Exception ex)
			{
				if (!exportCompleted)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(fxFolderProxy.Flush), null);
				}
				return false;
			});
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			throw new NotImplementedException();
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			throw new NotImplementedException();
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			throw new NotImplementedException();
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			throw new NotImplementedException();
		}

		private PropertyFilterFactory GetPropertyFilterFactory(PropertyTag[] additionalPropertyTags)
		{
			if (this.propertyTagsToExclude != null && (additionalPropertyTags == null || this.propertyTagsToExclude.IsSupersetOf(additionalPropertyTags)))
			{
				return this.propertyFilterFactory;
			}
			this.propertyTagsToExclude = new HashSet<PropertyTag>(PropertyFilterFactory.ExcludePropertiesForFxCopyFolder);
			if (additionalPropertyTags != null)
			{
				this.propertyTagsToExclude.UnionWith(additionalPropertyTags);
			}
			this.propertyFilterFactory = new PropertyFilterFactory(false, false, this.propertyTagsToExclude.ToArray<PropertyTag>());
			return this.propertyFilterFactory;
		}

		private HashSet<PropertyTag> propertyTagsToExclude;

		private PropertyFilterFactory propertyFilterFactory;
	}
}
