using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "ADQueryPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetADQueryPolicy : SetSystemConfigurationObjectTask<ADQueryPolicyIdParameter, ADQueryPolicy>
	{
		protected override ObjectId RootId
		{
			get
			{
				return this.ConfigurationSession.GetExchangeConfigurationContainer().Id.Parent.GetChildId("CN", "Windows NT");
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 41, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetADQueryPolicy.cs");
		}
	}
}
