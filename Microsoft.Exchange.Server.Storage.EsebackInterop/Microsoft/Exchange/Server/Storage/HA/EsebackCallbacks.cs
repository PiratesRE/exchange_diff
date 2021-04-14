using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal static class EsebackCallbacks
	{
		public static IEsebackCallbacks ManagedCallbacks
		{
			get
			{
				return EsebackCallbacks.<backing_store>ManagedCallbacks;
			}
			set
			{
				EsebackCallbacks.<backing_store>ManagedCallbacks = value;
			}
		}

		private static IEsebackCallbacks <backing_store>ManagedCallbacks;
	}
}
