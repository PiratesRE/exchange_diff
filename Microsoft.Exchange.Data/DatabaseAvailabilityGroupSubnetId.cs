using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DatabaseAvailabilityGroupSubnetId
	{
		public DatabaseAvailabilityGroupSubnetId(string expression)
		{
			this.m_ipRange = IPRange.Parse(expression);
		}

		public DatabaseAvailabilityGroupSubnetId(IPRange ipRange)
		{
			this.m_ipRange = ipRange;
		}

		public DatabaseAvailabilityGroupSubnetId(DatabaseAvailabilityGroupNetworkSubnet subnet)
		{
			this.m_ipRange = subnet.SubnetId.IPRange;
		}

		public static bool TryParse(string expression, out DatabaseAvailabilityGroupSubnetId subnetId)
		{
			subnetId = null;
			IPRange ipRange = null;
			if (IPRange.TryParse(expression, out ipRange))
			{
				subnetId = new DatabaseAvailabilityGroupSubnetId(ipRange);
				return true;
			}
			return false;
		}

		public static DatabaseAvailabilityGroupSubnetId Parse(string expression)
		{
			IPRange ipRange = IPRange.Parse(expression);
			return new DatabaseAvailabilityGroupSubnetId(ipRange);
		}

		public IPRange IPRange
		{
			get
			{
				return this.m_ipRange;
			}
		}

		public override string ToString()
		{
			return this.m_ipRange.ToString();
		}

		public static bool Equals(DatabaseAvailabilityGroupSubnetId s1, DatabaseAvailabilityGroupSubnetId s2)
		{
			int num = DagSubnetIdComparer.Comparer.Compare(s1, s2);
			return num == 0;
		}

		public bool Equals(DatabaseAvailabilityGroupSubnetId other)
		{
			return DatabaseAvailabilityGroupSubnetId.Equals(this, other);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as DatabaseAvailabilityGroupSubnetId);
		}

		public override int GetHashCode()
		{
			return this.m_ipRange.GetHashCode();
		}

		private IPRange m_ipRange;
	}
}
