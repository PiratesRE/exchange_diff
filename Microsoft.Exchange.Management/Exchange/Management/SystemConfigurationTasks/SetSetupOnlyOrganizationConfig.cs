using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "SetupOnlyOrganizationConfig", DefaultParameterSetName = "Identity")]
	public sealed class SetSetupOnlyOrganizationConfig : BaseOrganization
	{
		[Parameter(Mandatory = false)]
		public int ObjectVersion
		{
			get
			{
				return (int)base.Fields[OrganizationSchema.ObjectVersion];
			}
			set
			{
				base.Fields[OrganizationSchema.ObjectVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields[ADObjectSchema.Name];
			}
			set
			{
				base.Fields[ADObjectSchema.Name] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable result;
			try
			{
				Organization organization = (Organization)base.PrepareDataObject();
				if (base.Fields.IsModified(OrganizationSchema.ObjectVersion))
				{
					organization.ObjectVersion = this.ObjectVersion;
				}
				if (base.Fields.IsModified(ADObjectSchema.Name))
				{
					organization.Name = this.Name;
				}
				organization.BuildNumber = FileVersionInfo.GetVersionInfo(Path.Combine(ExchangeSetupContext.AssemblyPath, "ExSetup.exe")).ProductVersion;
				result = organization;
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}
	}
}
