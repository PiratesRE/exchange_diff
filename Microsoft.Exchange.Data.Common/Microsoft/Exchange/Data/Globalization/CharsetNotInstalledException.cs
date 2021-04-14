using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	public class CharsetNotInstalledException : InvalidCharsetException
	{
		public CharsetNotInstalledException(int codePage) : base(codePage, GlobalizationStrings.NotInstalledCodePage(codePage))
		{
		}

		public CharsetNotInstalledException(string charsetName) : base(charsetName, GlobalizationStrings.NotInstalledCharset((charsetName == null) ? "<null>" : charsetName))
		{
		}

		internal CharsetNotInstalledException(string charsetName, int codePage) : base(charsetName, codePage, GlobalizationStrings.NotInstalledCharsetCodePage(codePage, (charsetName == null) ? "<null>" : charsetName))
		{
		}

		public CharsetNotInstalledException(int codePage, string message) : base(codePage, message)
		{
		}

		public CharsetNotInstalledException(string charsetName, string message) : base(charsetName, message)
		{
		}

		public CharsetNotInstalledException(int codePage, string message, Exception innerException) : base(codePage, message, innerException)
		{
		}

		public CharsetNotInstalledException(string charsetName, string message, Exception innerException) : base(charsetName, message, innerException)
		{
		}

		protected CharsetNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
