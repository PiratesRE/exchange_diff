using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FUSE.Paxos;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class PaxosBasicInfo
	{
		[DataMember]
		public string Self { get; set; }

		[DataMember]
		public string[] Members { get; set; }

		[DataMember]
		public Round<string> LeaderHint { get; set; }

		[DataMember]
		public int CountExecuted { get; set; }

		[DataMember]
		public int CountTruncated { get; set; }

		[DataMember]
		public PaxosBasicInfo.GossipDictionary Gossip { get; set; }

		public bool IsLeader
		{
			get
			{
				return Utils.IsEqual(this.LeaderHint.replica, this.Self, StringComparison.OrdinalIgnoreCase);
			}
		}

		public PaxosBasicInfo Clone()
		{
			return (PaxosBasicInfo)base.MemberwiseClone();
		}

		public bool IsMember(string nodeName)
		{
			return this.Members != null && this.Members.Contains(nodeName, StringComparer.OrdinalIgnoreCase);
		}

		[CollectionDataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GossipDictionary : Dictionary<string, int>
		{
			public GossipDictionary()
			{
			}

			protected GossipDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}
	}
}
