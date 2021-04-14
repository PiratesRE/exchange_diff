using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncCalendarParameters
	{
		[DataMember]
		public string SyncState { get; set; }

		[DataMember]
		public TargetFolderId FolderId { get; set; }

		[DataMember]
		public string WindowStart { get; set; }

		[DataMember]
		public string WindowEnd { get; set; }

		[DataMember]
		public int MaxChangesReturned { get; set; }

		[DataMember]
		public bool RespondWithAdditionalData { get; set; }
	}
}
