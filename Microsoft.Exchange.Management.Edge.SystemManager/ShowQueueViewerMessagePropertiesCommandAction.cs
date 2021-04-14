using System;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class ShowQueueViewerMessagePropertiesCommandAction : ShowSelectionPropertiesCommandAction
	{
		protected override ExchangePropertyPageControl[] OnGetSingleSelectionPropertyPageControls()
		{
			QueueViewerMessagesResultPane queueViewerMessagesResultPane = base.ResultPane as QueueViewerMessagesResultPane;
			MonadDataHandler monadDataHandler = new MonadDataHandler(queueViewerMessagesResultPane.SelectedIdentity.ToString(), "get-message", "");
			monadDataHandler.SelectCommand.Parameters.AddWithValue("IncludeRecipientInfo", true);
			DataContext context = new DataContext(monadDataHandler);
			return new ExchangePropertyPageControl[]
			{
				new MessagePropertyPage
				{
					Context = context
				},
				new RecipientsInfoPropertyPage
				{
					Context = context
				}
			};
		}
	}
}
