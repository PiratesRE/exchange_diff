using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DDIParametersExtensions
	{
		public static void FaultIfNull(this DDIParameters properties, string parameterName)
		{
			if (properties == null)
			{
				throw new FaultException(new ArgumentNullException(parameterName).Message);
			}
		}
	}
}
