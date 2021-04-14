using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Cmdlet("Set", "EnvironmentVariable")]
	public sealed class SetEnvironmentVariable : ManageEnvironmentVariable
	{
		[Parameter(Mandatory = true)]
		public string Value
		{
			get
			{
				return (string)base.Fields["Value"];
			}
			set
			{
				base.Fields["Value"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.SetVariable(base.Name, this.Value, base.Target);
			TaskLogger.LogExit();
		}
	}
}
