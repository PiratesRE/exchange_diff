using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class InvalidUserOofSettingsXmlDocument : LocalizedException
	{
		public InvalidUserOofSettingsXmlDocument() : base(Strings.descCorruptUserOofSettingsXmlDocument)
		{
		}
	}
}
