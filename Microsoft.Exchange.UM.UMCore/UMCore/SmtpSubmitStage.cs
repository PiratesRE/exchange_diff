using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SmtpSubmitStage : SubmitStage, IUMNetworkResource
	{
		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				return PipelineDispatcher.PipelineResourceType.NetworkBound;
			}
		}

		public string NetworkResourceId
		{
			get
			{
				return "70cb6c39-83d9-4123-8013-d95aadda7712";
			}
		}

		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(5.0);
			}
		}

		internal override void Initialize(PipelineWorkItem workItem)
		{
			base.Initialize(workItem);
		}

		protected override void InternalDoSynchronousWork()
		{
			IUMCAMessage iumcamessage = base.WorkItem.Message as IUMCAMessage;
			ExAssert.RetailAssert(iumcamessage != null, "SmtpSubmitStage must operate on PipelineContext which implements IUMCAMessage");
			UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = null;
			try
			{
				UMRecipient camessageRecipient = iumcamessage.CAMessageRecipient;
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(camessageRecipient.ADRecipient.OrganizationId);
				MicrosoftExchangeRecipient microsoftExchangeRecipient = iadsystemConfigurationLookup.GetMicrosoftExchangeRecipient();
				if (base.MessageToSubmit != null)
				{
					VoiceMessagePipelineContext voiceMessagePipelineContext = base.WorkItem.Message as VoiceMessagePipelineContext;
					if (voiceMessagePipelineContext != null && voiceMessagePipelineContext.IsThisAProtectedMessage)
					{
						mailboxSessionLock = ((UMMailboxRecipient)voiceMessagePipelineContext.CAMessageRecipient).CreateSessionLock();
						if (mailboxSessionLock.Session == null || mailboxSessionLock.UnderlyingStoreRPCSessionDisconnected)
						{
							throw new WorkItemNeedsToBeRequeuedException();
						}
						base.MessageToSubmit.Load(StoreObjectSchema.ContentConversionProperties);
					}
					AcceptedDomain defaultAcceptedDomain = Utils.GetDefaultAcceptedDomain(camessageRecipient.ADRecipient);
					OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
					outboundConversionOptions.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(camessageRecipient).ScopedRecipientSession;
					SmtpSubmissionHelper.SubmitMessage(base.MessageToSubmit, microsoftExchangeRecipient.PrimarySmtpAddress.ToString(), base.WorkItem.Message.TenantGuid, camessageRecipient.MailAddress, outboundConversionOptions, Path.GetFileNameWithoutExtension(base.WorkItem.HeaderFilename));
				}
			}
			finally
			{
				if (mailboxSessionLock != null)
				{
					mailboxSessionLock.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SmtpSubmitStage>(this);
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.DropMessage, TimeSpan.FromSeconds(30.0), TimeSpan.FromDays(1.0));
		}
	}
}
