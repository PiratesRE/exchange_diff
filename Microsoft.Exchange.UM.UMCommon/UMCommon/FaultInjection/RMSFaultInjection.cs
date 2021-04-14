using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RMSFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			if (exceptionType != null)
			{
				if (RMSFaultInjection.RMSPermanentException.Equals(exceptionType))
				{
					exception = new RightsManagementPermanentException(RightsManagementFailureCode.UnknownFailure, new LocalizedString("This is a test purpose exception for testing"));
					return true;
				}
				if (RMSFaultInjection.RMSTransientException.Equals(exceptionType))
				{
					exception = new RightsManagementTransientException(new LocalizedString("This is a test purpose exception for testing"));
					return true;
				}
			}
			return false;
		}

		internal const uint RMSEncryptVoiceMail = 3034983741U;

		internal const uint RMSDecryptVoiceMail = 4108725565U;

		internal const uint RMSDecryptAttachement = 2900766013U;

		internal const uint StageNotRetry = 2162568509U;

		private static readonly string RMSPermanentException = "RightsManagementPermanentException";

		private static readonly string RMSTransientException = "RightsManagementTransientException";
	}
}
