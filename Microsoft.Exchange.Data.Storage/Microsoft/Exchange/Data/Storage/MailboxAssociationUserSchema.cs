using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociationUserSchema : MailboxAssociationBaseSchema
	{
		private MailboxAssociationUserSchema()
		{
		}

		public new static MailboxAssociationUserSchema Instance
		{
			get
			{
				if (MailboxAssociationUserSchema.instance == null)
				{
					MailboxAssociationUserSchema.instance = new MailboxAssociationUserSchema();
				}
				return MailboxAssociationUserSchema.instance;
			}
		}

		private static MailboxAssociationUserSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition JoinedBy = InternalSchema.MailboxAssociationJoinedBy;

		[Autoload]
		public static readonly StorePropertyDefinition LastVisitedDate = InternalSchema.MailboxAssociationLastVisitedDate;
	}
}
