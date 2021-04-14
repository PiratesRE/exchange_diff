using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ComInteropTlbTaskBase : RunProcessBase
	{
		public ComInteropTlbTaskBase(bool register)
		{
			this.register = register;
		}

		protected override void InternalProcessRecord()
		{
			base.ExeName = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "regasm.exe");
			base.Args = string.Concat(new string[]
			{
				this.register ? string.Empty : "/unregister ",
				"/tlb:\"",
				Path.Combine(ConfigurationContext.Setup.BinPath, "ComInterop.tlb"),
				"\" \"",
				Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Transport.Agent.ContentFilter.ComInterop.dll"),
				"\""
			});
			base.Timeout = 60000;
			base.InternalProcessRecord();
		}

		[Precondition(ExpectedResult = true, FailureDescriptionId = Strings.IDs.ComInteropDllNotFound)]
		internal FileExistsCondition ComInteropDllExistCheck
		{
			get
			{
				return new FileExistsCondition(Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Transport.Agent.ContentFilter.ComInterop.dll"));
			}
		}

		[Precondition(ExpectedResult = true, FailureDescriptionId = Strings.IDs.RegasmNotFound)]
		internal FileExistsCondition RegAsmExistCheck
		{
			get
			{
				return new FileExistsCondition(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "regasm.exe"));
			}
		}

		protected const string ComInteropDllName = "Microsoft.Exchange.Transport.Agent.ContentFilter.ComInterop.dll";

		protected const string ComInteropTlbName = "ComInterop.tlb";

		protected const string RegAsmFilename = "regasm.exe";

		private readonly bool register;
	}
}
