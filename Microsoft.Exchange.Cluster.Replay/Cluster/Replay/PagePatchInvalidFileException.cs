using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchInvalidFileException : PagePatchApiFailedException
	{
		public PagePatchInvalidFileException(string patchFile) : base(ReplayStrings.PagePatchInvalidFileException(patchFile))
		{
			this.patchFile = patchFile;
		}

		public PagePatchInvalidFileException(string patchFile, Exception innerException) : base(ReplayStrings.PagePatchInvalidFileException(patchFile), innerException)
		{
			this.patchFile = patchFile;
		}

		protected PagePatchInvalidFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.patchFile = (string)info.GetValue("patchFile", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("patchFile", this.patchFile);
		}

		public string PatchFile
		{
			get
			{
				return this.patchFile;
			}
		}

		private readonly string patchFile;
	}
}
