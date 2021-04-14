using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class ClusApiHook : IClusApiHook
	{
		public static void SetClusApiHook(IClusApiHook newHook)
		{
			ClusApiHook.instance = newHook;
		}

		public int CallBack(ClusApiHooks api, string hintStr, Func<int> func)
		{
			return func();
		}

		public static int CallBackDriver(ClusApiHooks api, string hintStr, Func<int> func)
		{
			return ClusApiHook.instance.CallBack(api, hintStr, func);
		}

		private static IClusApiHook instance = new ClusApiHook();
	}
}
