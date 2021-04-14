using System;
using Microsoft.Exchange.InstantMessaging;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class DeliverySuccessNotification
	{
		internal InstantMessageOCSProvider Provider { get; private set; }

		internal IIMModality Context { get; private set; }

		internal int MessageId { get; private set; }

		internal RequestDetailsLogger Logger { get; private set; }

		internal DeliverySuccessNotification(InstantMessageOCSProvider provider, IIMModality context, int messageId, RequestDetailsLogger logger)
		{
			this.Provider = provider;
			this.Context = context;
			this.MessageId = messageId;
			this.Logger = logger;
		}
	}
}
