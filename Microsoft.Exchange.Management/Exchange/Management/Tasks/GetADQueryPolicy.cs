using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ADQueryPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetADQueryPolicy : GetSystemConfigurationObjectTask<ADQueryPolicyIdParameter, ADQueryPolicy>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.ConfigurationSession.GetExchangeConfigurationContainer().Id.Parent.GetChildId("CN", "Windows NT");
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 43, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\GetADQueryPolicy.cs");
		}
	}
}
