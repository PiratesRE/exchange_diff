using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[Serializable]
	internal class MissingRequestedDatabaseException : MailboxServerLocatorException
	{
		public MissingRequestedDatabaseException(string databaseId) : base(databaseId)
		{
		}

		public MissingRequestedDatabaseException(string databaseId, Exception innerException) : base(databaseId, innerException)
		{
		}

		protected MissingRequestedDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
