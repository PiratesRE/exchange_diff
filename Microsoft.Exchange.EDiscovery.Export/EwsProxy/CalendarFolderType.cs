using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class CalendarFolderType : BaseFolderType
	{
		public CalendarPermissionReadAccessType SharingEffectiveRights
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

		public CalendarPermissionSetType PermissionSet
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

		private CalendarPermissionReadAccessType sharingEffectiveRightsField;

		private bool sharingEffectiveRightsFieldSpecified;

		private CalendarPermissionSetType permissionSetField;
	}
}
