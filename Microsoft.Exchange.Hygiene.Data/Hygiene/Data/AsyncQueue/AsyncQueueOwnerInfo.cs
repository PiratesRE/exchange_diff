using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueOwnerInfo : ConfigurablePropertyBag
	{
		public AsyncQueueOwnerInfo(string ownerId) : this(ownerId, AsyncQueueFlags.None)
		{
		}

		public AsyncQueueOwnerInfo(string ownerId, AsyncQueueFlags flags)
		{
			this.OwnerId = ownerId;
			this.Flags = flags;
		}

		public AsyncQueueOwnerInfo()
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ObjectId.ToString());
			}
		}

		public Guid ObjectId
		{
			get
			{
				this.identity = new Guid(DalHelper.GetMDHash(this.OwnerId) ?? new byte[16]);
				return this.identity;
			}
			set
			{
				this.identity = value;
			}
		}

		public AsyncQueueFlags Flags
		{
			get
			{
				return (AsyncQueueFlags)this[AsyncQueueOwnerInfoSchema.FlagsProperty];
			}
			set
			{
				this[AsyncQueueOwnerInfoSchema.FlagsProperty] = value;
			}
		}

		public string OwnerId
		{
			get
			{
				return (string)this[AsyncQueueOwnerInfoSchema.OwnerIdProperty];
			}
			set
			{
				this[AsyncQueueOwnerInfoSchema.OwnerIdProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueOwnerInfoSchema);
		}

		private Guid identity;
	}
}
