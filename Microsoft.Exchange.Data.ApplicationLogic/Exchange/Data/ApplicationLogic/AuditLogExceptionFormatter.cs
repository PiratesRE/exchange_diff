using System;
using System.Net;
using System.Security.Authentication;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class AuditLogExceptionFormatter : IExceptionLogFormatter
	{
		public static IExceptionLogFormatter Instance
		{
			get
			{
				return AuditLogExceptionFormatter.instance;
			}
		}

		public string FormatException(Exception exception)
		{
			AuditLogServiceException ex = exception as AuditLogServiceException;
			if (ex != null)
			{
				return string.Format("{0}[{1}]", AuditingOpticsLogger.DefaultExceptionFormatter.FormatException(exception), ex.Code);
			}
			if (exception is AuthenticationException || exception is WebException)
			{
				return string.Format("{0}[{1}]", AuditingOpticsLogger.DefaultExceptionFormatter.FormatException(exception), exception.Message);
			}
			return AuditingOpticsLogger.DefaultExceptionFormatter.FormatException(exception);
		}

		private static readonly IExceptionLogFormatter instance = new AuditLogExceptionFormatter();
	}
}
