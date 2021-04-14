using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetRequestKey
	{
		public string ServerName { get; private set; }

		public DateTime RequestCreationTimeUtc { get; private set; }

		public string UniqueStr { get; private set; }

		public SafetyNetRequestKey(SafetyNetInfo info) : this(info.SourceServerName, info.FailoverTimeUtc, info.UniqueStr)
		{
		}

		public SafetyNetRequestKey(string serverName, DateTime creationTimeUtc, string uniqueString)
		{
			this.ServerName = serverName;
			this.RequestCreationTimeUtc = creationTimeUtc;
			this.UniqueStr = uniqueString;
		}

		public static SafetyNetRequestKey Parse(string requestKeyName)
		{
			string[] array = requestKeyName.Split(new char[]
			{
				'*'
			});
			return new SafetyNetRequestKey(array[0], DateTimeHelper.ParseIntoDateTime(array[1]), array[2]);
		}

		public override string ToString()
		{
			if (this.m_toString == null)
			{
				this.m_toString = string.Join(SafetyNetRequestKey.FieldSeparatorStr, new string[]
				{
					this.ServerName,
					DateTimeHelper.ToPersistedString(this.RequestCreationTimeUtc),
					this.UniqueStr
				});
			}
			return this.m_toString;
		}

		private const char FieldSeparator = '*';

		private static readonly string FieldSeparatorStr = new string('*', 1);

		private string m_toString;
	}
}
