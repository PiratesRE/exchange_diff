using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Update", "ServiceExecutable")]
	public sealed class UpdateServiceExecutable : ManageServiceBase
	{
		[Parameter(Mandatory = true)]
		public string ServiceName
		{
			get
			{
				return (string)base.Fields["ServiceName"];
			}
			set
			{
				base.Fields["ServiceName"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string Executable
		{
			get
			{
				return (string)base.Fields["Executable"];
			}
			set
			{
				base.Fields["Executable"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string executablePath = "\"" + Path.Combine(ConfigurationContext.Setup.BinPath, this.Executable) + "\"";
			base.UpdateExecutable(this.ServiceName, executablePath);
			TaskLogger.LogExit();
		}
	}
}
