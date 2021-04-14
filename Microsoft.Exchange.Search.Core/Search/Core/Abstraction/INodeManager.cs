using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface INodeManager
	{
		bool AreAllNodesHealthy();

		bool IsNodeHealthy(string nodeName);

		bool IsNodeStopped(string nodeName);

		void StartNode(string nodeName);

		void KillNode(string nodeName);

		void KillAndRestartNode(string nodeName);

		void StopNode(string nodeName);
	}
}
