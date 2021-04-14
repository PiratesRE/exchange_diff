using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingBaseException : Exception
	{
		protected TrackingBaseException(TrackingError trackingError, Exception innerException, bool isAlreadyLogged) : base(trackingError.ToString(), innerException)
		{
			this.trackingError = trackingError;
			this.IsAlreadyLogged = isAlreadyLogged;
			ErrorCode errorCode;
			if (EnumValidator<ErrorCode>.TryParse(trackingError.ErrorCode, EnumParseOptions.Default, out errorCode))
			{
				this.isOverBudgetError = (errorCode == ErrorCode.BudgetExceeded || errorCode == ErrorCode.TimeBudgetExceeded || errorCode == ErrorCode.TotalBudgetExceeded);
			}
		}

		public bool IsAlreadyLogged { get; private set; }

		public TrackingError TrackingError
		{
			get
			{
				return this.trackingError;
			}
		}

		public bool IsOverBudgetError
		{
			get
			{
				return this.isOverBudgetError;
			}
		}

		internal static string FormatData(string dataFormat, params object[] dataParams)
		{
			if (!string.IsNullOrEmpty(dataFormat))
			{
				return string.Format(dataFormat, dataParams);
			}
			return string.Empty;
		}

		private bool isOverBudgetError;

		protected TrackingError trackingError;
	}
}
