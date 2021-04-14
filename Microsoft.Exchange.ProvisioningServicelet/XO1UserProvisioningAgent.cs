using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class XO1UserProvisioningAgent : ProvisioningAgent
	{
		public XO1UserProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.Component != ProvisioningComponent.XO1)
			{
				throw new ArgumentException("data needs to be for XO1.");
			}
		}

		public override IMailboxData MailboxData
		{
			get
			{
				return this.mailboxData;
			}
		}

		protected override Error CreateRecipient()
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17748, (long)this.GetHashCode(), "CreateRecipient");
			XO1UserProvisioningData xo1UserProvisioningData = (XO1UserProvisioningData)base.ProvisioningData;
			PSCommand command = new PSCommand().AddCommand("new-consumermailbox");
			if (!base.PopulateParamsToPSCommand(command, XO1UserProvisioningAgent.newConsumerMailboxParameterMap, xo1UserProvisioningData.Parameters))
			{
				throw new InvalidOperationException("No parameters were mapped for New-ConsumerMailbox.");
			}
			ExTraceGlobals.WorkerTracer.TraceInformation(17752, (long)this.GetHashCode(), "invoke new-ConsumerMailbox");
			Error error;
			ConsumerMailbox consumerMailbox = base.SafeRunPSCommand<ConsumerMailbox>(command, base.AgentContext.Runspace, out error, null, null);
			if ((error != null && error.Exception is WLCDManagedMemberExistsException) || consumerMailbox == null)
			{
				command = new PSCommand().AddCommand("get-consumermailbox");
				if (base.PopulateParamsToPSCommand(command, XO1UserProvisioningAgent.getConsumerMailboxParameterMap, xo1UserProvisioningData.Parameters))
				{
					Error error2;
					consumerMailbox = base.SafeRunPSCommand<ConsumerMailbox>(command, base.AgentContext.Runspace, out error2, null, null);
				}
			}
			if (consumerMailbox == null)
			{
				if (error == null)
				{
					error = new Error(new InvalidDataException("no ConsumerMailbox created or found, but no error either!"));
				}
				return error;
			}
			this.mailboxData = new ConsumerMailboxData(consumerMailbox.ExchangeGuid.Value, consumerMailbox.Database.ObjectGuid);
			this.mailboxData.Update(consumerMailbox.Identity.ToString(), OrganizationId.ForestWideOrgId);
			return null;
		}

		private static readonly string[][] newConsumerMailboxParameterMap = new string[][]
		{
			new string[]
			{
				"Identity",
				"WindowsLiveID"
			},
			new string[]
			{
				"FirstName",
				string.Empty
			},
			new string[]
			{
				"LastName",
				string.Empty
			},
			new string[]
			{
				"TimeZone",
				string.Empty
			},
			new string[]
			{
				"LocaleId",
				string.Empty
			},
			new string[]
			{
				"EmailAddresses",
				string.Empty
			},
			new string[]
			{
				"Database",
				string.Empty
			},
			new string[]
			{
				"MakeExoSecondary",
				string.Empty
			}
		};

		private static readonly string[][] getConsumerMailboxParameterMap = new string[][]
		{
			new string[]
			{
				"Identity",
				string.Empty
			}
		};

		private IMailboxData mailboxData;
	}
}
