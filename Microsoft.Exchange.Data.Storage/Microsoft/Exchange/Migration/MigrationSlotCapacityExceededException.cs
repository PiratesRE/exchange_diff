using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationSlotCapacityExceededException : MigrationTransientException
	{
		public MigrationSlotCapacityExceededException(Unlimited<int> availableCapacity, int requestedCapacity) : base(Strings.ErrorMigrationSlotCapacityExceeded(availableCapacity, requestedCapacity))
		{
			this.availableCapacity = availableCapacity;
			this.requestedCapacity = requestedCapacity;
		}

		public MigrationSlotCapacityExceededException(Unlimited<int> availableCapacity, int requestedCapacity, Exception innerException) : base(Strings.ErrorMigrationSlotCapacityExceeded(availableCapacity, requestedCapacity), innerException)
		{
			this.availableCapacity = availableCapacity;
			this.requestedCapacity = requestedCapacity;
		}

		protected MigrationSlotCapacityExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.availableCapacity = (Unlimited<int>)info.GetValue("availableCapacity", typeof(Unlimited<int>));
			this.requestedCapacity = (int)info.GetValue("requestedCapacity", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("availableCapacity", this.availableCapacity);
			info.AddValue("requestedCapacity", this.requestedCapacity);
		}

		public Unlimited<int> AvailableCapacity
		{
			get
			{
				return this.availableCapacity;
			}
		}

		public int RequestedCapacity
		{
			get
			{
				return this.requestedCapacity;
			}
		}

		private readonly Unlimited<int> availableCapacity;

		private readonly int requestedCapacity;
	}
}
