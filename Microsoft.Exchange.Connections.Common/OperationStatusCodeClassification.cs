using System;
using System.Globalization;

namespace Microsoft.Exchange.Connections.Common
{
	internal static class OperationStatusCodeClassification
	{
		public static bool IsLogonFailedDueToInvalidConnectionSettings(OperationStatusCode result)
		{
			OperationStatusCodeClassification.ThrowIfStatusIsNotFailure(result);
			switch (result)
			{
			case OperationStatusCode.ErrorInvalidCredentials:
			case OperationStatusCode.ErrorCannotCommunicateWithRemoteServer:
				return false;
			case OperationStatusCode.ErrorInvalidRemoteServer:
			case OperationStatusCode.ErrorUnsupportedProtocolVersion:
				return true;
			default:
				throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Unexpected OperationStatusCode value encountered in IsLogonFailedDueToInvalidConnectionSettings(): {0}", new object[]
				{
					result
				}));
			}
		}

		public static bool IsLogonFailedDueToInvalidCredentials(OperationStatusCode result)
		{
			OperationStatusCodeClassification.ThrowIfStatusIsNotFailure(result);
			switch (result)
			{
			case OperationStatusCode.ErrorInvalidCredentials:
				return true;
			case OperationStatusCode.ErrorCannotCommunicateWithRemoteServer:
			case OperationStatusCode.ErrorInvalidRemoteServer:
			case OperationStatusCode.ErrorUnsupportedProtocolVersion:
				return false;
			default:
				throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Unexpected OperationStatusCode value encountered in IsLogonFailedDueToInvalidCredentials(): {0}", new object[]
				{
					result
				}));
			}
		}

		public static bool IsLogonFailedDueToServerHavingTransientProblems(OperationStatusCode result)
		{
			OperationStatusCodeClassification.ThrowIfStatusIsNotFailure(result);
			switch (result)
			{
			case OperationStatusCode.ErrorInvalidCredentials:
			case OperationStatusCode.ErrorInvalidRemoteServer:
			case OperationStatusCode.ErrorUnsupportedProtocolVersion:
				return false;
			case OperationStatusCode.ErrorCannotCommunicateWithRemoteServer:
				return true;
			default:
				throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Unexpected OperationStatusCode value encountered in IsLogonFailedDueToServerHavingTransientProblems(): {0}", new object[]
				{
					result
				}));
			}
		}

		private static void ThrowIfStatusIsNotFailure(OperationStatusCode result)
		{
			if (result == OperationStatusCode.None || result == OperationStatusCode.Success)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The specified status code does not indicate a failure: {0}.", new object[]
				{
					result
				}));
			}
		}
	}
}
