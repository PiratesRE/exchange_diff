using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SeederInstanceBase : IIdentityGuid
	{
		public Guid DatabaseGuid
		{
			get
			{
				return this.SeederArgs.InstanceGuid;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.ConfigArgs.Name;
			}
		}

		protected SeederInstanceBase(RpcSeederArgs rpcArgs, ConfigurationArgs configArgs)
		{
			this.SeederArgs = rpcArgs;
			this.ConfigArgs = configArgs;
			this.m_seederStatus = new RpcSeederStatus();
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "SeederInstanceBase constructed with the following arguments: {0}; {1}", this.SeederArgs.ToString(), this.ConfigArgs.ToString());
			this.InitializePerfCounters();
			this.m_completedTimeUtc = DateTime.MaxValue;
			if (!string.IsNullOrEmpty(rpcArgs.SourceMachineName) && !SharedHelper.StringIEquals(rpcArgs.SourceMachineName, configArgs.SourceMachine))
			{
				this.m_fPassiveSeeding = true;
			}
		}

		public DateTime CompletedTimeUtc
		{
			get
			{
				return this.m_completedTimeUtc;
			}
			internal set
			{
				this.m_completedTimeUtc = value;
			}
		}

		public SeederState SeedState
		{
			get
			{
				SeederState lastStateRead;
				lock (this)
				{
					this.m_lastStateRead = this.m_seederStatus.State;
					lastStateRead = this.m_lastStateRead;
				}
				return lastStateRead;
			}
			internal set
			{
				this.m_seederStatus.State = value;
				this.m_lastStateRead = value;
			}
		}

		public string Name
		{
			get
			{
				return this.ConfigArgs.Name;
			}
		}

		public abstract string Identity { get; }

		protected ISetSeeding SetSeedingCallback
		{
			get
			{
				if (this.m_setSeedingCallback == null)
				{
					this.m_setSeedingCallback = this.GetSeederStatusCallback();
				}
				return this.m_setSeedingCallback;
			}
		}

		protected ISetGeneration SetGenerationCallback
		{
			get
			{
				if (this.m_setGenerationCallback == null)
				{
					this.m_setGenerationCallback = this.GetSetGenerationCallback();
				}
				return this.m_setGenerationCallback;
			}
		}

		protected ReplicaSeederPerfmonInstance SeederPerfmonInstance
		{
			get
			{
				return this.m_perfmon;
			}
		}

		protected abstract void ResetPerfmonSeedingProgress();

		public RpcSeederStatus GetSeedStatus()
		{
			RpcSeederStatus result;
			lock (this)
			{
				RpcSeederStatus seederStatus = this.m_seederStatus;
				result = new RpcSeederStatus(seederStatus);
			}
			return result;
		}

		public void BeginDbSeed()
		{
			SeederState seederState;
			SeederState seederState2;
			if (!this.UpdateState(SeederState.SeedInProgress, out seederState, out seederState2))
			{
				throw new SeederOperationAbortedException();
			}
			this.m_seederThread = new Thread(new ThreadStart(this.SeedThreadProc));
			this.m_seederThread.Start();
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeederInstanceBase.BeginDbSeed: Started background worker thread for seeding.");
		}

		public void WaitUntilStopped()
		{
			try
			{
				Thread seederThread = this.m_seederThread;
				if (seederThread != null)
				{
					seederThread.Join();
					this.m_seederThread = null;
				}
			}
			finally
			{
				this.Cleanup();
			}
		}

		protected void SeedThreadProc()
		{
			try
			{
				RpcErrorExceptionInfo rpcErrorExceptionInfo = SeederRpcExceptionWrapper.Instance.RunRpcServerOperation(this.ConfigArgs.Name, delegate()
				{
					this.SeedThreadProcInternal();
				});
				if (rpcErrorExceptionInfo != null && rpcErrorExceptionInfo.IsFailed())
				{
					this.LogErrorExceptionInfo(rpcErrorExceptionInfo, false);
				}
				ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "DatabaseSeederInstance: StoppedEvent set.");
			}
			finally
			{
				this.m_completedTimeUtc = DateTime.UtcNow;
			}
		}

		protected abstract void SeedThreadProcInternal();

		protected void CreateDirectoryIfNecessary(string directoryPath)
		{
			Exception ex = DirectoryOperations.TryCreateDirectory(directoryPath);
			if (ex != null)
			{
				this.LogError(ReplayStrings.SeederFailedToCreateDirectory(directoryPath, ex.ToString()));
			}
		}

		protected void LogError(string error)
		{
			ReplayCrimsonEvents.SeedingErrorOnTarget.Log<Guid, string, string>(this.DatabaseGuid, this.DatabaseName, error);
			ExTraceGlobals.SeederServerTracer.TraceError<string, string>((long)this.GetHashCode(), "LogError: Database ({0}) failed to seed. Reason: {1}", this.ConfigArgs.Name, error);
			bool flag = false;
			try
			{
				Monitor.Enter(this, ref flag);
				SeederState seedState = this.SeedState;
				Exception exception = this.GetException(error, seedState);
				throw exception;
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
					goto IL_60;
				}
				goto IL_60;
				IL_60:;
			}
		}

		protected void LogError(Exception ex)
		{
			ReplayCrimsonEvents.SeedingErrorOnTarget.Log<Guid, string, string>(this.DatabaseGuid, this.DatabaseName, ex.ToString());
			ExTraceGlobals.SeederServerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "LogError: Database ({0}) failed to seed. Reason: {1}", this.ConfigArgs.Name, ex);
			bool flag = false;
			try
			{
				Monitor.Enter(this, ref flag);
				SeederState seedState = this.SeedState;
				Exception exception = this.GetException(this.GetAppropriateErrorMessage(ex), seedState, ex);
				throw exception;
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
					goto IL_6C;
				}
				goto IL_6C;
				IL_6C:;
			}
		}

		protected void LogErrorExceptionInfo(RpcErrorExceptionInfo errorExceptionInfo, bool fThrow)
		{
			Exception ex = null;
			lock (this)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<string, RpcErrorExceptionInfo>((long)this.GetHashCode(), "LogErrorExceptionInfo: Database ({0}) failed to seed. errorExceptionInfo = ({1})", this.ConfigArgs.Name, errorExceptionInfo);
				SeederState seederState;
				SeederState origState;
				bool flag2 = this.UpdateState(SeederState.SeedFailed, out seederState, out origState);
				string errorMessageAndExceptionFromErrorExceptionInfo = this.GetErrorMessageAndExceptionFromErrorExceptionInfo(errorExceptionInfo, origState, out ex);
				string text = string.Format("Msg={0} Ex={1}", errorMessageAndExceptionFromErrorExceptionInfo, ex);
				ReplayCrimsonEvents.SeedingErrorOnTarget.Log<Guid, string, string>(this.DatabaseGuid, this.DatabaseName, text);
				if (flag2 || seederState == SeederState.SeedCancelled || !Cluster.StringIEquals(this.m_lastErrorMessage, errorMessageAndExceptionFromErrorExceptionInfo))
				{
					ExEventLog.EventTuple eventTupleFromState = this.GetEventTupleFromState(origState);
					eventTupleFromState.LogEvent(null, new object[]
					{
						this.ConfigArgs.Name,
						errorMessageAndExceptionFromErrorExceptionInfo
					});
					ExTraceGlobals.SeederServerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Database ({0}) failed to seed with the exception: {1}", this.ConfigArgs.Name, ex);
					this.m_seederStatus.ErrorInfo = errorExceptionInfo;
					this.m_lastErrorMessage = errorMessageAndExceptionFromErrorExceptionInfo;
					if (this.m_setSeedingCallback != null)
					{
						this.CallFailedDbSeed(eventTupleFromState, ex);
					}
				}
				else
				{
					this.m_lastErrorMessage = ex.Message;
				}
			}
			this.CloseSeeding(false);
			if (ex != null && fThrow)
			{
				throw ex;
			}
		}

		protected virtual void CallFailedDbSeed(ExEventLog.EventTuple tuple, Exception ex)
		{
			this.m_setSeedingCallback.FailedDbSeed(tuple, new LocalizedString(ex.Message), new ExtendedErrorInfo(ex));
		}

		protected abstract void CloseSeeding(bool wasSeedSuccessful);

		protected abstract void Cleanup();

		protected virtual bool UpdateState(SeederState intendedState, out SeederState actualState, out SeederState origState)
		{
			bool result = false;
			lock (this)
			{
				actualState = (origState = this.SeedState);
				if (SeedStateGraph.IsTransitionPossible(actualState, intendedState))
				{
					actualState = intendedState;
					this.m_seederStatus.State = intendedState;
					result = true;
					ExTraceGlobals.SeederServerTracer.TraceDebug<SeederState, SeederState>((long)this.GetHashCode(), "DatabaseSeederInstance.UpdateState: Updated seeder state from '{0}' to '{1}'", origState, intendedState);
				}
				else
				{
					ExTraceGlobals.SeederServerTracer.TraceDebug<SeederState, SeederState>((long)this.GetHashCode(), "DatabaseSeederInstance.UpdateState: Could not update seeder state from '{0}' to '{1}'", origState, intendedState);
				}
			}
			return result;
		}

		protected void CheckOperationCancelled()
		{
			if (this.m_fcancelled)
			{
				throw new SeederOperationAbortedException();
			}
		}

		private void InitializePerfCounters()
		{
			try
			{
				this.m_perfmon = ReplicaSeederPerfmon.GetInstance(this.ConfigArgs.Name);
				this.ResetPerfmonSeedingProgress();
				ExTraceGlobals.SeederServerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "DatabaseSeederInstance.InitializePerfCounters initialized for {0} ({1})", this.ConfigArgs.Name, this.Identity);
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<string, string, InvalidOperationException>((long)this.GetHashCode(), "Failed to initialize seeder performance counters for {0} ({1}): {2}", this.ConfigArgs.Name, this.Identity, ex);
				ReplayEventLogConstants.Tuple_SeederPerfCountersLoadFailure.LogEvent(null, new object[]
				{
					this.ConfigArgs.Name,
					ex.ToString()
				});
			}
		}

		private ISetSeeding GetSeederStatusCallback()
		{
			IReplicaInstanceManager replicaInstanceManager = this.ConfigArgs.ReplicaInstanceManager;
			if (replicaInstanceManager == null)
			{
				return null;
			}
			ISetSeeding setSeeding = null;
			try
			{
				setSeeding = replicaInstanceManager.GetSeederStatusCallback(this.SeederArgs.InstanceGuid);
			}
			catch (TaskServerTransientException ex)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<Guid, TaskServerTransientException>((long)this.GetHashCode(), "GetSeederStatusCallback ({0}): ReplicaInstanceManager threw exception: {1}", this.SeederArgs.InstanceGuid, ex);
				this.LogError(ex);
			}
			catch (TaskServerException ex2)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<Guid, TaskServerException>((long)this.GetHashCode(), "GetSeederStatusCallback ({0}): ReplicaInstanceManager threw exception: {1}", this.SeederArgs.InstanceGuid, ex2);
				this.LogError(ex2);
			}
			if (setSeeding == null)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetSeederStatusCallback ({0}): The RI is not running even after re-running ConfigUpdater. The configuration must have gone away.", this.SeederArgs.InstanceGuid);
				throw new SeederOperationAbortedException();
			}
			return setSeeding;
		}

		private ISetGeneration GetSetGenerationCallback()
		{
			IReplicaInstanceManager replicaInstanceManager = this.ConfigArgs.ReplicaInstanceManager;
			if (replicaInstanceManager == null)
			{
				return null;
			}
			ISetGeneration setGeneration = null;
			try
			{
				setGeneration = replicaInstanceManager.GetSetGenerationCallback(this.SeederArgs.InstanceGuid);
			}
			catch (TaskServerTransientException ex)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<Guid, TaskServerTransientException>((long)this.GetHashCode(), "GetSetGenerationCallback ({0}): ReplicaInstanceManager threw exception: {1}", this.SeederArgs.InstanceGuid, ex);
				this.LogError(ex);
			}
			catch (TaskServerException ex2)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<Guid, TaskServerException>((long)this.GetHashCode(), "GetSetGenerationCallback ({0}): ReplicaInstanceManager threw exception: {1}", this.SeederArgs.InstanceGuid, ex2);
				this.LogError(ex2);
			}
			if (setGeneration == null)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetSetGenerationCallback ({0}): The RI is not running even after re-running ConfigUpdater. The configuration must have gone away.", this.SeederArgs.InstanceGuid);
				throw new SeederOperationAbortedException();
			}
			return setGeneration;
		}

		private string GetErrorMessageAndExceptionFromErrorExceptionInfo(RpcErrorExceptionInfo errorExceptionInfo, SeederState origState, out Exception newException)
		{
			string text;
			if (errorExceptionInfo.ReconstitutedException != null)
			{
				text = this.GetMessageAndException(errorExceptionInfo.ReconstitutedException, origState, out newException);
			}
			else
			{
				text = errorExceptionInfo.ErrorMessage;
				newException = this.GetException(text, origState);
			}
			return text;
		}

		private string GetAppropriateErrorMessage(Exception ex)
		{
			string result = (ex != null) ? ex.Message : null;
			if (ex is SeedPrepareException)
			{
				result = ((SeedPrepareException)ex).ErrMessage;
			}
			else if (ex is SeedInProgressException)
			{
				result = ((SeedInProgressException)ex).ErrMessage;
			}
			else if (ex is IHaRpcServerBaseException)
			{
				result = ((IHaRpcServerBaseException)ex).ErrorMessage;
			}
			return result;
		}

		private string GetMessageAndException(Exception ex, SeederState origState, out Exception newException)
		{
			newException = null;
			string text;
			if (ex is SeedPrepareException)
			{
				newException = ex;
				text = ((SeedPrepareException)ex).ErrMessage;
			}
			else if (ex is SeedInProgressException)
			{
				newException = ex;
				text = ((SeedInProgressException)ex).ErrMessage;
			}
			else if (ex is SeederServerException)
			{
				newException = ex;
				text = ((SeederServerException)ex).ErrorMessage;
			}
			else if (ex is SeederServerTransientException)
			{
				newException = ex;
				text = ((SeederServerTransientException)ex).ErrorMessage;
			}
			else if (ex is IHaRpcServerBaseException)
			{
				text = ((IHaRpcServerBaseException)ex).ErrorMessage;
			}
			else
			{
				text = ex.Message;
			}
			if (newException == null)
			{
				newException = this.GetException(text, origState);
			}
			return text;
		}

		private Exception GetException(string error, SeederState origState)
		{
			return this.GetException(error, origState, null);
		}

		private Exception GetException(string error, SeederState origState, Exception innerException)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<SeederState>((long)this.GetHashCode(), "DatabaseSeederInstance: SeederState={0}", origState);
			switch (origState)
			{
			case SeederState.Unknown:
			case SeederState.SeedPrepared:
				return new SeedPrepareException(error, innerException);
			case SeederState.SeedInProgress:
				return new SeedInProgressException(error, innerException);
			case SeederState.SeedCancelled:
				return new SeederOperationAbortedException(innerException);
			}
			return new SeederOperationFailedException(error, innerException);
		}

		private ExEventLog.EventTuple GetEventTupleFromState(SeederState origState)
		{
			switch (origState)
			{
			case SeederState.Unknown:
			case SeederState.SeedPrepared:
				return ReplayEventLogConstants.Tuple_SeedInstancePrepareFailed;
			case SeederState.SeedInProgress:
				return ReplayEventLogConstants.Tuple_SeedInstanceInProgressFailed;
			case SeederState.SeedCancelled:
				return ReplayEventLogConstants.Tuple_SeedInstanceCancelled;
			}
			return ReplayEventLogConstants.Tuple_SeedInstanceAnotherError;
		}

		protected void ReadSeedTestHook()
		{
			this.m_testHookSeedDelayPerCallback = RegistryTestHook.SeedDelayPerCallbackInMilliSeconds;
			this.m_testHookSeedFailAfterProgressPercent = RegistryTestHook.SeedFailAfterProgressPercent;
			this.m_testHookSeedDisableTruncationCoordination = RegistryTestHook.SeedDisableTruncationCoordination;
		}

		protected readonly RpcSeederArgs SeederArgs;

		protected readonly ConfigurationArgs ConfigArgs;

		protected RpcSeederStatus m_seederStatus;

		protected ISetSeeding m_setSeedingCallback;

		protected ISetGeneration m_setGenerationCallback;

		protected string m_lastErrorMessage;

		protected bool m_fcancelled;

		protected bool m_fPassiveSeeding;

		private Thread m_seederThread;

		private ReplicaSeederPerfmonInstance m_perfmon;

		private DateTime m_completedTimeUtc;

		private SeederState m_lastStateRead;

		protected int m_testHookSeedDelayPerCallback;

		protected int m_testHookSeedFailAfterProgressPercent;

		protected bool m_testHookSeedDisableTruncationCoordination;
	}
}
