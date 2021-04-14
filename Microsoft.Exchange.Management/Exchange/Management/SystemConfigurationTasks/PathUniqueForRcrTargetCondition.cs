using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class PathUniqueForRcrTargetCondition
	{
		public static bool Verify(string pathToCheck, List<string> paths)
		{
			if (pathToCheck == null)
			{
				return true;
			}
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			foreach (string strB in paths)
			{
				if (string.Compare(pathToCheck, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return false;
				}
			}
			return true;
		}
	}
}
