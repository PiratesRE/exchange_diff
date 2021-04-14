using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "ExsetdataAtom")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallExsetdataAtom : ManageExsetdataAtom
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
			base.InstallAtom(this.AtomName);
			TaskLogger.LogExit();
		}
	}
}
