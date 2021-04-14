using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupMailboxJoinRequestMessageSchema : MessageItemSchema
	{
		public new static GroupMailboxJoinRequestMessageSchema Instance
		{
			get
			{
				if (GroupMailboxJoinRequestMessageSchema.instance == null)
				{
					GroupMailboxJoinRequestMessageSchema.instance = new GroupMailboxJoinRequestMessageSchema();
				}
				return GroupMailboxJoinRequestMessageSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition GroupSmtpAddress = InternalSchema.XGroupMailboxSmtpAddressId;

		private static GroupMailboxJoinRequestMessageSchema instance;
	}
}
