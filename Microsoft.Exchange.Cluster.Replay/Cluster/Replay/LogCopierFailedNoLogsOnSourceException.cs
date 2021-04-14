using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogCopierFailedNoLogsOnSourceException : TransientException
	{
		public LogCopierFailedNoLogsOnSourceException(string srcServer) : base(ReplayStrings.LogCopierInitFailedBecauseNoLogsOnSource(srcServer))
		{
			this.srcServer = srcServer;
		}

		public LogCopierFailedNoLogsOnSourceException(string srcServer, Exception innerException) : base(ReplayStrings.LogCopierInitFailedBecauseNoLogsOnSource(srcServer), innerException)
		{
			this.srcServer = srcServer;
		}

		protected LogCopierFailedNoLogsOnSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.srcServer = (string)info.GetValue("srcServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("srcServer", this.srcServer);
		}

		public string SrcServer
		{
			get
			{
				return this.srcServer;
			}
		}

		private readonly string srcServer;
	}
}
