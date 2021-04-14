using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InferenceNewMailNotificationHandler : NewMailNotificationHandler
	{
		public InferenceNewMailNotificationHandler(string subscriptionId, IMailboxContext userContext) : base(subscriptionId, userContext)
		{
		}

		protected override PropertyDefinition[] GetAdditionalPropsToLoad()
		{
			return InferenceNewMailNotificationHandler.additionalPropsToBind;
		}

		protected override void OnPayloadCreated(MessageItem newMessage, NewMailNotificationPayload payload)
		{
			object obj = newMessage.TryGetProperty(ItemSchema.IsClutter);
			bool isClutter = false;
			if (obj is bool)
			{
				isClutter = (bool)obj;
			}
			payload.IsClutter = isClutter;
		}

		private static readonly PropertyDefinition[] additionalPropsToBind = new PropertyDefinition[]
		{
			ItemSchema.IsClutter
		};
	}
}
