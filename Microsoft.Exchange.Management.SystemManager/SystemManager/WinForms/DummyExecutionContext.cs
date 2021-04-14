using System;
using System.Data;
using System.Windows.Forms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DummyExecutionContext : CommandExecutionContext
	{
		public override void Open(IUIService service)
		{
		}

		public override void Execute(TaskProfileBase profile, DataRow row, DataObjectStore store)
		{
			profile.Run(null, row, store);
		}

		public override void Close()
		{
		}
	}
}
