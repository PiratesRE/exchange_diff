using System;

namespace Microsoft.Exchange.Management.SnapIn
{
	public interface INodeSelectionService
	{
		void SelectNodeByName(string nodeName);

		void SelectNodeAndResultPaneByName(string nodeName, string resultPaneName);
	}
}
