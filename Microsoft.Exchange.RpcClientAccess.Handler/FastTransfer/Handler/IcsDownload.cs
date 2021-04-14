using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class IcsDownload : FastTransferDownload, IServiceProvider<IcsStateHelper>, IIcsStateCheckpoint, WatsonHelper.IProvideWatsonReportData
	{
		public IcsDownload(ReferenceCount<CoreFolder> sourceFolder, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, Func<IcsDownload, IFastTransferProcessor<FastTransferDownloadContext>> initialFastTransferObjectFactory, Logon logon) : base(FastTransferDownloadContext.CreateForIcs(sendOptions, logon.String8Encoding, logon.ResourceTracker, PropertyFilterFactory.IncludeAllFactory, false), logon)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IcsDownload>(this);
				this.initialFastTransferObjectFactory = initialFastTransferObjectFactory;
				this.icsStateHelper = new IcsStateHelper(sourceFolder);
				this.coreFolderReference = sourceFolder;
				this.coreFolderReference.AddRef();
				disposeGuard.Success();
			}
		}

		public ReferenceCount<CoreFolder> FolderReference
		{
			get
			{
				return this.coreFolderReference;
			}
		}

		public IcsState IcsState
		{
			get
			{
				base.CheckDisposed();
				return this.icsStateHelper.IcsState;
			}
		}

		protected override int InternalGetNextBuffer(ArraySegment<byte> buffer)
		{
			this.EnsureContextInitialized();
			return base.InternalGetNextBuffer(buffer);
		}

		protected override void InternalDispose()
		{
			this.coreFolderReference.Release();
			Util.DisposeIfPresent(this.icsStateHelper);
			base.InternalDispose();
		}

		IcsStateHelper IServiceProvider<IcsStateHelper>.Get()
		{
			base.CheckDisposed();
			return this.icsStateHelper;
		}

		IFastTransferProcessor<FastTransferDownloadContext> IIcsStateCheckpoint.CreateIcsStateCheckpointFastTransferObject()
		{
			base.CheckDisposed();
			return this.icsStateHelper.CreateIcsStateFastTransferObject();
		}

		protected override WatsonReportActionType WatsonReportActionType
		{
			get
			{
				return WatsonReportActionType.IcsDownload;
			}
		}

		private void EnsureContextInitialized()
		{
			if (!this.isInitialFastTransferObjectCreated)
			{
				base.Context.PushInitial(this.initialFastTransferObjectFactory(this));
				this.isInitialFastTransferObjectCreated = true;
			}
		}

		string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
		{
			base.CheckDisposed();
			return string.Format("SyncRootFolder.DisplayName: {0}\r\nFastTransferContext: {1}", this.coreFolderReference.ReferencedObject.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.DisplayName, string.Empty), base.Context);
		}

		private readonly Func<IcsDownload, IFastTransferProcessor<FastTransferDownloadContext>> initialFastTransferObjectFactory;

		private readonly IcsStateHelper icsStateHelper;

		private readonly ReferenceCount<CoreFolder> coreFolderReference;

		private bool isInitialFastTransferObjectCreated;
	}
}
