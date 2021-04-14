using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class ProcessWideServiceType
	{
		public static ServiceType ServiceType
		{
			get
			{
				return ProcessWideServiceType.serviceType;
			}
			set
			{
				lock (ProcessWideServiceType.serviceTypeLock)
				{
					ProcessWideServiceType.serviceType = value;
				}
			}
		}

		private static ServiceType serviceType = ServiceType.EOPService;

		private static object serviceTypeLock = new object();

		public static readonly HygienePropertyDefinition ServiceTypeProp = new HygienePropertyDefinition("serviceType", typeof(ServiceType));
	}
}
