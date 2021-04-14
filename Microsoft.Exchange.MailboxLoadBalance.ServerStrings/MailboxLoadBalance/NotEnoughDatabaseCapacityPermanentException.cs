using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NotEnoughDatabaseCapacityPermanentException : MailboxLoadBalancePermanentException
	{
		public NotEnoughDatabaseCapacityPermanentException(string databaseGuid, string capacityType, long requestedCapacityUnits, long availableCapacityUnits) : base(MigrationWorkflowServiceStrings.ErrorNotEnoughDatabaseCapacity(databaseGuid, capacityType, requestedCapacityUnits, availableCapacityUnits))
		{
			this.databaseGuid = databaseGuid;
			this.capacityType = capacityType;
			this.requestedCapacityUnits = requestedCapacityUnits;
			this.availableCapacityUnits = availableCapacityUnits;
		}

		public NotEnoughDatabaseCapacityPermanentException(string databaseGuid, string capacityType, long requestedCapacityUnits, long availableCapacityUnits, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorNotEnoughDatabaseCapacity(databaseGuid, capacityType, requestedCapacityUnits, availableCapacityUnits), innerException)
		{
			this.databaseGuid = databaseGuid;
			this.capacityType = capacityType;
			this.requestedCapacityUnits = requestedCapacityUnits;
			this.availableCapacityUnits = availableCapacityUnits;
		}

		protected NotEnoughDatabaseCapacityPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseGuid = (string)info.GetValue("databaseGuid", typeof(string));
			this.capacityType = (string)info.GetValue("capacityType", typeof(string));
			this.requestedCapacityUnits = (long)info.GetValue("requestedCapacityUnits", typeof(long));
			this.availableCapacityUnits = (long)info.GetValue("availableCapacityUnits", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseGuid", this.databaseGuid);
			info.AddValue("capacityType", this.capacityType);
			info.AddValue("requestedCapacityUnits", this.requestedCapacityUnits);
			info.AddValue("availableCapacityUnits", this.availableCapacityUnits);
		}

		public string DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public string CapacityType
		{
			get
			{
				return this.capacityType;
			}
		}

		public long RequestedCapacityUnits
		{
			get
			{
				return this.requestedCapacityUnits;
			}
		}

		public long AvailableCapacityUnits
		{
			get
			{
				return this.availableCapacityUnits;
			}
		}

		private readonly string databaseGuid;

		private readonly string capacityType;

		private readonly long requestedCapacityUnits;

		private readonly long availableCapacityUnits;
	}
}
