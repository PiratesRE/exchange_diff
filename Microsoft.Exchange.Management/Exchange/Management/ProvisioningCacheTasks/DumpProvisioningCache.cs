using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningCacheTasks
{
	[Cmdlet("Dump", "ProvisioningCache", DefaultParameterSetName = "OrganizationCache", SupportsShouldProcess = true)]
	public sealed class DumpProvisioningCache : ProvisioningCacheDiagnosticBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationDumpProvisioningCache(base.Server.ToString(), base.Application);
			}
		}

		protected override void ProcessReceivedData(byte[] buffer, int bufLen)
		{
			Exception ex = null;
			CachedEntryPresentationObject sendToPipeline = CachedEntryPresentationObject.TryFromReceivedData(buffer, bufLen, out ex);
			if (ex != null)
			{
				this.WriteWarning(new LocalizedString(ex.Message));
				return;
			}
			base.WriteObject(sendToPipeline);
		}

		internal override DiagnosticType GetDiagnosticType()
		{
			return DiagnosticType.Dump;
		}
	}
}
