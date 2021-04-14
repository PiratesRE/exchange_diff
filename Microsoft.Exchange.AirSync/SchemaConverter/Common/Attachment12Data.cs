using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class Attachment12Data
	{
		public string ContentId { get; set; }

		public string ContentLocation { get; set; }

		public string DisplayName { get; set; }

		public long EstimatedDataSize { get; set; }

		public string FileReference { get; set; }

		public bool IsInline { get; set; }

		public byte Method { get; set; }

		public AttachmentId Id { get; set; }
	}
}
