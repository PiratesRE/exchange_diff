using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	public class Body
	{
		internal Body(EmailMessage message)
		{
			this.Message = message;
		}

		public BodyFormat BodyFormat
		{
			get
			{
				return this.Message.Body_GetBodyFormat();
			}
		}

		public bool ConversionNeeded(int[] validCodepages)
		{
			return this.Message.Body_ConversionNeeded(validCodepages);
		}

		public string CharsetName
		{
			get
			{
				if (this.BodyFormat == BodyFormat.None)
				{
					return Charset.DefaultMimeCharset.Name;
				}
				return this.Message.Body_GetCharsetName();
			}
		}

		public MimePart MimePart
		{
			get
			{
				if (this.BodyFormat == BodyFormat.None)
				{
					return null;
				}
				return this.Message.Body_GetMimePart();
			}
		}

		public Stream GetContentReadStream()
		{
			if (this.BodyFormat == BodyFormat.None)
			{
				return DataStorage.NewEmptyReadStream();
			}
			return this.Message.Body_GetContentReadStream();
		}

		public Stream GetContentReadStreamOrNull()
		{
			if (this.BodyFormat == BodyFormat.None)
			{
				return null;
			}
			return this.Message.Body_GetContentReadStream();
		}

		public bool TryGetContentReadStream(out Stream stream)
		{
			if (this.BodyFormat == BodyFormat.None)
			{
				stream = DataStorage.NewEmptyReadStream();
				return true;
			}
			return this.Message.Body_TryGetContentReadStream(out stream);
		}

		public Stream GetContentWriteStream()
		{
			if (this.BodyFormat == BodyFormat.None)
			{
				throw new InvalidOperationException(EmailMessageStrings.CannotWriteBodyDoesNotExist);
			}
			return this.Message.Body_GetContentWriteStream(null);
		}

		public Stream GetContentWriteStream(string charsetName)
		{
			BodyFormat bodyFormat = this.BodyFormat;
			if (bodyFormat == BodyFormat.None)
			{
				throw new InvalidOperationException(EmailMessageStrings.CannotWriteBodyDoesNotExist);
			}
			if (BodyFormat.Rtf == bodyFormat)
			{
				throw new InvalidOperationException(EmailMessageStrings.NotSupportedForRtfBody);
			}
			Charset charset;
			Charset.TryGetCharset(this.CharsetName, out charset);
			Charset charset2 = Charset.GetCharset(charsetName);
			bool flag = charset == null || charset.CodePage != charset2.CodePage;
			if (flag)
			{
				charset2.GetEncoding();
			}
			return this.Message.Body_GetContentWriteStream(charset2);
		}

		internal EmailMessage Message;
	}
}
