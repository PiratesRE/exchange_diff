using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class StagedExchangeUserProvisioningAgent : UserProvisioningAgent
	{
		public StagedExchangeUserProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.Component != ProvisioningComponent.StagedExchangeMigration)
			{
				throw new ArgumentException("data needs to be for StagedExchangeMigration.");
			}
		}

		protected override string[][] GetMailboxParameterMap
		{
			get
			{
				return StagedExchangeUserProvisioningAgent.getMailboxParameterMap;
			}
		}

		protected override string[][] SetMailboxParameterMapForDCAdmin
		{
			get
			{
				return StagedExchangeUserProvisioningAgent.setMailboxParameterMapForDCAdmin;
			}
		}

		protected override Error Provision()
		{
			UserProvisioningData userProvisioningData = (UserProvisioningData)base.ProvisioningData;
			Mailbox mailbox;
			Error error = base.ConvertMailUserToMailbox(out mailbox);
			if (error != null && error.Exception is ManagementObjectNotFoundException)
			{
				base.GetMailbox(out mailbox);
				error = ((mailbox != null) ? null : error);
			}
			if (error != null)
			{
				return error;
			}
			this.mailboxData = new MailboxData(mailbox.ExchangeGuid, new Fqdn(mailbox.OriginatingServer), mailbox.LegacyExchangeDN, mailbox.Id, mailbox.ExchangeObjectId);
			this.mailboxData.Update(mailbox.Identity.ToString(), mailbox.OrganizationId);
			return null;
		}

		private static readonly string[][] getMailboxParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.WindowsLiveID.Name,
				"Identity"
			},
			new string[]
			{
				"Organization",
				string.Empty
			},
			new string[]
			{
				"MicrosoftOnlineServicesID",
				"Identity"
			}
		};

		private static readonly string[][] setMailboxParameterMapForDCAdmin = new string[][]
		{
			new string[]
			{
				ADUserSchema.ResetPasswordOnNextLogon.Name,
				string.Empty
			}
		};
	}
}
