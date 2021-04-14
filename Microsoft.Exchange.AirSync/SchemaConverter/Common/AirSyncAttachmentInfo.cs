using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class AirSyncAttachmentInfo : IEquatable<AirSyncAttachmentInfo>
	{
		public AttachmentId AttachmentId { get; set; }

		public bool IsInline { get; set; }

		public string ContentId { get; set; }

		public override bool Equals(object obj)
		{
			AirSyncAttachmentInfo other = obj as AirSyncAttachmentInfo;
			return this.Equals(other);
		}

		public bool Equals(AirSyncAttachmentInfo other)
		{
			return this.AttachmentId != null && (this.AttachmentId.Equals(other.AttachmentId) && this.IsInline == other.IsInline) && string.Compare(this.ContentId, other.ContentId, StringComparison.Ordinal) == 0;
		}

		public override int GetHashCode()
		{
			return ((this.AttachmentId == null) ? 0 : this.AttachmentId.GetHashCode()) ^ this.IsInline.GetHashCode() ^ this.ContentId.GetHashCode();
		}
	}
}
