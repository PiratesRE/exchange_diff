using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class ForeignPrincipal : DirectoryObject
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncForeignPrincipalSchema.Description, ref this.descriptionField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncForeignPrincipalSchema.DisplayName, ref this.displayNameField);
			processor.Process<DirectoryPropertyGuidSingle>(SyncForeignPrincipalSchema.LinkedPartnerGroupId, ref this.foreignPrincipalIdField);
			processor.Process<DirectoryPropertyGuidSingle>(SyncForeignPrincipalSchema.LinkedPartnerOrganizationId, ref this.foreignContextIdField);
		}

		[XmlElement(Order = 0)]
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

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
		public DirectoryPropertyGuidSingle ForeignContextId
		{
			get
			{
				return this.foreignContextIdField;
			}
			set
			{
				this.foreignContextIdField = value;
			}
		}

		[XmlElement(Order = 3)]
		public DirectoryPropertyGuidSingle ForeignPrincipalId
		{
			get
			{
				return this.foreignPrincipalIdField;
			}
			set
			{
				this.foreignPrincipalIdField = value;
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

		private DirectoryPropertyGuidSingle foreignContextIdField;

		private DirectoryPropertyGuidSingle foreignPrincipalIdField;

		private XmlAttribute[] anyAttrField;
	}
}
