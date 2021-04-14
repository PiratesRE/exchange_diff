using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMsoRawObjectCommand : SyntheticCommandWithPipelineInputNoOutput<RecipientOrOrganizationIdParameter>
	{
		private GetMsoRawObjectCommand() : base("Get-MsoRawObject")
		{
		}

		public GetMsoRawObjectCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMsoRawObjectCommand SetParameters(GetMsoRawObjectCommand.ExchangeIdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMsoRawObjectCommand SetParameters(GetMsoRawObjectCommand.SyncObjectIdParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMsoRawObjectCommand SetParameters(GetMsoRawObjectCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ExchangeIdentityParameters : ParametersBase
		{
			public virtual RecipientOrOrganizationIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual SwitchParameter IncludeBackLinks
			{
				set
				{
					base.PowerSharpParameters["IncludeBackLinks"] = value;
				}
			}

			public virtual SwitchParameter IncludeForwardLinks
			{
				set
				{
					base.PowerSharpParameters["IncludeForwardLinks"] = value;
				}
			}

			public virtual int LinksResultSize
			{
				set
				{
					base.PowerSharpParameters["LinksResultSize"] = value;
				}
			}

			public virtual SwitchParameter PopulateRawObject
			{
				set
				{
					base.PowerSharpParameters["PopulateRawObject"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}
		}

		public class SyncObjectIdParameters : ParametersBase
		{
			public virtual SyncObjectId ExternalObjectId
			{
				set
				{
					base.PowerSharpParameters["ExternalObjectId"] = value;
				}
			}

			public virtual string ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
				}
			}

			public virtual SwitchParameter IncludeBackLinks
			{
				set
				{
					base.PowerSharpParameters["IncludeBackLinks"] = value;
				}
			}

			public virtual SwitchParameter IncludeForwardLinks
			{
				set
				{
					base.PowerSharpParameters["IncludeForwardLinks"] = value;
				}
			}

			public virtual int LinksResultSize
			{
				set
				{
					base.PowerSharpParameters["LinksResultSize"] = value;
				}
			}

			public virtual SwitchParameter PopulateRawObject
			{
				set
				{
					base.PowerSharpParameters["PopulateRawObject"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IncludeBackLinks
			{
				set
				{
					base.PowerSharpParameters["IncludeBackLinks"] = value;
				}
			}

			public virtual SwitchParameter IncludeForwardLinks
			{
				set
				{
					base.PowerSharpParameters["IncludeForwardLinks"] = value;
				}
			}

			public virtual int LinksResultSize
			{
				set
				{
					base.PowerSharpParameters["LinksResultSize"] = value;
				}
			}

			public virtual SwitchParameter PopulateRawObject
			{
				set
				{
					base.PowerSharpParameters["PopulateRawObject"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}
		}
	}
}
