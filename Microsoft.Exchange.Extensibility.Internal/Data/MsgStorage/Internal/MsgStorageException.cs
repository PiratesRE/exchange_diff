using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	[Serializable]
	public class MsgStorageException : ExchangeDataException
	{
		internal MsgStorageException(MsgStorageErrorCode errorCode, string message) : base(string.Format("{0}, Errorcode = {1}", message, errorCode))
		{
			this.errorCode = errorCode;
		}

		internal MsgStorageException(MsgStorageErrorCode errorCode, string message, int hResult) : base(string.Format("{0}, Errorcode = {1}, HResult = {1}", message, errorCode, hResult))
		{
			base.HResult = hResult;
			this.errorCode = errorCode;
		}

		internal MsgStorageException(MsgStorageErrorCode errorCode, string message, Exception exc) : base(message, exc)
		{
			this.errorCode = errorCode;
		}

		protected MsgStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public MsgStorageErrorCode MsgStorageErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private const string ExceptionMessageFormatString = "{0}, Errorcode = {1}";

		private const string ExceptionMessageHResultFormatString = "{0}, Errorcode = {1}, HResult = {1}";

		private MsgStorageErrorCode errorCode;
	}
}
