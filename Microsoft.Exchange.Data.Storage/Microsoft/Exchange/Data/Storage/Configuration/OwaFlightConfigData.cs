using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OwaFlightConfigData : SerializableDataBase, IEquatable<OwaFlightConfigData>
	{
		[DataMember]
		public string RampId { get; set; }

		[DataMember]
		public bool IsFirstRelease { get; set; }

		public bool Equals(OwaFlightConfigData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || (this.RampId == other.RampId && this.IsFirstRelease == other.IsFirstRelease));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as OwaFlightConfigData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ this.IsFirstRelease.GetHashCode());
			return num * 397 ^ (this.RampId ?? string.Empty).GetHashCode();
		}
	}
}
