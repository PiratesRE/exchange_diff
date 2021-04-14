using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UnifiedContent;

namespace Microsoft.Filtering
{
	internal sealed class FileFipsDataStreamFilteringRequest : FipsDataStreamFilteringRequest
	{
		private FileFipsDataStreamFilteringRequest(string fileName, Stream fileStream, ContentManager contentManager) : base(Guid.NewGuid().ToString(), contentManager)
		{
			this.FileName = fileName;
			this.FileStream = fileStream;
			this.RecoveryOptions = RecoveryOptions.None;
		}

		public Stream FileStream { get; private set; }

		public string FileName { get; private set; }

		public override RecoveryOptions RecoveryOptions { get; set; }

		public static FileFipsDataStreamFilteringRequest CreateInstance(string fileName, Stream fileStream, ContentManager contentManager)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("fileName", fileName);
			ArgumentValidator.ThrowIfInvalidValue<Stream>("fileStream", fileStream, (Stream stream) => stream != null || stream.Length > 0L);
			ArgumentValidator.ThrowIfNull("contentManager", contentManager);
			return new FileFipsDataStreamFilteringRequest(fileName, fileStream, contentManager);
		}

		protected override void Serialize(UnifiedContentSerializer unifiedContentSerializer, bool bypassBodyTextTruncation = true)
		{
			unifiedContentSerializer.AddStream(UnifiedContentSerializer.EntryId.Attachment, this.FileStream, this.FileName);
		}
	}
}
