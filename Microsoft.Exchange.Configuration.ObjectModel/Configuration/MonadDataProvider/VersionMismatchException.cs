using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[Serializable]
	public class VersionMismatchException : LocalizedException
	{
		public SupportedVersionList SupportedVersionList { get; private set; }

		public VersionMismatchException(LocalizedString message, SupportedVersionList versionList) : base(message)
		{
			this.SupportedVersionList = versionList;
		}
	}
}
