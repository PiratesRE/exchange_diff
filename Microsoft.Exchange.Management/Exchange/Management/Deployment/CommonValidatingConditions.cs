using System;
using System.ComponentModel;
using System.ServiceProcess;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CommonValidatingConditions
	{
		public static ValidatingCondition StoreServiceExistsAndIsRunning
		{
			get
			{
				return new ValidatingCondition(new ValidationDelegate(CommonValidatingConditions.StoreServiceExistsAndIsRunningCheck), Strings.VerifyStoreServiceExists, false);
			}
		}

		private static bool StoreServiceExistsAndIsRunningCheck()
		{
			bool flag = false;
			using (ServiceController serviceController = new ServiceController("MSExchangeIS"))
			{
				try
				{
					ServiceControllerStatus status = serviceController.Status;
					flag = true;
				}
				catch (InvalidOperationException ex)
				{
					Win32Exception ex2 = ex.InnerException as Win32Exception;
					if (ex2 == null || 1060 != ex2.NativeErrorCode)
					{
						throw;
					}
					flag = false;
				}
			}
			if (!flag)
			{
				throw new LocalizedException(Strings.ExceptionServiceDoesNotExist("MSExchangeIS"));
			}
			bool result;
			using (ServiceController serviceController2 = new ServiceController("MSExchangeIS"))
			{
				if (serviceController2.Status != ServiceControllerStatus.Running)
				{
					throw new LocalizedException(Strings.ExceptionServiceIsNotRunning("MSExchangeIS"));
				}
				result = true;
			}
			return result;
		}
	}
}
