using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[Serializable]
	internal class MailboxServerLocatorException : DatabaseNotFoundException
	{
		public MailboxServerLocatorException(string databaseId) : base(databaseId)
		{
		}

		public MailboxServerLocatorException(string databaseId, Exception innerException) : base(databaseId, innerException)
		{
		}

		protected MailboxServerLocatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
