using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ReliableActions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class ActionInfo
	{
		public ActionInfo(Guid id, DateTime timestamp, string commandType, string rawData)
		{
			this.Version = 1;
			this.Id = id;
			this.Timestamp = timestamp;
			this.CommandType = commandType;
			this.RawData = rawData;
		}

		[DataMember]
		public int Version { get; private set; }

		[DataMember]
		public Guid Id { get; private set; }

		[DataMember]
		public DateTime Timestamp { get; private set; }

		[DataMember]
		public string RawData { get; private set; }

		[DataMember]
		public string CommandType { get; private set; }

		public const int CurrentVersion = 1;
	}
}
