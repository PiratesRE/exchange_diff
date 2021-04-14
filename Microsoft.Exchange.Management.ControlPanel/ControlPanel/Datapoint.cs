using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class Datapoint
	{
		public Datapoint(string name, string source, string requestId, string timestamp)
		{
			this.Name = name;
			this.Src = source;
			this.ReqId = requestId;
			this.Time = timestamp;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Src { get; set; }

		[DataMember]
		public string ReqId { get; set; }

		[DataMember]
		public string Time { get; set; }
	}
}
