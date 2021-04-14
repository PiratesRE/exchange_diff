using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class StoreException : Exception
	{
		public StoreException(LID lid, ErrorCodeValue errorCode, string message, Exception innerException) : base(StoreException.FormatMessage(lid, errorCode, message), innerException)
		{
			DiagnosticContext.TraceStoreError(lid, (uint)errorCode);
			this.errorCode = errorCode;
			this.lid = lid.Value;
		}

		public StoreException(LID lid, ErrorCodeValue errorCode, string message) : this(lid, errorCode, message, null)
		{
		}

		public StoreException(LID lid, ErrorCodeValue errorCode) : this(lid, errorCode, null, null)
		{
		}

		public ErrorCodeValue Error
		{
			get
			{
				return this.errorCode;
			}
		}

		public uint Lid
		{
			get
			{
				return this.lid;
			}
		}

		private static string FormatMessage(LID lid, ErrorCodeValue errorCode, string message)
		{
			return string.Format("ErrorCode: {0}, LID: {1}{2}{3}", new object[]
			{
				errorCode,
				lid.Value,
				string.IsNullOrEmpty(message) ? string.Empty : " - ",
				message ?? string.Empty
			});
		}

		private const string ErrorCodeSerializationLabel = "errorCode";

		private const string LidSerializationLabel = "lid";

		private readonly ErrorCodeValue errorCode;

		private readonly uint lid;
	}
}
