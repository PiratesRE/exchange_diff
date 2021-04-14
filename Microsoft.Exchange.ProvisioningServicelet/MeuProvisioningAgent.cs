using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal sealed class MeuProvisioningAgent : ProvisioningAgent
	{
		public MeuProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext) : base(data, agentContext)
		{
			if (data.ProvisioningType != ProvisioningType.MailEnabledUser)
			{
				throw new ArgumentException("data needs to be of MailEnabledUserProvisioningData type.");
			}
		}

		protected override Error CreateRecipient()
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17748, (long)this.GetHashCode(), "CreateRecipient");
			MailEnabledUserProvisioningData mailEnabledUserProvisioningData = (MailEnabledUserProvisioningData)base.ProvisioningData;
			PSCommand pscommand = new PSCommand().AddCommand("New-MailUser");
			if (!base.PopulateParamsToPSCommand(pscommand, MeuProvisioningAgent.newMailUserParameterMap, mailEnabledUserProvisioningData.Parameters))
			{
				throw new InvalidOperationException("No parameters were mapped for New-MailUser.");
			}
			if (!string.IsNullOrEmpty(mailEnabledUserProvisioningData.Password))
			{
				pscommand.AddParameter("Password", mailEnabledUserProvisioningData.Password.ConvertToSecureString());
			}
			ExTraceGlobals.WorkerTracer.TraceInformation(17752, (long)this.GetHashCode(), "invoke new-mailuser");
			Error error;
			MailUser mailUser = base.SafeRunPSCommand<MailUser>(pscommand, base.AgentContext.Runspace, out error, null, null);
			if (error != null && error.Exception is WLCDUnmanagedMemberExistsException && !mailEnabledUserProvisioningData.IsBPOS && !mailEnabledUserProvisioningData.EvictLiveId)
			{
				pscommand = new PSCommand().AddCommand("New-MailUser");
				pscommand.AddParameter("EvictLiveID");
				if (!string.IsNullOrEmpty(mailEnabledUserProvisioningData.Password))
				{
					pscommand.AddParameter("Password", mailEnabledUserProvisioningData.Password.ConvertToSecureString());
				}
				base.PopulateParamsToPSCommand(pscommand, MeuProvisioningAgent.newMailUserParameterMap, mailEnabledUserProvisioningData.Parameters);
				mailUser = base.SafeRunPSCommand<MailUser>(pscommand, base.AgentContext.Runspace, out error, null, null);
			}
			if (error != null && error.Exception is WLCDManagedMemberExistsException)
			{
				pscommand = new PSCommand().AddCommand("Get-MailUser");
				if (base.PopulateParamsToPSCommand(pscommand, MeuProvisioningAgent.getMailUserParameterMap, mailEnabledUserProvisioningData.Parameters))
				{
					Error error2;
					mailUser = base.SafeRunPSCommand<MailUser>(pscommand, base.AgentContext.Runspace, out error2, null, null);
				}
			}
			if (mailUser == null)
			{
				if (error == null)
				{
					error = new Error(new InvalidDataException("no mailuser created or found, but no error either!"));
				}
				return error;
			}
			this.UpdateProxyAddressesParameter(mailUser);
			pscommand = new PSCommand().AddCommand("Set-MailUser");
			pscommand.AddParameter("Identity", mailUser.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, MeuProvisioningAgent.setMailUserParameterMap, mailEnabledUserProvisioningData.Parameters))
			{
				ExTraceGlobals.WorkerTracer.TraceInformation(17776, (long)this.GetHashCode(), "invoke set-mailuser");
				base.SafeRunPSCommand<ADUser>(pscommand, base.AgentContext.Runspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			pscommand = new PSCommand().AddCommand("set-user");
			pscommand.AddParameter("Identity", mailUser.Identity);
			if (base.PopulateParamsToPSCommand(pscommand, MeuProvisioningAgent.setUserParameterMap, mailEnabledUserProvisioningData.Parameters))
			{
				ExTraceGlobals.WorkerTracer.TraceInformation(17780, (long)this.GetHashCode(), "invoke set-user");
				base.SafeRunPSCommand<ADUser>(pscommand, base.AgentContext.Runspace, out error, null, null);
				if (error != null)
				{
					return error;
				}
			}
			return null;
		}

		protected override void UpdateProxyAddressesParameter(MailEnabledRecipient recipient)
		{
			base.UpdateProxyAddressesParameter(recipient);
			if (((RecipientProvisioningData)base.ProvisioningData).IsSmtpAddressCheckWithAcceptedDomain)
			{
				base.RemoveSmtpProxyAddressesWithExternalDomain();
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

		private static readonly string[][] newMailUserParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.WindowsLiveID.Name,
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

		private static readonly string[][] setMailUserParameterMap = new string[][]
		{
			new string[]
			{
				ADRecipientSchema.EmailAddresses.Name,
				string.Empty
			}
		};

		private static readonly string[][] getMailUserParameterMap = new string[][]
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

		private static readonly string[][] setUserParameterMap = new string[][]
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
