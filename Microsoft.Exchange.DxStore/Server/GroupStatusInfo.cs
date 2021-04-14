using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FUSE.Paxos;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class GroupStatusInfo
	{
		public GroupStatusInfo()
		{
			this.StatusMap = new Dictionary<string, InstanceStatusInfo>();
		}

		[DataMember]
		public Dictionary<string, InstanceStatusInfo> StatusMap { get; set; }

		[DataMember]
		public int TotalRequested { get; set; }

		[DataMember]
		public int TotalSuccessful { get; set; }

		[DataMember]
		public int TotalFailed { get; set; }

		[DataMember]
		public bool IsMajoritySuccessfulyReplied { get; set; }

		[DataMember]
		public bool IsMajorityAgreeWithLeader { get; set; }

		[DataMember]
		public bool IsMajorityHavePaxosInitialized { get; set; }

		[DataMember]
		public GroupStatusInfo.NodeInstancePair HighestInstance { get; set; }

		[DataMember]
		public GroupStatusInfo.NodeInstancePair LocalInstance { get; set; }

		[DataMember]
		public Round<string> LeaderHint { get; set; }

		[DataMember]
		public DateTimeOffset CollectionStartTime { get; set; }

		[DataMember]
		public TimeSpan CollectionDuration { get; set; }

		public bool IsLeaderExist
		{
			get
			{
				return !string.IsNullOrEmpty(this.LeaderHint.replica);
			}
		}

		public string LeaderName
		{
			get
			{
				if (!this.IsLeaderExist)
				{
					return string.Empty;
				}
				return this.LeaderHint.replica;
			}
		}

		public bool IsAllRequestsSucceeded
		{
			get
			{
				return this.TotalRequested == this.TotalSuccessful;
			}
		}

		public int TotalNoReplies
		{
			get
			{
				return this.TotalRequested - (this.TotalSuccessful + this.TotalFailed);
			}
		}

		public bool IsSingleNodeGroup
		{
			get
			{
				return this.TotalRequested == 1;
			}
		}

		public int Lag
		{
			get
			{
				int result = 0;
				if (this.HighestInstance != null)
				{
					if (this.LocalInstance != null)
					{
						result = this.HighestInstance.InstanceNumber - this.LocalInstance.InstanceNumber;
					}
					else
					{
						result = this.HighestInstance.InstanceNumber;
					}
				}
				return result;
			}
		}

		public InstanceStatusInfo GetMemberInfo(string memberName)
		{
			InstanceStatusInfo result = null;
			if (this.StatusMap != null)
			{
				this.StatusMap.TryGetValue(memberName, out result);
			}
			return result;
		}

		public string GetDebugString(string identity)
		{
			return string.Format("{0}: MajoritySuccess: {1}, MajorityAgreeWithLeader: {2}, MajorityHavePaxos: {3}, Leader: {4}, TotalRequested: {5}, TotalSuccess: {6}, TotalFailed = {7}, Lag: {8}, HighestNode = {9}, HighestInstance = {10}", new object[]
			{
				identity,
				this.IsMajoritySuccessfulyReplied,
				this.IsMajorityAgreeWithLeader,
				this.IsMajorityHavePaxosInitialized,
				this.LeaderName,
				this.TotalRequested,
				this.TotalSuccessful,
				this.TotalFailed,
				this.Lag,
				(this.HighestInstance != null) ? this.HighestInstance.NodeName : "<unknown>",
				(this.HighestInstance != null) ? this.HighestInstance.InstanceNumber : -1
			});
		}

		public void Analyze(string self, InstanceGroupConfig groupConfig)
		{
			if (this.isAnalyzed)
			{
				return;
			}
			int num = this.TotalRequested / 2 + 1;
			this.IsMajoritySuccessfulyReplied = (this.TotalSuccessful >= num);
			IEnumerable<InstanceStatusInfo> source = from v in this.StatusMap.Values
			where v != null
			select v;
			IEnumerable<GroupStatusInfo.NodeInstancePair> enumerable = from s in source
			orderby s.LastInstanceExecuted descending
			select new GroupStatusInfo.NodeInstancePair
			{
				NodeName = s.Self,
				InstanceNumber = s.LastInstanceExecuted
			};
			foreach (GroupStatusInfo.NodeInstancePair nodeInstancePair in enumerable)
			{
				if (this.HighestInstance == null)
				{
					this.HighestInstance = nodeInstancePair;
				}
				if (Utils.IsEqual(nodeInstancePair.NodeName, self, StringComparison.OrdinalIgnoreCase))
				{
					this.LocalInstance = nodeInstancePair;
					break;
				}
			}
			PaxosBasicInfo[] array = (from s in source
			where s.PaxosInfo != null
			select s.PaxosInfo).ToArray<PaxosBasicInfo>();
			this.IsMajorityHavePaxosInitialized = (array.Length >= num);
			IEnumerable<Round<string>> source2 = from p in array
			where !string.IsNullOrEmpty(p.LeaderHint.replica)
			select p.LeaderHint;
			IOrderedEnumerable<IGrouping<string, Round<string>>> source3 = from s in source2
			group s by s.replica into g
			orderby g.Count<Round<string>>() descending
			select g;
			this.IsMajorityAgreeWithLeader = false;
			IGrouping<string, Round<string>> grouping = source3.FirstOrDefault<IGrouping<string, Round<string>>>();
			if (grouping != null)
			{
				this.LeaderHint = grouping.FirstOrDefault<Round<string>>();
				int num2 = grouping.Count<Round<string>>();
				if (num2 >= num)
				{
					this.IsMajorityAgreeWithLeader = true;
				}
			}
			this.isAnalyzed = true;
		}

		public string[] GetPaxosMembers(string nodeName)
		{
			InstanceStatusInfo instanceStatusInfo = this.StatusMap.Values.FirstOrDefault((InstanceStatusInfo v) => v != null && Utils.IsEqual(v.Self, nodeName, StringComparison.OrdinalIgnoreCase));
			if (instanceStatusInfo != null && instanceStatusInfo.PaxosInfo != null)
			{
				return instanceStatusInfo.PaxosInfo.Members;
			}
			return null;
		}

		public string[] GetLeaderPaxosMembers()
		{
			if (this.IsLeaderExist)
			{
				return this.GetPaxosMembers(this.LeaderName);
			}
			return null;
		}

		public PaxosBasicInfo GetBestPossiblePaxosConfig()
		{
			InstanceStatusInfo instanceStatusInfo = (from s in this.StatusMap.Values
			where s != null && s.PaxosInfo != null
			select s).OrderByDescending(delegate(InstanceStatusInfo s)
			{
				if (!s.IsLeader)
				{
					return 0;
				}
				return 1;
			}).ThenByDescending((InstanceStatusInfo s) => s.LastInstanceExecuted).FirstOrDefault<InstanceStatusInfo>();
			if (instanceStatusInfo != null)
			{
				return instanceStatusInfo.PaxosInfo;
			}
			return null;
		}

		public bool IsAllNodesWithValidPaxosKnowAbout(string nodeName)
		{
			InstanceStatusInfo[] array = (from s in this.StatusMap.Values
			where s != null && s.PaxosInfo != null && s.PaxosInfo.Members.Length > 0
			select s).ToArray<InstanceStatusInfo>();
			int num = array.Count((InstanceStatusInfo s) => s.PaxosInfo.IsMember(nodeName));
			return array.Length == num;
		}

		private bool isAnalyzed;

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class NodeInstancePair
		{
			[DataMember]
			public string NodeName { get; set; }

			[DataMember]
			public int InstanceNumber { get; set; }
		}
	}
}
