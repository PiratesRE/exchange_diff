using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[XmlType(AnonymousType = true)]
	[Serializable]
	public class TextMessagingHostingDataRegions
	{
		[XmlElement("Region", Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataRegionsRegion[] Region
		{
			get
			{
				return this.regionField;
			}
			set
			{
				this.regionField = value;
			}
		}

		private TextMessagingHostingDataRegionsRegion[] regionField;
	}
}
