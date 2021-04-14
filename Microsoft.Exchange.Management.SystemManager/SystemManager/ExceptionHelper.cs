using System;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ExceptionHelper
	{
		public static bool IsUICriticalException(Exception ex)
		{
			return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException || ex is IndexOutOfRangeException || ex is AccessViolationException || ex is SecurityException;
		}

		public static bool IsWellknownExceptionFromServer(Exception ex)
		{
			return ex != null && (ex is RemoteException || ex is PSRemotingTransportException || ex is CommandNotFoundException || ex is ParameterBindingException || ex is InvalidOperationException || ex is PSRemotingDataStructureException);
		}

		public static bool IsWellknownCommandExecutionException(Exception ex)
		{
			return ex is CommandExecutionException && ExceptionHelper.IsWellknownExceptionFromServer(ex.InnerException);
		}
	}
}
