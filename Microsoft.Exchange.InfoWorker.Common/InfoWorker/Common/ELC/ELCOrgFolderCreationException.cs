using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCOrgFolderCreationException : IWTransientException
	{
		public ELCOrgFolderCreationException(string mailbox, string folderName, Exception innerException) : base(Strings.descFailedToCreateOrganizationalFolder(folderName, mailbox), innerException)
		{
		}
	}
}
