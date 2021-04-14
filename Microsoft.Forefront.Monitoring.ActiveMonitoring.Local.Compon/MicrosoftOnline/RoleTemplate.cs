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
	public class RoleTemplate : DirectoryObject
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

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyGuidSingle roleTemplateIdField;

		private DirectoryPropertyBooleanSingle roleDisabledField;

		private DirectoryPropertyStringSingleLength1To40 wellKnownObjectField;

		private DirectoryPropertyXmlTaskSetScopeReference taskSetScopeReferenceField;

		private XmlAttribute[] anyAttrField;
	}
}
