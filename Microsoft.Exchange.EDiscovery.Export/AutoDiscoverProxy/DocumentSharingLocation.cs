using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class DocumentSharingLocation
	{
		public string ServiceUrl
		{
			get
			{
				return this.serviceUrlField;
			}
			set
			{
				this.serviceUrlField = value;
			}
		}

		public string LocationUrl
		{
			get
			{
				return this.locationUrlField;
			}
			set
			{
				this.locationUrlField = value;
			}
		}

		public string DisplayName
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

		[XmlArrayItem("FileExtension", IsNullable = false)]
		public string[] SupportedFileExtensions
		{
			get
			{
				return this.supportedFileExtensionsField;
			}
			set
			{
				this.supportedFileExtensionsField = value;
			}
		}

		public bool ExternalAccessAllowed
		{
			get
			{
				return this.externalAccessAllowedField;
			}
			set
			{
				this.externalAccessAllowedField = value;
			}
		}

		public bool AnonymousAccessAllowed
		{
			get
			{
				return this.anonymousAccessAllowedField;
			}
			set
			{
				this.anonymousAccessAllowedField = value;
			}
		}

		public bool CanModifyPermissions
		{
			get
			{
				return this.canModifyPermissionsField;
			}
			set
			{
				this.canModifyPermissionsField = value;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.isDefaultField;
			}
			set
			{
				this.isDefaultField = value;
			}
		}

		private string serviceUrlField;

		private string locationUrlField;

		private string displayNameField;

		private string[] supportedFileExtensionsField;

		private bool externalAccessAllowedField;

		private bool anonymousAccessAllowedField;

		private bool canModifyPermissionsField;

		private bool isDefaultField;
	}
}
