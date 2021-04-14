using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DatabaseAvailabilityGroupNetworkSubnet
	{
		public DatabaseAvailabilityGroupNetworkSubnet.SubnetState State { get; set; }

		public DatabaseAvailabilityGroupSubnetId SubnetId { get; set; }

		internal DatabaseAvailabilityGroupNetworkSubnet(DatabaseAvailabilityGroupSubnetId netId)
		{
			this.State = DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Unknown;
			this.SubnetId = netId;
		}

		internal DatabaseAvailabilityGroupNetworkSubnet()
		{
		}

		public override bool Equals(object obj)
		{
			DatabaseAvailabilityGroupNetworkSubnet databaseAvailabilityGroupNetworkSubnet = obj as DatabaseAvailabilityGroupNetworkSubnet;
			return databaseAvailabilityGroupNetworkSubnet != null && this.SubnetId.IPRange.Equals(databaseAvailabilityGroupNetworkSubnet.SubnetId.IPRange);
		}

		public override int GetHashCode()
		{
			return this.SubnetId.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{{{0},{1}}}", this.SubnetId.ToString(), this.State.ToString());
		}

		public enum SubnetState
		{
			[LocDescription(DataStrings.IDs.Unknown)]
			Unknown,
			[LocDescription(DataStrings.IDs.Up)]
			Up,
			[LocDescription(DataStrings.IDs.Down)]
			Down,
			[LocDescription(DataStrings.IDs.Partitioned)]
			Partitioned,
			[LocDescription(DataStrings.IDs.Misconfigured)]
			Misconfigured,
			[LocDescription(DataStrings.IDs.Unavailable)]
			Unavailable
		}
	}
}
