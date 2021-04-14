using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class NodeInfo : BaseRow
	{
		protected NodeInfo() : base(null, null)
		{
		}

		[DataMember]
		public string ID { get; internal set; }

		[DataMember]
		public string Name { get; internal set; }

		[DataMember]
		public bool CanNewSubNode { get; internal set; }
	}
}
