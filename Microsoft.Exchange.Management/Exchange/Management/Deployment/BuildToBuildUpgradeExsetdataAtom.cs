using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("BuildToBuildUpgrade", "ExsetdataAtom")]
	public sealed class BuildToBuildUpgradeExsetdataAtom : ManageExsetdataAtom
	{
		[Parameter(Mandatory = true)]
		public AtomID AtomName
		{
			get
			{
				return (AtomID)base.Fields["AtomName"];
			}
			set
			{
				base.Fields["AtomName"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.BuildToBuildUpgradeAtom(this.AtomName);
			TaskLogger.LogExit();
		}
	}
}
