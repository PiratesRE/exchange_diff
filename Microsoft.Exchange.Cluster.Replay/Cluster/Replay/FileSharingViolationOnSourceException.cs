using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileSharingViolationOnSourceException : LocalizedException
	{
		public FileSharingViolationOnSourceException(string serverName, string fileFullPath) : base(ReplayStrings.FileSharingViolationOnSourceException(serverName, fileFullPath))
		{
			this.serverName = serverName;
			this.fileFullPath = fileFullPath;
		}

		public FileSharingViolationOnSourceException(string serverName, string fileFullPath, Exception innerException) : base(ReplayStrings.FileSharingViolationOnSourceException(serverName, fileFullPath), innerException)
		{
			this.serverName = serverName;
			this.fileFullPath = fileFullPath;
		}

		protected FileSharingViolationOnSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.fileFullPath = (string)info.GetValue("fileFullPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("fileFullPath", this.fileFullPath);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string FileFullPath
		{
			get
			{
				return this.fileFullPath;
			}
		}

		private readonly string serverName;

		private readonly string fileFullPath;
	}
}
