using System;
using System.Xml;

namespace Microsoft.Exchange.Compliance.Xml
{
	internal class XmlDtdException : XmlException
	{
		public override string Message
		{
			get
			{
				return "For security reasons DTD is prohibited in this XML document.";
			}
		}
	}
}
