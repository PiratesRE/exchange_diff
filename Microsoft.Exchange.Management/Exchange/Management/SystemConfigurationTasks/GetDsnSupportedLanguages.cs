using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "DsnSupportedLanguages")]
	public sealed class GetDsnSupportedLanguages : Task
	{
		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			int[] sendToPipeline = LanguagePackInfo.expectedCultureLcids.ToArray();
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}
	}
}
