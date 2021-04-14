using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchFileDeletionException : PagePatchApiFailedException
	{
		public PagePatchFileDeletionException(string file, string error) : base(ReplayStrings.PagePatchFileDeletionException(file, error))
		{
			this.file = file;
			this.error = error;
		}

		public PagePatchFileDeletionException(string file, string error, Exception innerException) : base(ReplayStrings.PagePatchFileDeletionException(file, error), innerException)
		{
			this.file = file;
			this.error = error;
		}

		protected PagePatchFileDeletionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
			info.AddValue("error", this.error);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string file;

		private readonly string error;
	}
}
