using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementFileNotSubsetException : LastLogReplacementException
	{
		public LastLogReplacementFileNotSubsetException(string dbCopy, string subsetFile, string superSetFile) : base(ReplayStrings.LastLogReplacementFileNotSubsetException(dbCopy, subsetFile, superSetFile))
		{
			this.dbCopy = dbCopy;
			this.subsetFile = subsetFile;
			this.superSetFile = superSetFile;
		}

		public LastLogReplacementFileNotSubsetException(string dbCopy, string subsetFile, string superSetFile, Exception innerException) : base(ReplayStrings.LastLogReplacementFileNotSubsetException(dbCopy, subsetFile, superSetFile), innerException)
		{
			this.dbCopy = dbCopy;
			this.subsetFile = subsetFile;
			this.superSetFile = superSetFile;
		}

		protected LastLogReplacementFileNotSubsetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.subsetFile = (string)info.GetValue("subsetFile", typeof(string));
			this.superSetFile = (string)info.GetValue("superSetFile", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("subsetFile", this.subsetFile);
			info.AddValue("superSetFile", this.superSetFile);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string SubsetFile
		{
			get
			{
				return this.subsetFile;
			}
		}

		public string SuperSetFile
		{
			get
			{
				return this.superSetFile;
			}
		}

		private readonly string dbCopy;

		private readonly string subsetFile;

		private readonly string superSetFile;
	}
}
