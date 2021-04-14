using System;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[Serializable]
	public class DalRetrievalException : FfoReportingException
	{
		internal DalRetrievalException()
		{
		}

		internal DalRetrievalException(string message, Exception innerException)
		{
		}
	}
}
