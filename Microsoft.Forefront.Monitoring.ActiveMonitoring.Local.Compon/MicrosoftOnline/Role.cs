using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class Role : DirectoryObject
	{
		public DirectoryPropertyBooleanSingle BelongsToFirstLoginObjectSet
		{
			get
			{
				return this.belongsToFirstLoginObjectSetField;
			}
			set
			{
				this.belongsToFirstLoginObjectSetField = value;
			}
		}

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

		public DirectoryPropertyStringSingleMailNickname MailNickname
		{
			get
			{
				return this.mailNicknameField;
			}
			set
			{
				this.mailNicknameField = value;
			}
		}

		public DirectoryPropertyBooleanSingle RoleDisabled
		{
			get
			{
				return this.roleDisabledField;
			}
			set
			{
				this.roleDisabledField = value;
			}
		}

		public DirectoryPropertyGuidSingle RoleTemplateId
		{
			get
			{
				return this.roleTemplateIdField;
			}
			set
			{
				this.roleTemplateIdField = value;
			}
		}

		public DirectoryPropertyXmlServiceInfo ServiceInfo
		{
			get
			{
				return this.serviceInfoField;
			}
			set
			{
				this.serviceInfoField = value;
			}
		}

		public DirectoryPropertyXmlTaskSetScopeReference TaskSetScopeReference
		{
			get
			{
				return this.taskSetScopeReferenceField;
			}
			set
			{
				this.taskSetScopeReferenceField = value;
			}
		}

		public DirectoryPropertyXmlValidationError ValidationError
		{
			get
			{
				return this.validationErrorField;
			}
			set
			{
				this.validationErrorField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To40 WellKnownObject
		{
			get
			{
				return this.wellKnownObjectField;
			}
			set
			{
				this.wellKnownObjectField = value;
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

		private DirectoryPropertyBooleanSingle belongsToFirstLoginObjectSetField;

		private DirectoryPropertyBooleanSingle builtinField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyBooleanSingle roleDisabledField;

		private DirectoryPropertyGuidSingle roleTemplateIdField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyXmlTaskSetScopeReference taskSetScopeReferenceField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private DirectoryPropertyStringSingleLength1To40 wellKnownObjectField;

		private XmlAttribute[] anyAttrField;
	}
}
