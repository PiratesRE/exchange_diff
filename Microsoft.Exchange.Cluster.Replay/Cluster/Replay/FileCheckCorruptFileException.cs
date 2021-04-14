using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckCorruptFileException : FileCheckException
	{
		public FileCheckCorruptFileException(string file, string errorMessage) : base(ReplayStrings.FileCheckCorruptFile(file, errorMessage))
		{
			this.file = file;
			this.errorMessage = errorMessage;
		}

		public FileCheckCorruptFileException(string file, string errorMessage, Exception innerException) : base(ReplayStrings.FileCheckCorruptFile(file, errorMessage), innerException)
		{
			this.file = file;
			this.errorMessage = errorMessage;
		}

		protected FileCheckCorruptFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string file;

		private readonly string errorMessage;
	}
}
