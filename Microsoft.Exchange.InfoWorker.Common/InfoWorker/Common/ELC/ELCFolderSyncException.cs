using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCFolderSyncException : IWTransientException
	{
		public ELCFolderSyncException(string mailbox, string folderName, Exception innerException) : base(Strings.descFailedToSyncFolder(folderName, mailbox), innerException)
		{
		}
	}
}
