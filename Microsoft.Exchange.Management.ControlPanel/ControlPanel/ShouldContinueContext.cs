using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ShouldContinueContext : ErrorRecordContext
	{
		public ShouldContinueContext()
		{
			this.CmdletsPrompted = new List<string>();
		}

		[DataMember]
		public List<string> CmdletsPrompted { get; set; }
	}
}
