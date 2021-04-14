using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class SkipException : AIPermanentException
	{
		public SkipException(Exception innerException) : base(Strings.descSkipException, innerException)
		{
		}

		public SkipException(LocalizedString explain) : base(explain, null)
		{
		}

		public SkipException(LocalizedString explain, Exception innerException) : base(explain, innerException)
		{
		}
	}
}
