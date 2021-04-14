using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileIOonSourceException : LocalizedException
	{
		public FileIOonSourceException(string serverName, string fileFullPath, string ioErrorMessage) : base(Strings.FileIOonSourceException(serverName, fileFullPath, ioErrorMessage))
		{
			this.serverName = serverName;
			this.fileFullPath = fileFullPath;
			this.ioErrorMessage = ioErrorMessage;
		}

		public FileIOonSourceException(string serverName, string fileFullPath, string ioErrorMessage, Exception innerException) : base(Strings.FileIOonSourceException(serverName, fileFullPath, ioErrorMessage), innerException)
		{
			this.serverName = serverName;
			this.fileFullPath = fileFullPath;
			this.ioErrorMessage = ioErrorMessage;
		}

		protected FileIOonSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.fileFullPath = (string)info.GetValue("fileFullPath", typeof(string));
			this.ioErrorMessage = (string)info.GetValue("ioErrorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("fileFullPath", this.fileFullPath);
			info.AddValue("ioErrorMessage", this.ioErrorMessage);
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

		public string IoErrorMessage
		{
			get
			{
				return this.ioErrorMessage;
			}
		}

		private readonly string serverName;

		private readonly string fileFullPath;

		private readonly string ioErrorMessage;
	}
}
