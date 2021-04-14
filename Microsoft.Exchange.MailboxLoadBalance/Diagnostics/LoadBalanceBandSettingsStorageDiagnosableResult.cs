using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceBandSettingsStorageDiagnosableResult
	{
		[DataMember]
		public PersistedBandDefinition[] PersistedBands { get; set; }

		[DataMember]
		public Band[] ActiveBands { get; set; }

		[DataMember]
		public PersistedBandDefinition ModifiedBand { get; set; }
	}
}
