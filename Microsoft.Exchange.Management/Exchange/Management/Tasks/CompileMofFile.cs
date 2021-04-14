using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Compile", "MofFile")]
	public class CompileMofFile : Task
	{
		[Parameter(Mandatory = true)]
		public string MofFilePath
		{
			get
			{
				return (string)base.Fields["MofFilePath"];
			}
			set
			{
				base.Fields["MofFilePath"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			MofCompiler mofCompiler = new MofCompiler();
			WbemCompileStatusInfo wbemCompileStatusInfo = default(WbemCompileStatusInfo);
			wbemCompileStatusInfo.InitializeStatusInfo();
			int num = mofCompiler.CompileFile(this.MofFilePath, null, null, null, null, 0, 0, 0, ref wbemCompileStatusInfo);
			if (num == 0)
			{
				TaskLogger.Log(Strings.ExchangeTracingProviderInstalledSuccess);
				return;
			}
			if (num == 262145)
			{
				TaskLogger.Log(Strings.ExchangeTracingProviderAlreadyExists);
				return;
			}
			base.WriteError(new ExchangeTracingProviderInstallException(wbemCompileStatusInfo.HResult), ErrorCategory.NotSpecified, wbemCompileStatusInfo);
		}
	}
}
