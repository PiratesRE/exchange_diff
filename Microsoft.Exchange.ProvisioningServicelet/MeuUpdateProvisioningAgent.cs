using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal sealed class MeuUpdateProvisioningAgent : ProvisioningAgent
	{
		public MeuUpdateProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.ProvisioningType != ProvisioningType.MailEnabledUserUpdate)
			{
				throw new ArgumentException("data needs to be of MailEnabledUserUpdateProvisioningData type.");
			}
			if (((MailEnabledUserUpdateProvisioningData)data).IsEmpty())
			{
				throw new ArgumentException("data cannot be empty.");
			}
		}

		protected override Error CreateRecipient()
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17748, (long)this.GetHashCode(), "CreateRecipient");
			string identity = ((MailEnabledUserUpdateProvisioningData)base.ProvisioningData).Identity;
			PSCommand pscommand = new PSCommand().AddCommand("Set-MailUser");
			if (base.PopulateParamsToPSCommand(pscommand, MeuUpdateProvisioningAgent.setMailUserParameterMap, base.ProvisioningData.Parameters))
			{
				ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-mailuser");
				pscommand.AddParameter("Identity", identity);
				Error error;
				base.SafeRunPSCommand<MailUser>(pscommand, base.AgentContext.Runspace, out error, new ProvisioningAgent.ErrorMessageOperation(Strings.FailedToUpdateProperty), null);
				if (error != null)
				{
					error.Message = Strings.FailedToUpdateProperty(error.Exception.Message);
					return error;
				}
			}
			pscommand = new PSCommand().AddCommand("set-user");
			if (base.PopulateParamsToPSCommand(pscommand, MeuUpdateProvisioningAgent.setUserParameterMap, base.ProvisioningData.Parameters))
			{
				ExTraceGlobals.WorkerTracer.TraceInformation(17780, (long)this.GetHashCode(), "invoke set-user");
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

		private static readonly string[][] setMailUserParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.GrantSendOnBehalfTo.Name,
				string.Empty
			}
		};

		private static readonly string[][] setUserParameterMap = new string[][]
		{
			new string[]
			{
				ADOrgPersonSchema.Manager.Name,
				string.Empty
			}
		};
	}
}
