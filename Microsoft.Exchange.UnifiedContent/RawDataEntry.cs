using System;
using System.IO;

namespace Microsoft.Exchange.UnifiedContent
{
	internal class RawDataEntry
	{
		internal RawDataEntry(Stream entryStream, long entryPosition)
		{
			using (SharedContentReader sharedContentReader = new SharedContentReader(entryStream))
			{
				this.entryPosition = entryPosition;
				sharedContentReader.ValidateEntryId(286331153U);
				this.extractedContentEntryPosition = sharedContentReader.ReadInt64();
				this.dataStream = sharedContentReader.ReadStream();
				sharedContentReader.ValidateAtEndOfEntry();
			}
		}

		internal long EntryPosition
		{
			get
			{
				return this.entryPosition;
			}
		}

		internal long ExtractedContentEntryPosition
		{
			get
			{
				return this.extractedContentEntryPosition;
			}
		}

		internal Stream DataStream
		{
			get
			{
				return this.dataStream;
			}
		}

		private const uint EntryId = 286331153U;

		private readonly long entryPosition;

		private readonly long extractedContentEntryPosition;

		private readonly Stream dataStream;
	}
}
