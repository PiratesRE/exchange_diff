using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckLogfileSignatureException : FileCheckException
	{
		public FileCheckLogfileSignatureException(string logfile, string logfileSignature, string expectedSignature) : base(ReplayStrings.FileCheckLogfileSignature(logfile, logfileSignature, expectedSignature))
		{
			this.logfile = logfile;
			this.logfileSignature = logfileSignature;
			this.expectedSignature = expectedSignature;
		}

		public FileCheckLogfileSignatureException(string logfile, string logfileSignature, string expectedSignature, Exception innerException) : base(ReplayStrings.FileCheckLogfileSignature(logfile, logfileSignature, expectedSignature), innerException)
		{
			this.logfile = logfile;
			this.logfileSignature = logfileSignature;
			this.expectedSignature = expectedSignature;
		}

		protected FileCheckLogfileSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logfile = (string)info.GetValue("logfile", typeof(string));
			this.logfileSignature = (string)info.GetValue("logfileSignature", typeof(string));
			this.expectedSignature = (string)info.GetValue("expectedSignature", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logfile", this.logfile);
			info.AddValue("logfileSignature", this.logfileSignature);
			info.AddValue("expectedSignature", this.expectedSignature);
		}

		public string Logfile
		{
			get
			{
				return this.logfile;
			}
		}

		public string LogfileSignature
		{
			get
			{
				return this.logfileSignature;
			}
		}

		public string ExpectedSignature
		{
			get
			{
				return this.expectedSignature;
			}
		}

		private readonly string logfile;

		private readonly string logfileSignature;

		private readonly string expectedSignature;
	}
}
