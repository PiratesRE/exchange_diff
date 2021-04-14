using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class SecuritySignedXml : SignedXml
	{
		public SecuritySignedXml(XmlDocument xmlDocument, XmlElement[] elementsToSign) : base(xmlDocument)
		{
			base.SignedInfo.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";
			this.elementsToSign = new Dictionary<string, XmlElement>(elementsToSign.Length);
			foreach (XmlElement xmlElement in elementsToSign)
			{
				string attributeValue = WSSecurityUtility.Id.GetAttributeValue(xmlElement);
				this.elementsToSign.Add(attributeValue, xmlElement);
			}
		}

		public override XmlElement GetIdElement(XmlDocument document, string idValue)
		{
			return this.elementsToSign[idValue];
		}

		private Dictionary<string, XmlElement> elementsToSign;
	}
}
