using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CorruptSyncStateException : CorruptDataException
	{
		public CorruptSyncStateException(string syncStateName, LocalizedString message) : base(message)
		{
			this.SyncStateName = syncStateName;
		}

		public CorruptSyncStateException(string message, Exception innerException) : this(new LocalizedString(message), innerException)
		{
		}

		public CorruptSyncStateException(LocalizedString message, Exception innerException) : this("<NoName>", message, innerException)
		{
		}

		public CorruptSyncStateException(string syncStateName, LocalizedString message, Exception innerException) : base(message, innerException)
		{
			this.SyncStateName = syncStateName;
		}

		protected CorruptSyncStateException(SerializationInfo info, StreamingContext context) : this("<NoName>", info, context)
		{
		}

		protected CorruptSyncStateException(string syncStateName, SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.SyncStateName = syncStateName;
		}

		public string SyncStateName { get; private set; }

		public const string NoName = "<NoName>";
	}
}
