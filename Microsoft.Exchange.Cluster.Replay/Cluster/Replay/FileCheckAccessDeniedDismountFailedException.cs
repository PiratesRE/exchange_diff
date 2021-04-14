using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckAccessDeniedDismountFailedException : FileCheckException
	{
		public FileCheckAccessDeniedDismountFailedException(string file, string dismountError) : base(ReplayStrings.FileCheckAccessDeniedDismountFailedException(file, dismountError))
		{
			this.file = file;
			this.dismountError = dismountError;
		}

		public FileCheckAccessDeniedDismountFailedException(string file, string dismountError, Exception innerException) : base(ReplayStrings.FileCheckAccessDeniedDismountFailedException(file, dismountError), innerException)
		{
			this.file = file;
			this.dismountError = dismountError;
		}

		protected FileCheckAccessDeniedDismountFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
			this.dismountError = (string)info.GetValue("dismountError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
			info.AddValue("dismountError", this.dismountError);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		public string DismountError
		{
			get
			{
				return this.dismountError;
			}
		}

		private readonly string file;

		private readonly string dismountError;
	}
}
