using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class SchemaValidationException : ServicePermanentException, IProvideXmlNodeArray
	{
		public SchemaValidationException(Exception innerException, int lineNumber, int linePosition, string violation) : base((CoreResources.IDs)2523006528U, innerException)
		{
			this.BuildDetailsFromException(lineNumber, linePosition, violation);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private void BuildDetailsFromException(int lineNumber, int linePosition, string failureReason)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlElement xmlElement = ServiceXml.CreateElement(safeXmlDocument, SchemaValidationException.DummyElementName, "http://schemas.microsoft.com/exchange/services/2006/types");
			safeXmlDocument.AppendChild(xmlElement);
			this.details = new XmlNodeArray();
			this.details.Nodes.Add(ServiceXml.CreateTextElement(xmlElement, SchemaValidationException.LineNumberElementName, lineNumber.ToString()));
			this.details.Nodes.Add(ServiceXml.CreateTextElement(xmlElement, SchemaValidationException.LinePositionElementName, linePosition.ToString()));
			this.details.Nodes.Add(ServiceXml.CreateTextElement(xmlElement, SchemaValidationException.ViolationElementName, failureReason));
		}

		public XmlNodeArray NodeArray
		{
			get
			{
				return this.details;
			}
		}

		internal static readonly string LineNumberElementName = "LineNumber";

		internal static readonly string LinePositionElementName = "LinePosition";

		internal static readonly string ViolationElementName = "Violation";

		internal static readonly string DummyElementName = "Dummy";

		private XmlNodeArray details;
	}
}
