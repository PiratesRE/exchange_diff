using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "Pop3Container")]
	public sealed class InstallPop3Container : InstallContainerTaskBase<Pop3Container>
	{
		public InstallPop3Container()
		{
			string protocolName = new Pop3AdConfiguration().ProtocolName;
			base.Name = new string[]
			{
				protocolName
			};
		}

		protected override ADObjectId GetBaseContainer()
		{
			return Pop3Container.GetBaseContainer(base.DataSession as ITopologyConfigurationSession);
		}
	}
}
