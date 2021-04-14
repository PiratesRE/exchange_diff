using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyTipsData : SerializableDataBase, IEquatable<PolicyTipsData>
	{
		[DataMember]
		public bool IsPolicyTipsEnabled { get; set; }

		public bool Equals(PolicyTipsData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || this.IsPolicyTipsEnabled.Equals(other.IsPolicyTipsEnabled));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as PolicyTipsData);
		}

		protected override int InternalGetHashCode()
		{
			return this.IsPolicyTipsEnabled.GetHashCode();
		}
	}
}
