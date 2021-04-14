using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport
{
	internal static class LocalComputer
	{
		public static bool TryGetIPAddresses(out List<IPAddress> localIPAddresses, out NetworkInformationException exception)
		{
			try
			{
				localIPAddresses = ComputerInformation.GetLocalIPAddresses();
				exception = null;
				return true;
			}
			catch (NetworkInformationException ex)
			{
				exception = ex;
			}
			localIPAddresses = null;
			return false;
		}
	}
}
