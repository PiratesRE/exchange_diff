using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetLogReplayStatus : DisposableBase, ILogReplayStatus
	{
		internal static IDisposable SetRecordPassiveReplayFailureTestHook(Action<Exception> testDelegate)
		{
			return JetLogReplayStatus.recordPassiveReplayFailureTestHook.SetTestHook(testDelegate);
		}

		private bool PatchRequestPending
		{
			get
			{
				return this.patchPageNumber != 0U;
			}
		}

		private static byte[] SerializeDatabaseInfo(JET_DBINFOMISC dbInfo)
		{
			byte[] result = null;
			if (dbInfo != null)
			{
				result = InteropShim.SerializeDatabaseInfo(dbInfo);
			}
			return result;
		}

		public JetLogReplayStatus(Func<bool, bool> passiveDatabaseAttachDetachHandler)
		{
			this.lockObject = new object();
			this.wakeEvent = new ManualResetEvent(false);
			this.corruptedPages = new List<uint>(0);
			this.passiveDatabaseAttachDetachHandler = passiveDatabaseAttachDetachHandler;
			this.logReplayInitiatedEventHandle = new ManualResetEvent(false);
		}

		public void InitializeWithInstance(Instance jetInstanceConfiguredPreInit)
		{
			this.jetInstance = jetInstanceConfiguredPreInit;
			this.enableDbScanInRecovery = this.jetInstance.Parameters.EnableDbScanInRecovery;
			this.jetParamsAreUsable = true;
		}

		public void SetLogReplayInitiatedEvent()
		{
			base.CheckDisposed();
			if (!this.logReplayInitiatedEventHandleWasSet)
			{
				this.logReplayInitiatedEventHandleWasSet = true;
				bool flag = this.logReplayInitiatedEventHandle.Set();
				if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.LogReplayStatusTracer.TraceDebug<string>(0L, "Signalling LogReplayInitiated event handle {0}.", flag ? "succeeded" : "FAILED");
				}
			}
		}

		public void WaitLogReplayInitiatedEvent()
		{
			base.CheckDisposed();
			bool flag = ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag)
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Waiting for log replay to be initiated by the async. log replay thread.");
			}
			bool assertCondition = this.logReplayInitiatedEventHandle.WaitOne(TimeSpan.FromMinutes(10.0));
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Passive database mount timeout.");
			if (flag)
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Done waiting for log replay to be initiated.");
			}
			this.logReplayInitiatedEventHandle.Dispose();
			this.logReplayInitiatedEventHandle = null;
		}

		void ILogReplayStatus.GetReplayStatus(out uint nextLogToReplay, out byte[] databaseInfo, out uint patchPageNumber, out byte[] patchToken, out byte[] patchData, out uint[] corruptPages)
		{
			base.CheckDisposed();
			using (LockManager.Lock(this.lockObject))
			{
				nextLogToReplay = this.nextLogToReplay;
				databaseInfo = JetLogReplayStatus.SerializeDatabaseInfo(this.databaseInfo);
				patchPageNumber = this.patchPageNumber;
				patchToken = this.patchToken;
				patchData = this.patchData;
				corruptPages = ((this.corruptedPages.Count > 0) ? this.corruptedPages.ToArray() : null);
				this.corruptedPages.Clear();
				this.patchPageNumber = 0U;
				this.patchToken = null;
				this.patchData = null;
				this.wakeEvent.Set();
			}
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<uint, uint, string>(0L, "GetReplayStatus: nextLogToReplay={0}, patchPageNumber={1}, corruptPages={2}", nextLogToReplay, patchPageNumber, (corruptPages == null) ? string.Empty : string.Format("{0} entries", corruptPages.Length));
			}
			if (this.passiveReplayException != null && patchData == null && corruptPages == null)
			{
				throw new FatalDatabaseException("LogReplayFailure", this.passiveReplayException);
			}
		}

		void ILogReplayStatus.SetMaxLogGenerationToReplay(uint value, uint flags)
		{
			base.CheckDisposed();
			if ((flags & 1U) != 0U)
			{
				if ((flags & 2U) != 0U)
				{
					this.haRequestsDbScanEnable = true;
				}
				else if (this.haRequestsDbScanEnable)
				{
					using (LockManager.Lock(this.lockObject))
					{
						this.haRequestsDbScanEnable = false;
						if (this.jetParamsAreUsable)
						{
							this.DisableDbScan();
						}
					}
				}
			}
			this.SetMember<uint>(ref this.maxLogGenerationToReplay, value);
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<uint, bool>(0L, "Set max log generation to replay to {0}. ScanEnable={1}", value, this.haRequestsDbScanEnable);
			}
		}

		void ILogReplayStatus.GetDatabaseInformation(out byte[] databaseInfo)
		{
			base.CheckDisposed();
			using (LockManager.Lock(this.lockObject))
			{
				databaseInfo = JetLogReplayStatus.SerializeDatabaseInfo(this.databaseInfo);
			}
		}

		internal void RecordPassiveReplayFailure(Exception e)
		{
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<Exception>(0L, "RecordPassiveReplayFailure: {0}", e);
			}
			this.passiveReplayException = e;
			if (JetLogReplayStatus.recordPassiveReplayFailureTestHook.Value != null)
			{
				JetLogReplayStatus.recordPassiveReplayFailureTestHook.Value(e);
			}
		}

		internal JET_err RecoveryControlForPassiveDatabase(JET_RECOVERYCONTROL status, JET_err errDefault)
		{
			base.CheckDisposed();
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<JET_RECOVERYCONTROL, bool>(0L, "RecoveryControl callback {0}, logReplayInitiatedEventHandleWasSet={1}", status, this.logReplayInitiatedEventHandleWasSet);
			}
			JET_err jet_err = errDefault;
			if (status is JET_SNOPENLOG)
			{
				this.HandleLogOpen((JET_SNOPENLOG)status);
				jet_err = this.WaitForReplayToContinue(errDefault);
			}
			else if (status is JET_SNBEGINUNDO)
			{
				if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(4106628413U);
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.transitionToActive, "Shouldn't begin undo unless we are transitioning to active");
			}
			else if (status is JET_SNMISSINGLOG)
			{
				jet_err = this.HandleMissingLog((JET_SNMISSINGLOG)status, errDefault);
			}
			else if (!(status is JET_SNOPENCHECKPOINT))
			{
				if (status is JET_SNNOTIFICATIONEVENT)
				{
					JET_SNNOTIFICATIONEVENT jet_SNNOTIFICATIONEVENT = (JET_SNNOTIFICATIONEVENT)status;
					if (!this.transitionToActive && jet_SNNOTIFICATIONEVENT.EventID == 301)
					{
						return (JET_err)(-1);
					}
				}
				else if (status is JET_SNSIGNALERROR)
				{
					if (JET_err.SoftRecoveryOnBackupDatabase == status.errDefault)
					{
						jet_err = JET_err.Success;
					}
				}
				else if (status is JET_SNDBATTACHED)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.logReplayInitiatedEventHandleWasSet, "DbAttached should never be the first callback.");
					using (LockManager.Lock(this.lockObject))
					{
						this.jetParamsAreUsable = true;
					}
					if (ConfigurationSchema.EnableReadFromPassive.Value && this.passiveDatabaseAttachDetachHandler != null)
					{
						this.passiveDatabaseAttachDetachHandler(true);
					}
				}
				else if (status is JET_SNDBDETACHED)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.logReplayInitiatedEventHandleWasSet, "DbDetached should never be the first callback.");
					using (LockManager.Lock(this.lockObject))
					{
						this.jetParamsAreUsable = false;
					}
					if (ConfigurationSchema.EnableReadFromPassive.Value && this.passiveDatabaseAttachDetachHandler != null)
					{
						this.passiveDatabaseAttachDetachHandler(false);
					}
				}
			}
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<JET_err>(0L, "RecoveryControl callback returns {0}", jet_err);
			}
			this.SetLogReplayInitiatedEvent();
			return jet_err;
		}

		internal void CorruptedPage(JET_SNCORRUPTEDPAGE corruptedPage)
		{
			base.CheckDisposed();
			int pageNumber = corruptedPage.pageNumber;
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<int>(0L, "CorruptedPage callback: {0}", pageNumber);
			}
			using (LockManager.Lock(this.lockObject))
			{
				this.corruptedPages.Add(checked((uint)pageNumber));
			}
		}

		internal void Progress(JET_SNT snt, JET_SNPROG snprog)
		{
			base.CheckDisposed();
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<int, int>(0L, "Progress callback {0}/{1}", snprog.cunitDone, snprog.cunitTotal);
			}
		}

		internal void PagePatch(JET_SNPATCHREQUEST snpatch)
		{
			base.CheckDisposed();
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug<int>(0L, "PagePatch callback: {0}", snpatch.pageNumber);
			}
			using (LockManager.Lock(this.lockObject))
			{
				this.patchPageNumber = checked((uint)snpatch.pageNumber);
				this.patchToken = snpatch.pvToken;
				this.patchData = snpatch.pvData;
			}
			this.WaitForReplayToContinue(JET_err.Success);
		}

		public JET_err InitStatusCallback(JET_SESID sesid, JET_SNP snp, JET_SNT snt, object data)
		{
			if (snp != JET_SNP.Restore)
			{
				switch (snp)
				{
				case (JET_SNP)18:
				{
					JET_RECOVERYCONTROL jet_RECOVERYCONTROL = (JET_RECOVERYCONTROL)data;
					return this.RecoveryControlForPassiveDatabase(jet_RECOVERYCONTROL, jet_RECOVERYCONTROL.errDefault);
				}
				case (JET_SNP)19:
					switch (snt)
					{
					case (JET_SNT)1101:
					{
						JET_SNPATCHREQUEST snpatch = (JET_SNPATCHREQUEST)data;
						this.PagePatch(snpatch);
						break;
					}
					case (JET_SNT)1102:
					{
						JET_SNCORRUPTEDPAGE corruptedPage = (JET_SNCORRUPTEDPAGE)data;
						this.CorruptedPage(corruptedPage);
						break;
					}
					}
					break;
				default:
					return JET_err.Success;
				}
			}
			else if (snt == JET_SNT.Progress)
			{
				JET_SNPROG snprog = (JET_SNPROG)data;
				this.Progress(snt, snprog);
			}
			return JET_err.Success;
		}

		internal void TransitionToActive()
		{
			base.CheckDisposed();
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Transitioning to active");
			}
			using (LockManager.Lock(this.lockObject))
			{
				this.transitionToActive = true;
				if (this.jetParamsAreUsable)
				{
					this.DisableDbScan();
				}
				this.wakeEvent.Set();
			}
		}

		internal void Cancel()
		{
			base.CheckDisposed();
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Cancelling replay");
			}
			this.SetMember<bool>(ref this.isCancelled, true);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetLogReplayStatus>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.wakeEvent.Dispose();
				if (this.logReplayInitiatedEventHandle != null)
				{
					this.logReplayInitiatedEventHandle.Dispose();
				}
			}
		}

		private void SetMember<T>(ref T member, T value)
		{
			using (LockManager.Lock(this.lockObject))
			{
				member = value;
				this.wakeEvent.Set();
			}
		}

		private JET_err HandleMissingLog(JET_SNMISSINGLOG missingLogStatus, JET_err errDefault)
		{
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Missing log {0} (generation {1}), IsCurrentLog={2}, NextAction={3}, ErrDefault={4}", new object[]
				{
					missingLogStatus.wszLogFile,
					missingLogStatus.lGenMissing,
					missingLogStatus.fCurrentLog,
					missingLogStatus.eNextAction,
					missingLogStatus.errDefault
				});
			}
			if (JET_RECOVERYACTIONS.MissingLogContinueToRedo == missingLogStatus.eNextAction)
			{
				return JET_err.Success;
			}
			using (LockManager.Lock(this.lockObject))
			{
				if (JET_RECOVERYACTIONS.MissingLogContinueToUndo == missingLogStatus.eNextAction && this.transitionToActive)
				{
					if (missingLogStatus.fCurrentLog)
					{
						return JET_err.Success;
					}
					if ((long)missingLogStatus.lGenMissing > (long)((ulong)this.maxLogGenerationToReplay))
					{
						return JET_err.Success;
					}
				}
			}
			return errDefault;
		}

		private void DisableDbScan()
		{
			if (this.jetInstance != null && this.jetInstance.Parameters.EnableDbScanInRecovery != 0)
			{
				this.jetInstance.Parameters.EnableDbScanInRecovery = 0;
			}
		}

		private void EnableDbScan()
		{
			if (this.jetInstance != null && this.jetInstance.Parameters.EnableDbScanInRecovery != this.enableDbScanInRecovery)
			{
				this.jetInstance.Parameters.EnableDbScanInRecovery = this.enableDbScanInRecovery;
			}
		}

		private void HandleLogOpen(JET_SNOPENLOG openLogStatus)
		{
			if (ExTraceGlobals.LogReplayStatusTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.LogReplayStatusTracer.TraceDebug(0L, "Opening log {0} (generation {1}), IsCurrentLog={2} for {3}", new object[]
				{
					openLogStatus.wszLogFile,
					openLogStatus.lGenNext,
					openLogStatus.fCurrentLog,
					openLogStatus.eReason
				});
			}
			if (JET_OpenLog.ForRedo != openLogStatus.eReason)
			{
				return;
			}
			using (LockManager.Lock(this.lockObject))
			{
				if (openLogStatus.cdbinfomisc > 0 && openLogStatus.rgdbinfomisc.Length > 0)
				{
					this.databaseInfo = openLogStatus.rgdbinfomisc[0];
				}
				this.nextLogToReplay = checked((uint)openLogStatus.lGenNext);
				if (this.jetInstance != null)
				{
					if (!this.haRequestsDbScanEnable || this.transitionToActive)
					{
						this.DisableDbScan();
					}
					else
					{
						this.EnableDbScan();
					}
				}
			}
		}

		private JET_err WaitForReplayToContinue(JET_err errDefault)
		{
			JET_err result;
			for (;;)
			{
				using (LockManager.Lock(this.lockObject))
				{
					this.wakeEvent.Reset();
					if (this.isCancelled)
					{
						result = JET_err.RecoveredWithoutUndo;
						break;
					}
					if (this.transitionToActive)
					{
						result = errDefault;
						break;
					}
					if (!this.PatchRequestPending && this.nextLogToReplay <= this.maxLogGenerationToReplay)
					{
						result = errDefault;
						break;
					}
				}
				this.wakeEvent.WaitOne();
			}
			return result;
		}

		private static readonly Hookable<Action<Exception>> recordPassiveReplayFailureTestHook = Hookable<Action<Exception>>.Create(true, null);

		private readonly object lockObject;

		private readonly ManualResetEvent wakeEvent;

		private readonly List<uint> corruptedPages;

		private readonly Func<bool, bool> passiveDatabaseAttachDetachHandler;

		private EventWaitHandle logReplayInitiatedEventHandle;

		private bool logReplayInitiatedEventHandleWasSet;

		private JET_DBINFOMISC databaseInfo = new JET_DBINFOMISC();

		private bool isCancelled;

		private bool transitionToActive;

		private bool jetParamsAreUsable;

		private uint nextLogToReplay;

		private uint maxLogGenerationToReplay;

		private uint patchPageNumber;

		private byte[] patchToken;

		private byte[] patchData;

		private Exception passiveReplayException;

		private Instance jetInstance;

		private int enableDbScanInRecovery;

		private bool haRequestsDbScanEnable;

		[Flags]
		internal enum LogReplayRpcFlags : uint
		{
			None = 0U,
			SetDbScan = 1U,
			EnableDbScan = 2U
		}
	}
}
