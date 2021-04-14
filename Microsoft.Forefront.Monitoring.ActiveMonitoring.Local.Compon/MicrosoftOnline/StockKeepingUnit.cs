using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class StockKeepingUnit : DirectoryObject
	{
		public DirectoryPropertyStringSingleLength1To256 PartNumber
		{
			get
			{
				return this.partNumberField;
			}
			set
			{
				this.partNumberField = value;
			}
		}

		public DirectoryPropertyReferenceServicePlan ServicePlanGranted
		{
			get
			{
				return this.servicePlanGrantedField;
			}
			set
			{
				this.servicePlanGrantedField = value;
			}
		}

		public DirectoryPropertyGuidSingle SkuId
		{
			get
			{
				return this.skuIdField;
			}
			set
			{
				this.skuIdField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 TargetClass
		{
			get
			{
				return this.targetClassField;
			}
			set
			{
				this.targetClassField = value;
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

		private DirectoryPropertyStringSingleLength1To256 partNumberField;

		private DirectoryPropertyReferenceServicePlan servicePlanGrantedField;

		private DirectoryPropertyGuidSingle skuIdField;

		private DirectoryPropertyInt32SingleMin0 targetClassField;

		private XmlAttribute[] anyAttrField;
	}
}
