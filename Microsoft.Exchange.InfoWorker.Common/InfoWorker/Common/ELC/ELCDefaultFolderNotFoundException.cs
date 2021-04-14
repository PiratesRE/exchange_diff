using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCDefaultFolderNotFoundException : IWTransientException
	{
		public ELCDefaultFolderNotFoundException(string folderName, string mailboxName, Exception innerException) : base(Strings.descElcCannotFindDefaultFolder(folderName, mailboxName), innerException)
		{
		}
	}
}
