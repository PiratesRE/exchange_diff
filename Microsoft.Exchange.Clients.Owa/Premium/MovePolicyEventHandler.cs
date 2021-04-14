using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("MovePolicy")]
	internal sealed class MovePolicyEventHandler : PolicyEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(MovePolicyEventHandler));
		}

		public MovePolicyEventHandler() : base(PolicyProvider.MovePolicyProvider)
		{
		}

		protected override PolicyContextMenuBase InternalGetPolicyMenu(ref OwaStoreObjectId itemId)
		{
			if (itemId != null)
			{
				if (itemId.OwaStoreObjectIdType != OwaStoreObjectIdType.MailBoxObject && itemId.OwaStoreObjectIdType != OwaStoreObjectIdType.Conversation)
				{
					throw new OwaInvalidRequestException("Only mailbox and conversation objects can be handled.");
				}
				if (itemId.IsConversationId)
				{
					OwaStoreObjectId[] allItemIds = ConversationUtilities.GetAllItemIds(base.UserContext, new OwaStoreObjectId[]
					{
						itemId
					}, new StoreObjectId[0]);
					if (allItemIds.Length == 1)
					{
						itemId = allItemIds[0];
					}
				}
			}
			return MovePolicyContextMenu.Create(base.UserContext);
		}

		public const string EventNamespace = "MovePolicy";
	}
}
