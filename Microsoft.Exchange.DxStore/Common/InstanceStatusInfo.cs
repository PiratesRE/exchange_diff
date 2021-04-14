using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class InstanceStatusInfo
	{
		[DataMember]
		public string Self { get; set; }

		[DataMember]
		public int StateRaw { get; set; }

		[DataMember]
		public int LastInstanceExecuted { get; set; }

		[DataMember]
		public InstanceGroupMemberConfig[] MemberConfigs { get; set; }

		[DataMember]
		public PaxosBasicInfo PaxosInfo { get; set; }

		[DataMember]
		public ProcessBasicInfo HostProcessInfo { get; set; }

		[DataMember]
		public DateTimeOffset CommitAckOldestItemTime { get; set; }

		public bool IsLeader
		{
			get
			{
				return this.PaxosInfo != null && this.PaxosInfo.IsLeader;
			}
		}

		public InstanceState State
		{
			get
			{
				InstanceState result;
				try
				{
					result = (InstanceState)this.StateRaw;
				}
				catch
				{
					result = InstanceState.Unknown;
				}
				return result;
			}
			set
			{
				this.StateRaw = (int)value;
			}
		}

		public bool IsValidPaxosMembersExist()
		{
			return this.PaxosInfo != null && this.PaxosInfo.Members != null && this.PaxosInfo.Members.Length > 0;
		}

		public bool IsValidLeaderExist()
		{
			return this.IsValidPaxosMembersExist() && !string.IsNullOrWhiteSpace(this.PaxosInfo.LeaderHint.replica);
		}

		public bool AreMembersSame(InstanceGroupMemberConfig[] members)
		{
			Tuple<string[], string[], string[]> tuple = Utils.DiffArrays<string>(this.PaxosInfo.Members, (from m in members
			select m.Name).ToArray<string>());
			return tuple.Item2.Length == 0 && tuple.Item3.Length == 0;
		}
	}
}
