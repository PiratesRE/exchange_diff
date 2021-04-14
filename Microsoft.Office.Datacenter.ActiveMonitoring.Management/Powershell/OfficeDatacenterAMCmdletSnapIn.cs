using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management.Powershell
{
	[RunInstaller(true)]
	public class OfficeDatacenterAMCmdletSnapIn : CustomPSSnapIn
	{
		public override string Description
		{
			get
			{
				return "Provides a Windows Powershell interface for Office Datacenter Monitoring Service Management Tools.";
			}
		}

		public override string Name
		{
			get
			{
				return OfficeDatacenterAMCmdletSnapIn.PSSnapInName;
			}
		}

		public override string Vendor
		{
			get
			{
				return "Microsoft Corporation";
			}
		}

		public override Collection<CmdletConfigurationEntry> Cmdlets
		{
			get
			{
				if (this.cmdlets == null)
				{
					this.cmdlets = new Collection<CmdletConfigurationEntry>();
					foreach (CmdletConfigurationEntry item in CmdletConfiguration.CmdletConfigurationEntries)
					{
						this.cmdlets.Add(item);
					}
				}
				return this.cmdlets;
			}
		}

		public override Collection<FormatConfigurationEntry> Formats
		{
			get
			{
				if (this.formats == null)
				{
					this.formats = new Collection<FormatConfigurationEntry>();
					foreach (FormatConfigurationEntry item in CmdletConfiguration.FormatConfigurationEntries)
					{
						this.formats.Add(item);
					}
				}
				return this.formats;
			}
		}

		public static readonly string PSSnapInName = " Microsoft.Office.Datacenter.ActiveMonitoring.Management.Powershell";

		private Collection<CmdletConfigurationEntry> cmdlets;

		private Collection<FormatConfigurationEntry> formats;
	}
}
