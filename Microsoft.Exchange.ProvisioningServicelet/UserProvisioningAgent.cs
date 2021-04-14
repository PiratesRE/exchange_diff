using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UserProvisioningAgent : ProvisioningAgent
	{
		public UserProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.ProvisioningType != ProvisioningType.User)
			{
				throw new ArgumentException("data needs to be of UserProvisioningData type.");
			}
		}

		public override IMailboxData MailboxData
		{
			get
			{
				return this.mailboxData;
			}
		}

		protected virtual string[][] NewMailboxParameterMap
		{
			get
			{
				return null;
			}
		}

		protected virtual string[][] GetMailboxParameterMap
		{
			get
			{
				return null;
			}
		}

		protected virtual string[][] SetMailboxParameterMap
		{
			get
			{
				return null;
			}
		}

		protected virtual string[][] SetMailboxParameterMapForPreexistingMailbox
		{
			get
			{
				return null;
			}
		}

		protected virtual string[][] SetMailboxParameterMapForDCAdmin
		{
			get
			{
				return null;
			}
		}

		protected virtual string[][] SetUserParameterMap
		{
			get
			{
				return null;
			}
		}

		protected virtual string[][] ImportRecipientDataPropertyParameterMapForDCAdmin
		{
			get
			{
				return null;
			}
		}

		protected override Error CreateRecipient()
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17748, (long)this.GetHashCode(), "CreateRecipient");
			Error result;
			try
			{
				result = this.Provision();
			}
			catch (Exception ex)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3840290109U);
				result = new Error(ProvisioningAgent.FilterTaskException(ex));
			}
			return result;
		}

		protected abstract Error Provision();

		protected override void IncrementPerfCounterForAttempt()
		{
			base.IncrementPerfCounterForAttempt();
			BulkUserProvisioningCounters.NumberOfMailboxesAttempted.Increment();
			BulkUserProvisioningCounters.RateOfMailboxesAttempted.Increment();
		}

		protected override void IncrementPerfCounterForFailure()
		{
			base.IncrementPerfCounterForFailure();
			BulkUserProvisioningCounters.NumberOfMailboxesFailed.Increment();
		}

		protected override void IncrementPerfCounterForCompletion()
		{
			base.IncrementPerfCounterForCompletion();
			BulkUserProvisioningCounters.NumberOfMailboxesCreated.Increment();
			BulkUserProvisioningCounters.RateOfMailboxesCreated.Increment();
		}

		protected Error SetMailbox(Mailbox mailbox)
		{
			this.UpdateProxyAddressesParameter(mailbox);
			ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-mailbox");
			PSCommand pscommand = new PSCommand().AddCommand("set-mailbox");
			pscommand.AddParameter("Identity", mailbox.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, this.SetMailboxParameterMap, base.ProvisioningData.Parameters))
			{
				Error error;
				base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.Runspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			pscommand = new PSCommand().AddCommand("set-mailbox");
			pscommand.AddParameter("Identity", mailbox.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, this.SetMailboxParameterMapForDCAdmin, base.ProvisioningData.Parameters))
			{
				Error error;
				base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.DatacenterRunspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			ExTraceGlobals.WorkerTracer.TraceInformation(17780, (long)this.GetHashCode(), "invoke Import-RecipientDataProperty");
			pscommand = new PSCommand().AddCommand("Import-RecipientDataProperty");
			pscommand.AddParameter("Identity", mailbox.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, this.ImportRecipientDataPropertyParameterMapForDCAdmin, base.ProvisioningData.Parameters))
			{
				pscommand.AddParameter("SpokenName");
				pscommand.AddParameter("Confirm", false);
				Error error;
				base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.DatacenterRunspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			ExTraceGlobals.WorkerTracer.TraceInformation(17780, (long)this.GetHashCode(), "invoke set-user");
			pscommand = new PSCommand().AddCommand("set-user");
			pscommand.AddParameter("Identity", mailbox.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, this.SetUserParameterMap, base.ProvisioningData.Parameters))
			{
				Error error;
				base.SafeRunPSCommand<ADUser>(pscommand, base.AgentContext.Runspace, out error, null, null);
				return error;
			}
			return null;
		}

		protected Error SetMailboxForPreexistingMailbox(Mailbox mailbox)
		{
			this.UpdateProxyAddressesParameter(mailbox);
			ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-mailbox [for Pre-existing mailbox]");
			PSCommand pscommand = new PSCommand().AddCommand("set-mailbox");
			pscommand.AddParameter("Identity", mailbox.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, this.SetMailboxParameterMapForPreexistingMailbox, base.ProvisioningData.Parameters))
			{
				Error error;
				base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.Runspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			return null;
		}

		protected Error NewMailbox(out Mailbox mailbox)
		{
			ExTraceGlobals.WorkerTracer.TraceInformation(17752, (long)this.GetHashCode(), "invoke new-mailbox");
			UserProvisioningData userProvisioningData = (UserProvisioningData)base.ProvisioningData;
			PSCommand pscommand = new PSCommand().AddCommand("new-mailbox");
			if (!base.PopulateParamsToPSCommand(pscommand, this.NewMailboxParameterMap, userProvisioningData.Parameters))
			{
				throw new InvalidOperationException("Developer error: No parameters were mapped for new-mailbox.");
			}
			if (!string.IsNullOrEmpty(userProvisioningData.Password))
			{
				pscommand.AddParameter("Password", userProvisioningData.Password.ConvertToSecureString());
			}
			Error error;
			mailbox = base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.Runspace, out error, null, null);
			if (error == null)
			{
				return null;
			}
			ExTraceGlobals.WorkerTracer.TraceError<Exception>(17760, (long)this.GetHashCode(), "new-mailbox failed with {0}", error.Exception);
			if (error.Exception is WLCDUnmanagedMemberExistsException && !userProvisioningData.IsBPOS && !userProvisioningData.EvictLiveId)
			{
				ExTraceGlobals.WorkerTracer.TraceInformation(17764, (long)this.GetHashCode(), "invoke new-mailbox -EvictLiveID");
				pscommand = new PSCommand().AddCommand("new-mailbox");
				pscommand.AddParameter("EvictLiveID");
				if (!string.IsNullOrEmpty(userProvisioningData.Password))
				{
					pscommand.AddParameter("Password", userProvisioningData.Password.ConvertToSecureString());
				}
				base.PopulateParamsToPSCommand(pscommand, this.NewMailboxParameterMap, base.ProvisioningData.Parameters);
				mailbox = base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.Runspace, out error, null, null);
			}
			return error;
		}

		protected Error GetMailbox(out Mailbox mailbox)
		{
			PSCommand command = new PSCommand().AddCommand("get-mailbox");
			if (!base.PopulateParamsToPSCommand(command, this.GetMailboxParameterMap, base.ProvisioningData.Parameters))
			{
				throw new InvalidOperationException("Developer error: No parameters were mapped for get-mailbox.");
			}
			Error error;
			mailbox = base.SafeRunPSCommand<Mailbox>(command, base.AgentContext.Runspace, out error, null, null);
			if (error != null)
			{
				return error;
			}
			return null;
		}

		protected Error ConvertMailUserToMailbox(out Mailbox mailbox)
		{
			UserProvisioningData userProvisioningData = (UserProvisioningData)base.ProvisioningData;
			mailbox = null;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3863358781U);
			PSCommand pscommand = new PSCommand().AddCommand("get-recipient");
			if (!base.PopulateParamsToPSCommand(pscommand, this.GetMailboxParameterMap, userProvisioningData.Parameters))
			{
				throw new InvalidOperationException("Developer error: No parameters were mapped for get-recipient.");
			}
			pscommand.AddParameter("RecipientType", RecipientType.MailUser.ToString());
			Error error;
			ReducedRecipient reducedRecipient = base.SafeRunPSCommand<ReducedRecipient>(pscommand, base.AgentContext.Runspace, out error, null, null);
			if (error != null)
			{
				return error;
			}
			if (reducedRecipient.ResourceType == null && !string.IsNullOrEmpty(userProvisioningData.Password))
			{
				pscommand = new PSCommand().AddCommand("Set-MailUser");
				pscommand.AddParameter("Identity", reducedRecipient.Identity);
				pscommand.AddParameter("Password", userProvisioningData.Password.ConvertToSecureString());
				base.PopulateParamsToPSCommand(pscommand, this.SetMailboxParameterMapForDCAdmin, userProvisioningData.Parameters);
				base.SafeRunPSCommand<MailUser>(pscommand, base.AgentContext.DatacenterRunspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			pscommand = new PSCommand().AddCommand("enable-mailbox");
			pscommand.AddParameter("Identity", reducedRecipient.Identity);
			ExchangeResourceType valueOrDefault = reducedRecipient.ResourceType.GetValueOrDefault();
			ExchangeResourceType? exchangeResourceType;
			if (exchangeResourceType != null)
			{
				switch (valueOrDefault)
				{
				case ExchangeResourceType.Room:
					pscommand.AddParameter("Room");
					pscommand.AddParameter("AccountDisabled", true);
					break;
				case ExchangeResourceType.Equipment:
					pscommand.AddParameter("Equipment");
					pscommand.AddParameter("AccountDisabled", true);
					break;
				}
			}
			mailbox = base.SafeRunPSCommand<Mailbox>(pscommand, base.AgentContext.DatacenterRunspace, out error, null, null);
			if (error != null)
			{
				return error;
			}
			return null;
		}

		protected override void UpdateProxyAddressesParameter(MailEnabledRecipient recipient)
		{
			base.UpdateProxyAddressesParameter(recipient);
			base.RemoveSmtpProxyAddressesWithExternalDomain();
		}

		protected IMailboxData mailboxData;
	}
}
