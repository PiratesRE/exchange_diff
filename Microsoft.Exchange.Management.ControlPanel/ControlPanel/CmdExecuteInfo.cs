using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CmdExecuteInfo
	{
		[DataMember]
		public string LogId { get; set; }

		[DataMember]
		public CmdletStatus Status { get; set; }

		[DataMember]
		public string CommandText { get; set; }

		[DataMember]
		public string Exception { get; set; }

		[DataMember]
		public string StartTime { get; set; }
	}
}
