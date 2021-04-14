using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WlmCapacityExceededReservationException : ResourceReservationException
	{
		public WlmCapacityExceededReservationException(string resourceName, string resourceType, string wlmResourceKey, int wlmResourceMetricType, int capacity) : base(MrsStrings.ErrorWlmCapacityExceeded3(resourceName, resourceType, wlmResourceKey, wlmResourceMetricType, capacity))
		{
			this.resourceName = resourceName;
			this.resourceType = resourceType;
			this.wlmResourceKey = wlmResourceKey;
			this.wlmResourceMetricType = wlmResourceMetricType;
			this.capacity = capacity;
		}

		public WlmCapacityExceededReservationException(string resourceName, string resourceType, string wlmResourceKey, int wlmResourceMetricType, int capacity, Exception innerException) : base(MrsStrings.ErrorWlmCapacityExceeded3(resourceName, resourceType, wlmResourceKey, wlmResourceMetricType, capacity), innerException)
		{
			this.resourceName = resourceName;
			this.resourceType = resourceType;
			this.wlmResourceKey = wlmResourceKey;
			this.wlmResourceMetricType = wlmResourceMetricType;
			this.capacity = capacity;
		}

		protected WlmCapacityExceededReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.resourceType = (string)info.GetValue("resourceType", typeof(string));
			this.wlmResourceKey = (string)info.GetValue("wlmResourceKey", typeof(string));
			this.wlmResourceMetricType = (int)info.GetValue("wlmResourceMetricType", typeof(int));
			this.capacity = (int)info.GetValue("capacity", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("resourceType", this.resourceType);
			info.AddValue("wlmResourceKey", this.wlmResourceKey);
			info.AddValue("wlmResourceMetricType", this.wlmResourceMetricType);
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

		public string WlmResourceKey
		{
			get
			{
				return this.wlmResourceKey;
			}
		}

		public int WlmResourceMetricType
		{
			get
			{
				return this.wlmResourceMetricType;
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

		private readonly string wlmResourceKey;

		private readonly int wlmResourceMetricType;

		private readonly int capacity;
	}
}
