using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.UnifiedContent
{
	internal interface IExtractedContent
	{
		string FileName { get; }

		TextExtractionStatus TextExtractionStatus { get; }

		int RefId { get; }

		Dictionary<string, object> Properties { get; }

		IList<IExtractedContent> GetChildren();

		Stream GetContentReadStream();

		bool IsModified(Stream rawStream);

		bool IsModified(uint hash);
	}
}
