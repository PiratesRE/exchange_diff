using System;
using System.ServiceModel;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	internal static class ClientServices
	{
		public static void CallService(ClientServices.GenericCallDelegate del)
		{
			try
			{
				del();
			}
			catch (TimeoutException ex)
			{
				throw new FailedCommunicationException(ex.Message, ex);
			}
			catch (CommunicationException ex2)
			{
				throw new FailedCommunicationException(ex2.Message, ex2);
			}
		}

		public static NetNamedPipeBinding SetupBinding(TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			return new NetNamedPipeBinding
			{
				OpenTimeout = openTimeout,
				SendTimeout = sendTimeout,
				ReceiveTimeout = receiveTimeout
			};
		}

		public delegate void GenericCallDelegate();
	}
}
