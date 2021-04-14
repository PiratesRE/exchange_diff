using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ReferenceAttachment")]
	[Serializable]
	public class ReferenceAttachmentType : AttachmentType
	{
		[DataMember(EmitDefaultValue = false)]
		public string AttachLongPathName { get; set; }

		public static bool IsReferenceAttachmentSupported()
		{
			ExTraceGlobals.GetItemCallTracer.TraceDebug<ExchangeVersion, bool>(0L, "[ReferenceAttachmentType.IsReferenceAttachmentSupported] Exchange Version: {0}, RefAttachmentDisabled: {1}", ExchangeVersion.Current, EWSSettings.DisableReferenceAttachment);
			return ExchangeVersion.Current > ExchangeVersion.ExchangeV2_4 && !EWSSettings.DisableReferenceAttachment;
		}

		[DataMember(EmitDefaultValue = false)]
		public string ProviderType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ProviderEndpointUrl { get; set; }
	}
}
