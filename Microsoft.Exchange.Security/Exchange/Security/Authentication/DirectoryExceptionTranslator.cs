using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class DirectoryExceptionTranslator : IExceptionTranslator
	{
		public bool TryTranslate(Exception exception, out Exception translatedException)
		{
			translatedException = null;
			if (exception is DataValidationException || exception is DataSourceOperationException || exception is DataSourceTransientException)
			{
				translatedException = new TokenMungingException("Directory exception occurred during token munging", exception);
				return true;
			}
			return false;
		}
	}
}
