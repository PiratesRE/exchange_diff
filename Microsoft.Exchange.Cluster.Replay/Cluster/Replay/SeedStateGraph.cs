using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SeedStateGraph
	{
		static SeedStateGraph()
		{
			SeedStateGraph.m_stateGraph[0, 1] = true;
			SeedStateGraph.m_stateGraph[0, 4] = true;
			SeedStateGraph.m_stateGraph[0, 5] = true;
			SeedStateGraph.m_stateGraph[1, 2] = true;
			SeedStateGraph.m_stateGraph[1, 4] = true;
			SeedStateGraph.m_stateGraph[1, 5] = true;
			SeedStateGraph.m_stateGraph[2, 3] = true;
			SeedStateGraph.m_stateGraph[2, 4] = true;
			SeedStateGraph.m_stateGraph[2, 5] = true;
			SeedStateGraph.m_stateGraph[5, 5] = true;
			SeedStateGraph.m_stateGraph[3, 5] = true;
		}

		public static bool IsTransitionPossible(SeederState fromState, SeederState toState)
		{
			return SeedStateGraph.m_stateGraph[(int)fromState, (int)toState];
		}

		private static bool[,] m_stateGraph = new bool[6, 6];
	}
}
