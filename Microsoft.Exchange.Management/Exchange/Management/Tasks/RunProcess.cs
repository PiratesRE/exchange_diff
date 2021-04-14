using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.StartSetupProcess)]
	[Cmdlet("start", "SetupProcess", SupportsShouldProcess = true)]
	public class RunProcess : RunProcessBase
	{
		[Parameter(Mandatory = true)]
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.ExeName = this.Name;
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		private const string ExeNameParameter = "Name";
	}
}
