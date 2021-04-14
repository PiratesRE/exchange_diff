using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class Region : DirectoryObject
	{
		public DirectoryPropertyStringLength1To3 CountryLetterCodes
		{
			get
			{
				return this.countryLetterCodesField;
			}
			set
			{
				this.countryLetterCodesField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To16 RegionName
		{
			get
			{
				return this.regionNameField;
			}
			set
			{
				this.regionNameField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private DirectoryPropertyStringLength1To3 countryLetterCodesField;

		private DirectoryPropertyStringSingleLength1To16 regionNameField;

		private XmlAttribute[] anyAttrField;
	}
}
