using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.PiiRedaction
{
	[Cmdlet("Disable", "ExchangePiiMapping")]
	public sealed class DisableExchangePiiMapping : Task
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			base.ExchangeRunspaceConfig.EnablePiiMap = false;
			TaskLogger.Trace("Exchange PII mapping is disabled.", new object[0]);
		}
	}
}
