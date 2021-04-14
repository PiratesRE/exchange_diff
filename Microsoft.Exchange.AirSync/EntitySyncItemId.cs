using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EntitySyncItemId : MailboxSyncItemId
	{
		public EntitySyncItemId()
		{
		}

		protected EntitySyncItemId(StoreObjectId id) : base(id)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return EntitySyncItemId.typeId;
			}
			set
			{
				EntitySyncItemId.typeId = value;
			}
		}

		public string UID { get; set; }

		public static EntitySyncItemId CreateFromId(StoreId storeId)
		{
			return new EntitySyncItemId(StoreId.GetStoreObjectId(storeId));
		}

		public static string GetEntityID(StoreId storeId)
		{
			return IdConverter.Instance.ToStringId(StoreId.GetStoreObjectId(storeId), Command.CurrentCommand.MailboxSession);
		}

		public static AttachmentId GetAttachmentId(string attachmentId)
		{
			IList<AttachmentId> attachmentIds = IdConverter.GetAttachmentIds(attachmentId);
			if (attachmentIds.Count > 0)
			{
				return attachmentIds[0];
			}
			return null;
		}

		public override ICustomSerializable BuildObject()
		{
			return new EntitySyncItemId();
		}

		private static ushort typeId;
	}
}
