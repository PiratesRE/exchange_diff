using System;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Management.PowerShell
{
	public sealed class EdgePSSnapIn : ExchangePSSnapIn
	{
		public override string Name
		{
			get
			{
				return AdminPSSnapIn.PSSnapInName;
			}
		}

		public override Collection<CmdletConfigurationEntry> Cmdlets
		{
			get
			{
				if (this.cmdlets == null)
				{
					this.cmdlets = new Collection<CmdletConfigurationEntry>();
					foreach (CmdletConfigurationEntry item in CmdletConfigurationEntries.ExchangeCmdletConfigurationEntries)
					{
						this.cmdlets.Add(item);
					}
					foreach (CmdletConfigurationEntry item2 in CmdletConfigurationEntries.ExchangeEdgeCmdletConfigurationEntries)
					{
						this.cmdlets.Add(item2);
					}
				}
				return this.cmdlets;
			}
		}

		private Collection<CmdletConfigurationEntry> cmdlets;
	}
}
