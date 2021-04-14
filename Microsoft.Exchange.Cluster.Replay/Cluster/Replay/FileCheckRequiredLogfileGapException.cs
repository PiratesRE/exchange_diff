using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckRequiredLogfileGapException : FileCheckException
	{
		public FileCheckRequiredLogfileGapException(string logfile) : base(ReplayStrings.FileCheckRequiredLogfileGapException(logfile))
		{
			this.logfile = logfile;
		}

		public FileCheckRequiredLogfileGapException(string logfile, Exception innerException) : base(ReplayStrings.FileCheckRequiredLogfileGapException(logfile), innerException)
		{
			this.logfile = logfile;
		}

		protected FileCheckRequiredLogfileGapException(SerializationInfo info, StreamingContext context) : base(info, context)
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
