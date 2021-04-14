using System;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
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
	[Cmdlet("Suspend", "MailboxDatabaseCopy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.Low)]
	public sealed class SuspendDatabaseCopy : DatabaseCopyStateAction
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.EnableReplayLag)
				{
					return Strings.ConfirmationMessageEnableReplayLag(base.DatabaseName, base.Server.Name);
				}
				if (!this.ActivationOnly)
				{
					return Strings.ConfirmationMessageSuspendDatabaseCopyIdentity(base.DatabaseName, base.Server.Name);
				}
				return Strings.ConfirmationMessageSuspendDatabaseCopyActivationIdentity(base.DatabaseName, base.Server.Name);
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "EnableReplayLag", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
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
		public string SuspendComment
		{
			get
			{
				return (string)base.Fields["SuspendComment"];
			}
			set
			{
				base.Fields["SuspendComment"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter ActivationOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ActivationOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ActivationOnly"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EnableReplayLag")]
		public SwitchParameter EnableReplayLag
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnableReplayLag"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EnableReplayLag"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.IsSuspendOperation = true;
			base.IsReplayLagManagementOperation = this.EnableReplayLag;
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
					if (base.IsOperationRunOnSource && !this.ActivationOnly && !this.EnableReplayLag)
					{
						base.WriteError(new InvalidOperationException(Strings.SuspendSgcOnHostServer(base.DatabaseName, base.Server.Name)), ErrorCategory.InvalidOperation, this.Identity);
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
			ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId, string>((long)this.GetHashCode(), "Suspend-DBC: ProcessRecordWorker: {0}, {1}", this.DataObject.Identity, this.SuspendComment);
			ExTraceGlobals.PFDTracer.TracePfd<int, ObjectId, string>((long)this.GetHashCode(), "PFD CRS {0} Suspend-DBC Initiated for : ProcessRecordWorker: {1}, {2}", 25051, this.DataObject.Identity, this.SuspendComment);
			if (this.EnableReplayLag)
			{
				Database database = this.DataObject.GetDatabase<Database>();
				ReplayRpcClientHelper.RpccEnableReplayLag(base.Server.Name, database.Guid, ActionInitiatorType.Administrator);
				return;
			}
			if (this.SuspendComment != null && this.SuspendComment.Length > 512)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "Suspend-DBC {0}: ProcessRecordWorker: SuspendComment length (length={1}, max length={2}) is too long: {3}", new object[]
				{
					this.DataObject.Identity,
					this.SuspendComment.Length,
					512,
					this.SuspendComment
				});
				base.WriteError(new SuspendCommentTooLongException(this.SuspendComment.Length, 512), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (!base.Stopping)
			{
				if (!base.UseRpc)
				{
					this.m_suspendThread = this.BeginSuspendUsingState(replayConfiguration);
				}
				else
				{
					this.m_suspendThread = base.BeginRpcOperation();
				}
				try
				{
					TimeSpan timeSpan = base.UseRpc ? DatabaseCopyStateAction.TimeoutRpc : SuspendDatabaseCopy.Timeout;
					if (!this.m_suspendThread.Join(timeSpan))
					{
						ExTraceGlobals.CmdletsTracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "Suspend-DBC: suspend is being slow: timeout={0}", timeSpan);
						this.WriteWarning(Strings.SuspendSgcTimeout);
						if (!base.UseRpc)
						{
							this.m_suspendThread.Join();
						}
						else
						{
							this.m_event.WaitOne();
						}
					}
				}
				finally
				{
					if (this.m_Exception != null)
					{
						if (this.m_Terminating)
						{
							base.ThrowTerminatingError(this.m_Exception, ErrorCategory.NotSpecified, null);
						}
						else
						{
							ErrorCategory category;
							this.TranslateException(ref this.m_Exception, out category);
							if (this.m_Exception is ReplayServiceSuspendWantedSetException)
							{
								base.WriteWarning(this.m_Exception.Message);
								this.m_fSuccess = true;
							}
							else if (this.m_Exception is ReplayServiceSuspendRpcPartialSuccessCatalogFailedException)
							{
								base.WriteWarning(this.m_Exception.Message);
								this.m_fSuccess = true;
							}
							else if (!this.m_fFallbackToState)
							{
								this.WriteError(this.m_Exception, category, null, false);
							}
							else if (!base.Stopping)
							{
								ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "ProcessRecordWorker: There was an RPC connection error, so now falling back to Suspend through the State, for {0}.", this.DataObject.Identity);
								this.WriteWarning(Strings.SuspendSgcFallbackToState(this.DataObject.Identity.ToString(), this.m_Exception.Message));
							}
						}
					}
				}
			}
			if (!this.m_fSuccess && base.Stopping)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "Suspend was cancelled for {0}", this.DataObject.Identity);
				return;
			}
			if (!base.UseRpc && this.m_Exception == null && this.m_fSuccess)
			{
				replayConfiguration.ReplayState.SuspendMessage = this.SuspendComment;
			}
			else if (base.UseRpc && this.m_fFallbackToState)
			{
				ReplayConfiguration replayConfiguration2 = base.ConstructReplayConfiguration(this.DataObject.GetDatabase<Database>());
				replayConfiguration2.ReplayState.SuspendLockRemote.EnterSuspend();
				replayConfiguration2.ReplayState.SuspendMessage = this.SuspendComment;
				this.m_fSuccess = true;
			}
			if (this.m_fSuccess)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, ObjectId>((long)this.GetHashCode(), "Suspended ({0}) for {1}", this.SuspendComment, this.DataObject.Identity);
				ExTraceGlobals.PFDTracer.TracePfd<int, string, ObjectId>((long)this.GetHashCode(), "PFD CRS {0} Sucessfully Suspended ({1}) for {2}", 20955, this.SuspendComment, this.DataObject.Identity);
				if (this.m_fFallbackToState)
				{
					ReplayEventLogConstants.Tuple_SuspendMarkedForDatabaseCopy.LogEvent(null, new object[]
					{
						this.DataObject.Identity
					});
				}
			}
			TaskLogger.LogExit();
		}

		private Thread BeginSuspendUsingState(ReplayConfiguration replayConfiguration)
		{
			ReplayState replayState = replayConfiguration.ReplayState;
			Thread thread = new Thread(new ParameterizedThreadStart(this.SuspendStateThreadProc));
			thread.IsBackground = true;
			thread.Start(replayState);
			return thread;
		}

		private void SuspendStateThreadProc(object stateObject)
		{
			try
			{
				ReplayState replayState = (ReplayState)stateObject;
				replayState.SuspendLockRemote.EnterSuspend();
				this.m_fSuccess = true;
			}
			catch (Exception ex)
			{
				if (AmExceptionHelper.IsKnownClusterException(this, ex))
				{
					this.m_Exception = ex;
				}
				else
				{
					this.m_Exception = ex;
					this.m_Terminating = true;
				}
			}
		}

		protected override void RpcOperation()
		{
			Database database = this.DataObject.GetDatabase<Database>();
			if (!base.IsActivationRpcSupported)
			{
				ReplayRpcClientWrapper.RequestSuspend(base.GetServerFqdn(), database.Guid, this.SuspendComment);
				return;
			}
			DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation;
			if (this.ActivationOnly)
			{
				flags = DatabaseCopyActionFlags.Activation;
			}
			if (base.IsRequestSuspend3RpcSupported)
			{
				ReplayRpcClientHelper.RequestSuspend3(base.GetServerFqdn(), database.Guid, this.SuspendComment, (uint)flags, 2U);
				return;
			}
			ReplayRpcClientWrapper.RequestSuspend2(base.GetServerFqdn(), database.Guid, this.SuspendComment, (uint)flags);
		}

		private const string propnameSuspendComment = "SuspendComment";

		public const int SuspendCommentLengthLimit = 512;

		public static readonly TimeSpan Timeout = new TimeSpan(0, 0, 15);

		private Thread m_suspendThread;

		private bool m_Terminating;
	}
}
