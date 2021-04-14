using System;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookMessageEventArgs : EventArgs
	{
		public FacebookMessageEventArgs(Message messageTransferred)
		{
			ArgumentValidator.ThrowIfNull("MessageTransferred", messageTransferred);
			this.messageTransferred = messageTransferred;
		}

		public Message MessageTransferred
		{
			get
			{
				return this.messageTransferred;
			}
		}

		private Message messageTransferred;
	}
}
