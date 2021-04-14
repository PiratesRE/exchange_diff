using System;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class RemoveQueueMessagesTaskCommandAction : ResultsTaskCommandAction
	{
		protected override void OnInitialize()
		{
			base.OnInitialize();
			QueueViewerQueuesResultPane queueViewerQueuesResultPane = base.DataListViewResultPane as QueueViewerQueuesResultPane;
			base.RefreshOnFinish = queueViewerQueuesResultPane.CreateRefreshableObject(new object[]
			{
				RefreshCategories.Message
			});
		}

		protected override bool ConfirmOperation(WorkUnitCollectionEventArgs inputArgs)
		{
			QueueViewerQueuesResultPane queueViewerQueuesResultPane = base.DataListViewResultPane as QueueViewerQueuesResultPane;
			QueueIdentity queueIdentity = QueueIdentity.Parse(queueViewerQueuesResultPane.SelectedIdentity.ToString());
			string message = base.SingleSelectionConfirmation(queueIdentity.ToString());
			bool flag = DialogResult.Yes == queueViewerQueuesResultPane.ShowMessage(message, MessageBoxButtons.YesNo);
			if (flag)
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ExtensibleMessageInfoSchema.Queue, queueIdentity);
				base.Parameters.Remove("Filter");
				base.Parameters.AddWithValue("Filter", queryFilter.GenerateInfixString(FilterLanguage.Monad));
				base.Parameters.Remove("Server");
				base.Parameters.AddWithValue("Server", queueViewerQueuesResultPane.ServerName);
			}
			return flag;
		}
	}
}
