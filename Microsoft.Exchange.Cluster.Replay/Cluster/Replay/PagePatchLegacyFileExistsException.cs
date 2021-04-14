using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchLegacyFileExistsException : PagePatchApiFailedException
	{
		public PagePatchLegacyFileExistsException() : base(ReplayStrings.PagePatchLegacyFileExistsException)
		{
		}

		public PagePatchLegacyFileExistsException(Exception innerException) : base(ReplayStrings.PagePatchLegacyFileExistsException, innerException)
		{
		}

		protected PagePatchLegacyFileExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
