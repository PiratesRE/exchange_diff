using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	public class TextHeader : Header
	{
		public TextHeader(string name, string value) : this(name, Header.GetHeaderId(name, true))
		{
			Type type = Header.TypeFromHeaderId(base.HeaderId);
			if (base.HeaderId != HeaderId.Unknown && type != typeof(TextHeader) && type != typeof(AsciiTextHeader))
			{
				throw new ArgumentException(Strings.NameNotValidForThisHeaderType(name, "TextHeader", type.Name));
			}
			this.Value = value;
		}

		internal TextHeader(string name, HeaderId headerId) : base(name, headerId)
		{
		}

		public sealed override string Value
		{
			get
			{
				if (this.decodedValue == null)
				{
					DecodingResults decodingResults;
					string text = this.GetDecodedValue(base.GetHeaderDecodingOptions(), out decodingResults);
					if (decodingResults.DecodingFailed)
					{
						MimeCommon.ThrowDecodingFailedException(ref decodingResults);
					}
					this.decodedValue = text;
				}
				return this.decodedValue;
			}
			set
			{
				base.SetRawValue(null, true);
				this.decodedValue = value;
			}
		}

		internal override byte[] RawValue
		{
			get
			{
				MimeStringList mimeStringList;
				if (base.RawLength == 0 && this.decodedValue != null && this.decodedValue.Length != 0)
				{
					mimeStringList = this.GetEncodedValue(base.GetDocumentEncodingOptions(), ValueEncodingStyle.Normal);
				}
				else
				{
					mimeStringList = base.Lines;
				}
				if (mimeStringList.Length == 0)
				{
					return MimeString.EmptyByteArray;
				}
				byte[] array = mimeStringList.GetSz();
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

		internal override void ForceParse()
		{
			string value = this.Value;
		}

		internal override void RawValueAboutToChange()
		{
			this.decodedValue = null;
		}

		public sealed override MimeNode Clone()
		{
			TextHeader textHeader = new TextHeader(base.Name, base.HeaderId);
			this.CopyTo(textHeader);
			return textHeader;
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
			TextHeader textHeader = destination as TextHeader;
			if (textHeader == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			textHeader.decodedValue = this.decodedValue;
		}

		public override bool TryGetValue(out string value)
		{
			DecodingResults decodingResults;
			return this.TryGetValue(base.GetHeaderDecodingOptions(), out decodingResults, out value);
		}

		public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
		{
			value = this.GetDecodedValue(decodingOptions, out decodingResults);
			if (decodingResults.DecodingFailed)
			{
				value = null;
				return false;
			}
			return true;
		}

		internal string GetDecodedValue(DecodingOptions decodingOptions, out DecodingResults decodingResults)
		{
			string result = null;
			if (base.Lines.Length == 0)
			{
				result = ((this.decodedValue != null) ? this.decodedValue : string.Empty);
				decodingResults = default(DecodingResults);
				return result;
			}
			if (decodingOptions.Charset == null)
			{
				decodingOptions.Charset = base.GetDefaultHeaderDecodingCharset(null, null);
			}
			if (!MimeCommon.TryDecodeValue(base.Lines, 4026531840U, decodingOptions, out decodingResults, out result))
			{
				result = null;
			}
			return result;
		}

		internal MimeStringList GetEncodedValue(EncodingOptions encodingOptions, ValueEncodingStyle encodingStyle)
		{
			if (string.IsNullOrEmpty(this.decodedValue))
			{
				return base.Lines;
			}
			return MimeCommon.EncodeValue(this.decodedValue, encodingOptions, encodingStyle);
		}

		public sealed override bool IsValueValid(string value)
		{
			return true;
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			long num = base.WriteName(stream, ref scratchBuffer);
			currentLineLength.IncrementBy((int)num);
			MimeStringList mimeStringList;
			if (base.RawLength == 0 && this.decodedValue != null && this.decodedValue.Length != 0)
			{
				mimeStringList = this.GetEncodedValue(encodingOptions, ValueEncodingStyle.Normal);
			}
			else if ((byte)(EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != 0)
			{
				this.ForceParse();
				mimeStringList = this.GetEncodedValue(encodingOptions, ValueEncodingStyle.Normal);
			}
			else
			{
				bool flag = false;
				if (!base.IsDirty && base.RawLength != 0)
				{
					if (base.IsProtected)
					{
						num += Header.WriteLines(base.Lines, stream);
						currentLineLength.SetAs(0);
						return num;
					}
					if (!base.IsHeaderLineTooLong(num, out flag))
					{
						num += Header.WriteLines(base.Lines, stream);
						currentLineLength.SetAs(0);
						return num;
					}
				}
				mimeStringList = base.Lines;
				if (flag)
				{
					mimeStringList = Header.MergeLines(mimeStringList);
				}
			}
			num += Header.QuoteAndFold(stream, mimeStringList, 4026531840U, false, mimeStringList.Length > 0, encodingOptions.AllowUTF8, 0, ref currentLineLength, ref scratchBuffer);
			return num + Header.WriteLineEnd(stream, ref currentLineLength);
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			throw new NotSupportedException(Strings.ChildrenCannotBeAddedToTextHeader);
		}

		private string decodedValue;
	}
}
