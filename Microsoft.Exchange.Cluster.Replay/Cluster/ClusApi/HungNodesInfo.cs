using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class HungNodesInfo
	{
		public int CurrentGumId { get; private set; }

		public AmServerName CurrentLockOwnerName { get; private set; }

		public long HungNodesAsBitmask { get; private set; }

		public Dictionary<int, AmServerName> NodeMap { get; private set; }

		private HungNodesInfo(int currentGumId, AmServerName lockOwnerName, long hungNodesMask)
		{
			this.CurrentGumId = currentGumId;
			this.CurrentLockOwnerName = lockOwnerName;
			this.HungNodesAsBitmask = hungNodesMask;
			this.NodeMap = HungNodesInfo.GenerateHungNodeMap(hungNodesMask);
		}

		public static Dictionary<int, AmServerName> GenerateHungNodeMap(long hungNodesMask)
		{
			Dictionary<int, AmServerName> dictionary = new Dictionary<int, AmServerName>(64);
			foreach (int num in AmClusterNode.GetNodeIdsFromNodeMask(hungNodesMask))
			{
				AmServerName nameById = AmClusterNode.GetNameById(num);
				if (!AmServerName.IsNullOrEmpty(nameById))
				{
					dictionary[num] = nameById;
				}
				else
				{
					AmTrace.Error("Failed to map nodeId {0} to node name", new object[]
					{
						num
					});
				}
			}
			return dictionary;
		}

		public static HungNodesInfo GetNodesHungInClusDbUpdate()
		{
			HungNodesInfo result = null;
			TimeSpan timeout = TimeSpan.FromSeconds((double)RegistryParameters.OpenClusterTimeoutInSec);
			using (AmCluster amCluster = AmCluster.OpenByName(AmServerName.LocalComputerName, timeout, string.Empty))
			{
				int num = 0;
				AmServerName currentGumLockOwnerInfo = amCluster.GetCurrentGumLockOwnerInfo(out num);
				if (!AmServerName.IsNullOrEmpty(currentGumLockOwnerInfo))
				{
					Thread.Sleep(RegistryParameters.ClusdbHungNodesConfirmDurationInMSec);
					string context = string.Format("GUM={0}", num);
					using (AmCluster amCluster2 = AmCluster.OpenByName(currentGumLockOwnerInfo, timeout, context))
					{
						using (IAmClusterNode amClusterNode = amCluster2.OpenNode(currentGumLockOwnerInfo))
						{
							int num2 = 0;
							long hungNodesMask = amClusterNode.GetHungNodesMask(out num2);
							if (num != num2)
							{
								throw new HungDetectionGumIdChangedException(num, num2, currentGumLockOwnerInfo.ToString(), hungNodesMask);
							}
							result = new HungNodesInfo(num, currentGumLockOwnerInfo, hungNodesMask);
						}
					}
				}
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendFormat("GUM ID: {0} Owner: {1}\nHung nodes:\n", this.CurrentGumId, this.CurrentLockOwnerName);
			foreach (KeyValuePair<int, AmServerName> keyValuePair in this.NodeMap)
			{
				stringBuilder.AppendFormat("{0} => {1}\n", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}
	}
}
