using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.UnifiedContent
{
	internal class ExtractedContentEntry
	{
		internal ExtractedContentEntry(Stream entryStream, long entryPosition)
		{
			this.Properties = new Dictionary<string, object>();
			using (SharedContentReader sharedContentReader = new SharedContentReader(entryStream))
			{
				this.EntryPos = entryPosition;
				sharedContentReader.ValidateEntryId(572662306U);
				this.ParentPos = sharedContentReader.ReadInt64();
				this.NextSiblingPos = sharedContentReader.ReadInt64();
				this.FirstChildPos = sharedContentReader.ReadInt64();
				this.FileName = sharedContentReader.ReadString();
				this.Properties.Add("Parsing::ParsingKeys::Subject", sharedContentReader.ReadString());
				this.TextExtractionStatus = sharedContentReader.ReadUInt32();
				this.RefId = sharedContentReader.ReadUInt32();
				this.TextStream = sharedContentReader.ReadStream();
				sharedContentReader.ValidateAtEndOfEntry();
			}
		}

		internal long EntryPos { get; private set; }

		internal long ParentPos { get; private set; }

		internal long FirstChildPos { get; private set; }

		internal long NextSiblingPos { get; private set; }

		internal string FileName { get; private set; }

		internal uint TextExtractionStatus { get; private set; }

		internal uint RefId { get; private set; }

		internal Stream TextStream { get; private set; }

		internal Dictionary<string, object> Properties { get; private set; }

		public const string Subject = "Parsing::ParsingKeys::Subject";

		private const uint EntryId = 572662306U;
	}
}
