using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class Attachment16Data : Attachment14Data
	{
		public Attachment16Data(AttachmentAction changeType)
		{
			this.ChangeType = changeType;
			this.ContentType = string.Empty;
			this.ClientId = null;
			this.Content = null;
		}

		public AttachmentAction ChangeType { get; set; }

		public string ContentType { get; set; }

		public string ClientId { get; set; }

		public byte[] Content { get; set; }
	}
}
