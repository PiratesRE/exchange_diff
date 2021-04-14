using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchTooManyPagesToPatchException : PagePatchApiFailedException
	{
		public PagePatchTooManyPagesToPatchException(int numPages, int maxSupported) : base(ReplayStrings.PagePatchTooManyPagesToPatchException(numPages, maxSupported))
		{
			this.numPages = numPages;
			this.maxSupported = maxSupported;
		}

		public PagePatchTooManyPagesToPatchException(int numPages, int maxSupported, Exception innerException) : base(ReplayStrings.PagePatchTooManyPagesToPatchException(numPages, maxSupported), innerException)
		{
			this.numPages = numPages;
			this.maxSupported = maxSupported;
		}

		protected PagePatchTooManyPagesToPatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.numPages = (int)info.GetValue("numPages", typeof(int));
			this.maxSupported = (int)info.GetValue("maxSupported", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("numPages", this.numPages);
			info.AddValue("maxSupported", this.maxSupported);
		}

		public int NumPages
		{
			get
			{
				return this.numPages;
			}
		}

		public int MaxSupported
		{
			get
			{
				return this.maxSupported;
			}
		}

		private readonly int numPages;

		private readonly int maxSupported;
	}
}
