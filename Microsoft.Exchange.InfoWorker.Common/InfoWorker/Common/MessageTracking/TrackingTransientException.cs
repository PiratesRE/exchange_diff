using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingTransientException : TrackingBaseException
	{
		public TrackingTransientException(TrackingError trackingError, Exception innerException, bool isAlreadyLogged) : base(trackingError, innerException, isAlreadyLogged)
		{
		}

		public static TrackingTransientException RaiseE(ErrorCode errorCode)
		{
			throw new TrackingTransientException(errorCode, string.Empty, string.Empty, string.Empty, new object[0]);
		}

		public static TrackingTransientException RaiseETD(ErrorCode errorCode, string target, string dataFormat, params object[] dataParams)
		{
			throw new TrackingTransientException(errorCode, target, string.Empty, dataFormat, dataParams);
		}

		public static TrackingTransientException RaiseETX(ErrorCode errorCode, string target, string exception)
		{
			throw new TrackingTransientException(errorCode, target, exception, string.Empty, new object[0]);
		}

		public static TrackingTransientException AddAndRaiseE(TrackingErrorCollection errors, ErrorCode errorCode)
		{
			throw new TrackingTransientException(errors, errorCode, string.Empty, string.Empty, string.Empty, new object[0]);
		}

		public static TrackingTransientException AddAndRaiseETD(TrackingErrorCollection errors, ErrorCode errorCode, string target, string dataFormat, params object[] dataParams)
		{
			throw new TrackingTransientException(errors, errorCode, target, string.Empty, dataFormat, dataParams);
		}

		public static TrackingTransientException AddAndRaiseETX(TrackingErrorCollection errors, ErrorCode errorCode, string target, string exception)
		{
			throw new TrackingTransientException(errors, errorCode, target, exception, string.Empty, new object[0]);
		}

		private TrackingTransientException(ErrorCode errorCode, string target, string exception, string dataFormat, params object[] dataParams) : base(new TrackingError(errorCode, target, TrackingBaseException.FormatData(dataFormat, dataParams), exception), null, false)
		{
		}

		private TrackingTransientException(TrackingErrorCollection errors, ErrorCode errorCode, string target, string exception, string dataFormat, params object[] dataParams) : base(errors.Add(errorCode, target, string.Format(dataFormat, dataParams), exception), null, true)
		{
		}
	}
}
