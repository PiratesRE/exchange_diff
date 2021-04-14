using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookClientMessageInspector : IClientMessageInspector
	{
		public event EventHandler<FacebookMessageEventArgs> MessageDownloaded;

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			EventHandler<FacebookMessageEventArgs> messageDownloaded = this.MessageDownloaded;
			if (messageDownloaded != null && reply != null)
			{
				messageDownloaded(this, new FacebookMessageEventArgs(reply));
			}
		}
	}
}
