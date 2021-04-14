using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCUnknownDefaultFolderException : IWPermanentException
	{
		public ELCUnknownDefaultFolderException(string folderName, string mailbox) : base(Strings.descUnknownDefFolder(folderName, mailbox))
		{
		}
	}
}
