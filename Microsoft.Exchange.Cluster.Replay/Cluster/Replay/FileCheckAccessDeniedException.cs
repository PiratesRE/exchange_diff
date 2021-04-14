using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckAccessDeniedException : FileCheckException
	{
		public FileCheckAccessDeniedException(string file) : base(ReplayStrings.FileCheckAccessDenied(file))
		{
			this.file = file;
		}

		public FileCheckAccessDeniedException(string file, Exception innerException) : base(ReplayStrings.FileCheckAccessDenied(file), innerException)
		{
			this.file = file;
		}

		protected FileCheckAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
