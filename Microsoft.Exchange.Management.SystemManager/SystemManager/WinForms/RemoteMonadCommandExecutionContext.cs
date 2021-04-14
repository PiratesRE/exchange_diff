using System;
using System.Data;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class RemoteMonadCommandExecutionContext : MonadCommandExecutionContext
	{
		public override void Open(IUIService service)
		{
			this.uiService = service;
			this.commandInteractionHandler = ((service != null) ? new WinFormsCommandInteractionHandler(service) : new CommandInteractionHandler());
		}

		private void ConnectTo(string targetForest)
		{
			this.connection = new MonadConnection("timeout=30", this.commandInteractionHandler, null, PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo(this.uiService, ConnectedForestsInfoSingleton.GetInstance().ForestInfoOf(targetForest)));
			this.connection.Open();
		}

		public override void Execute(TaskProfileBase profile, DataRow row, DataObjectStore store)
		{
			this.ConnectTo((string)row["TargetForest"]);
			base.Execute(profile, row, store);
		}

		private IUIService uiService;
	}
}
