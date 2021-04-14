using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExchangeUserProvisioningAgent : UserProvisioningAgent
	{
		public ExchangeUserProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.Component != ProvisioningComponent.ExchangeMigration)
			{
				throw new ArgumentException("data needs to be for ExchangeMigration.");
			}
		}

		protected override string[][] NewMailboxParameterMap
		{
			get
			{
				return ExchangeUserProvisioningAgent.newMailboxParameterMap;
			}
		}

		protected override string[][] GetMailboxParameterMap
		{
			get
			{
				return ExchangeUserProvisioningAgent.getMailboxParameterMap;
			}
		}

		protected override string[][] SetMailboxParameterMap
		{
			get
			{
				return ExchangeUserProvisioningAgent.setMailboxParameterMap;
			}
		}

		protected override string[][] SetMailboxParameterMapForPreexistingMailbox
		{
			get
			{
				return ExchangeUserProvisioningAgent.setMailboxParameterMapForPreexistingMailbox;
			}
		}

		protected override string[][] ImportRecipientDataPropertyParameterMapForDCAdmin
		{
			get
			{
				return ExchangeUserProvisioningAgent.importRecipientDataPropertyParameterMapForDCAdmin;
			}
		}

		protected override string[][] SetUserParameterMap
		{
			get
			{
				return ExchangeUserProvisioningAgent.setUserParameterMap;
			}
		}

		protected override Error Provision()
		{
			UserProvisioningData userProvisioningData = (UserProvisioningData)base.ProvisioningData;
			if (userProvisioningData.Action == ProvisioningAction.UpdateExisting)
			{
				Mailbox mailbox;
				Error error = base.GetMailbox(out mailbox);
				if (error != null)
				{
					return error;
				}
				this.mailboxData = new MailboxData(mailbox.ExchangeGuid, new Fqdn(mailbox.OriginatingServer), mailbox.LegacyExchangeDN, mailbox.Id, mailbox.ExchangeObjectId);
				this.mailboxData.Update(mailbox.Identity.ToString(), mailbox.OrganizationId);
				error = base.SetMailboxForPreexistingMailbox(mailbox);
				if (error != null)
				{
					return error;
				}
			}
			else
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2196122941U);
				Mailbox mailbox2;
				Error error2 = base.NewMailbox(out mailbox2);
				if (error2 != null && error2.Exception is WLCDManagedMemberExistsException && userProvisioningData.Action == ProvisioningAction.CreateNewOrUpdateExisting && base.GetMailbox(out mailbox2) == null)
				{
					error2 = null;
				}
				if (error2 != null)
				{
					return error2;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3940953405U);
				this.mailboxData = new MailboxData(mailbox2.ExchangeGuid, new Fqdn(mailbox2.OriginatingServer), mailbox2.LegacyExchangeDN, mailbox2.Id, mailbox2.ExchangeObjectId);
				this.mailboxData.Update(mailbox2.Identity.ToString(), mailbox2.OrganizationId);
				error2 = base.SetMailbox(mailbox2);
				if (error2 != null)
				{
					return error2;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2464558397U);
			}
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
				ADUserSchema.ResetPasswordOnNextLogon.Name,
				string.Empty
			},
			new string[]
			{
				"Organization",
				string.Empty
			},
			new string[]
			{
				"MicrosoftOnlineServicesID",
				string.Empty
			}
		};

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

		private static readonly string[][] setMailboxParameterMap = new string[][]
		{
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
			}
		};

		private static readonly string[][] setMailboxParameterMapForPreexistingMailbox = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.EmailAddresses.Name,
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

		private static readonly string[][] importRecipientDataPropertyParameterMapForDCAdmin = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.UMSpokenName.Name,
				"FileData"
			}
		};
	}
}
