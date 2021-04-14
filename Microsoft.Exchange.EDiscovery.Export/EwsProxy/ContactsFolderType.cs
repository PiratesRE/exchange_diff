using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class ContactsFolderType : BaseFolderType
	{
		public PermissionReadAccessType SharingEffectiveRights
		{
			get
			{
				return this.sharingEffectiveRightsField;
			}
			set
			{
				this.sharingEffectiveRightsField = value;
			}
		}

		[XmlIgnore]
		public bool SharingEffectiveRightsSpecified
		{
			get
			{
				return this.sharingEffectiveRightsFieldSpecified;
			}
			set
			{
				this.sharingEffectiveRightsFieldSpecified = value;
			}
		}

		public PermissionSetType PermissionSet
		{
			get
			{
				return this.permissionSetField;
			}
			set
			{
				this.permissionSetField = value;
			}
		}

		private PermissionReadAccessType sharingEffectiveRightsField;

		private bool sharingEffectiveRightsFieldSpecified;

		private PermissionSetType permissionSetField;
	}
}
