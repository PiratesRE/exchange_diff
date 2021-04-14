using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToOpenBackupFileHandleException : TransientException
	{
		public FailedToOpenBackupFileHandleException(string databaseSource, string serverSrc, int ec, string errorMessage) : base(ReplayStrings.FailedToOpenBackupFileHandle(databaseSource, serverSrc, ec, errorMessage))
		{
			this.databaseSource = databaseSource;
			this.serverSrc = serverSrc;
			this.ec = ec;
			this.errorMessage = errorMessage;
		}

		public FailedToOpenBackupFileHandleException(string databaseSource, string serverSrc, int ec, string errorMessage, Exception innerException) : base(ReplayStrings.FailedToOpenBackupFileHandle(databaseSource, serverSrc, ec, errorMessage), innerException)
		{
			this.databaseSource = databaseSource;
			this.serverSrc = serverSrc;
			this.ec = ec;
			this.errorMessage = errorMessage;
		}

		protected FailedToOpenBackupFileHandleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseSource = (string)info.GetValue("databaseSource", typeof(string));
			this.serverSrc = (string)info.GetValue("serverSrc", typeof(string));
			this.ec = (int)info.GetValue("ec", typeof(int));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseSource", this.databaseSource);
			info.AddValue("serverSrc", this.serverSrc);
			info.AddValue("ec", this.ec);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string DatabaseSource
		{
			get
			{
				return this.databaseSource;
			}
		}

		public string ServerSrc
		{
			get
			{
				return this.serverSrc;
			}
		}

		public int Ec
		{
			get
			{
				return this.ec;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string databaseSource;

		private readonly string serverSrc;

		private readonly int ec;

		private readonly string errorMessage;
	}
}
