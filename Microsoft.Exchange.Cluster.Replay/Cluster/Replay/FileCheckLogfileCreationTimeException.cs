using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckLogfileCreationTimeException : FileCheckException
	{
		public FileCheckLogfileCreationTimeException(string logfile, DateTime previousGenerationCreationTime, DateTime previousGenerationCreationTimeActual) : base(ReplayStrings.FileCheckLogfileCreationTime(logfile, previousGenerationCreationTime, previousGenerationCreationTimeActual))
		{
			this.logfile = logfile;
			this.previousGenerationCreationTime = previousGenerationCreationTime;
			this.previousGenerationCreationTimeActual = previousGenerationCreationTimeActual;
		}

		public FileCheckLogfileCreationTimeException(string logfile, DateTime previousGenerationCreationTime, DateTime previousGenerationCreationTimeActual, Exception innerException) : base(ReplayStrings.FileCheckLogfileCreationTime(logfile, previousGenerationCreationTime, previousGenerationCreationTimeActual), innerException)
		{
			this.logfile = logfile;
			this.previousGenerationCreationTime = previousGenerationCreationTime;
			this.previousGenerationCreationTimeActual = previousGenerationCreationTimeActual;
		}

		protected FileCheckLogfileCreationTimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logfile = (string)info.GetValue("logfile", typeof(string));
			this.previousGenerationCreationTime = (DateTime)info.GetValue("previousGenerationCreationTime", typeof(DateTime));
			this.previousGenerationCreationTimeActual = (DateTime)info.GetValue("previousGenerationCreationTimeActual", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logfile", this.logfile);
			info.AddValue("previousGenerationCreationTime", this.previousGenerationCreationTime);
			info.AddValue("previousGenerationCreationTimeActual", this.previousGenerationCreationTimeActual);
		}

		public string Logfile
		{
			get
			{
				return this.logfile;
			}
		}

		public DateTime PreviousGenerationCreationTime
		{
			get
			{
				return this.previousGenerationCreationTime;
			}
		}

		public DateTime PreviousGenerationCreationTimeActual
		{
			get
			{
				return this.previousGenerationCreationTimeActual;
			}
		}

		private readonly string logfile;

		private readonly DateTime previousGenerationCreationTime;

		private readonly DateTime previousGenerationCreationTimeActual;
	}
}
