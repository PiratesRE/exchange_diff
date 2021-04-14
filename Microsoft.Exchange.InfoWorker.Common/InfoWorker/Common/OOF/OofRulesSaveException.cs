using System;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class OofRulesSaveException : IWTransientException
	{
		public OofRulesSaveException() : base(Strings.descOofRuleSaveException, null)
		{
		}

		public OofRulesSaveException(Exception innerException) : base(Strings.descOofRuleSaveException, innerException)
		{
		}
	}
}
