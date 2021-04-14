using System;
using System.Reflection;
using System.Resources;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	internal static class CoreResources
	{
		public static string WorkItemFailedDefaultError(string result)
		{
			return string.Format(CoreResources.ResourceManager.GetString("WorkItemFailedDefaultError"), result);
		}

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Office.Datacenter.WorkerTaskFramework.CoreResources", typeof(CoreResources).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			WorkItemFailedDefaultError
		}
	}
}
