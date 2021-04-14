using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security
{
	[ComVisible(true)]
	[Serializable]
	public sealed class XmlSyntaxException : SystemException
	{
		public XmlSyntaxException() : base(Environment.GetResourceString("XMLSyntax_InvalidSyntax"))
		{
			base.SetErrorCode(-2146233320);
		}

		public XmlSyntaxException(string message) : base(message)
		{
			base.SetErrorCode(-2146233320);
		}

		public XmlSyntaxException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233320);
		}

		public XmlSyntaxException(int lineNumber) : base(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("XMLSyntax_SyntaxError"), lineNumber))
		{
			base.SetErrorCode(-2146233320);
		}

		public XmlSyntaxException(int lineNumber, string message) : base(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("XMLSyntax_SyntaxErrorEx"), lineNumber, message))
		{
			base.SetErrorCode(-2146233320);
		}

		internal XmlSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
