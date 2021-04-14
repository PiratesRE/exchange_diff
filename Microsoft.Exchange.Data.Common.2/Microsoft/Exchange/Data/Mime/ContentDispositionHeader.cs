using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class ContentDispositionHeader : ComplexHeader
	{
		public ContentDispositionHeader() : base("Content-Disposition", HeaderId.ContentDisposition)
		{
			this.disp = "attachment";
			this.parsed = true;
		}

		public ContentDispositionHeader(string value) : base("Content-Disposition", HeaderId.ContentDisposition)
		{
			this.Value = value;
		}

		public sealed override string Value
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.disp;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (ValueParser.ParseToken(value, 0, false) != value.Length)
				{
					throw new ArgumentException("Value should be a valid token", "value");
				}
				if (!this.parsed)
				{
					base.Parse();
				}
				if (value != this.disp)
				{
					base.SetRawValue(null, true);
					this.parsed = true;
					this.disp = Header.NormalizeString(value);
				}
			}
		}

		internal override byte[] RawValue
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				byte[] array = ByteString.StringToBytes(this.disp, false);
				if (array == null)
				{
					array = MimeString.EmptyByteArray;
				}
				return array;
			}
			set
			{
				base.RawValue = value;
			}
		}

		internal override void RawValueAboutToChange()
		{
			this.Reset();
		}

		public sealed override MimeNode Clone()
		{
			ContentDispositionHeader contentDispositionHeader = new ContentDispositionHeader();
			this.CopyTo(contentDispositionHeader);
			return contentDispositionHeader;
		}

		public sealed override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			ContentDispositionHeader contentDispositionHeader = destination as ContentDispositionHeader;
			if (contentDispositionHeader == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType, "destination");
			}
			base.CopyTo(destination);
			contentDispositionHeader.parsed = this.parsed;
			contentDispositionHeader.disp = this.disp;
		}

		public sealed override bool IsValueValid(string value)
		{
			return value != null && ValueParser.ParseToken(value, 0, false) == value.Length;
		}

		internal override long WriteValue(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			if (this.disp.Length == 0)
			{
				this.disp = "attachment";
			}
			return base.WriteValue(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
		}

		internal override void ParseValue(ValueParser parser, bool storeValue)
		{
			MimeStringList mimeStringList = default(MimeStringList);
			parser.ParseCFWS(false, ref mimeStringList, true);
			MimeString mimeString = parser.ParseToken();
			if (storeValue)
			{
				if (mimeString.Length == 0)
				{
					this.disp = string.Empty;
					return;
				}
				this.disp = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
			}
		}

		internal override void AppendLine(MimeString line, bool markDirty)
		{
			if (this.parsed)
			{
				this.Reset();
			}
			base.AppendLine(line, markDirty);
		}

		private void Reset()
		{
			base.InternalRemoveAll();
			this.parsed = false;
			this.disp = null;
		}

		internal const bool AllowUTF8Value = false;

		private const string DefaultDisposition = "attachment";

		private string disp;
	}
}
