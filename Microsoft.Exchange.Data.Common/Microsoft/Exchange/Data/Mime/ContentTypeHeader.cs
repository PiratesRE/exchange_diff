using System;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class ContentTypeHeader : ComplexHeader
	{
		public ContentTypeHeader() : base("Content-Type", HeaderId.ContentType)
		{
			this.value = "text/plain";
			this.type = "text";
			this.subType = "plain";
			this.parsed = true;
		}

		public ContentTypeHeader(string value) : base("Content-Type", HeaderId.ContentType)
		{
			this.Value = value;
		}

		public string MediaType
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (ValueParser.ParseToken(value, 0, false) != value.Length)
				{
					throw new ArgumentException("Value should be valid MIME token", "value");
				}
				if (!this.parsed)
				{
					base.Parse();
				}
				if (value != this.type)
				{
					string text = this.subType;
					base.SetRawValue(null, true);
					this.parsed = true;
					this.type = Header.NormalizeString(value);
					this.subType = text;
					this.value = ContentTypeHeader.ComposeContentTypeValue(this.type, this.subType);
					if (this.type == "multipart")
					{
						this.EnsureBoundary();
					}
				}
			}
		}

		public string SubType
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.subType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (ValueParser.ParseToken(value, 0, false) != value.Length)
				{
					throw new ArgumentException("Value should be valid MIME token", "value");
				}
				if (!this.parsed)
				{
					base.Parse();
				}
				if (value != this.subType)
				{
					string text = this.type;
					base.SetRawValue(null, true);
					this.parsed = true;
					this.type = text;
					this.subType = Header.NormalizeString(value);
					this.value = ContentTypeHeader.ComposeContentTypeValue(this.type, this.subType);
				}
			}
		}

		public sealed override string Value
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.value;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int num = ValueParser.ParseToken(value, 0, false);
				if (num == 0 || num > value.Length - 2 || value[num] != '/' || ValueParser.ParseToken(value, num + 1, false) != value.Length - num - 1)
				{
					throw new ArgumentException("Value should be a valid content type in the form 'token/token'", "value");
				}
				if (!this.parsed)
				{
					base.Parse();
				}
				if (value != this.value)
				{
					base.SetRawValue(null, true);
					this.parsed = true;
					this.value = Header.NormalizeString(value);
					this.type = Header.NormalizeString(this.value, 0, num);
					this.subType = Header.NormalizeString(this.value, num + 1, this.value.Length - num - 1);
					if (this.type == "multipart")
					{
						this.EnsureBoundary();
					}
				}
			}
		}

		internal bool IsMultipart
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.type == "multipart";
			}
		}

		internal bool IsEmbeddedMessage
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.value == "message/rfc822";
			}
		}

		internal bool IsAnyMessage
		{
			get
			{
				if (!this.parsed)
				{
					base.Parse();
				}
				return this.type == "message";
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
				byte[] array = ByteString.StringToBytes(this.value, false);
				if (array == null)
				{
					array = MimeString.EmptyByteArray;
				}
				return array;
			}
			set
			{
				base.RawValue = value;
				base.Parse();
				if (this.type == "multipart")
				{
					this.EnsureBoundary();
				}
			}
		}

		public sealed override MimeNode Clone()
		{
			ContentTypeHeader contentTypeHeader = new ContentTypeHeader();
			this.CopyTo(contentTypeHeader);
			return contentTypeHeader;
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
			ContentTypeHeader contentTypeHeader = destination as ContentTypeHeader;
			if (contentTypeHeader == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType, "destination");
			}
			base.CopyTo(destination);
			contentTypeHeader.type = this.type;
			contentTypeHeader.subType = this.subType;
			contentTypeHeader.value = this.value;
			contentTypeHeader.parsed = this.parsed;
		}

		public sealed override bool IsValueValid(string value)
		{
			if (value == null)
			{
				return false;
			}
			int num = ValueParser.ParseToken(value, 0, false);
			return num != 0 && num <= value.Length - 2 && value[num] == '/' && ValueParser.ParseToken(value, num + 1, false) == value.Length - num - 1;
		}

		internal static byte[] CreateBoundary()
		{
			string text = Guid.NewGuid().ToString();
			byte[] array = new byte[text.Length + 2];
			array[0] = 95;
			ByteString.StringToBytes(text, array, 1, false);
			array[1 + text.Length] = 95;
			return array;
		}

		internal override void RawValueAboutToChange()
		{
			this.Reset();
		}

		internal override void ParseValue(ValueParser parser, bool storeValue)
		{
			MimeStringList empty = MimeStringList.Empty;
			parser.ParseCFWS(false, ref empty, true);
			MimeString mimeString = parser.ParseToken();
			MimeString mimeString2 = MimeString.Empty;
			parser.ParseCFWS(false, ref empty, true);
			byte b = parser.ParseGet();
			if (b == 47)
			{
				parser.ParseCFWS(false, ref empty, true);
				mimeString2 = parser.ParseToken();
			}
			else if (b != 0)
			{
				parser.ParseUnget();
			}
			if (storeValue)
			{
				if (mimeString.Length == 0)
				{
					this.type = "text";
				}
				else
				{
					this.type = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
				}
				if (mimeString2.Length == 0)
				{
					if (this.type == "multipart")
					{
						this.subType = "mixed";
					}
					else if (this.type == "text")
					{
						this.subType = "plain";
					}
					else
					{
						this.type = "application";
						this.subType = "octet-stream";
					}
				}
				else
				{
					this.subType = Header.NormalizeString(mimeString2.Data, mimeString2.Offset, mimeString2.Length, false);
				}
				this.value = ContentTypeHeader.ComposeContentTypeValue(this.type, this.subType);
			}
			if (this.type == "multipart")
			{
				this.handleISO2022 = false;
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

		private static string ComposeContentTypeValue(string type, string subType)
		{
			int num = type.Length + 1 + subType.Length;
			if (num >= 2 && num <= 32)
			{
				int num2 = 0;
				num2 = MimeData.HashValueAdd(num2, type);
				num2 = MimeData.HashValueAdd(num2, "/");
				num2 = MimeData.HashValueAdd(num2, subType);
				num2 = MimeData.HashValueFinish(num2);
				int num3 = MimeData.valueHashTable[num2];
				if (num3 > 0)
				{
					string text;
					for (;;)
					{
						text = MimeData.values[num3].value;
						if (text.Length == num && text.StartsWith(type, StringComparison.OrdinalIgnoreCase) && text[type.Length] == '/' && text.EndsWith(subType, StringComparison.OrdinalIgnoreCase))
						{
							break;
						}
						num3++;
						if ((int)MimeData.values[num3].hash != num2)
						{
							goto IL_A7;
						}
					}
					return text;
				}
			}
			IL_A7:
			return type + "/" + subType;
		}

		private void EnsureBoundary()
		{
			if (base["boundary"] == null)
			{
				MimeParameter mimeParameter = new MimeParameter("boundary");
				base.InternalAppendChild(mimeParameter);
				mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
			}
		}

		private void Reset()
		{
			base.InternalRemoveAll();
			this.parsed = false;
			this.value = null;
			this.type = null;
			this.subType = null;
		}

		internal const bool AllowUTF8Value = false;

		internal const bool AllowUTF8Boundary = false;

		internal const bool AllowUTF8Charset = false;

		private string value;

		private string type;

		private string subType;
	}
}
