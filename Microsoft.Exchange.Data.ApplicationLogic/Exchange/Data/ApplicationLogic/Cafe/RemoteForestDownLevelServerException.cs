using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[Serializable]
	internal class RemoteForestDownLevelServerException : MailboxServerLocatorException
	{
		public string ResourceForest { get; private set; }

		public RemoteForestDownLevelServerException(string databaseId, string resourceForest) : base(databaseId)
		{
			this.ResourceForest = resourceForest;
		}

		public RemoteForestDownLevelServerException(string databaseId, string resourceForest, Exception innerException) : base(databaseId, innerException)
		{
			this.ResourceForest = resourceForest;
		}

		protected RemoteForestDownLevelServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
