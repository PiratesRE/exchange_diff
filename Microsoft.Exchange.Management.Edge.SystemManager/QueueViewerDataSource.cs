using System;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.Tasks;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class QueueViewerDataSource : TaskDataSource
	{
		public QueueViewerDataSource(string noun) : base(noun)
		{
			base.RefreshCommandText = Strings.RefreshLabelText;
		}

		protected override void OnDoRefreshWork(RefreshRequestEventArgs e)
		{
			MonadCommand monadCommand = e.Argument as MonadCommand;
			if (monadCommand == null || monadCommand.Parameters.Contains("Identity") || (monadCommand.Parameters.Contains("server") && !string.IsNullOrEmpty((string)monadCommand.Parameters["server"].Value)))
			{
				base.OnDoRefreshWork(e);
				return;
			}
			DataTable result = base.Table.Clone();
			e.Result = result;
			e.ReportProgress(100, 100, "", null);
		}

		internal const string ServerParameter = "server";
	}
}
