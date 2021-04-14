using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	public struct HtmlAttributeReader
	{
		internal HtmlAttributeReader(HtmlReader reader)
		{
			this.reader = reader;
		}

		public bool ReadNext()
		{
			return this.reader.AttributeReader_ReadNextAttribute();
		}

		public HtmlAttributeId Id
		{
			get
			{
				return this.reader.AttributeReader_GetCurrentAttributeId();
			}
		}

		public bool NameIsLong
		{
			get
			{
				return this.reader.AttributeReader_CurrentAttributeNameIsLong();
			}
		}

		public string ReadName()
		{
			return this.reader.AttributeReader_ReadCurrentAttributeName();
		}

		public int ReadName(char[] buffer, int offset, int count)
		{
			return this.reader.AttributeReader_ReadCurrentAttributeName(buffer, offset, count);
		}

		internal void WriteNameTo(ITextSink sink)
		{
			this.reader.AttributeReader_WriteCurrentAttributeNameTo(sink);
		}

		public bool HasValue
		{
			get
			{
				return this.reader.AttributeReader_CurrentAttributeHasValue();
			}
		}

		public bool ValueIsLong
		{
			get
			{
				return this.reader.AttributeReader_CurrentAttributeValueIsLong();
			}
		}

		public string ReadValue()
		{
			return this.reader.AttributeReader_ReadCurrentAttributeValue();
		}

		public int ReadValue(char[] buffer, int offset, int count)
		{
			return this.reader.AttributeReader_ReadCurrentAttributeValue(buffer, offset, count);
		}

		internal void WriteValueTo(ITextSink sink)
		{
			this.reader.AttributeReader_WriteCurrentAttributeValueTo(sink);
		}

		private HtmlReader reader;
	}
}
