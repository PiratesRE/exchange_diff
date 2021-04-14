using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class AppConfigurationErrorsException : AIPermanentException
	{
		public AppConfigurationErrorsException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
		}
	}
}
