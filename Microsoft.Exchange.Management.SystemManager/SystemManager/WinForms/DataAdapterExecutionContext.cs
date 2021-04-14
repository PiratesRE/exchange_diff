using System;
using System.Data;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal abstract class DataAdapterExecutionContext : IDisposable
	{
		public abstract void Open(IUIService service, WorkUnitCollection workUnits, bool enforceViewEntireForest, ResultsLoaderProfile profile);

		public abstract void Close();

		public abstract void Execute(AbstractDataTableFiller filler, DataTable table, ResultsLoaderProfile profile);

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
	}
}
