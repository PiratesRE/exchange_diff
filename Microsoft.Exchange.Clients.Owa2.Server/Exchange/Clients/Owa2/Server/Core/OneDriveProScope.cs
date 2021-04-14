using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OneDriveProScope : AttachmentDataProviderScope
	{
		public OneDriveProScope(OneDriveProScopeType type, string displayName, string ariaLabel) : base(displayName, ariaLabel)
		{
			this.Type = type;
		}

		[DataMember]
		[SimpleConfigurationProperty("type")]
		public OneDriveProScopeType Type { get; set; }
	}
}
