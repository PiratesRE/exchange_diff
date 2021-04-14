using System;

namespace Microsoft.Exchange.Rpc.QueueViewer
{
	internal static class QVErrorCode
	{
		// Note: this type is marked as 'beforefieldinit'.
		static QVErrorCode()
		{
			QVErrorCode.QV_E_ENDPOINT_NOT_REGISTERED = 1753;
			QVErrorCode.QV_E_RPC_SERVER_UNAVAILABLE = 1722;
			QVErrorCode.QV_E_OBJECT_NOT_FOUND = -2147220991;
			QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH = -2147220990;
			QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET = -2147220989;
			QVErrorCode.QV_E_FILTER_TYPE_NOT_SUPPORTED = -2147220988;
			QVErrorCode.QV_E_INVALID_FIELD_NAME = -2147220987;
			QVErrorCode.QV_E_INVALID_OPERATION = -2147220986;
			QVErrorCode.QV_E_TEXT_MATCHING_NOT_SUPPORTED = -2147220985;
			QVErrorCode.QV_E_INCOMPLETE_IDENTITY = -2147220984;
			QVErrorCode.QV_E_INVALID_IDENTITY_FOR_EQUALITY = -2147220983;
			QVErrorCode.QV_E_MESSAGE_NOT_SUSPENDED = -2147220982;
			QVErrorCode.QV_E_TOO_MANY_RESULTS = -2147220981;
			QVErrorCode.QV_E_INVALID_IDENTITY_STRING = -2147220980;
			QVErrorCode.QV_E_COMPARISON_NOT_SUPPORTED = -2147220979;
			QVErrorCode.QV_E_INVALID_CLIENT_DATA = -2147220978;
			QVErrorCode.QV_E_INVALID_SERVER_DATA = -2147220977;
			QVErrorCode.QV_E_INVALID_SERVER_VERSION = -2147220976;
			QVErrorCode.QV_E_INVALID_SERVER_COLLECTION = -2147220975;
			QVErrorCode.QV_E_QUEUE_RESUBMIT_IN_PROGRESS = -2147220974;
			QVErrorCode.QV_E_REDIRECT_MESSAGE_IN_PROGRESS = -2147220973;
		}

		public static int QV_E_OBJECT_NOT_FOUND;

		public static int QV_E_MULTIPLE_IDENTITY_MATCH;

		public static int QV_E_AMBIGUOUS_PARAMETER_SET;

		public static int QV_E_FILTER_TYPE_NOT_SUPPORTED;

		public static int QV_E_INVALID_FIELD_NAME;

		public static int QV_E_INVALID_OPERATION;

		public static int QV_E_TEXT_MATCHING_NOT_SUPPORTED;

		public static int QV_E_INCOMPLETE_IDENTITY;

		public static int QV_E_INVALID_IDENTITY_FOR_EQUALITY;

		public static int QV_E_MESSAGE_NOT_SUSPENDED;

		public static int QV_E_TOO_MANY_RESULTS;

		public static int QV_E_INVALID_IDENTITY_STRING;

		public static int QV_E_COMPARISON_NOT_SUPPORTED;

		public static int QV_E_INVALID_CLIENT_DATA;

		public static int QV_E_INVALID_SERVER_DATA;

		public static int QV_E_INVALID_SERVER_VERSION;

		public static int QV_E_INVALID_SERVER_COLLECTION;

		public static int QV_E_QUEUE_RESUBMIT_IN_PROGRESS;

		public static int QV_E_REDIRECT_MESSAGE_IN_PROGRESS;

		public static int QV_E_RPC_SERVER_UNAVAILABLE = 1722;

		public static int QV_E_ENDPOINT_NOT_REGISTERED = 1753;

		public static int QV_E_UNKNOWN_INTERFACE = 1717;
	}
}
