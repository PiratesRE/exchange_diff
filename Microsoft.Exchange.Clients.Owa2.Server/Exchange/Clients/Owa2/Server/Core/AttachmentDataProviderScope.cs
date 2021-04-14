using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(OneDriveProScope))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AttachmentDataProviderScope
	{
		public AttachmentDataProviderScope(string displayName, string ariaLabel)
		{
			this.DisplayName = displayName;
			this.AriaLabel = ariaLabel;
		}

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string AriaLabel { get; set; }
	}
}
