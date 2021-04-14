using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace Microsoft.Exchange.Common
{
	internal static class ServiceControllerUtils
	{
		internal static bool IsInstalled(string serviceName)
		{
			bool result;
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				result = ServiceControllerUtils.IsInstalled(serviceController);
			}
			return result;
		}

		internal static bool IsInstalled(ServiceController serviceController)
		{
			if (serviceController == null)
			{
				throw new ArgumentNullException("serviceController");
			}
			bool result = false;
			try
			{
				ServiceControllerStatus status = serviceController.Status;
				result = true;
			}
			catch (InvalidOperationException ex)
			{
				Win32Exception ex2 = ex.InnerException as Win32Exception;
				if (ex2 != null && (1060 == ex2.NativeErrorCode || 1072 == ex2.NativeErrorCode))
				{
					result = false;
				}
				else
				{
					if (ex2 == null || 1058 != ex2.NativeErrorCode)
					{
						throw;
					}
					result = true;
				}
			}
			return result;
		}
	}
}
