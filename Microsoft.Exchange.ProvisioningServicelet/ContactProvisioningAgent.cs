using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal sealed class ContactProvisioningAgent : ProvisioningAgent
	{
		public ContactProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.ProvisioningType != ProvisioningType.Contact)
			{
				throw new ArgumentException("data needs to be of ContactProvisioningData type.");
			}
		}

		protected override Error CreateRecipient()
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17748, (long)this.GetHashCode(), "CreateRecipient");
			ExTraceGlobals.WorkerTracer.TraceInformation(17752, (long)this.GetHashCode(), "invoke new-mailcontact");
			PSCommand pscommand = new PSCommand().AddCommand("New-MailContact");
			if (!base.PopulateParamsToPSCommand(pscommand, ContactProvisioningAgent.newMailContactParameterMap, base.ProvisioningData.Parameters))
			{
				return new Error(new InvalidOperationException("Developer error: No parameters were mapped for cmdlet."));
			}
			Error error;
			MailContact mailContact = base.SafeRunPSCommand<MailContact>(pscommand, base.AgentContext.Runspace, out error, null, new uint?(3433442621U));
			if (error != null)
			{
				if (error.Exception is InvalidOperationException || error.Exception is NotAcceptedDomainException || error.Exception is ProxyAddressExistsException)
				{
					return error;
				}
				pscommand = new PSCommand().AddCommand("Get-MailContact");
				if (base.PopulateParamsToPSCommand(pscommand, ContactProvisioningAgent.getMailContactParameterMap, base.ProvisioningData.Parameters))
				{
					Error error2;
					mailContact = base.SafeRunPSCommand<MailContact>(pscommand, base.AgentContext.Runspace, out error2, null, null);
				}
			}
			if (mailContact == null)
			{
				if (error == null)
				{
					error = new Error(new InvalidDataException("no contact created or found, but no error either!"));
				}
				return error;
			}
			this.UpdateProxyAddressesParameter(mailContact);
			ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-mailcontact");
			pscommand = new PSCommand().AddCommand("Set-MailContact");
			pscommand.AddParameter("Identity", mailContact.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, ContactProvisioningAgent.setMailContactParameterMap, base.ProvisioningData.Parameters))
			{
				base.SafeRunPSCommand<ADUser>(pscommand, base.AgentContext.Runspace, out error, null, new uint?(3768986941U));
				if (error != null)
				{
					return error;
				}
			}
			ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-Contact");
			pscommand = new PSCommand().AddCommand("Set-Contact");
			pscommand.AddParameter("Identity", mailContact.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, ContactProvisioningAgent.setContactParameterMap, base.ProvisioningData.Parameters))
			{
				base.SafeRunPSCommand<ADUser>(pscommand, base.AgentContext.Runspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2963680573U);
			return null;
		}

		protected override void UpdateProxyAddressesParameter(MailEnabledRecipient recipient)
		{
			base.UpdateProxyAddressesParameter(recipient);
			if (((ContactProvisioningData)base.ProvisioningData).IsSmtpAddressCheckWithAcceptedDomain)
			{
				base.RemoveSmtpProxyAddressesWithAcceptedDomain();
			}
		}

		protected override void IncrementPerfCounterForAttempt()
		{
			base.IncrementPerfCounterForAttempt();
			BulkUserProvisioningCounters.NumberOfContactsAttempted.Increment();
			BulkUserProvisioningCounters.RateOfContactsAttempted.Increment();
		}

		protected override void IncrementPerfCounterForFailure()
		{
			base.IncrementPerfCounterForFailure();
			BulkUserProvisioningCounters.NumberOfContactsFailed.Increment();
		}

		protected override void IncrementPerfCounterForCompletion()
		{
			base.IncrementPerfCounterForCompletion();
			BulkUserProvisioningCounters.NumberOfContactsCreated.Increment();
			BulkUserProvisioningCounters.RateOfContactsCreated.Increment();
		}

		private static readonly string[][] newMailContactParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.ExternalEmailAddress.Name,
				string.Empty
			},
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
				"Organization",
				string.Empty
			}
		};

		private static readonly string[][] setMailContactParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.EmailAddresses.Name,
				string.Empty
			}
		};

		private static readonly string[][] getMailContactParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.ExternalEmailAddress.Name,
				"Identity"
			},
			new string[]
			{
				"Organization",
				string.Empty
			}
		};

		private static readonly string[][] setContactParameterMap = new string[][]
		{
			new string[]
			{
				ADOrgPersonSchema.Company.Name,
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
				ADOrgPersonSchema.MobilePhone.Name,
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
				ADOrgPersonSchema.Title.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.City.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.CountryOrRegion.Name,
				string.Empty
			},
			new string[]
			{
				ADOrgPersonSchema.HomePhone.Name,
				string.Empty
			},
			new string[]
			{
				ADRecipientSchema.Notes.Name,
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
			}
		};
	}
}
