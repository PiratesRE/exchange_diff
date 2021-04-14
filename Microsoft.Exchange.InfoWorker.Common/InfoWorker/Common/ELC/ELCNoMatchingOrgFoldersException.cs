using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCNoMatchingOrgFoldersException : IWPermanentException
	{
		public ELCNoMatchingOrgFoldersException(string folderName) : base(Strings.descElcNoMatchingOrgFolder(folderName))
		{
		}
	}
}
