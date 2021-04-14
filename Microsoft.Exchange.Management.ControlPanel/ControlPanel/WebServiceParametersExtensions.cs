using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class WebServiceParametersExtensions
	{
		public static void FaultIfNull(this WebServiceParameters properties)
		{
			if (properties == null)
			{
				throw new FaultException(new ArgumentNullException("properties").Message);
			}
		}
	}
}
