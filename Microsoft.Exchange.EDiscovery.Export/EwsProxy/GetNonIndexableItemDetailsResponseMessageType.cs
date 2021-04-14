using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetNonIndexableItemDetailsResponseMessageType : ResponseMessageType
	{
		public NonIndexableItemDetailResultType NonIndexableItemDetailsResult
		{
			get
			{
				return this.nonIndexableItemDetailsResultField;
			}
			set
			{
				this.nonIndexableItemDetailsResultField = value;
			}
		}

		private NonIndexableItemDetailResultType nonIndexableItemDetailsResultField;
	}
}
