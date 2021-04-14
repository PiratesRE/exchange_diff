using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoBodyProperty : XsoProperty, IBodyProperty, IMIMERelatedProperty, IProperty
	{
		public XsoBodyProperty(PropertyType type) : base(null, type)
		{
		}

		public XsoBodyProperty() : base(null)
		{
		}

		public bool IsOnSMIMEMessage
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				return messageItem != null && ObjectClass.IsSmime(messageItem.ClassName);
			}
		}

		public Stream RtfData
		{
			get
			{
				if (!this.RtfPresent)
				{
					throw new ConversionException("Cannot pull RtfData, Rtf property not present on this item");
				}
				if (this.rtfData == null)
				{
					Item item = (Item)base.XsoItem;
					item.Load();
					long num;
					IList<AttachmentLink> list;
					this.rtfData = BodyConversionUtilities.ConvertToRtfStream(item, -1L, out num, out list);
					this.rtfSize = (int)num;
				}
				return this.rtfData;
			}
		}

		public bool RtfPresent
		{
			get
			{
				Item item = (Item)base.XsoItem;
				return item.Body != null && item.Body.Format == BodyFormat.ApplicationRtf;
			}
		}

		public int RtfSize
		{
			get
			{
				if (this.rtfSize < 0)
				{
					if (!this.RtfPresent)
					{
						throw new ConversionException("Cannot pull RtfSize, Rtf property not present on this item");
					}
					Item item = (Item)base.XsoItem;
					long size = item.Body.Size;
					if (size > 2147483647L || size < 0L)
					{
						throw new ConversionException("Invalid body size: " + size.ToString(CultureInfo.InvariantCulture));
					}
					this.rtfSize = (int)size;
				}
				return this.rtfSize;
			}
		}

		public Stream TextData
		{
			get
			{
				if (!this.TextPresent)
				{
					throw new ConversionException("Cannot pull TextData, TextBody property not present on this item");
				}
				if (this.textData == null)
				{
					this.GetTextBody();
				}
				return this.textData;
			}
		}

		public bool TextPresent
		{
			get
			{
				Item item = (Item)base.XsoItem;
				return item.Body != null;
			}
		}

		public int TextSize
		{
			get
			{
				if (this.textSize < 0)
				{
					if (!this.TextPresent)
					{
						throw new ConversionException("Cannot pull TextData, TextBody property not present on this item");
					}
					this.GetTextBody();
				}
				return this.textSize;
			}
		}

		public Stream GetTextData(int length)
		{
			if (!this.TextPresent)
			{
				throw new ConversionException("Cannot pull TextData, TextBody property not present on this item");
			}
			if (length == 0)
			{
				return XsoBodyProperty.emptyStream;
			}
			return this.GetTextBody(length);
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			Item item = (Item)base.XsoItem;
			IBodyProperty bodyProperty = (IBodyProperty)srcProperty;
			if (bodyProperty.RtfPresent)
			{
				if (bodyProperty.RtfData != null && bodyProperty.RtfData.Length > 0L)
				{
					using (Stream stream = XsoBodyProperty.OpenBodyWriteStream(item, BodyFormat.ApplicationRtf))
					{
						StreamHelper.CopyStream(bodyProperty.RtfData, stream);
						return;
					}
				}
				using (TextWriter textWriter = item.Body.OpenTextWriter(BodyFormat.TextPlain))
				{
					textWriter.Write(string.Empty);
					return;
				}
			}
			if (bodyProperty.TextPresent)
			{
				using (Stream stream2 = XsoBodyProperty.OpenBodyWriteStream(item, BodyFormat.TextPlain))
				{
					StreamHelper.CopyStream(bodyProperty.TextData, stream2);
					return;
				}
			}
			throw new ConversionException("Source body property does not have Rtf or Text body present");
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			Item item = (Item)base.XsoItem;
			using (TextWriter textWriter = item.Body.OpenTextWriter(BodyFormat.TextPlain))
			{
				textWriter.Write(string.Empty);
			}
		}

		public override void Unbind()
		{
			this.textData = null;
			this.rtfData = null;
			this.textSize = -1;
			this.rtfSize = -1;
			base.Unbind();
		}

		private static Stream OpenBodyWriteStream(Item item, BodyFormat bodyFormat)
		{
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(bodyFormat, "utf-8");
			return item.Body.OpenWriteStream(configuration);
		}

		private void GetTextBody()
		{
			this.textData = this.GetTextBody(-1);
		}

		private Stream GetTextBody(int length)
		{
			Item item = (Item)base.XsoItem;
			if (string.Equals(item.ClassName, "IPM.Note.SMIME", StringComparison.OrdinalIgnoreCase))
			{
				string s = Strings.SMIMENotSupportedBodyText.ToString(item.Session.PreferedCulture);
				this.textData = new MemoryStream(Encoding.UTF8.GetBytes(s));
				return this.textData;
			}
			long num;
			IList<AttachmentLink> list;
			Stream result = BodyConversionUtilities.ConvertToPlainTextStream(item, (long)length, out num, out list);
			this.textSize = (int)num;
			return result;
		}

		private static readonly Stream emptyStream = new MemoryStream(0);

		private int textSize = -1;

		private int rtfSize = -1;

		private Stream textData;

		private Stream rtfData;
	}
}
