using System;

namespace Microsoft.Exchange.UM.Rpc
{
	internal abstract class UMErrorCode
	{
		internal static bool IsPermanent(int errorCode)
		{
			return errorCode == -2147466750;
		}

		internal static bool IsUserInputError(int errorCode)
		{
			return errorCode == -2147466743;
		}

		internal static bool IsNetworkError(int errorCode)
		{
			return errorCode == 1727 || errorCode == 1722 || errorCode == 1753;
		}

		internal const int UMSUCCESS = 0;

		internal const int UMEACCESSDENIED = 5;

		internal const int UMESERVERUNAVAILABLE = 1722;

		internal const int UMEENDPOINTNOTREGISTERED = 1753;

		internal const int UMRPCCALLFAILEDDNE = 1727;

		internal const int UMEGENERIC = -2147466752;

		internal const int UMETRANSIENT = -2147466751;

		internal const int UMEINVALIDREQUEST = -2147466750;

		internal const int UMEINCOMPATIBLEVERSION = -2147466749;

		internal const int UMENONUNIQUERECIPIENT = -2147466748;

		internal const int UMERECIPIENTNOTFOUND = -2147466747;

		internal const int UMERECOGNIZERNOTINSTALLED = -2147466746;

		internal const int UMENOSPEECHDETECTED = -2147466743;

		internal const int UMEREQUESTTIMEDOUT = -2147466742;

		internal const int UMESPEECHGRAMMARERROR = -2147466741;
	}
}
