using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TaskSet : DirectoryObject
	{
		public DirectoryPropertyBooleanSingle Builtin
		{
			get
			{
				return this.builtinField;
			}
			set
			{
				this.builtinField = value;
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

		public DirectoryPropertyGuid TaskIdList
		{
			get
			{
				return this.taskIdListField;
			}
			set
			{
				this.taskIdListField = value;
			}
		}

		public DirectoryPropertyGuidSingle TaskSetId
		{
			get
			{
				return this.taskSetIdField;
			}
			set
			{
				this.taskSetIdField = value;
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

		private DirectoryPropertyBooleanSingle builtinField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyGuid taskIdListField;

		private DirectoryPropertyGuidSingle taskSetIdField;

		private XmlAttribute[] anyAttrField;
	}
}
