using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCDuplicateFolderNamesArgumentException : LocalizedException
	{
		public ELCDuplicateFolderNamesArgumentException(string folderName) : base(Strings.descInputFolderNamesContainDuplicates(folderName))
		{
		}
	}
}
