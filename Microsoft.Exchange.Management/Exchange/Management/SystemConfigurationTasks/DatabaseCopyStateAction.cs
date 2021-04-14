using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class DatabaseCopyStateAction : DatabaseCopyActionTask
	{
		protected bool UseRpc
		{
			get
			{
				return this.m_UseRpc;
			}
		}

		protected bool IsSuspendOperation
		{
			get
			{
				return this.m_IsSuspend;
			}
			set
			{
				this.m_IsSuspend = value;
			}
		}

		protected bool IsActivationRpcSupported
		{
			get
			{
				return this.m_isActivationRpcSupported;
			}
			set
			{
				this.m_isActivationRpcSupported = value;
			}
		}

		protected bool IsRequestSuspend3RpcSupported
		{
			get
			{
				return this.m_isRequestSuspend3RpcSupported;
			}
			set
			{
				this.m_isRequestSuspend3RpcSupported = value;
			}
		}

		protected bool IsOperationRunOnSource
		{
			get
			{
				return this.m_isOperationRunOnSource;
			}
			set
			{
				this.m_isOperationRunOnSource = value;
			}
		}

		protected bool IsReplayLagManagementOperation { get; set; }

		protected Server Server
		{
			get
			{
				return this.m_server;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is TaskServerException || AmExceptionHelper.IsKnownClusterException(this, e) || base.IsKnownException(e);
		}

		protected override void InternalStopProcessing()
		{
			TaskLogger.LogEnter();
			if (!this.m_event.Set())
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "InternalStopProcessing: failed to set signal.");
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, DatabaseCopyIdParameter>((long)this.GetHashCode(), "DatabaseCopyStateAction: enter InternalValidate(DB,ident): {0}, {1}", base.DatabaseName, this.Identity);
				base.InternalValidate();
				if (base.HasErrors)
				{
					TaskLogger.LogExit();
				}
				else
				{
					Database database = this.DataObject.GetDatabase<Database>();
					DatabaseAvailabilityGroup dagForDatabase = DagTaskHelper.GetDagForDatabase(database, base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
					DagTaskHelper.PreventTaskWhenTPREnabled(dagForDatabase, this);
					this.m_UseRpc = ReplayRpcVersionControl.IsSuspendRpcSupported(this.Server.AdminDisplayVersion);
					ServerVersion serverVersion = this.IsReplayLagManagementOperation ? ReplayRpcVersionControl.GetCopyStatusEx4RpcSupportVersion : ReplayRpcVersionControl.SuspendRpcSupportVersion;
					if (this.m_UseRpc)
					{
						if (this.IsSuspendOperation)
						{
							base.WriteVerbose(Strings.SuspendSgcUseRpc(this.Server.AdminDisplayVersion.ToString(), serverVersion.ToString()));
						}
						else
						{
							base.WriteVerbose(Strings.ResumeSgcUseRpc(this.Server.AdminDisplayVersion.ToString(), serverVersion.ToString()));
						}
					}
					else if (this.IsReplayLagManagementOperation)
					{
						base.WriteError(new ReplayLagRpcUnsupportedException(this.Server.Name, this.Server.AdminDisplayVersion.ToString(), ReplayRpcVersionControl.GetCopyStatusEx4RpcSupportVersion.ToString()), ExchangeErrorCategory.ServerOperation, this.Server);
					}
					else if (this.IsSuspendOperation)
					{
						base.WriteVerbose(Strings.SuspendSgcUseState(this.Server.Name, this.Server.AdminDisplayVersion.ToString(), ReplayRpcVersionControl.SuspendRpcSupportVersion.ToString()));
					}
					else
					{
						base.WriteVerbose(Strings.ResumeSgcUseState(this.Server.Name, this.Server.AdminDisplayVersion.ToString(), ReplayRpcVersionControl.SuspendRpcSupportVersion.ToString()));
					}
					this.IsActivationRpcSupported = ReplayRpcVersionControl.IsActivationRpcSupported(this.Server.AdminDisplayVersion);
					this.IsRequestSuspend3RpcSupported = ReplayRpcVersionControl.IsRequestSuspend3RpcSupported(this.Server.AdminDisplayVersion);
					this.IsOperationRunOnSource = false;
					DatabaseLocationInfo databaseLocationInfo;
					if (database.ReplicationType == ReplicationType.Remote && RemoteReplayConfiguration.IsServerRcrSource(ADObjectWrapperFactory.CreateWrapper(database), ADObjectWrapperFactory.CreateWrapper(this.Server), out databaseLocationInfo))
					{
						this.IsOperationRunOnSource = true;
					}
					ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseCopyStateAction: leave InternalValidate: {0}", base.DatabaseName);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		internal abstract void ProcessRecordWorker(ReplayConfiguration replayConfiguration);

		protected abstract void RpcOperation();

		protected void RpcThreadProc()
		{
			try
			{
				this.RpcOperation();
				this.m_fSuccess = true;
			}
			catch (TaskServerTransientException ex)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "RpcThreadProc: Caught exception in RPC: {0}", ex.ToString());
				if (!base.Stopping)
				{
					this.m_Exception = ex;
				}
				else
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "RpcThreadProc: Cancel was requested, so leaving RPC Thread.");
				}
			}
			catch (TaskServerException ex2)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "RpcThreadProc: Caught exception in RPC: {0}", ex2.ToString());
				if (!base.Stopping)
				{
					this.m_Exception = ex2;
					if (ex2 is ReplayServiceDownException)
					{
						this.m_fFallbackToState = true;
					}
				}
				else
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "RpcThreadProc: Cancel was requested, so leaving RPC Thread.");
				}
			}
			finally
			{
				if (!this.m_event.Set())
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "RpcThreadProc: failed to set signal.");
				}
			}
		}

		protected Thread BeginRpcOperation()
		{
			if (this.IsSuspendOperation)
			{
				base.WriteVerbose(Strings.SuspendSgcRpcRequest(this.DataObject.Identity.ToString()));
			}
			else
			{
				base.WriteVerbose(Strings.ResumeSgcRpcRequest(this.DataObject.Identity.ToString()));
			}
			Thread thread = new Thread(new ThreadStart(this.RpcThreadProc));
			thread.IsBackground = true;
			thread.Start();
			return thread;
		}

		protected sealed override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				base.Validate(this.DataObject);
				if (!base.HasErrors)
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "DatabaseCopyStateAction: enter InternalProcessRecord: {0}", this.DataObject.Identity);
					Database database = this.DataObject.GetDatabase<Database>();
					if (!this.m_UseRpc)
					{
						ReplayConfiguration replayConfiguration = base.ConstructReplayConfiguration(database);
						this.ProcessRecordWorker(replayConfiguration);
					}
					else
					{
						this.ProcessRecordWorker(null);
					}
					ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "DatabaseCopyStateAction: leave InternalProcessRecord: {0}", this.DataObject.Identity);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected string GetServerFqdn()
		{
			return this.Server.Fqdn;
		}

		public static readonly TimeSpan TimeoutRpc = new TimeSpan(0, 0, 20);

		protected ManualResetEvent m_event = new ManualResetEvent(false);

		protected bool m_fFallbackToState;

		protected bool m_fSuccess;

		protected Exception m_Exception;

		private bool m_UseRpc;

		private bool m_IsSuspend;

		private bool m_isActivationRpcSupported;

		private bool m_isRequestSuspend3RpcSupported;

		private bool m_isOperationRunOnSource;
	}
}
