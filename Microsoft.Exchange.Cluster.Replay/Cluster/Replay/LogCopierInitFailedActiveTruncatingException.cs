using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogCopierInitFailedActiveTruncatingException : TransientException
	{
		public LogCopierInitFailedActiveTruncatingException(string srcServer, long startingLogGen, long srcLowestGen) : base(ReplayStrings.LogCopierInitFailedActiveTruncatingException(srcServer, startingLogGen, srcLowestGen))
		{
			this.srcServer = srcServer;
			this.startingLogGen = startingLogGen;
			this.srcLowestGen = srcLowestGen;
		}

		public LogCopierInitFailedActiveTruncatingException(string srcServer, long startingLogGen, long srcLowestGen, Exception innerException) : base(ReplayStrings.LogCopierInitFailedActiveTruncatingException(srcServer, startingLogGen, srcLowestGen), innerException)
		{
			this.srcServer = srcServer;
			this.startingLogGen = startingLogGen;
			this.srcLowestGen = srcLowestGen;
		}

		protected LogCopierInitFailedActiveTruncatingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.srcServer = (string)info.GetValue("srcServer", typeof(string));
			this.startingLogGen = (long)info.GetValue("startingLogGen", typeof(long));
			this.srcLowestGen = (long)info.GetValue("srcLowestGen", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("srcServer", this.srcServer);
			info.AddValue("startingLogGen", this.startingLogGen);
			info.AddValue("srcLowestGen", this.srcLowestGen);
		}

		public string SrcServer
		{
			get
			{
				return this.srcServer;
			}
		}

		public long StartingLogGen
		{
			get
			{
				return this.startingLogGen;
			}
		}

		public long SrcLowestGen
		{
			get
			{
				return this.srcLowestGen;
			}
		}

		private readonly string srcServer;

		private readonly long startingLogGen;

		private readonly long srcLowestGen;
	}
}
