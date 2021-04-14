using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociationGroupSchema : MailboxAssociationBaseSchema
	{
		private MailboxAssociationGroupSchema()
		{
		}

		public new static MailboxAssociationGroupSchema Instance
		{
			get
			{
				if (MailboxAssociationGroupSchema.instance == null)
				{
					MailboxAssociationGroupSchema.instance = new MailboxAssociationGroupSchema();
				}
				return MailboxAssociationGroupSchema.instance;
			}
		}

		private static MailboxAssociationGroupSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition PinDate = InternalSchema.MailboxAssociationPinDate;
	}
}
