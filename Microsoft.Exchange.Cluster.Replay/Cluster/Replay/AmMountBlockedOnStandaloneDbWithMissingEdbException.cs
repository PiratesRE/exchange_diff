using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmMountBlockedOnStandaloneDbWithMissingEdbException : AmServerException
	{
		public AmMountBlockedOnStandaloneDbWithMissingEdbException(string dbName, long highestLogGen, string edbFilePath) : base(ReplayStrings.AmMountBlockedOnStandaloneDbWithMissingEdbException(dbName, highestLogGen, edbFilePath))
		{
			this.dbName = dbName;
			this.highestLogGen = highestLogGen;
			this.edbFilePath = edbFilePath;
		}

		public AmMountBlockedOnStandaloneDbWithMissingEdbException(string dbName, long highestLogGen, string edbFilePath, Exception innerException) : base(ReplayStrings.AmMountBlockedOnStandaloneDbWithMissingEdbException(dbName, highestLogGen, edbFilePath), innerException)
		{
			this.dbName = dbName;
			this.highestLogGen = highestLogGen;
			this.edbFilePath = edbFilePath;
		}

		protected AmMountBlockedOnStandaloneDbWithMissingEdbException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.highestLogGen = (long)info.GetValue("highestLogGen", typeof(long));
			this.edbFilePath = (string)info.GetValue("edbFilePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("highestLogGen", this.highestLogGen);
			info.AddValue("edbFilePath", this.edbFilePath);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public long HighestLogGen
		{
			get
			{
				return this.highestLogGen;
			}
		}

		public string EdbFilePath
		{
			get
			{
				return this.edbFilePath;
			}
		}

		private readonly string dbName;

		private readonly long highestLogGen;

		private readonly string edbFilePath;
	}
}
