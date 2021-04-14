using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogCopierFailsBecauseLogGapException : TransientException
	{
		public LogCopierFailsBecauseLogGapException(string srcServer, string missingFileName) : base(ReplayStrings.LogCopierFailsBecauseLogGap(srcServer, missingFileName))
		{
			this.srcServer = srcServer;
			this.missingFileName = missingFileName;
		}

		public LogCopierFailsBecauseLogGapException(string srcServer, string missingFileName, Exception innerException) : base(ReplayStrings.LogCopierFailsBecauseLogGap(srcServer, missingFileName), innerException)
		{
			this.srcServer = srcServer;
			this.missingFileName = missingFileName;
		}

		protected LogCopierFailsBecauseLogGapException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.srcServer = (string)info.GetValue("srcServer", typeof(string));
			this.missingFileName = (string)info.GetValue("missingFileName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("srcServer", this.srcServer);
			info.AddValue("missingFileName", this.missingFileName);
		}

		public string SrcServer
		{
			get
			{
				return this.srcServer;
			}
		}

		public string MissingFileName
		{
			get
			{
				return this.missingFileName;
			}
		}

		private readonly string srcServer;

		private readonly string missingFileName;
	}
}
