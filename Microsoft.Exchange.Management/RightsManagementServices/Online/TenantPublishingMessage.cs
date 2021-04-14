using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DebuggerStepThrough]
	[DataContract(Name = "TenantPublishingMessage", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class TenantPublishingMessage : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public string TenantCertificationUrl
		{
			get
			{
				return this.TenantCertificationUrlField;
			}
			set
			{
				this.TenantCertificationUrlField = value;
			}
		}

		[DataMember]
		public string TenantLicensingUrl
		{
			get
			{
				return this.TenantLicensingUrlField;
			}
			set
			{
				this.TenantLicensingUrlField = value;
			}
		}

		[DataMember(Order = 2)]
		public string TenantKeyExportUrl
		{
			get
			{
				return this.TenantKeyExportUrlField;
			}
			set
			{
				this.TenantKeyExportUrlField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string TenantCertificationUrlField;

		private string TenantLicensingUrlField;

		private string TenantKeyExportUrlField;
	}
}
