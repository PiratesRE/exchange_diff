using System;
using Microsoft.Exchange.Connections.Common;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	internal static class EasServerResponseExtensions
	{
		internal static void ThrowIfStatusIsFailed<T>(this IEasServerResponse<T> response, T status) where T : struct, IConvertible
		{
			if (!response.IsSucceeded(status))
			{
				EasServerResponseExtensions.ThrowAppropriateException(response, status.ToInt32(null), status.ToString());
			}
		}

		private static void ThrowAppropriateException(IHaveAnHttpStatus response, int statusInt, string statusString)
		{
			EasCommonStatus easCommonStatus = (EasCommonStatus)(statusInt & 65280);
			EasCommonStatus easCommonStatus2 = easCommonStatus;
			if (easCommonStatus2 <= EasCommonStatus.RequiresFolderSync)
			{
				if (easCommonStatus2 == EasCommonStatus.RequiresSyncKeyReset)
				{
					throw new EasRequiresSyncKeyResetException(statusString);
				}
				if (easCommonStatus2 == EasCommonStatus.RequiresFolderSync)
				{
					throw new EasRequiresFolderSyncException(statusString);
				}
			}
			else
			{
				if (easCommonStatus2 == EasCommonStatus.PermanentError)
				{
					throw new EasCommandFailedPermanentException(statusString, response.HttpStatus.ToString());
				}
				if (easCommonStatus2 == EasCommonStatus.BackOff)
				{
					throw new EasRetryAfterException(TimeSpan.FromMinutes(5.0), statusString);
				}
			}
			throw new EasCommandFailedTransientException(statusString, response.HttpStatus.ToString());
		}
	}
}
