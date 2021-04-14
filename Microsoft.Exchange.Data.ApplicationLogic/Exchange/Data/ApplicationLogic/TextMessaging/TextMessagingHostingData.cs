using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[XmlRoot(Namespace = "", IsNullable = false)]
	[XmlType(AnonymousType = true)]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TextMessagingHostingData
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingData_locDefinition _locDefinition
		{
			get
			{
				return this._locDefinitionField;
			}
			set
			{
				this._locDefinitionField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataRegions Regions
		{
			get
			{
				return this.regionsField;
			}
			set
			{
				this.regionsField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataCarriers Carriers
		{
			get
			{
				return this.carriersField;
			}
			set
			{
				this.carriersField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServices Services
		{
			get
			{
				return this.servicesField;
			}
			set
			{
				this.servicesField = value;
			}
		}

		[XmlAttribute]
		public string Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		private TextMessagingHostingData_locDefinition _locDefinitionField;

		private TextMessagingHostingDataRegions regionsField;

		private TextMessagingHostingDataCarriers carriersField;

		private TextMessagingHostingDataServices servicesField;

		private string versionField;
	}
}
