using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct OscNetworkMoniker : IEquatable<OscNetworkMoniker>
	{
		internal OscNetworkMoniker(Guid providerGuid, string networkId, string userId)
		{
			Util.ThrowOnNullOrEmptyArgument(userId, "userId");
			if (string.IsNullOrEmpty(networkId))
			{
				this.moniker = string.Format("{0}-{1}", OscNetworkMoniker.ToStringEnclosedByCurlyBracesLowerCase(providerGuid), userId).ToLowerInvariant();
				return;
			}
			this.moniker = string.Format("{0}-{1}-{2}", OscNetworkMoniker.ToStringEnclosedByCurlyBracesLowerCase(providerGuid), networkId, userId).ToLowerInvariant();
		}

		internal OscNetworkMoniker(string moniker)
		{
			Util.ThrowOnNullOrEmptyArgument(moniker, "moniker");
			this.moniker = moniker;
		}

		public bool Equals(OscNetworkMoniker other)
		{
			return this.moniker.Equals(other.moniker, StringComparison.OrdinalIgnoreCase);
		}

		public bool Equals(string other)
		{
			return this.moniker.Equals(other, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other is string)
			{
				return this.Equals((string)other);
			}
			return other is OscNetworkMoniker && this.Equals((OscNetworkMoniker)other);
		}

		public override int GetHashCode()
		{
			return this.moniker.GetHashCode();
		}

		public override string ToString()
		{
			return this.moniker.ToString();
		}

		private static string ToStringEnclosedByCurlyBracesLowerCase(Guid g)
		{
			return g.ToString("B", CultureInfo.InvariantCulture).ToLowerInvariant();
		}

		private readonly string moniker;
	}
}
