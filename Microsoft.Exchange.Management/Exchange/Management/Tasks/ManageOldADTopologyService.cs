using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public class ManageOldADTopologyService : ManageADTopologyService
	{
		internal string ServiceName
		{
			get
			{
				return this.Name;
			}
		}

		protected override string Name
		{
			get
			{
				return "ADTopologyService";
			}
		}

		protected string CurrentName
		{
			get
			{
				return "MSExchangeADTopology";
			}
		}
	}
}
