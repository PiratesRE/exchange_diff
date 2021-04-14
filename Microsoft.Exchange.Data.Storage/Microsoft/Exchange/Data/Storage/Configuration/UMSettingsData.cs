using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMSettingsData : SerializableDataBase, IEquatable<UMSettingsData>
	{
		[DataMember]
		public string PlayOnPhoneDialString { get; set; }

		[DataMember]
		public bool IsRequireProtectedPlayOnPhone { get; set; }

		[DataMember]
		public bool IsUMEnabled { get; set; }

		public bool Equals(UMSettingsData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || (this.IsRequireProtectedPlayOnPhone == other.IsRequireProtectedPlayOnPhone && this.IsUMEnabled == other.IsUMEnabled && this.PlayOnPhoneDialString == other.PlayOnPhoneDialString));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as UMSettingsData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ this.IsUMEnabled.GetHashCode());
			num = (num * 397 ^ this.IsRequireProtectedPlayOnPhone.GetHashCode());
			return num * 397 ^ (this.PlayOnPhoneDialString ?? string.Empty).GetHashCode();
		}
	}
}
