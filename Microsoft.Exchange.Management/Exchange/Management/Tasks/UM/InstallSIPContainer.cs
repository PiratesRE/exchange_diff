using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Install", "SIPContainer")]
	public sealed class InstallSIPContainer : InstallContainerTaskBase<SIPContainer>
	{
		public InstallSIPContainer()
		{
			base.Name = new string[]
			{
				"SIP"
			};
		}

		protected override ADObjectId GetBaseContainer()
		{
			return SIPContainer.GetBaseContainer(base.DataSession as ITopologyConfigurationSession);
		}
	}
}
