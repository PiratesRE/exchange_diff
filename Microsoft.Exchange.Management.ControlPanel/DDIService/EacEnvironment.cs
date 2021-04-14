using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.DDIService
{
	public class EacEnvironment : IEacEnvironment
	{
		private EacEnvironment()
		{
		}

		public static IEacEnvironment Instance { get; internal set; } = new EacEnvironment();

		public bool IsForefrontForOffice
		{
			get
			{
				return DatacenterRegistry.IsForefrontForOffice();
			}
		}

		public bool IsDataCenter
		{
			get
			{
				return Util.IsDataCenter;
			}
		}
	}
}
