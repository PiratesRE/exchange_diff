using System;
using System.Threading;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class PartitionId : IEquatable<PartitionId>
	{
		internal PartitionId(string fqdn)
		{
			this.ValidatePartitionFqdn(fqdn);
			this.forestFQDN = fqdn;
		}

		internal PartitionId(Guid partitionObjectId)
		{
			if (partitionObjectId == Guid.Empty)
			{
				throw new ArgumentNullException("partitionObjectId");
			}
			this.forestFQDN = ADAccountPartitionLocator.GetAccountPartitionFqdnByPartitionGuid(partitionObjectId);
			this.ValidatePartitionFqdn(this.forestFQDN);
			this.partitionObjectId = new Guid?(partitionObjectId);
		}

		internal PartitionId(string fqdn, Guid partitionObjectId) : this(fqdn)
		{
			if (partitionObjectId == Guid.Empty)
			{
				throw new ArgumentNullException("partitionObjectId");
			}
			this.partitionObjectId = new Guid?(partitionObjectId);
		}

		internal PartitionId(ADObjectId adObjectId)
		{
			if (string.IsNullOrEmpty(adObjectId.PartitionFQDN))
			{
				throw new ArgumentException("adObjectId");
			}
			this.forestFQDN = adObjectId.PartitionFQDN;
		}

		internal static bool TryParse(string input, out PartitionId partitionId)
		{
			partitionId = null;
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			string[] array = input.Split(new char[]
			{
				':'
			});
			if (array.Length != 2)
			{
				return false;
			}
			Guid? guid = null;
			string text = null;
			Guid value;
			if (!array[0].Equals("<null>", StringComparison.OrdinalIgnoreCase) && Guid.TryParse(array[0], out value))
			{
				guid = new Guid?(value);
			}
			if (!array[1].Equals("<null>", StringComparison.OrdinalIgnoreCase))
			{
				text = array[1];
			}
			try
			{
				if (guid != null && text != null)
				{
					partitionId = new PartitionId(text, guid.Value);
				}
				else if (guid != null)
				{
					partitionId = new PartitionId(guid.Value);
				}
				else
				{
					if (text == null)
					{
						return false;
					}
					partitionId = new PartitionId(text);
				}
				return true;
			}
			catch (ArgumentException)
			{
			}
			return false;
		}

		public static bool operator ==(PartitionId partition1, PartitionId partition2)
		{
			return (partition1 == null && partition2 == null) || (partition1 != null && partition2 != null && string.Equals(partition1.ForestFQDN.Trim(), partition2.ForestFQDN.Trim(), StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsLocalForestPartition(string partitionFqdn)
		{
			return partitionFqdn.Equals(TopologyProvider.LocalForestFqdn, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator !=(PartitionId partition1, PartitionId partition2)
		{
			return !(partition1 == partition2);
		}

		public static PartitionId LocalForest
		{
			get
			{
				if (PartitionId.localForest == null)
				{
					Interlocked.CompareExchange<PartitionId>(ref PartitionId.localForest, new PartitionId(TopologyProvider.LocalForestFqdn), null);
				}
				return PartitionId.localForest;
			}
		}

		public static bool TryParse(string fqdn, out PartitionId partitionId, out Exception ex)
		{
			ex = null;
			partitionId = null;
			bool result;
			try
			{
				partitionId = new PartitionId(fqdn);
				result = true;
			}
			catch (Exception ex2)
			{
				ex = ex2;
				result = false;
			}
			return result;
		}

		public bool IsLocalForestPartition()
		{
			return PartitionId.IsLocalForestPartition(this.ForestFQDN);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", (this.PartitionObjectId != null) ? this.PartitionObjectId.Value.ToString() : "<null>", this.ForestFQDN ?? "<null>");
		}

		public bool Equals(PartitionId partitionId)
		{
			return partitionId != null && partitionId == this;
		}

		public override bool Equals(object obj)
		{
			PartitionId partitionId = obj as PartitionId;
			return partitionId != null && this.Equals(partitionId);
		}

		public override int GetHashCode()
		{
			return this.forestFQDN.GetHashCode();
		}

		internal string ForestFQDN
		{
			get
			{
				return this.forestFQDN;
			}
		}

		internal Guid? PartitionObjectId
		{
			get
			{
				return this.partitionObjectId;
			}
		}

		internal void ValidatePartitionFqdn(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			if (!Fqdn.IsValidFqdn(fqdn.Trim()))
			{
				throw new ArgumentException(string.Format("Invalid fqdn parameter value: '{0}'", fqdn.Trim()));
			}
			if (Datacenter.IsMicrosoftHostedOnly(true) && !Datacenter.IsDatacenterDedicated(true) && !PartitionId.IsLocalForestPartition(fqdn) && !fqdn.EndsWith("outlook.com", StringComparison.OrdinalIgnoreCase) && !fqdn.EndsWith("exchangelabs.com", StringComparison.OrdinalIgnoreCase) && !fqdn.EndsWith("outlook.cn", StringComparison.OrdinalIgnoreCase) && !fqdn.EndsWith("extest.microsoft.com", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(DirectoryStrings.InvalidPartitionFqdn(fqdn));
			}
		}

		private readonly string forestFQDN;

		private readonly Guid? partitionObjectId;

		private static PartitionId localForest;
	}
}
