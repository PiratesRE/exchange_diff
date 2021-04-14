using System;
using System.Data;
using System.Windows.Forms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal abstract class CommandExecutionContext : IDisposable
	{
		public abstract void Open(IUIService service);

		public abstract void Close();

		public abstract void Execute(TaskProfileBase profile, DataRow row, DataObjectStore store);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		public virtual bool ShouldReload
		{
			get
			{
				return true;
			}
		}
	}
}
