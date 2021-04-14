using System;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Resume", "MailboxDatabaseCopy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumeDatabaseCopy : DatabaseCopyStateAction
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "DisableReplayLag", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override DatabaseCopyIdParameter Identity
		{
			get
			{
				return (DatabaseCopyIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter ReplicationOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ReplicationOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ReplicationOnly"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DisableReplayLag")]
		public SwitchParameter DisableReplayLag
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableReplayLag"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DisableReplayLag"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DisableReplayLag")]
		public string DisableReplayLagReason
		{
			get
			{
				return (string)base.Fields["DisableReplayLagReason"];
			}
			set
			{
				base.Fields["DisableReplayLagReason"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.ReplicationOnly)
				{
					return Strings.ConfirmationMessageResumeDatabaseCopyReplicationIdentity(base.DatabaseName, base.Server.Name);
				}
				if (this.DisableReplayLag)
				{
					return Strings.ConfirmationMessageDisableReplayLag(base.DatabaseName, base.Server.Name);
				}
				return Strings.ConfirmationMessageResumeDatabaseCopyIdentity(base.DatabaseName, base.Server.Name);
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.IsSuspendOperation = false;
			base.IsReplayLagManagementOperation = this.DisableReplayLag;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (base.HasErrors)
				{
					TaskLogger.LogExit();
				}
				else
				{
					if (base.IsOperationRunOnSource && !this.DisableReplayLag)
					{
						this.WriteWarning(Strings.ResumeSgcOnHostServer(base.DatabaseName, base.Server.Name));
					}
					base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, base.Server, true, new DataAccessTask<DatabaseCopy>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		internal override void ProcessRecordWorker(ReplayConfiguration replayConfiguration)
		{
			ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "Resume-DBC: InternalProcessRecord: {0}", this.DataObject.Identity);
			ExTraceGlobals.PFDTracer.TracePfd<int, ObjectId>((long)this.GetHashCode(), "PFD CRS {0} Resume-DBC initiated for : InternalProcessRecord: {1}", 28635, this.DataObject.Identity);
			if (this.DisableReplayLag)
			{
				Database database = this.DataObject.GetDatabase<Database>();
				ReplayRpcClientHelper.RpccDisableReplayLag(base.Server.Name, database.Guid, this.DisableReplayLagReason, ActionInitiatorType.Administrator);
				return;
			}
			if (!base.UseRpc)
			{
				this.ResumeUsingState(replayConfiguration);
			}
			else if (!base.Stopping)
			{
				Thread thread = base.BeginRpcOperation();
				try
				{
					TimeSpan timeoutRpc = DatabaseCopyStateAction.TimeoutRpc;
					if (!thread.Join(timeoutRpc))
					{
						ExTraceGlobals.CmdletsTracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "Resume-DBC: resume is being slow: timeout={0}", timeoutRpc);
						this.WriteWarning(Strings.ResumeSgcTimeout);
						this.m_event.WaitOne();
					}
				}
				finally
				{
					if (this.m_Exception != null)
					{
						ErrorCategory category;
						this.TranslateException(ref this.m_Exception, out category);
						if (this.m_Exception is ReplayServiceSuspendWantedClearedException)
						{
							base.WriteWarning(this.m_Exception.Message);
							this.m_fSuccess = true;
						}
						else if (this.m_Exception is ReplayServiceResumeRpcPartialSuccessCatalogFailedException)
						{
							base.WriteWarning(this.m_Exception.Message);
							this.m_fSuccess = true;
						}
						else if (this.m_Exception is ReplayServiceResumeRpcFailedSeedingException)
						{
							base.WriteWarning(this.m_Exception.Message);
						}
						else if (!this.m_fFallbackToState)
						{
							this.WriteError(this.m_Exception, category, null, false);
						}
						else if (!base.Stopping)
						{
							ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "ProcessRecordWorker: There was an RPC connection error, so now falling back to Resume through the State, for {0}.", this.DataObject.Identity);
							this.WriteWarning(Strings.ResumeSgcFallbackToState(this.DataObject.Identity.ToString(), this.m_Exception.Message));
						}
					}
				}
				if (!this.m_fSuccess && base.Stopping)
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "Resume was cancelled for {0}", this.DataObject.Identity);
					return;
				}
				if (!this.m_fSuccess && this.m_fFallbackToState)
				{
					ReplayConfiguration replayConfiguration2 = base.ConstructReplayConfiguration(this.DataObject.GetDatabase<Database>());
					this.ResumeUsingState(replayConfiguration2);
				}
			}
			ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "ResumeReplay() for {0}", this.DataObject.Identity);
			if (this.m_fSuccess)
			{
				ExTraceGlobals.PFDTracer.TracePfd<int, ObjectId>((long)this.GetHashCode(), "PFD CRS {0} Sucessfully ResumeReplay() for {1}", 24539, this.DataObject.Identity);
				if (this.m_fFallbackToState)
				{
					ReplayEventLogConstants.Tuple_ResumeMarkedForDatabaseCopy.LogEvent(null, new object[]
					{
						this.DataObject.Identity
					});
				}
			}
		}

		private void ResumeUsingState(ReplayConfiguration replayConfiguration)
		{
			ReplayState replayState = replayConfiguration.ReplayState;
			replayState.SuspendLockRemote.TryLeaveSuspend();
			replayState.SuspendMessage = null;
			this.m_fSuccess = true;
		}

		protected override void RpcOperation()
		{
			Database database = this.DataObject.GetDatabase<Database>();
			if (base.IsActivationRpcSupported)
			{
				DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation;
				if (this.ReplicationOnly)
				{
					flags = DatabaseCopyActionFlags.Replication;
				}
				ReplayRpcClientHelper.RequestResume2(base.GetServerFqdn(), database.Guid, (uint)flags);
				return;
			}
			ReplayRpcClientWrapper.RequestResume(base.GetServerFqdn(), database.Guid);
		}
	}
}
