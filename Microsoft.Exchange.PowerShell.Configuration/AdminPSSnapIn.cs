using System;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Management.PowerShell
{
	public sealed class AdminPSSnapIn : ExchangePSSnapIn
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
					foreach (CmdletConfigurationEntry item2 in CmdletConfigurationEntries.ExchangeNonEdgeCmdletConfigurationEntries)
					{
						this.cmdlets.Add(item2);
					}
					foreach (CmdletConfigurationEntry item3 in CmdletConfigurationEntries.ExchangeNonGallatinCmdletConfigurationEntries)
					{
						this.cmdlets.Add(item3);
					}
				}
				return this.cmdlets;
			}
		}

		public static readonly string PSSnapInName = "Microsoft.Exchange.Management.PowerShell.E2010";

		private Collection<CmdletConfigurationEntry> cmdlets;
	}
}
