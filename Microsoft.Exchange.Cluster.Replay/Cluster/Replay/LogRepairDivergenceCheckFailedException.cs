using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogRepairDivergenceCheckFailedException : LocalizedException
	{
		public LogRepairDivergenceCheckFailedException(string localEndOfLogFilename, string remoteDataInTempFilename, string exceptionText) : base(ReplayStrings.LogRepairDivergenceCheckFailedError(localEndOfLogFilename, remoteDataInTempFilename, exceptionText))
		{
			this.localEndOfLogFilename = localEndOfLogFilename;
			this.remoteDataInTempFilename = remoteDataInTempFilename;
			this.exceptionText = exceptionText;
		}

		public LogRepairDivergenceCheckFailedException(string localEndOfLogFilename, string remoteDataInTempFilename, string exceptionText, Exception innerException) : base(ReplayStrings.LogRepairDivergenceCheckFailedError(localEndOfLogFilename, remoteDataInTempFilename, exceptionText), innerException)
		{
			this.localEndOfLogFilename = localEndOfLogFilename;
			this.remoteDataInTempFilename = remoteDataInTempFilename;
			this.exceptionText = exceptionText;
		}

		protected LogRepairDivergenceCheckFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.localEndOfLogFilename = (string)info.GetValue("localEndOfLogFilename", typeof(string));
			this.remoteDataInTempFilename = (string)info.GetValue("remoteDataInTempFilename", typeof(string));
			this.exceptionText = (string)info.GetValue("exceptionText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("localEndOfLogFilename", this.localEndOfLogFilename);
			info.AddValue("remoteDataInTempFilename", this.remoteDataInTempFilename);
			info.AddValue("exceptionText", this.exceptionText);
		}

		public string LocalEndOfLogFilename
		{
			get
			{
				return this.localEndOfLogFilename;
			}
		}

		public string RemoteDataInTempFilename
		{
			get
			{
				return this.remoteDataInTempFilename;
			}
		}

		public string ExceptionText
		{
			get
			{
				return this.exceptionText;
			}
		}

		private readonly string localEndOfLogFilename;

		private readonly string remoteDataInTempFilename;

		private readonly string exceptionText;
	}
}
