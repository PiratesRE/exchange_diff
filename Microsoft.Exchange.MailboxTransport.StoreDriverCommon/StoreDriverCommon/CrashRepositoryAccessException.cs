using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class CrashRepositoryAccessException : LocalizedException
	{
		public CrashRepositoryAccessException(string errorDescription, Exception innerException) : base(LocalizedString.Empty, innerException)
		{
			this.ErrorDescription = errorDescription;
		}

		public CrashRepositoryAccessException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
			this.ErrorDescription = string.Empty;
		}

		protected CrashRepositoryAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ErrorDescription = string.Empty;
		}

		public string ErrorDescription { get; private set; }
	}
}
