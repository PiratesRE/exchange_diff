using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class BulkProvisionUserProvisioningAgent : UserProvisioningAgent
	{
		public BulkProvisionUserProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.Component != ProvisioningComponent.BulkProvision)
			{
				throw new ArgumentException("data needs to be for BulkProvision.");
			}
		}

		protected override string[][] NewMailboxParameterMap
		{
			get
			{
				return BulkProvisionUserProvisioningAgent.newMailboxParameterMap;
			}
		}

		protected override string[][] SetMailboxParameterMap
		{
			get
			{
				return BulkProvisionUserProvisioningAgent.setMailboxParameterMap;
			}
		}

		protected override string[][] SetUserParameterMap
		{
			get
			{
				return BulkProvisionUserProvisioningAgent.setUserParameterMap;
			}
		}

		protected override Error Provision()
		{
			UserProvisioningData userProvisioningData = (UserProvisioningData)base.ProvisioningData;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2196122941U);
			Mailbox mailbox;
			Error error = base.NewMailbox(out mailbox);
			if (error != null)
			{
				return error;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3940953405U);
			this.mailboxData = new MailboxData(mailbox.ExchangeGuid, new Fqdn(mailbox.OriginatingServer), mailbox.LegacyExchangeDN, mailbox.Id, mailbox.ExchangeObjectId);
			this.mailboxData.Update(mailbox.Identity.ToString(), mailbox.OrganizationId);
			error = base.SetMailbox(mailbox);
			if (error != null)
			{
				return error;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2464558397U);
			return null;
		}

		private static readonly string[][] newMailboxParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.DisplayName.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.FirstName.Name,
				string.Empty
			},
			new string[]
			{
				"FederatedIdentity",
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Initials.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.LastName.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.MailboxPlan.Name,
				string.Empty
			},
			new string[]
			{
				ADUserSchema.ResetPasswordOnNextLogon.Name,
				string.Empty
			},
			new string[]
			{
				ADObjectSchema.Name.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.WindowsLiveID.Name,
				string.Empty
			},
			new string[]
			{
				"UseExistingLiveId",
				string.Empty
			},
			new string[]
			{
				"Organization",
				string.Empty
			},
			new string[]
			{
				"ImportLiveID",
				string.Empty
			},
			new string[]
			{
				"EvictLiveID",
				string.Empty
			},
			new string[]
			{
				"MicrosoftOnlineServicesID",
				string.Empty
			},
			new string[]
			{
				"UseExistingMicrosoftOnlineServicesID",
				string.Empty
			}
		};

		private static readonly string[][] setMailboxParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.CustomAttribute1.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute2.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute3.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute4.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute5.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute6.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute7.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute8.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute9.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute10.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute11.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute12.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute13.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute14.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.CustomAttribute15.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.EmailAddresses.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Languages.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.ResourceCapacity.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.ResourceType.Name,
				"Type"
			},
			new string[]
			{
				ADRecipientSchema.ForwardingSmtpAddress.Name,
				string.Empty
			}
		};

		private static readonly string[][] setUserParameterMap = new string[][]
		{
			new string[]
			{
				ADOrgPersonSchema.City.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Company.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.CountryOrRegion.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Department.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Fax.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.HomePhone.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.MobilePhone.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.Notes.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Office.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Phone.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.PostalCode.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.StateOrProvince.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.StreetAddress.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.Title.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.WebPage.Name,
				string.Empty
			}
		};
	}
}
