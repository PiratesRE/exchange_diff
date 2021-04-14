using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class IcsDownloadPassThru : ServerObject, IServiceProvider<IcsStateHelper>, IIcsStateCheckpoint
	{
		public IcsDownloadPassThru(ReferenceCount<CoreFolder> sourceFolder, int maxFastTransferBlockSize, Func<IcsDownloadPassThru, SynchronizerProviderBase> initialSynchronizerFactory, Logon logon) : base(logon)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IcsDownloadPassThru>(this);
				this.icsStateHelper = new IcsStateHelper(sourceFolder);
				this.coreFolderReference = sourceFolder;
				this.coreFolderReference.AddRef();
				this.initialSynchronizerFactory = initialSynchronizerFactory;
				this.maxFastTransferBlockSize = maxFastTransferBlockSize;
				this.residualCacheSize = 0;
				this.doneInCache = false;
				this.passThruState = IcsDownloadPassThru.IcsDownloadPassThruState.Initial;
				disposeGuard.Success();
			}
		}

		public static SynchronizerConfigFlags GetSynchronizerConfigFlags(SyncFlag syncFlags, SyncExtraFlag extraFlags, FastTransferSendOption sendOptions)
		{
			SynchronizerConfigFlags synchronizerConfigFlags = SynchronizerConfigFlags.None;
			if ((ushort)(syncFlags & SyncFlag.Unicode) == 1)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.Unicode;
			}
			if ((ushort)(syncFlags & SyncFlag.NoDeletions) == 2)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.NoDeletions;
			}
			if ((ushort)(syncFlags & SyncFlag.NoSoftDeletions) == 4)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.NoSoftDeletions;
			}
			if ((ushort)(syncFlags & SyncFlag.ReadState) == 8)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.ReadState;
			}
			if ((ushort)(syncFlags & SyncFlag.Associated) == 16)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.Associated;
			}
			if ((ushort)(syncFlags & SyncFlag.Normal) == 32)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.Normal;
			}
			if ((ushort)(syncFlags & SyncFlag.NoConflicts) == 64)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.NoConflicts;
			}
			if ((ushort)(syncFlags & SyncFlag.OnlySpecifiedProps) == 128)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.OnlySpecifiedProps;
			}
			if ((ushort)(syncFlags & SyncFlag.NoForeignKeys) == 256)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.NoForeignKeys;
			}
			if ((ushort)(syncFlags & SyncFlag.CatchUp) == 1024)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.Catchup;
			}
			if ((ushort)(syncFlags & SyncFlag.BestBody) == 8192)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.BestBody;
			}
			if ((ushort)(syncFlags & SyncFlag.IgnoreSpecifiedOnAssociated) == 16384)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.IgnoreSpecifiedOnAssociated;
			}
			if ((ushort)(syncFlags & SyncFlag.ProgressMode) == 32768)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.ProgressMode;
			}
			if ((extraFlags & SyncExtraFlag.OrderByDeliveryTime) == SyncExtraFlag.OrderByDeliveryTime)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.OrderByDeliveryTime;
			}
			if ((extraFlags & SyncExtraFlag.ManifestMode) == SyncExtraFlag.ManifestMode)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.ManifestMode;
			}
			if ((extraFlags & SyncExtraFlag.CatchUpFull) == SyncExtraFlag.CatchUpFull)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.CatchupFull;
			}
			if ((byte)(sendOptions & FastTransferSendOption.UseCpId) == 2)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.UseCpId;
			}
			if ((byte)(sendOptions & FastTransferSendOption.ForceUnicode) == 8)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.ForceUnicode;
			}
			if ((byte)(sendOptions & FastTransferSendOption.RecoverMode) == 4)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.FXRecoverMode;
			}
			if ((byte)(sendOptions & FastTransferSendOption.Unicode) == 1)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.Unicode;
			}
			if ((byte)(sendOptions & FastTransferSendOption.PartialItem) == 16)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.PartialItem;
			}
			if ((byte)(sendOptions & FastTransferSendOption.SendPropErrors) == 32)
			{
				synchronizerConfigFlags |= SynchronizerConfigFlags.SendPropErrors;
			}
			return synchronizerConfigFlags;
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

		public int GetNextBuffer(ArraySegment<byte> buffer, out uint steps, out uint progress, out FastTransferState state)
		{
			base.CheckDisposed();
			this.EnsureSynchronizerInitialized();
			if (this.passThruState == IcsDownloadPassThru.IcsDownloadPassThruState.Initial)
			{
				this.passThruState = IcsDownloadPassThru.IcsDownloadPassThruState.InProgress;
			}
			int num = 0;
			steps = 0U;
			progress = 0U;
			state = FastTransferState.Partial;
			while (this.maxFastTransferBlockSize <= buffer.Count - num)
			{
				byte[] array;
				FastTransferState fastTransferState;
				this.synchronizer.GetBuffer(out array, out steps, out progress, out fastTransferState, out this.residualCacheSize, out this.doneInCache);
				state = (FastTransferState)fastTransferState;
				if (state == FastTransferState.Done)
				{
					this.passThruState = IcsDownloadPassThru.IcsDownloadPassThruState.Done;
				}
				if (array.Length > 0)
				{
					Array.Copy(array, 0, buffer.Array, buffer.Offset + num, array.Length);
					num += array.Length;
				}
				if (state == FastTransferState.Error || state == FastTransferState.Done)
				{
					return num;
				}
			}
			if (num == 0)
			{
				throw new BufferTooSmallException();
			}
			return num;
		}

		public bool IsAvailableInCache(int sizeNeeded)
		{
			bool flag = false;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3425054013U, ref flag);
			return (this.doneInCache || sizeNeeded <= this.residualCacheSize || sizeNeeded - this.residualCacheSize < this.maxFastTransferBlockSize) && !flag;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.synchronizer);
			this.coreFolderReference.Release();
			Util.DisposeIfPresent(this.icsStateHelper);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsDownloadPassThru>(this);
		}

		IcsStateHelper IServiceProvider<IcsStateHelper>.Get()
		{
			base.CheckDisposed();
			return this.icsStateHelper;
		}

		IFastTransferProcessor<FastTransferDownloadContext> IIcsStateCheckpoint.CreateIcsStateCheckpointFastTransferObject()
		{
			base.CheckDisposed();
			if (this.passThruState == IcsDownloadPassThru.IcsDownloadPassThruState.InProgress)
			{
				return new FastTransferInjectFailure(new RopExecutionException("Cannot retrieve ICS state if haven't completely downloaded ICS stream", (ErrorCode)2147746067U));
			}
			if (this.passThruState == IcsDownloadPassThru.IcsDownloadPassThruState.Done)
			{
				ISession session = new SessionAdaptor(this.coreFolderReference.ReferencedObject.Session);
				IPropertyBag propertyBag = new MemoryPropertyBag(session);
				IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
				StorageIcsState state = icsStateStream.ToXsoState();
				this.synchronizer.GetFinalState(ref state);
				icsStateStream.FromXsoState(state);
				this.IcsState.Load(IcsStateOrigin.ServerFinal, propertyBag);
				this.passThruState = IcsDownloadPassThru.IcsDownloadPassThruState.Final;
			}
			return this.icsStateHelper.CreateIcsStateFastTransferObject();
		}

		private void EnsureSynchronizerInitialized()
		{
			if (this.synchronizer == null)
			{
				this.synchronizer = this.initialSynchronizerFactory(this);
			}
		}

		private readonly IcsStateHelper icsStateHelper;

		private readonly ReferenceCount<CoreFolder> coreFolderReference;

		private readonly Func<IcsDownloadPassThru, SynchronizerProviderBase> initialSynchronizerFactory;

		private readonly int maxFastTransferBlockSize;

		private SynchronizerProviderBase synchronizer;

		private int residualCacheSize;

		private bool doneInCache;

		private IcsDownloadPassThru.IcsDownloadPassThruState passThruState;

		private enum IcsDownloadPassThruState
		{
			Initial,
			InProgress,
			Done,
			Final
		}
	}
}
