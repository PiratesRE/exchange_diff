using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal sealed class ContactUpdateProvisioningAgent : ProvisioningAgent
	{
		public ContactUpdateProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.ProvisioningType != ProvisioningType.ContactUpdate)
			{
				throw new ArgumentException("data needs to be of ContactUpdateProvisioningData type.");
			}
			if (((ContactUpdateProvisioningData)data).IsEmpty())
			{
				throw new ArgumentException("data cannot be empty.");
			}
		}

		protected override Error CreateRecipient()
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17748, (long)this.GetHashCode(), "CreateRecipient");
			string identity = ((ContactUpdateProvisioningData)base.ProvisioningData).Identity;
			ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-MailContact");
			PSCommand pscommand = new PSCommand().AddCommand("Set-MailContact");
			if (base.PopulateParamsToPSCommand(pscommand, ContactUpdateProvisioningAgent.setMailContactParameterMap, base.ProvisioningData.Parameters))
			{
				pscommand.AddParameter("Identity", identity);
				Error error;
				base.SafeRunPSCommand<MailContact>(pscommand, base.AgentContext.Runspace, out error, new ProvisioningAgent.ErrorMessageOperation(Strings.FailedToUpdateProperty), null);
				if (error != null)
				{
					return error;
				}
			}
			ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-Contact");
			pscommand = new PSCommand().AddCommand("Set-Contact");
			if (base.PopulateParamsToPSCommand(pscommand, ContactUpdateProvisioningAgent.setContactParameterMap, base.ProvisioningData.Parameters))
			{
				pscommand.AddParameter("Identity", identity);
				Error error2;
				base.SafeRunPSCommand<ADUser>(pscommand, base.AgentContext.Runspace, out error2, new ProvisioningAgent.ErrorMessageOperation(Strings.FailedToUpdateProperty), null);
				if (error2 != null)
				{
					return error2;
				}
			}
			return null;
		}

		protected override void IncrementPerfCounterForAttempt()
		{
			base.IncrementPerfCounterForAttempt();
			BulkUserProvisioningCounters.NumberOfUpdateContactsAttempted.Increment();
			BulkUserProvisioningCounters.RateOfUpdateContactsAttempted.Increment();
		}

		protected override void IncrementPerfCounterForFailure()
		{
			base.IncrementPerfCounterForFailure();
			BulkUserProvisioningCounters.NumberOfUpdateContactsFailed.Increment();
		}

		protected override void IncrementPerfCounterForCompletion()
		{
			base.IncrementPerfCounterForCompletion();
			BulkUserProvisioningCounters.NumberOfUpdateContactsCreated.Increment();
			BulkUserProvisioningCounters.RateOfUpdateContactsCreated.Increment();
		}

		private static readonly string[][] setContactParameterMap = new string[][]
		{
			new string[]
			{
				ADOrgPersonSchema.Manager.Name,
				string.Empty
			}
		};

		private static readonly string[][] setMailContactParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.GrantSendOnBehalfTo.Name,
				string.Empty
			}
		};
	}
}
