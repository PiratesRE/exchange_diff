using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReseedCheckMissingLogfileException : LocalizedException
	{
		public ReseedCheckMissingLogfileException(string logfile) : base(ReplayStrings.ReseedCheckMissingLogfile(logfile))
		{
			this.logfile = logfile;
		}

		public ReseedCheckMissingLogfileException(string logfile, Exception innerException) : base(ReplayStrings.ReseedCheckMissingLogfile(logfile), innerException)
		{
			this.logfile = logfile;
		}

		protected ReseedCheckMissingLogfileException(SerializationInfo info, StreamingContext context) : base(info, context)
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
