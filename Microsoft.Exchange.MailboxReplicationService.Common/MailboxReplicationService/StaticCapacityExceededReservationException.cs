using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StaticCapacityExceededReservationException : ResourceReservationException
	{
		public StaticCapacityExceededReservationException(string resourceName, string resourceType, int capacity) : base(MrsStrings.ErrorStaticCapacityExceeded1(resourceName, resourceType, capacity))
		{
			this.resourceName = resourceName;
			this.resourceType = resourceType;
			this.capacity = capacity;
		}

		public StaticCapacityExceededReservationException(string resourceName, string resourceType, int capacity, Exception innerException) : base(MrsStrings.ErrorStaticCapacityExceeded1(resourceName, resourceType, capacity), innerException)
		{
			this.resourceName = resourceName;
			this.resourceType = resourceType;
			this.capacity = capacity;
		}

		protected StaticCapacityExceededReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.resourceType = (string)info.GetValue("resourceType", typeof(string));
			this.capacity = (int)info.GetValue("capacity", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("resourceType", this.resourceType);
			info.AddValue("capacity", this.capacity);
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		public string ResourceType
		{
			get
			{
				return this.resourceType;
			}
		}

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
		}

		private readonly string resourceName;

		private readonly string resourceType;

		private readonly int capacity;
	}
}
