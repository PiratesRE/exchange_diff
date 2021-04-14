using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckJustCreatedEDBException : FileCheckException
	{
		public FileCheckJustCreatedEDBException(string file) : base(ReplayStrings.FileCheckJustCreatedEDB(file))
		{
			this.file = file;
		}

		public FileCheckJustCreatedEDBException(string file, Exception innerException) : base(ReplayStrings.FileCheckJustCreatedEDB(file), innerException)
		{
			this.file = file;
		}

		protected FileCheckJustCreatedEDBException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		private readonly string file;
	}
}
