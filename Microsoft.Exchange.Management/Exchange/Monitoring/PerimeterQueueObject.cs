using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class PerimeterQueueObject : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PerimeterQueueObject.schema;
			}
		}

		internal PerimeterQueueObject(PerimeterQueue queue, PerimeterQueueStatus status) : base(new PerimeterQueuePropertyBag(false, 16))
		{
			this.Identity = new PerimeterQueueId(queue.Name);
			this.MessageCount = queue.Value;
			this.Status = status;
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this.propertyBag[PerimeterQueueStatusSchema.Identity];
			}
			internal set
			{
				this.propertyBag[PerimeterQueueStatusSchema.Identity] = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this.propertyBag[PerimeterQueueStatusSchema.MessageCount];
			}
			internal set
			{
				this.propertyBag[PerimeterQueueStatusSchema.MessageCount] = value;
			}
		}

		public PerimeterQueueStatus Status
		{
			get
			{
				return (PerimeterQueueStatus)this.propertyBag[PerimeterQueueStatusSchema.Status];
			}
			internal set
			{
				this.propertyBag[PerimeterQueueStatusSchema.Status] = value;
			}
		}

		private static PerimeterQueueStatusSchema schema = ObjectSchema.GetInstance<PerimeterQueueStatusSchema>();
	}
}
