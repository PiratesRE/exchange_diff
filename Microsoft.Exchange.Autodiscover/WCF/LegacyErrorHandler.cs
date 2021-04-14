using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal class LegacyErrorHandler : IErrorHandler
	{
		public bool HandleError(Exception error)
		{
			LegacyErrorHandler.ReportException(error, this, HttpContext.Current);
			return true;
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			LegacyErrorHandler.ReportException(error, this, HttpContext.Current);
		}

		private static void ReportException(Exception exception, object responsibleObject, HttpContext httpContext)
		{
			bool flag = true;
			if (exception is FaultException)
			{
				FaultException ex = exception as FaultException;
				if (ex.Code.IsSenderFault)
				{
					flag = false;
				}
			}
			try
			{
				if (flag)
				{
					Common.ReportException(exception, responsibleObject, httpContext);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
