using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class Contract : DirectoryObject
	{
		public DirectoryPropertyGuidSingle ContractId
		{
			get
			{
				return this.contractIdField;
			}
			set
			{
				this.contractIdField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 ContractType
		{
			get
			{
				return this.contractTypeField;
			}
			set
			{
				this.contractTypeField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 DefaultGeography
		{
			get
			{
				return this.defaultGeographyField;
			}
			set
			{
				this.defaultGeographyField = value;
			}
		}

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

		public DirectoryPropertyXmlSupportRole SupportRole
		{
			get
			{
				return this.supportRoleField;
			}
			set
			{
				this.supportRoleField = value;
			}
		}

		public DirectoryPropertyGuidSingle TargetContextId
		{
			get
			{
				return this.targetContextIdField;
			}
			set
			{
				this.targetContextIdField = value;
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

		private DirectoryPropertyGuidSingle contractIdField;

		private DirectoryPropertyInt32SingleMin0 contractTypeField;

		private DirectoryPropertyStringSingleLength1To128 defaultGeographyField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyXmlSupportRole supportRoleField;

		private DirectoryPropertyGuidSingle targetContextIdField;

		private XmlAttribute[] anyAttrField;
	}
}
