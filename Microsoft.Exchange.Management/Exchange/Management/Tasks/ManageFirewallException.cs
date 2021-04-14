using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ManageFirewallException : Task
	{
		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath BinaryPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["BinaryPath"];
			}
			set
			{
				base.Fields["BinaryPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExchangeFirewallRule FirewallRule
		{
			get
			{
				return (ExchangeFirewallRule)base.Fields["FirewallRule"];
			}
			set
			{
				base.Fields["FirewallRule"] = value;
			}
		}
	}
}
