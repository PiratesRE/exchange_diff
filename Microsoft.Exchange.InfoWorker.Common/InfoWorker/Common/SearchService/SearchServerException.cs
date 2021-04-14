using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.SearchService
{
	[Serializable]
	internal class SearchServerException : LocalizedException
	{
		internal SearchServerException(int errorCode) : base(SearchServerException.FormatErrorCode(errorCode))
		{
			base.ErrorCode = errorCode;
		}

		internal SearchServerException(int errorCode, string serverMessage) : base(SearchServerException.FormatErrorCode(errorCode))
		{
			base.ErrorCode = errorCode;
			this.ServerMessage = serverMessage;
		}

		internal SearchServerException(int errorCode, LocalizedString message) : base(message)
		{
			base.ErrorCode = errorCode;
		}

		internal SearchServerException(int errorCode, LocalizedString message, Exception innerException) : base(message, innerException)
		{
			base.ErrorCode = errorCode;
		}

		protected SearchServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string Message
		{
			get
			{
				if (string.IsNullOrEmpty(this.ServerMessage))
				{
					return base.Message;
				}
				return base.Message + " " + Strings.SearchServerErrorMessage(this.ServerMessage);
			}
		}

		internal string ServerMessage
		{
			get
			{
				return this.serverMessage;
			}
			set
			{
				this.serverMessage = value;
			}
		}

		private static LocalizedString FormatErrorCode(int errorCode)
		{
			switch (errorCode)
			{
			case -2147220991:
				return Strings.UnknownError;
			case -2147220990:
				return Strings.SearchObjectNotFound;
			case -2147220989:
				return Strings.WrongTargetServer;
			case -2147220988:
				return Strings.SearchArgument;
			case -2147220987:
				return Strings.ObjectNotFound;
			case -2147220986:
				return Strings.StorePermanantError;
			case -2147220985:
				return Strings.StoreTransientError;
			case -2147220984:
				return Strings.AqsParserError;
			case -2147220983:
				return Strings.ServerShutdown;
			case -2147220981:
				return Strings.SearchQueryEmpty;
			case -2147220979:
				return Strings.SearchThrottled;
			case -2147220978:
				return Strings.SearchDisabled;
			}
			return Strings.SearchServerError(errorCode);
		}

		private string serverMessage;
	}
}
