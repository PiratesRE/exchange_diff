using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CapacityExceededReservationException : ResourceReservationException
	{
		public CapacityExceededReservationException(string resourceName, int capacity) : base(MrsStrings.ErrorStaticCapacityExceeded(resourceName, capacity))
		{
			this.resourceName = resourceName;
			this.capacity = capacity;
		}

		public CapacityExceededReservationException(string resourceName, int capacity, Exception innerException) : base(MrsStrings.ErrorStaticCapacityExceeded(resourceName, capacity), innerException)
		{
			this.resourceName = resourceName;
			this.capacity = capacity;
		}

		protected CapacityExceededReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.capacity = (int)info.GetValue("capacity", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("capacity", this.capacity);
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
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

		private readonly int capacity;
	}
}
