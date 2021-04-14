using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class OwaOrgConfigData : SerializableDataBase, IEquatable<OwaOrgConfigData>
	{
		[DataMember]
		public uint MailTipsLargeAudienceThreshold { get; set; }

		[DataMember]
		public bool PublicComputersDetectionEnabled { get; set; }

		public bool Equals(OwaOrgConfigData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || (this.MailTipsLargeAudienceThreshold.Equals(other.MailTipsLargeAudienceThreshold) && this.PublicComputersDetectionEnabled.Equals(other.PublicComputersDetectionEnabled)));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as OwaOrgConfigData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ this.MailTipsLargeAudienceThreshold.GetHashCode());
			return num * 397 ^ this.PublicComputersDetectionEnabled.GetHashCode();
		}
	}
}
