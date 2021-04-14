using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class OwaConfigurationBaseData : SerializableDataBase, IEquatable<OwaConfigurationBaseData>
	{
		[DataMember]
		public OwaAttachmentPolicyData AttachmentPolicy { get; set; }

		[DataMember]
		public ulong SegmentationFlags { get; set; }

		[DataMember]
		public string DefaultTheme { get; set; }

		[DataMember]
		public bool UseGB18030 { get; set; }

		[DataMember]
		public bool UseISO885915 { get; set; }

		[DataMember]
		public string OutboundCharset { get; set; }

		[DataMember]
		public string InstantMessagingType { get; set; }

		[DataMember]
		public bool InstantMessagingEnabled { get; set; }

		[DataMember]
		public bool PlacesEnabled { get; set; }

		[DataMember]
		public bool WeatherEnabled { get; set; }

		[DataMember]
		public bool AllowCopyContactsToDeviceAddressBook { get; set; }

		[DataMember]
		public string AllowOfflineOn { get; set; }

		[DataMember]
		public bool RecoverDeletedItemsEnabled { get; set; }

		public bool Equals(OwaConfigurationBaseData other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (this.AttachmentPolicy != null)
			{
				return this.AttachmentPolicy.Equals(other.AttachmentPolicy) && this.SegmentationFlags == other.SegmentationFlags && this.DefaultTheme == other.DefaultTheme && this.UseGB18030 == other.UseGB18030 && this.UseISO885915 == other.UseISO885915 && this.OutboundCharset == other.OutboundCharset && this.InstantMessagingType == other.InstantMessagingType && this.InstantMessagingEnabled == other.InstantMessagingEnabled && this.PlacesEnabled == other.PlacesEnabled && this.WeatherEnabled == other.WeatherEnabled && this.AllowCopyContactsToDeviceAddressBook == other.AllowCopyContactsToDeviceAddressBook && this.AllowOfflineOn == other.AllowOfflineOn && this.RecoverDeletedItemsEnabled == other.RecoverDeletedItemsEnabled;
			}
			return null == other.AttachmentPolicy;
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as OwaConfigurationBaseData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ ((this.AttachmentPolicy == null) ? 0 : this.AttachmentPolicy.GetHashCode()));
			num = (num * 397 ^ this.SegmentationFlags.GetHashCode());
			num = (num * 397 ^ ((this.DefaultTheme == null) ? 0 : this.DefaultTheme.GetHashCode()));
			num = (num * 397 ^ this.UseGB18030.GetHashCode());
			num = (num * 397 ^ this.UseISO885915.GetHashCode());
			num = (num * 397 ^ ((this.OutboundCharset == null) ? 0 : this.OutboundCharset.GetHashCode()));
			num = (num * 397 ^ ((this.InstantMessagingType == null) ? 0 : this.InstantMessagingType.GetHashCode()));
			num = (num * 397 ^ this.InstantMessagingEnabled.GetHashCode());
			num = (num * 397 ^ this.PlacesEnabled.GetHashCode());
			num = (num * 397 ^ this.WeatherEnabled.GetHashCode());
			num = (num * 397 ^ this.AllowCopyContactsToDeviceAddressBook.GetHashCode());
			num = (num * 397 ^ ((this.AllowOfflineOn == null) ? 0 : this.AllowOfflineOn.GetHashCode()));
			return num * 397 ^ this.RecoverDeletedItemsEnabled.GetHashCode();
		}
	}
}
