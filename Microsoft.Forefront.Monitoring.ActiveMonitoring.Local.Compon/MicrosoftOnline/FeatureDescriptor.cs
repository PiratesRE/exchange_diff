using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[Serializable]
	public class FeatureDescriptor : DirectoryObject
	{
		public DirectoryPropertyStringSingleLength1To1024 Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public DirectoryPropertyGuidSingle FeatureDescriptorId
		{
			get
			{
				return this.featureDescriptorIdField;
			}
			set
			{
				this.featureDescriptorIdField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 FeatureStatus
		{
			get
			{
				return this.featureStatusField;
			}
			set
			{
				this.featureStatusField = value;
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

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyGuidSingle featureDescriptorIdField;

		private DirectoryPropertyInt32SingleMin0 featureStatusField;

		private XmlAttribute[] anyAttrField;
	}
}
