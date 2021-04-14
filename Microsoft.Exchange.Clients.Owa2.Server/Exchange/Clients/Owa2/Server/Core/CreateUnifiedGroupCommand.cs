using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateUnifiedGroupCommand : ServiceCommand<CreateUnifiedGroupResponse>
	{
		public CreateUnifiedGroupCommand(CallContext callContext, CreateUnifiedGroupRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "request", "CreateUnifiedGroupCommand::CreateUnifiedGroupCommand");
			this.request = request;
			this.request.Validate();
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(CreateModernGroupCommandMetadata), new Type[0]);
			this.notifier = new CreateGroupNotifier(UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true));
		}

		protected override CreateUnifiedGroupResponse InternalExecute()
		{
			CreateUnifiedGroupResponse createUnifiedGroupResponse = new CreateUnifiedGroupResponse();
			if (!this.IsModernGroupUnique())
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(new UnifiedGroupAlreadyExistsException(), FaultParty.Sender);
			}
			Stopwatch stopwatch = new Stopwatch();
			try
			{
				stopwatch.Start();
				CreateUnifiedGroupTask task = new CreateUnifiedGroupTask(base.CallContext.AccessingADUser, base.CallContext.AccessingPrincipal, base.CallContext.ADRecipientSessionContext.GetADRecipientSession());
				task.Name = this.request.Name;
				task.Alias = this.request.Alias;
				task.Description = this.request.Description;
				task.Type = ((this.request.GroupType == ModernGroupObjectType.Public) ? ModernGroupTypeInfo.Public : ModernGroupTypeInfo.Private);
				task.AADComplete += delegate()
				{
					this.PushCreatedPersona(task.ExternalDirectoryObjectId);
				};
				task.AutoSubscribeNewGroupMembers = new bool?(this.request.AutoSubscribeNewGroupMembers);
				task.Language = this.request.Language;
				IFeature autoSubscribeSetByDefault = VariantConfiguration.GetSnapshot(base.CallContext.AccessingPrincipal.GetContext(null), null, null).OwaClientServer.AutoSubscribeSetByDefault;
				IFeature autoSubscribeNewGroupMembers = VariantConfiguration.GetSnapshot(base.CallContext.AccessingPrincipal.GetContext(null), null, null).OwaClientServer.AutoSubscribeNewGroupMembers;
				if (autoSubscribeNewGroupMembers != null && autoSubscribeNewGroupMembers.Enabled && autoSubscribeSetByDefault != null)
				{
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.AutoSubscribeOptionDefault, autoSubscribeSetByDefault.Enabled);
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.AutoSubscribeOptionReceived, this.request.AutoSubscribeNewGroupMembers);
				}
				base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.DescriptionSpecified, (!string.IsNullOrEmpty(this.request.Description)) ? 1 : 0);
				if (!task.Run())
				{
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.Exception, task.ErrorException);
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ExceptionType, task.ErrorException.GetType());
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ErrorAction, task.ErrorAction);
					if (!string.IsNullOrEmpty(task.ErrorCode))
					{
						base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ErrorCode, task.ErrorCode);
					}
					UnifiedGroupsTask.UnifiedGroupsAction errorAction = task.ErrorAction;
					if (errorAction == UnifiedGroupsTask.UnifiedGroupsAction.AADCreate)
					{
						base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ExceptionLocation, "AAD");
						throw new InternalServerErrorException(task.ErrorException);
					}
					if (errorAction != UnifiedGroupsTask.UnifiedGroupsAction.ExchangeCreate)
					{
						base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ExceptionLocation, "Unknown");
						throw new InvalidOperationException("Unexpected error action: " + task.ErrorAction, task.ErrorException);
					}
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ExceptionLocation, "Mailbox");
					base.CallContext.ProtocolLog.Set(ServiceCommonMetadata.GenericErrors, task.ErrorException);
					createUnifiedGroupResponse.Error = "Provision failed for group " + this.request.Alias;
					createUnifiedGroupResponse.FailureState = CreateUnifiedGroupResponse.GroupProvisionFailureState.FailedMailboxProvision;
				}
				else
				{
					Persona persona = new Persona
					{
						ADObjectId = task.ADObjectGuid,
						Alias = this.request.Alias,
						DisplayName = this.request.Name,
						EmailAddress = new EmailAddressWrapper
						{
							EmailAddress = task.SmtpAddress,
							MailboxType = MailboxHelper.MailboxTypeType.GroupMailbox.ToString()
						}
					};
					createUnifiedGroupResponse.Persona = persona;
					createUnifiedGroupResponse.ExternalDirectoryObjectId = task.ExternalDirectoryObjectId;
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.GroupSmtpAddress, ExtensibleLogger.FormatPIIValue(task.SmtpAddress));
				}
				this.LogElapsedTime(CreateModernGroupCommandMetadata.GroupCreationTime, task.CreationDiagnostics.GroupCreationTime);
				this.LogElapsedTime(CreateModernGroupCommandMetadata.AADIdentityCreationTime, task.CreationDiagnostics.AADIdentityCreationTime);
				this.LogElapsedTime(CreateModernGroupCommandMetadata.AADCompleteCallbackTime, task.CreationDiagnostics.AADCompleteCallbackTime);
				this.LogElapsedTime(CreateModernGroupCommandMetadata.SharePointNotificationTime, task.CreationDiagnostics.SharePointNotificationTime);
				this.LogElapsedTime(CreateModernGroupCommandMetadata.MailboxCreationTime, task.CreationDiagnostics.MailboxCreationTime);
			}
			finally
			{
				this.LogElapsedTime(CreateModernGroupCommandMetadata.TotalProcessingTime, new TimeSpan?(stopwatch.Elapsed));
			}
			return createUnifiedGroupResponse;
		}

		private bool IsModernGroupUnique()
		{
			bool result = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			AADClient aadclient = AADClientFactory.Create(base.CallContext.ADRecipientSessionContext.OrganizationId, GraphProxyVersions.Version14);
			if (aadclient == null)
			{
				return true;
			}
			try
			{
				result = aadclient.IsAliasUnique(this.request.Alias);
				this.LogElapsedTime(CreateModernGroupCommandMetadata.AADAliasQueryTime, new TimeSpan?(stopwatch.Elapsed));
			}
			catch (AADException ex)
			{
				this.LogElapsedTime(CreateModernGroupCommandMetadata.AADAliasQueryTime, new TimeSpan?(stopwatch.Elapsed));
				ExTraceGlobals.ModernGroupsTracer.TraceError<SmtpAddress, AADException>((long)this.GetHashCode(), "CreateUnifiedGroupCommand: User: {0}. Exception: {1}. AADClient.IsAliasUnique failed", base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress, ex);
				base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.Exception, ex);
			}
			return result;
		}

		private void PushCreatedPersona(string externalDirectoryObjectId)
		{
			try
			{
				this.notifier.RegisterWithPendingRequestNotifier();
				this.notifier.Payload = new CreateGroupNotificationPayload
				{
					ExternalDirectoryObjectId = externalDirectoryObjectId,
					PushToken = this.request.PushToken
				};
				this.notifier.PickupData();
			}
			finally
			{
				this.notifier.UnregisterWithPendingRequestNotifier();
			}
		}

		private void LogElapsedTime(CreateModernGroupCommandMetadata key, TimeSpan? value)
		{
			if (value != null)
			{
				base.CallContext.ProtocolLog.Set(key, (long)value.Value.TotalMilliseconds);
			}
		}

		private readonly CreateUnifiedGroupRequest request;

		private readonly CreateGroupNotifier notifier;
	}
}
