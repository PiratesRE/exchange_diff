using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckLogfileMissingException : FileCheckException
	{
		public FileCheckLogfileMissingException(string logfile) : base(ReplayStrings.FileCheckLogfileMissing(logfile))
		{
			this.logfile = logfile;
		}

		public FileCheckLogfileMissingException(string logfile, Exception innerException) : base(ReplayStrings.FileCheckLogfileMissing(logfile), innerException)
		{
			this.logfile = logfile;
		}

		protected FileCheckLogfileMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logfile = (string)info.GetValue("logfile", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logfile", this.logfile);
		}

		public string Logfile
		{
			get
			{
				return this.logfile;
			}
		}

		private readonly string logfile;
	}
}
