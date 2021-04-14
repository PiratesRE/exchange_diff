using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ReplicaInstanceStageTransitionGraph
	{
		static ReplicaInstanceStageTransitionGraph()
		{
			ReplicaInstanceStageTransitionGraph.m_graph[0, 1] = true;
			ReplicaInstanceStageTransitionGraph.m_graph[1, 2] = true;
			ReplicaInstanceStageTransitionGraph.m_graph[2, 3] = true;
		}

		public static bool IsTransitionPossible(ReplicaInstanceStage fromStage, ReplicaInstanceStage toStage)
		{
			return ReplicaInstanceStageTransitionGraph.m_graph[(int)fromStage, (int)toStage];
		}

		private static bool[,] m_graph = new bool[5, 5];
	}
}
