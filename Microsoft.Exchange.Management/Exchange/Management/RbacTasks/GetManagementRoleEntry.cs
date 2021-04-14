using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Get", "ManagementRoleEntry", DefaultParameterSetName = "Identity")]
	public sealed class GetManagementRoleEntry : GetMultitenancySystemConfigurationObjectTask<RoleEntryIdParameter, ExchangeRoleEntryPresentation>
	{
		private new OrganizationIdParameter Organization { get; set; }

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override RoleEntryIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter]
		public string[] Parameters
		{
			get
			{
				return (string[])base.Fields[RbacCommonParameters.ParameterParameters];
			}
			set
			{
				RoleEntry.FormatParameters(value);
				base.Fields[RbacCommonParameters.ParameterParameters] = value;
			}
		}

		[Parameter]
		public string PSSnapinName
		{
			get
			{
				return (string)base.Fields[RbacCommonParameters.ParameterPSSnapinName];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterPSSnapinName] = value;
			}
		}

		[Parameter]
		public ManagementRoleEntryType[] Type
		{
			get
			{
				return (ManagementRoleEntryType[])base.Fields[RbacCommonParameters.ParameterType];
			}
			set
			{
				base.VerifyValues<ManagementRoleEntryType>(GetManagementRoleEntry.AllowedRoleEntryTypes, value);
				base.Fields[RbacCommonParameters.ParameterType] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			this.Identity.Parameters = this.Parameters;
			this.Identity.PSSnapinName = this.PSSnapinName;
			if (this.Type != null && this.Type.Length > 0)
			{
				ManagementRoleEntryType managementRoleEntryType = (ManagementRoleEntryType)0;
				foreach (ManagementRoleEntryType managementRoleEntryType2 in this.Type)
				{
					managementRoleEntryType |= managementRoleEntryType2;
				}
				this.Identity.Type = managementRoleEntryType;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		private static readonly ManagementRoleEntryType[] AllowedRoleEntryTypes = new ManagementRoleEntryType[]
		{
			ManagementRoleEntryType.Cmdlet,
			ManagementRoleEntryType.Script,
			ManagementRoleEntryType.ApplicationPermission,
			ManagementRoleEntryType.WebService
		};
	}
}
