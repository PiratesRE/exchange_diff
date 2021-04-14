using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal interface IBody
	{
		BodyFormat GetBodyFormat();

		string GetCharsetName();

		MimePart GetMimePart();

		Stream GetContentReadStream();

		bool TryGetContentReadStream(out Stream stream);

		Stream GetContentWriteStream(Charset charset);

		void SetNewContent(DataStorage storage, long start, long end);

		bool ConversionNeeded(int[] validCodepages);
	}
}
