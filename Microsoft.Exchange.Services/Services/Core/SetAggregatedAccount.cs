using System;
using System.Security;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SetAggregatedAccount : AggregatedAccountCommandBase<AggregatedAccountType>
	{
		public SetAggregatedAccount(CallContext callContext, SetAggregatedAccountRequest request) : base(callContext, request, ExTraceGlobals.SetAggregatedAccountTracer, typeof(SetAggregatedAccountMetadata))
		{
			SetAggregatedAccount <>4__this = this;
			this.connectionSettingsManager = Hookable<Func<ConnectionSettingsManager>>.Create(true, () => ConnectionSettingsManager.GetInstanceForModernOutlook(new LoggingAdapter(callContext, <>4__this.tracer)));
			this.EmailAddress = SmtpAddress.Parse(request.EmailAddress);
			this.UserName = request.UserName;
			this.Authentication = request.Authentication;
			this.Password = request.Password.AsSecureString();
			this.IncomingServer = request.IncomingServer;
			this.IncomingPort = request.IncomingPort;
			if (!string.IsNullOrEmpty(request.IncrementalSyncInterval))
			{
				TimeSpan.TryParse(request.IncrementalSyncInterval, out this.IncrementalSyncInterval);
			}
			this.Security = request.Security;
			this.currentConnectionSettings = null;
			this.setSyncRequest = Hookable<Action>.Create(true, new Action(this.SetSyncRequest));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new SetAggregatedAccountResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<AggregatedAccountType> InternalExecute()
		{
			SyncRequestStatistics syncRequestStatistics = null;
			base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.GetSyncRequestStatisticsCmdletTime, delegate
			{
				syncRequestStatistics = this.GetSyncRequestStatistics();
			});
			base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.GetUpdatedConnectionSettingsTime, delegate
			{
				this.currentConnectionSettings = this.GetUpdatedConnectionSettings(syncRequestStatistics);
			});
			base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.TestLogonWithCurrentSettingsTime, delegate
			{
				this.TestLogonWithCurrentSettings();
			});
			base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.SetSyncRequestCmdletTime, delegate
			{
				this.setSyncRequest.Value();
			});
			this.TraceSuccess("Successfully updated aggregated account.");
			return new ServiceResult<AggregatedAccountType>(new AggregatedAccountType(syncRequestStatistics.TargetExchangeGuid, syncRequestStatistics.RequestGuid, (string)this.EmailAddress, this.UserName, ConnectionSettingsConverter.BuildPublicRepresentation(this.currentConnectionSettings)));
		}

		internal override void SetLogMetadataEnumProperties()
		{
			this.verifyEnvironmentTimeEnum = SetAggregatedAccountMetadata.VerifyEnvironmentTime;
			this.verifyUserIdentityTypeTimeEnum = SetAggregatedAccountMetadata.VerifyUserIdentityTypeTime;
			this.totalTimeEnum = SetAggregatedAccountMetadata.TotalTime;
		}

		protected override string TypeName
		{
			get
			{
				return SetAggregatedAccount.SetAggregatedAccountName;
			}
		}

		private void SetSyncRequest()
		{
			using (PSLocalTask<SetSyncRequest, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetSyncRequestTask(base.CallContext.AccessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				ConnectionSettingsConverter.UpdateSetSyncRequestCmdlet(this.currentConnectionSettings, pslocalTask.Task);
				pslocalTask.Task.Identity = SyncRequestIdParameter.Create(base.CallContext.AccessingADUser, (string)this.EmailAddress);
				if (this.Password != null)
				{
					pslocalTask.Task.Password = this.Password;
				}
				if (this.IncrementalSyncInterval != TimeSpan.Zero)
				{
					pslocalTask.Task.IncrementalSyncInterval = this.IncrementalSyncInterval;
				}
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					this.TraceError(string.Format("Could not update aggregated account in unified mailbox with NetId {0}. Exception: {1}", this.netId, pslocalTask.Error.Exception));
					throw new CannotSetAggregatedAccountException((CoreResources.IDs)4089853131U, pslocalTask.Error.Exception);
				}
			}
		}

		private SyncRequestStatistics GetSyncRequestStatistics()
		{
			Exception ex = null;
			SyncRequestStatistics syncRequestStatistics = base.GetSyncRequestStatisticsForAggregatedAccountGetter.Value(this.EmailAddress, ex);
			if (syncRequestStatistics == null)
			{
				throw new CannotSetAggregatedAccountException((CoreResources.IDs)4089853131U, ex);
			}
			if (syncRequestStatistics.EmailAddress != this.EmailAddress)
			{
				throw new CannotSetAggregatedAccountException(CoreResources.IDs.ErrorNoSyncRequestsMatchedSpecifiedEmailAddress);
			}
			if (!syncRequestStatistics.Flags.HasFlag(RequestFlags.TargetIsAggregatedMailbox))
			{
				throw new CannotSetAggregatedAccountException(CoreResources.IDs.ErrorFoundSyncRequestForNonAggregatedAccount);
			}
			return syncRequestStatistics;
		}

		private ConnectionSettings GetUpdatedConnectionSettings(SyncRequestStatistics syncRequestStatistics)
		{
			ConnectionSettings originalSettings = ConnectionSettingsConverter.BuildConnectionSettings(syncRequestStatistics);
			if (string.IsNullOrEmpty(this.UserName))
			{
				this.UserName = syncRequestStatistics.RemoteCredentialUsername;
			}
			return ConnectionSettingsConverter.BuildUpdateConnectionSettings((!string.IsNullOrEmpty(this.IncomingServer)) ? Fqdn.Parse(this.IncomingServer) : null, (!string.IsNullOrEmpty(this.IncomingPort)) ? new int?(int.Parse(this.IncomingPort)) : null, this.Security, this.Authentication, originalSettings);
		}

		private void TestLogonWithCurrentSettings()
		{
			bool testWasSuccessful = false;
			base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.TestUserCanLogonTime, delegate
			{
				testWasSuccessful = this.checkUserCanLogon.Value(this.currentConnectionSettings, this.EmailAddress, ref this.UserName, this.Password);
			});
			if (testWasSuccessful)
			{
				this.TraceSuccess(string.Format("Calling TestUserCanLogon(EmailAddress={0}) for [{1}] succeeded.", this.EmailAddress, this.currentConnectionSettings));
				base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.CacheValidatedSettings, delegate
				{
					this.connectionSettingsManager.Value().SetConnectionSettings(this.EmailAddress, this.currentConnectionSettings);
				});
				return;
			}
			if (OperationStatusCodeClassification.IsLogonFailedDueToInvalidCredentials(this.currentConnectionSettings.IncomingConnectionSettings.LogonResult))
			{
				this.TraceError(string.Format("Calling TestUserCanLogon(EmailAddress={0}) for [{1}] failed due to invalid credentials.", this.EmailAddress, this.currentConnectionSettings));
				base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.CacheNotValidatedSettings, delegate
				{
					this.connectionSettingsManager.Value().SetConnectionSettings(this.EmailAddress, this.currentConnectionSettings);
				});
				throw new CannotSetAggregatedAccountException(ResponseCodeType.ErrorInvalidAggregatedAccountCredentials, (CoreResources.IDs)3667869681U);
			}
			if (OperationStatusCodeClassification.IsLogonFailedDueToServerHavingTransientProblems(this.currentConnectionSettings.IncomingConnectionSettings.LogonResult))
			{
				this.TraceError(string.Format("Calling TestUserCanLogon(EmailAddress={0}) for [{1}] failed due to remote server having transient problems.", this.EmailAddress, this.currentConnectionSettings));
				base.ExecuteWithProtocolLogging(SetAggregatedAccountMetadata.CacheNotValidatedSettings, delegate
				{
					this.connectionSettingsManager.Value().SetConnectionSettings(this.EmailAddress, this.currentConnectionSettings);
				});
				throw new CannotSetAggregatedAccountException(ResponseCodeType.ErrorServerTemporaryUnavailable, (CoreResources.IDs)3120707856U);
			}
			throw new CannotSetAggregatedAccountException(ResponseCodeType.ErrorNoConnectionSettingsAvailableForAggregatedAccount, CoreResources.ErrorNoConnectionSettingsAvailableForAggregatedAccount((string)this.EmailAddress));
		}

		private static readonly string SetAggregatedAccountName = "SetAggregatedAccount";

		private readonly string Authentication;

		private readonly SmtpAddress EmailAddress;

		private string UserName;

		private readonly SecureString Password;

		private readonly string IncomingServer;

		private readonly string IncomingPort;

		private readonly TimeSpan IncrementalSyncInterval;

		private readonly string Security;

		private ConnectionSettings currentConnectionSettings;

		internal readonly Hookable<Func<ConnectionSettingsManager>> connectionSettingsManager;

		internal readonly Hookable<AddAggregatedAccount.CheckUserCanLogonWithSettings> checkUserCanLogon = Hookable<AddAggregatedAccount.CheckUserCanLogonWithSettings>.Create(true, delegate(ConnectionSettings connectionSettings, SmtpAddress email, ref string userName, SecureString password)
		{
			return connectionSettings.TestUserCanLogon(email, ref userName, password);
		});

		internal readonly Hookable<Action> setSyncRequest;
	}
}
