using System;
using System.IO;

namespace Microsoft.Exchange.Data.Mime
{
	internal interface IMimeHandlerInternal
	{
		void PartStart(bool isInline, string inlineFileName, out PartParseOptionInternal partParseOption, out Stream outerContentWriteStream);

		void HeaderStart(HeaderId headerId, string name, out HeaderParseOptionInternal headerParseOption);

		void Header(Header header);

		void EndOfHeaders(string mediaType, ContentTransferEncoding cte, out PartContentParseOptionInternal partContentParseOption);

		void PartContent(byte[] buffer, int offset, int length);

		void PartEnd();

		void EndOfFile();
	}
}
