using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class AddAggregatedAccount : AggregatedAccountCommandBase<AggregatedAccountType>
	{
		public AddAggregatedAccount(CallContext callContext, AddAggregatedAccountRequest request) : base(callContext, request, ExTraceGlobals.AddAggregatedAccountTracer, typeof(AddAggregatedAccountMetadata))
		{
			AddAggregatedAccount <>4__this = this;
			this.connectionSettingsManager = Hookable<Func<ConnectionSettingsManager>>.Create(true, () => ConnectionSettingsManager.GetInstanceForModernOutlook(new LoggingAdapter(callContext, <>4__this.tracer)));
			this.EmailAddress = SmtpAddress.Parse(request.EmailAddress);
			if (!TimeSpan.TryParse(request.IncrementalSyncInterval, out this.IncrementalSyncInterval))
			{
				this.IncrementalSyncInterval = AddAggregatedAccount.DefaultIncrementalSyncInterval;
			}
			this.MailboxGuid = Guid.NewGuid();
			this.UserName = request.UserName;
			this.Password = request.Password.AsSecureString();
			this.currentConnectionSettings = ConnectionSettingsConverter.BuildConnectionSettings(request);
			this.addAggregatedAccountToADUser = Hookable<Action>.Create(true, new Action(this.AddAggregatedAccountToADUser));
			this.removeAggregatedAccountToADUser = Hookable<Action>.Create(true, new Action(this.RemoveAggregatedAccountFromADUser));
			this.createSyncRequest = Hookable<Func<ConnectionSettings, Exception, SyncRequest>>.Create(true, (ConnectionSettings connSettings, Exception exception) => this.CreateSyncRequest(connSettings, out exception));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			AggregatedAccountType aggregatedAccountType = base.Result.Value;
			if (aggregatedAccountType == null && this.currentConnectionSettings != null)
			{
				aggregatedAccountType = new AggregatedAccountType(this.MailboxGuid, this.subscriptionGuid, (string)this.EmailAddress, this.UserName, ConnectionSettingsConverter.BuildPublicRepresentation(this.currentConnectionSettings));
			}
			return new AddAggregatedAccountResponse(base.Result.Code, base.Result.Error, aggregatedAccountType);
		}

		internal override ServiceResult<AggregatedAccountType> InternalExecute()
		{
			base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.CheckShouldProceedWithRequest, delegate
			{
				this.CheckShouldProceedWithRequestOrThrow();
			});
			if (!this.VerifyCurrentConnectionSettings())
			{
				this.DetectConnectionSettingsForRequestedEmail();
			}
			if (this.currentConnectionSettings.IncomingConnectionSettings.ConnectionType == ConnectionSettingsType.Office365)
			{
				Office365ConnectionSettings office365ConnectionSettings = (Office365ConnectionSettings)this.currentConnectionSettings.IncomingConnectionSettings;
				Office365ConnectionSettings office365ConnectionSettings2 = this.TryDetectExistingLinkedOrgIdInfo();
				if (office365ConnectionSettings2 == null)
				{
					this.CreateOrgIdMsaLink(office365ConnectionSettings);
				}
				else
				{
					if (!office365ConnectionSettings2.IsSameAccount(office365ConnectionSettings))
					{
						this.TraceError(string.Format("A link for EmailAddress={0} ({1}) cannot be added because we do not support more than one link. Existing link: {2}", this.EmailAddress, this.currentConnectionSettings, office365ConnectionSettings2));
						throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorCannotLinkMoreThanOneO365AccountToAnMsa, CoreResources.ErrorCannotLinkMoreThanOneO365AccountToAnMsa(office365ConnectionSettings2.ToString()));
					}
					this.TraceSuccess(string.Format("A MSA <-> OrgID link to EmailAddress={0} ({1}) already exists.", this.EmailAddress, this.currentConnectionSettings));
				}
				return new ServiceResult<AggregatedAccountType>(new AggregatedAccountType(this.MailboxGuid, this.subscriptionGuid, (string)this.EmailAddress, this.UserName, ConnectionSettingsConverter.BuildPublicRepresentation(this.currentConnectionSettings)));
			}
			base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.AddAggregatedMailboxGuidToADUserTime, delegate
			{
				this.addAggregatedAccountToADUser.Value();
			});
			Exception newSyncRequestTaskException = null;
			SyncRequest syncRequest = null;
			base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.NewSyncRequestCmdletTime, delegate
			{
				syncRequest = this.createSyncRequest.Value(this.currentConnectionSettings, newSyncRequestTaskException);
			});
			if (syncRequest != null && newSyncRequestTaskException == null)
			{
				this.TraceSuccess(string.Format("Successfully created new sync request {0} for the unified mailbox with NetId {1}", syncRequest, this.netId));
				this.subscriptionGuid = syncRequest.RequestGuid;
				return new ServiceResult<AggregatedAccountType>(new AggregatedAccountType(this.MailboxGuid, syncRequest.RequestGuid, (string)this.EmailAddress, this.UserName, ConnectionSettingsConverter.BuildPublicRepresentation(this.currentConnectionSettings)));
			}
			base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.MailboxCleanupTime, delegate
			{
				this.removeAggregatedAccountToADUser.Value();
			});
			throw new CannotAddAggregatedAccountException(CoreResources.IDs.ErrorCannotAddAggregatedAccount, newSyncRequestTaskException);
		}

		private void AddAggregatedAccountToADUser()
		{
			using (PSLocalTask<SetMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetMailboxTask(base.CallContext.AccessingPrincipal, "AddAggregatedMailbox"))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Identity = new MailboxIdParameter(base.CallContext.AccessingADUser.ObjectId);
				pslocalTask.Task.AggregatedMailboxGuid = this.MailboxGuid;
				pslocalTask.Task.AddAggregatedAccount = new SwitchParameter(true);
				pslocalTask.Task.ForestWideDomainControllerAffinityByExecutingUser = new SwitchParameter(true);
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					this.TraceError(string.Format("Could not add aggregated mailbox guid {0} to unified mailbox with NetId {1}. Exception: {2}.", this.MailboxGuid, this.netId, pslocalTask.Error.Exception));
					base.CallContext.ProtocolLog.Set(AddAggregatedAccountMetadata.SetMailboxCmdletError, pslocalTask.Error.Exception);
					throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorCannotAddAggregatedAccountMailbox, CoreResources.IDs.ErrorCannotAddAggregatedAccountMailbox);
				}
				ADIdentityInformationCache.Singleton.RemoveUserIdentity(base.CallContext.AccessingPrincipal.Sid, base.CallContext.ADRecipientSessionContext.OrganizationPrefix);
				ExchangePrincipalCache.Remove(base.CallContext.AccessingPrincipal);
				this.TraceSuccess(string.Format("Successfully added aggregated mailbox guid {0} to unified mailbox with NetId {1}", this.MailboxGuid, this.netId));
			}
		}

		private SyncRequest CreateSyncRequest(ConnectionSettings connectionSettings, out Exception newSyncRequestTaskException)
		{
			newSyncRequestTaskException = null;
			SyncRequest syncRequest2;
			using (PSLocalTask<NewSyncRequest, SyncRequest> newSyncRequest = CmdletTaskFactory.Instance.CreateNewSyncRequestTask(base.CallContext.AccessingPrincipal, "AutoDetect"))
			{
				newSyncRequest.CaptureAdditionalIO = true;
				ConnectionSettingsConverter.UpdateNewSyncRequestCmdlet(this.EmailAddress, connectionSettings, newSyncRequest.Task);
				newSyncRequest.Task.Mailbox = new MailboxIdParameter(base.CallContext.AccessingADUser.ObjectId);
				newSyncRequest.Task.AggregatedMailboxGuid = this.MailboxGuid;
				newSyncRequest.Task.Name = this.EmailAddress.ToString();
				newSyncRequest.Task.RemoteEmailAddress = this.EmailAddress;
				newSyncRequest.Task.SkipMerging = new SkippableMergeComponent[]
				{
					SkippableMergeComponent.InitialConnectionValidation
				};
				newSyncRequest.Task.ForestWideDomainControllerAffinityByExecutingUser = new SwitchParameter(true);
				newSyncRequest.Task.BadItemLimit = Unlimited<int>.UnlimitedValue;
				newSyncRequest.Task.LargeItemLimit = Unlimited<int>.UnlimitedValue;
				newSyncRequest.Task.AcceptLargeDataLoss = new SwitchParameter(true);
				if (this.UserName != null)
				{
					newSyncRequest.Task.UserName = this.UserName;
				}
				if (this.Password != null)
				{
					newSyncRequest.Task.Password = this.Password;
				}
				if (this.IncrementalSyncInterval != TimeSpan.Zero)
				{
					newSyncRequest.Task.IncrementalSyncInterval = this.IncrementalSyncInterval;
				}
				Exception newSyncRequestException = null;
				SyncRequest syncRequest = null;
				FaultInjection.FaultInjectionPoint((FaultInjection.LIDs)3951439165U, delegate
				{
					newSyncRequest.Task.Execute();
					if (newSyncRequest.Error != null)
					{
						newSyncRequestException = newSyncRequest.Error.Exception;
					}
					syncRequest = newSyncRequest.Result;
				}, delegate
				{
					newSyncRequestException = new Exception("This is an exception from fault injection action");
				});
				newSyncRequestTaskException = newSyncRequestException;
				if (newSyncRequestException != null || syncRequest == null)
				{
					this.TraceError(string.Format("Could not add aggregated account to unified mailbox with NetId {0}. Error: {1}", this.netId, (newSyncRequestException != null) ? newSyncRequestException.ToString() : "Failed to create a new sync request."));
					base.CallContext.ProtocolLog.Set(AddAggregatedAccountMetadata.SetMailboxCmdletError, newSyncRequestException);
				}
				base.CallContext.ProtocolLog.Set(AddAggregatedAccountMetadata.NewSyncRequestGuid, syncRequest);
				syncRequest2 = syncRequest;
			}
			return syncRequest2;
		}

		private void RemoveAggregatedAccountFromADUser()
		{
			using (PSLocalTask<SetMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetMailboxTask(base.CallContext.AccessingPrincipal, "RemoveAggregatedMailbox"))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Identity = new MailboxIdParameter(base.CallContext.AccessingADUser.ObjectId);
				pslocalTask.Task.AggregatedMailboxGuid = this.MailboxGuid;
				pslocalTask.Task.RemoveAggregatedAccount = new SwitchParameter(true);
				pslocalTask.Task.ForestWideDomainControllerAffinityByExecutingUser = new SwitchParameter(true);
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					this.TraceError(string.Format("Could not remove aggregated mailbox guid {0} from unified mailbox with NetId {1}. Exception: {2}.", this.MailboxGuid, this.netId, pslocalTask.Error.Exception));
					base.CallContext.ProtocolLog.Set(AddAggregatedAccountMetadata.SetMailboxCmdletError, pslocalTask.Error.Exception);
				}
				else
				{
					this.TraceSuccess(string.Format("Successfully removed aggregated mailbox guid {0} from unified mailbox with NetId {1}", this.MailboxGuid, this.netId));
				}
			}
		}

		internal override void SetLogMetadataEnumProperties()
		{
			this.verifyEnvironmentTimeEnum = AddAggregatedAccountMetadata.VerifyEnvironmentTime;
			this.verifyUserIdentityTypeTimeEnum = AddAggregatedAccountMetadata.VerifyUserIdentityTypeTime;
			this.totalTimeEnum = AddAggregatedAccountMetadata.TotalTime;
		}

		private void CheckAggregatedAccountsCountAndThrowIfLimitReached(int count, int limit)
		{
			if (count >= limit)
			{
				this.TraceError(CoreResources.ErrorAggregatedAccountLimitReached(limit));
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorAggregatedAccountLimitReached, CoreResources.ErrorAggregatedAccountLimitReached(limit));
			}
		}

		private void CheckShouldProceedWithRequestOrThrow()
		{
			MailboxSession arg = this.getMailboxIdentityMailboxSession.Value(base.CallContext);
			AggregatedAccountHelper aggregatedAccountHelper = this.createAggregatedAccountHelper.Value(arg, base.CallContext.AccessingADUser);
			List<AggregatedAccountInfo> listOfAccounts = aggregatedAccountHelper.GetListOfAccounts();
			base.CallContext.ProtocolLog.Set(AddAggregatedAccountMetadata.NumberOfAggregatedAccounts, listOfAccounts.Count);
			this.CheckAggregatedAccountsCountAndThrowIfLimitReached(listOfAccounts.Count, AddAggregatedAccount.MaxAggregatedAccountsAllowed.Value);
			if (listOfAccounts.Any((AggregatedAccountInfo o) => o.SmtpAddress == this.EmailAddress))
			{
				this.TraceError(CoreResources.ErrorAggregatedAccountAlreadyExists);
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorAggregatedAccountAlreadyExists, CoreResources.ErrorAggregatedAccountAlreadyExists);
			}
		}

		protected override string TypeName
		{
			get
			{
				return AddAggregatedAccount.AddAggregatedAccountName;
			}
		}

		private void DetectConnectionSettingsForRequestedEmail()
		{
			try
			{
				foreach (ConnectionSettings connectionSettings in this.connectionSettingsManager.Value().GetConnectionSettingsMatchingEmail(this.EmailAddress))
				{
					this.currentConnectionSettings = connectionSettings;
					if (ConnectionTypeClassification.IsExchangeServer(this.currentConnectionSettings.IncomingConnectionSettings.ConnectionType))
					{
						this.TraceError(string.Format("The detected connection settings: [{0}] are not currently supported for aggregation!", this.currentConnectionSettings));
						this.currentConnectionSettings = null;
						throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorAccountNotSupportedForAggregation, CoreResources.ErrorAccountNotSupportedForAggregation((string)this.EmailAddress));
					}
					if (this.VerifyCurrentConnectionSettings())
					{
						this.TraceSuccess(string.Format("Successfully detected connection settings: {0}.", this.currentConnectionSettings));
						return;
					}
				}
			}
			catch (DataSourceTransientException arg)
			{
				this.TraceError(string.Format("Transient directory exception: ", arg));
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorNoConnectionSettingsAvailableForAggregatedAccount, CoreResources.ErrorActiveDirectoryTransientError);
			}
			catch (DataSourceOperationException arg2)
			{
				this.TraceError(string.Format("Permanent directory exception: ", arg2));
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorNoConnectionSettingsAvailableForAggregatedAccount, CoreResources.ErrorActiveDirectoryPermanentError);
			}
			this.TraceError(string.Format("Could not find connection settings for {0}.", this.EmailAddress));
			this.currentConnectionSettings = null;
			throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorNoConnectionSettingsAvailableForAggregatedAccount, CoreResources.ErrorNoConnectionSettingsAvailableForAggregatedAccount((string)this.EmailAddress));
		}

		private Office365ConnectionSettings TryDetectExistingLinkedOrgIdInfo()
		{
			return null;
		}

		private void CreateOrgIdMsaLink(Office365ConnectionSettings connectionSettings)
		{
			if (connectionSettings.AdUser == null)
			{
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorCannotAddAggregatedAccount, CoreResources.ErrorUserADObjectNotFound);
			}
			this.TraceSuccess(string.Format("A link to EmailAddress={0} ({1}) was created successfully.", this.EmailAddress, this.currentConnectionSettings));
		}

		private bool VerifyCurrentConnectionSettings()
		{
			if (this.currentConnectionSettings == null)
			{
				return false;
			}
			bool curentConnectionSettingsAreUsable = false;
			base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.TestUserCanLogonTime, delegate
			{
				curentConnectionSettingsAreUsable = this.checkUserCanLogon.Value(this.currentConnectionSettings, this.EmailAddress, ref this.UserName, this.Password);
			});
			if (curentConnectionSettingsAreUsable)
			{
				this.TraceSuccess(string.Format(CultureInfo.InvariantCulture, "Calling TestUserCanLogon(EmailAddress={0}) for [{1}] succeeded.", new object[]
				{
					this.EmailAddress,
					this.currentConnectionSettings
				}));
				base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.CacheValidatedSettings, delegate
				{
					this.connectionSettingsManager.Value().SetConnectionSettings(this.EmailAddress, this.currentConnectionSettings);
				});
				return true;
			}
			if (OperationStatusCodeClassification.IsLogonFailedDueToInvalidCredentials(this.currentConnectionSettings.IncomingConnectionSettings.LogonResult))
			{
				this.TraceError(string.Format(CultureInfo.InvariantCulture, "Calling TestUserCanLogon(EmailAddress={0}) for [{1}] failed due to invalid credentials.", new object[]
				{
					this.EmailAddress,
					this.currentConnectionSettings
				}));
				base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.CacheNotValidatedSettings, delegate
				{
					this.connectionSettingsManager.Value().SetConnectionSettings(this.EmailAddress, this.currentConnectionSettings);
				});
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorInvalidAggregatedAccountCredentials, (CoreResources.IDs)3667869681U);
			}
			if (OperationStatusCodeClassification.IsLogonFailedDueToServerHavingTransientProblems(this.currentConnectionSettings.IncomingConnectionSettings.LogonResult))
			{
				this.TraceError(string.Format("Calling TestUserCanLogon(EmailAddress={0}) for [{1}] failed due to remote server having transient problems.", this.EmailAddress, this.currentConnectionSettings));
				base.ExecuteWithProtocolLogging(AddAggregatedAccountMetadata.CacheNotValidatedSettings, delegate
				{
					this.connectionSettingsManager.Value().SetConnectionSettings(this.EmailAddress, this.currentConnectionSettings);
				});
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorServerTemporaryUnavailable, (CoreResources.IDs)3120707856U);
			}
			this.TraceError(string.Format(CultureInfo.InvariantCulture, "Calling TestUserCanLogon(EmailAddress={0}) for [{1}] failed.", new object[]
			{
				this.EmailAddress,
				this.currentConnectionSettings
			}));
			if (this.currentConnectionSettings != null && this.currentConnectionSettings.SourceId == ConnectionSettings.UserSpecified)
			{
				this.currentConnectionSettings = null;
				throw new CannotAddAggregatedAccountException(ResponseCodeType.ErrorNoConnectionSettingsAvailableForAggregatedAccount, CoreResources.ErrorNoConnectionSettingsAvailableForAggregatedAccount((string)this.EmailAddress));
			}
			return false;
		}

		private static readonly string AddAggregatedAccountName = "AddAggregatedAccount";

		private static TimeSpan DefaultIncrementalSyncInterval = TimeSpan.FromMinutes(15.0);

		internal static readonly IntAppSettingsEntry MaxAggregatedAccountsAllowed = new IntAppSettingsEntry("AddAggregatedAccount.MaxAggregatedAccountsAllowed", 10, ExTraceGlobals.AddAggregatedAccountTracer);

		private readonly SmtpAddress EmailAddress;

		private readonly TimeSpan IncrementalSyncInterval;

		private string UserName;

		private readonly SecureString Password;

		private readonly Guid MailboxGuid;

		private Guid subscriptionGuid;

		private ConnectionSettings currentConnectionSettings;

		internal readonly Hookable<Func<ConnectionSettingsManager>> connectionSettingsManager;

		internal readonly Hookable<AddAggregatedAccount.CheckUserCanLogonWithSettings> checkUserCanLogon = Hookable<AddAggregatedAccount.CheckUserCanLogonWithSettings>.Create(true, delegate(ConnectionSettings connectionSettings, SmtpAddress email, ref string userName, SecureString password)
		{
			return connectionSettings.TestUserCanLogon(email, ref userName, password);
		});

		internal readonly Hookable<Action> addAggregatedAccountToADUser;

		internal readonly Hookable<Action> removeAggregatedAccountToADUser;

		internal readonly Hookable<Func<ConnectionSettings, Exception, SyncRequest>> createSyncRequest;

		internal delegate bool CheckUserCanLogonWithSettings(ConnectionSettings connectionSettings, SmtpAddress email, ref string userName, SecureString password);
	}
}
