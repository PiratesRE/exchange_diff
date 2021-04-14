using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingFatalException : TrackingBaseException
	{
		public TrackingFatalException(TrackingError trackingError, Exception innerException, bool isAlreadyLogged) : base(trackingError, innerException, isAlreadyLogged)
		{
		}

		public static TrackingFatalException RaiseE(ErrorCode errorCode)
		{
			throw new TrackingFatalException(errorCode, string.Empty, string.Empty, string.Empty, new object[0]);
		}

		public static TrackingFatalException RaiseED(ErrorCode errorCode, string dataFormat, params object[] dataParams)
		{
			throw new TrackingFatalException(errorCode, string.Empty, string.Empty, dataFormat, dataParams);
		}

		public static TrackingFatalException RaiseET(ErrorCode errorCode, string target)
		{
			throw new TrackingFatalException(errorCode, target, string.Empty, string.Empty, new object[0]);
		}

		public static TrackingFatalException RaiseETD(ErrorCode errorCode, string target, string dataFormat, params object[] dataParams)
		{
			throw new TrackingFatalException(errorCode, target, string.Empty, dataFormat, dataParams);
		}

		public static TrackingFatalException RaiseETX(ErrorCode errorCode, string target, string exception)
		{
			throw new TrackingFatalException(errorCode, target, exception, string.Empty, new object[0]);
		}

		public static TrackingFatalException RaiseEDX(ErrorCode errorCode, string exception, string dataFormat, params object[] dataParams)
		{
			throw new TrackingFatalException(errorCode, string.Empty, exception, dataFormat, dataParams);
		}

		public static TrackingFatalException AddAndRaiseE(TrackingErrorCollection errors, ErrorCode errorCode)
		{
			throw new TrackingFatalException(errors, errorCode, string.Empty, string.Empty, string.Empty, new object[0]);
		}

		public static TrackingFatalException AddAndRaiseED(TrackingErrorCollection errors, ErrorCode errorCode, string dataFormat, params object[] dataParams)
		{
			throw new TrackingFatalException(errors, errorCode, string.Empty, string.Empty, dataFormat, dataParams);
		}

		public static TrackingFatalException AddAndRaiseETD(TrackingErrorCollection errors, ErrorCode errorCode, string target, string dataFormat, params object[] dataParams)
		{
			throw new TrackingFatalException(errors, errorCode, target, string.Empty, dataFormat, dataParams);
		}

		public static TrackingFatalException AddAndRaiseETX(TrackingErrorCollection errors, ErrorCode errorCode, string target, string exception)
		{
			throw new TrackingFatalException(errors, errorCode, target, exception, string.Empty, new object[0]);
		}

		private TrackingFatalException(ErrorCode errorCode, string target, string exception, string dataFormat, params object[] dataParams) : base(new TrackingError(errorCode, target, TrackingBaseException.FormatData(dataFormat, dataParams), exception), null, false)
		{
		}

		private TrackingFatalException(TrackingErrorCollection errors, ErrorCode errorCode, string target, string exception, string dataFormat, params object[] dataParams) : base(errors.Add(errorCode, target, string.Format(dataFormat, dataParams), exception), null, true)
		{
		}
	}
}
