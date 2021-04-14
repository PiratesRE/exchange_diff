using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal sealed class ScopeMappingEndpointManager
	{
		private ScopeMappingEndpointManager()
		{
		}

		internal static ScopeMappingEndpointManager Instance
		{
			get
			{
				if (ScopeMappingEndpointManager.instance == null)
				{
					lock (ScopeMappingEndpointManager.locker)
					{
						if (ScopeMappingEndpointManager.instance == null)
						{
							ScopeMappingEndpointManager.instance = new ScopeMappingEndpointManager();
						}
					}
				}
				return ScopeMappingEndpointManager.instance;
			}
		}

		private static ScopeMappingEndpointManager instance = null;

		private static object locker = new object();
	}
}
