using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class Scope : DirectoryObject
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

		public DirectoryPropertyInt32SingleMin0 ScopeFilterOn
		{
			get
			{
				return this.scopeFilterOnField;
			}
			set
			{
				this.scopeFilterOnField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ScopeFilterValue
		{
			get
			{
				return this.scopeFilterValueField;
			}
			set
			{
				this.scopeFilterValueField = value;
			}
		}

		public DirectoryPropertyGuidSingle ScopeId
		{
			get
			{
				return this.scopeIdField;
			}
			set
			{
				this.scopeIdField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 ScopeParameterType
		{
			get
			{
				return this.scopeParameterTypeField;
			}
			set
			{
				this.scopeParameterTypeField = value;
			}
		}

		public DirectoryPropertyInt64Single ScopeTargetTypes
		{
			get
			{
				return this.scopeTargetTypesField;
			}
			set
			{
				this.scopeTargetTypesField = value;
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

		private DirectoryPropertyInt32SingleMin0 scopeFilterOnField;

		private DirectoryPropertyStringSingleLength1To1024 scopeFilterValueField;

		private DirectoryPropertyGuidSingle scopeIdField;

		private DirectoryPropertyInt32SingleMin0 scopeParameterTypeField;

		private DirectoryPropertyInt64Single scopeTargetTypesField;

		private XmlAttribute[] anyAttrField;
	}
}
