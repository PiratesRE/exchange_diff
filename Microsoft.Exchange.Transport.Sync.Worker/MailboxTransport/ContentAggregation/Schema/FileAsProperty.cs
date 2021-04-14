using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Properties.XSO;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FileAsProperty : ContactProperty<string>
	{
		public FileAsProperty(IXSOPropertyManager propertyManager) : base(propertyManager, new PropertyDefinition[]
		{
			FileAsProperty.FileAsXSOProperty,
			FileAsProperty.FileAsIdXSOProperty
		})
		{
		}

		public override string ReadProperty(Item item)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			return SyncUtilities.SafeGetProperty<string>(item, FileAsProperty.FileAsXSOProperty);
		}

		public override void WriteProperty(Item item, string value)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			if (value == null)
			{
				if (base.IsItemNew(item))
				{
					item[FileAsProperty.FileAsIdXSOProperty] = FileAsMapping.LastCommaFirst;
					return;
				}
			}
			else
			{
				item[FileAsProperty.FileAsXSOProperty] = value;
			}
		}

		private static readonly PropertyDefinition FileAsXSOProperty = ContactBaseSchema.FileAs;

		private static readonly PropertyDefinition FileAsIdXSOProperty = ContactSchema.FileAsId;
	}
}
