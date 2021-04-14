using System;
using System.ServiceProcess;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Search
{
	internal static class SearchCommon
	{
		internal static bool IsServiceRunning(string serviceName, string serverName, out bool scError)
		{
			bool result = true;
			scError = false;
			using (ServiceController serviceController = new ServiceController(serviceName, serverName))
			{
				try
				{
					result = (serviceController.Status == ServiceControllerStatus.Running);
				}
				catch (InvalidOperationException)
				{
					result = false;
					scError = true;
				}
			}
			return result;
		}

		private static LocalizedString ShortErrorMsgFromException(Exception exception)
		{
			if (exception.InnerException != null)
			{
				return Strings.MapiTransactionShortErrorMsgFromExceptionWithInnerException(exception.GetType().ToString(), exception.Message, exception.InnerException.GetType().ToString(), exception.InnerException.Message);
			}
			return Strings.MapiTransactionShortErrorMsgFromException(exception.GetType().ToString(), exception.Message);
		}

		internal static LocalizedString DiagnoseException(string serverFqdn, Guid mdbGuid, Exception exception)
		{
			try
			{
				bool flag = false;
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", serverFqdn, null, null, null))
				{
					MdbStatus[] array = exRpcAdmin.ListMdbStatus(new Guid[]
					{
						mdbGuid
					});
					if (array.Length != 0)
					{
						flag = ((array[0].Status & MdbStatusFlags.Online) == MdbStatusFlags.Online);
					}
				}
				if (!flag)
				{
					return Strings.MapiTransactionDiagnosticTargetDatabaseDismounted;
				}
			}
			catch (MapiPermanentException exception2)
			{
				return Strings.MapiTransactionDiagnosticStoreStateCheckFailure(SearchCommon.ShortErrorMsgFromException(exception2));
			}
			catch (MapiRetryableException exception3)
			{
				return Strings.MapiTransactionDiagnosticStoreStateCheckFailure(SearchCommon.ShortErrorMsgFromException(exception3));
			}
			return SearchCommon.ShortErrorMsgFromException(exception);
		}

		internal const uint INDEX_PARTIAL = 1U;
	}
}
