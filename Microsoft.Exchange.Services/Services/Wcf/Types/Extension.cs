using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class Extension
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Version { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string ProviderName { get; set; }

		[DataMember]
		public string IconUrl { get; set; }

		[DataMember]
		public string HighResolutionIconUrl { get; set; }

		[DataMember]
		public ExtensionInstallScope Origin { get; set; }

		[DataMember]
		public ExtensionType Type { get; set; }

		[DataMember]
		public string AppStatus { get; set; }

		[DataMember]
		public string EndNodeUrl { get; set; }

		[DataMember]
		public LicenseType LicenseType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string AuthTokenId { get; set; }

		[DataMember(Name = "Rule")]
		public ActivationRule ActivationRule { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Settings { get; set; }

		[DataMember]
		public RequestedCapabilities RequestedCapabilities { get; set; }

		[DataMember]
		public bool DisableEntityHighlighting { get; set; }

		[DataMember]
		public FormSettings[] FormSettingsList { get; set; }
	}
}
