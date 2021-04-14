using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMMailboxExtension : BaseRow
	{
		[DataMember]
		public string DisplayName { get; set; }
	}
}
