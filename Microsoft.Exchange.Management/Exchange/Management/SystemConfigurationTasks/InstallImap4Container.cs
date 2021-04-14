using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "Imap4Container")]
	public sealed class InstallImap4Container : InstallContainerTaskBase<Imap4Container>
	{
		public InstallImap4Container()
		{
			string protocolName = new Imap4AdConfiguration().ProtocolName;
			base.Name = new string[]
			{
				protocolName
			};
		}

		protected override ADObjectId GetBaseContainer()
		{
			return Imap4Container.GetBaseContainer(base.DataSession as ITopologyConfigurationSession);
		}
	}
}
