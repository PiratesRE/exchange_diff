using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ResponseSchemaValidationException : ServicePermanentException, IProvideXmlNodeArray
	{
		public ResponseSchemaValidationException(Exception innerException, int lineNumber, int linePosition, string violation, string badResponse) : base(CoreResources.IDs.ErrorResponseSchemaValidation, innerException)
		{
			this.BuildDetailsFromException(lineNumber, linePosition, violation, badResponse);
		}

		private void BuildDetailsFromException(int lineNumber, int linePosition, string failureReason, string badResponse)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlElement xmlElement = ServiceXml.CreateElement(safeXmlDocument, SchemaValidationException.DummyElementName, "http://schemas.microsoft.com/exchange/services/2006/types");
			safeXmlDocument.AppendChild(xmlElement);
			this.details = new XmlNodeArray();
			this.details.Nodes.Add(ServiceXml.CreateTextElement(xmlElement, SchemaValidationException.LineNumberElementName, lineNumber.ToString()));
			this.details.Nodes.Add(ServiceXml.CreateTextElement(xmlElement, SchemaValidationException.LinePositionElementName, linePosition.ToString()));
			this.details.Nodes.Add(ServiceXml.CreateTextElement(xmlElement, SchemaValidationException.ViolationElementName, failureReason));
			XmlElement xmlElement2 = ServiceXml.CreateElement(xmlElement, ResponseSchemaValidationException.BadResponseElementName);
			xmlElement2.AppendChild(safeXmlDocument.CreateCDataSection(badResponse));
			this.details.Nodes.Add(xmlElement2);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		public XmlNodeArray NodeArray
		{
			get
			{
				return this.details;
			}
		}

		private static readonly string BadResponseElementName = "BadResponse";

		private XmlNodeArray details;
	}
}
