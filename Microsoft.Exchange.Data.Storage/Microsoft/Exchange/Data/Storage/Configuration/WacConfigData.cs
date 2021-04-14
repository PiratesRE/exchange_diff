using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WacConfigData : SerializableDataBase, IEquatable<WacConfigData>
	{
		[DataMember]
		public string[] WacViewableFileTypes { get; set; }

		[DataMember]
		public string[] WacEditableFileTypes { get; set; }

		[DataMember]
		public bool IsWacEditingEnabled { get; set; }

		[DataMember]
		public bool WacDiscoverySucceeded { get; set; }

		[DataMember]
		public string OneDriveDocumentsUrl { get; set; }

		[DataMember]
		public string OneDriveDocumentsDisplayName { get; set; }

		[DataMember]
		public bool OneDriveDiscoverySucceeded { get; set; }

		public WacConfigData()
		{
			this.WacViewableFileTypes = new string[0];
			this.WacEditableFileTypes = new string[0];
			this.IsWacEditingEnabled = false;
			this.OneDriveDocumentsUrl = string.Empty;
			this.OneDriveDocumentsDisplayName = string.Empty;
			this.OneDriveDiscoverySucceeded = false;
		}

		public bool Equals(WacConfigData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || (SerializableDataBase.ArrayContentsEquals<string>(this.WacViewableFileTypes, other.WacViewableFileTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.WacEditableFileTypes, other.WacEditableFileTypes) && this.IsWacEditingEnabled == other.IsWacEditingEnabled && this.WacDiscoverySucceeded == other.WacDiscoverySucceeded && string.CompareOrdinal(this.OneDriveDocumentsUrl, other.OneDriveDocumentsUrl) == 0 && string.CompareOrdinal(this.OneDriveDocumentsDisplayName, other.OneDriveDocumentsDisplayName) == 0 && this.OneDriveDiscoverySucceeded == other.OneDriveDiscoverySucceeded));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as WacConfigData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.WacViewableFileTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.WacEditableFileTypes));
			num = (num * 397 ^ this.OneDriveDocumentsUrl.GetHashCode());
			num = (num * 397 ^ this.OneDriveDocumentsDisplayName.GetHashCode());
			num <<= 3;
			num |= (this.IsWacEditingEnabled ? 4 : 0);
			num |= (this.WacDiscoverySucceeded ? 2 : 0);
			return num | (this.OneDriveDiscoverySucceeded ? 1 : 0);
		}
	}
}
