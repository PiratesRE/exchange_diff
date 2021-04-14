using System;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class InvalidUserOofSettings : InvalidParameterException
	{
		public InvalidUserOofSettings() : base(Strings.descInvalidUserOofSettings)
		{
		}
	}
}
