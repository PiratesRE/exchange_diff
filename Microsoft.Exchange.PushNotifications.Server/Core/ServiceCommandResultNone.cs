using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	[DataContract]
	public class ServiceCommandResultNone
	{
		private ServiceCommandResultNone()
		{
		}

		public static ServiceCommandResultNone Instance
		{
			get
			{
				return ServiceCommandResultNone.instance;
			}
		}

		private static readonly ServiceCommandResultNone instance = new ServiceCommandResultNone();
	}
}
