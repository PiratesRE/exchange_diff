using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCOrgFolderExistsException : LocalizedException
	{
		public ELCOrgFolderExistsException(string folderName) : base(Strings.descElcFolderExists(folderName))
		{
		}
	}
}
