using System;
using System.IO;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal interface IMimeHandler
	{
		void PartStart(bool isInline, string inlineFileName, out PartParseOption partParseOption, out Stream outerContentWriteStream);

		void HeaderStart(HeaderId headerId, string name, out HeaderParseOption headerParseOption);

		void Header(Header header);

		void EndOfHeaders(string mediaType, ContentTransferEncoding cte, out PartContentParseOption partContentParseOption);

		void PartContent(byte[] buffer, int offset, int length);

		void PartEnd();

		void EndOfFile();
	}
}
