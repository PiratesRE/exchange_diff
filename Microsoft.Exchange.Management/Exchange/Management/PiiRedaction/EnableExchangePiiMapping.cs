using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.PiiRedaction
{
	[Cmdlet("Enable", "ExchangePiiMapping")]
	public sealed class EnableExchangePiiMapping : Task
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			base.ExchangeRunspaceConfig.EnablePiiMap = true;
			if (!base.NeedSuppressingPiiData)
			{
				base.WriteWarning("Exchange Pii mapping will not take effect because you have View-Only PII permission.");
				return;
			}
			TaskLogger.Trace("Exchange PII mapping is enabled.", new object[0]);
		}
	}
}
