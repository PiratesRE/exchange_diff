using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class MailStorageNotFoundException : LocalizedException
	{
		public MailStorageNotFoundException() : base(Strings.descFailedToGetUserOofPolicy)
		{
		}
	}
}
