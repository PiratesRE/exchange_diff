using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class EntitySyncWatermark : MailboxSyncWatermark
	{
		public EntitySyncWatermark()
		{
		}

		protected EntitySyncWatermark(int changeNumber) : base(changeNumber)
		{
		}

		public new static EntitySyncWatermark Create()
		{
			return new EntitySyncWatermark();
		}

		public new static EntitySyncWatermark CreateForSingleItem()
		{
			return new EntitySyncWatermark();
		}

		public new static EntitySyncWatermark CreateWithChangeNumber(int changeNumber)
		{
			return new EntitySyncWatermark(changeNumber);
		}

		public override ICustomSerializable BuildObject()
		{
			return new EntitySyncWatermark();
		}

		public override object Clone()
		{
			EntitySyncWatermark entitySyncWatermark = EntitySyncWatermark.CreateWithChangeNumber(base.ChangeNumber);
			if (base.IcsState != null)
			{
				entitySyncWatermark.IcsState = (byte[])base.IcsState.Clone();
			}
			return entitySyncWatermark;
		}

		public override object CustomClone()
		{
			return EntitySyncWatermark.CreateWithChangeNumber(base.ChangeNumber);
		}
	}
}
