using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class ViewQueueMessagesCommandAction : ResultsCommandAction
	{
		protected override void OnExecute()
		{
			base.OnExecute();
			QueueViewerQueuesResultPane queueViewerQueuesResultPane = base.ResultPane as QueueViewerQueuesResultPane;
			QueueViewerResultPane queueViewerResultPane = base.ResultPane.ContainerResultPane as QueueViewerResultPane;
			QueryFilter data = new ComparisonFilter(ComparisonOperator.Equal, ExtensibleMessageInfoSchema.Queue, queueViewerQueuesResultPane.SelectedIdentity);
			queueViewerResultPane.MessageListResultPane.Text = queueViewerQueuesResultPane.SelectedName;
			queueViewerResultPane.CommandMessagesView.Text = new LocalizedString(queueViewerResultPane.MessageListResultPane.Text);
			if (!queueViewerResultPane.ResultPaneTabs.Contains(queueViewerResultPane.MessageListResultPane))
			{
				queueViewerResultPane.ResultPaneTabs.Add(queueViewerResultPane.MessageListResultPane);
				queueViewerResultPane.CommandMessagesView.Visible = true;
				QueueViewerResultPaneBase messageListResultPane = queueViewerResultPane.MessageListResultPane;
				messageListResultPane.SettingsKey = messageListResultPane.Name;
				messageListResultPane.LoadComponentSettings();
			}
			queueViewerResultPane.MessageListResultPane.Datasource.BeginInit();
			queueViewerResultPane.SelectedResultPane = queueViewerResultPane.MessageListResultPane;
			queueViewerResultPane.MessageListResultPane.ObjectList.FilterControl.PersistedExpression = WinformsHelper.Serialize(data);
			queueViewerResultPane.MessageListResultPane.Datasource.GoToFirstPage();
			queueViewerResultPane.MessageListResultPane.Datasource.EndInit();
			queueViewerResultPane.MessageListResultPane.Datasource.Refresh();
		}
	}
}
