using System;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class ParserException : RuleParsingException
	{
		public ParserException(string message) : base(message, 0, 0)
		{
		}

		public ParserException(LocalizedString message) : base(message, 0, 0)
		{
		}

		public ParserException(string message, XmlReader reader) : base(message, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition)
		{
		}

		public ParserException(LocalizedString message, XmlReader reader) : base(message, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition)
		{
		}

		public ParserException(Exception e, XmlReader reader) : base(e.Message, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition, e)
		{
		}

		public ParserException(XmlException e) : base(e.Message, e.LineNumber, e.LinePosition, e)
		{
		}

		protected ParserException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
