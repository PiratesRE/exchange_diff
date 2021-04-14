using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal sealed class AcquireToken : IEquatable<AcquireToken>
	{
		public static bool operator ==(AcquireToken obj1, AcquireToken obj2)
		{
			return object.ReferenceEquals(obj1, obj2) || (!object.ReferenceEquals(obj1, null) && !object.ReferenceEquals(obj2, null) && obj1.tokenId.Equals(obj2.tokenId));
		}

		public static bool operator !=(AcquireToken obj1, AcquireToken obj2)
		{
			return !(obj1 == obj2);
		}

		public bool Equals(AcquireToken other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as AcquireToken);
		}

		public override string ToString()
		{
			return this.tokenId.ToString();
		}

		public override int GetHashCode()
		{
			return this.tokenId.GetHashCode();
		}

		private readonly Guid tokenId = Guid.NewGuid();
	}
}
