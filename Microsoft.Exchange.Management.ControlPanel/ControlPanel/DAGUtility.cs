using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class DAGUtility
	{
		public static string ExtractDAGNameFromDAGNetworkIdentity(object rawIdentity)
		{
			if (rawIdentity == null)
			{
				return string.Empty;
			}
			string text = rawIdentity as string;
			string[] array = text.Split(new char[]
			{
				'\\'
			});
			if (array.Length == 2 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]))
			{
				return array[0];
			}
			return string.Empty;
		}

		public static Identity ComposeDAGNetworkIdentity(object dagName, object dagNetworkName)
		{
			string rawIdentity = dagName + "\\" + dagNetworkName;
			return new Identity(rawIdentity, (string)dagNetworkName);
		}
	}
}
