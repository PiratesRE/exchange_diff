using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	public class InvalidCharsetException : ExchangeDataException
	{
		public InvalidCharsetException(int codePage) : base(GlobalizationStrings.InvalidCodePage(codePage))
		{
			this.codePage = codePage;
		}

		public InvalidCharsetException(string charsetName) : base(GlobalizationStrings.InvalidCharset((charsetName == null) ? "<null>" : charsetName))
		{
			this.charsetName = charsetName;
		}

		public InvalidCharsetException(int codePage, string message) : base(message)
		{
			this.codePage = codePage;
		}

		public InvalidCharsetException(string charsetName, string message) : base(message)
		{
			this.charsetName = charsetName;
		}

		internal InvalidCharsetException(string charsetName, int codePage, string message) : base(message)
		{
			this.codePage = codePage;
			this.charsetName = charsetName;
		}

		public InvalidCharsetException(int codePage, string message, Exception innerException) : base(message, innerException)
		{
			this.codePage = codePage;
		}

		public InvalidCharsetException(string charsetName, string message, Exception innerException) : base(message, innerException)
		{
			this.charsetName = charsetName;
		}

		protected InvalidCharsetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.codePage = info.GetInt32("codePage");
			this.charsetName = info.GetString("charsetName");
		}

		public int CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public string CharsetName
		{
			get
			{
				return this.charsetName;
			}
		}

		private int codePage;

		private string charsetName;
	}
}
