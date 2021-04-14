using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class SharingFolderNotFoundException : SharingSynchronizationException
	{
		public SharingFolderNotFoundException() : base(Strings.SharingFolderNotFoundException)
		{
		}

		public SharingFolderNotFoundException(Exception innerException) : base(Strings.SharingFolderNotFoundException, innerException)
		{
		}
	}
}
