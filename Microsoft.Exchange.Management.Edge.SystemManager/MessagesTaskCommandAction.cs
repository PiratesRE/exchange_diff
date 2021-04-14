using System;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class MessagesTaskCommandAction : QueueViewerTaskCommandAction
	{
		protected override void OnInitialize()
		{
			base.OnInitialize();
			QueueViewerMessagesResultPane queueViewerMessagesResultPane = base.DataListViewResultPane as QueueViewerMessagesResultPane;
			base.RefreshOnFinish = queueViewerMessagesResultPane.CreateRefreshableObject(new object[]
			{
				RefreshCategories.Message
			});
		}
	}
}
