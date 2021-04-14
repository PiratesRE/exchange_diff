using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckLogfileGenerationException : FileCheckException
	{
		public FileCheckLogfileGenerationException(string logfile, long logfileGeneration, long expectedGeneration) : base(ReplayStrings.FileCheckLogfileGeneration(logfile, logfileGeneration, expectedGeneration))
		{
			this.logfile = logfile;
			this.logfileGeneration = logfileGeneration;
			this.expectedGeneration = expectedGeneration;
		}

		public FileCheckLogfileGenerationException(string logfile, long logfileGeneration, long expectedGeneration, Exception innerException) : base(ReplayStrings.FileCheckLogfileGeneration(logfile, logfileGeneration, expectedGeneration), innerException)
		{
			this.logfile = logfile;
			this.logfileGeneration = logfileGeneration;
			this.expectedGeneration = expectedGeneration;
		}

		protected FileCheckLogfileGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logfile = (string)info.GetValue("logfile", typeof(string));
			this.logfileGeneration = (long)info.GetValue("logfileGeneration", typeof(long));
			this.expectedGeneration = (long)info.GetValue("expectedGeneration", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logfile", this.logfile);
			info.AddValue("logfileGeneration", this.logfileGeneration);
			info.AddValue("expectedGeneration", this.expectedGeneration);
		}

		public string Logfile
		{
			get
			{
				return this.logfile;
			}
		}

		public long LogfileGeneration
		{
			get
			{
				return this.logfileGeneration;
			}
		}

		public long ExpectedGeneration
		{
			get
			{
				return this.expectedGeneration;
			}
		}

		private readonly string logfile;

		private readonly long logfileGeneration;

		private readonly long expectedGeneration;
	}
}
