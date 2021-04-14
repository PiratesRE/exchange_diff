using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMLicensingFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			if (exceptionType != null && UMLicensingFaultInjection.RecipientTaskException.Equals(exceptionType))
			{
				exception = new StorageTransientException(new LocalizedString("This is a test purpose exception for testing"));
				return true;
			}
			return false;
		}

		internal const uint UMLicensingRetryOnUMDisable = 3341167933U;

		private static readonly string RecipientTaskException = "RecipientTaskException";
	}
}
