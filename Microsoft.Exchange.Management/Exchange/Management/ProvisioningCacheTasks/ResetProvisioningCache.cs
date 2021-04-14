using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningCacheTasks
{
	[Cmdlet("Reset", "ProvisioningCache", DefaultParameterSetName = "OrganizationCache", SupportsShouldProcess = true)]
	public sealed class ResetProvisioningCache : ProvisioningCacheDiagnosticBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationResetProvisioningCache(base.Server.ToString(), base.Application);
			}
		}

		protected override void ProcessReceivedData(byte[] buffer, int bufLen)
		{
			string @string = Encoding.UTF8.GetString(buffer, 0, bufLen);
			base.WriteObject(@string);
		}

		internal override DiagnosticType GetDiagnosticType()
		{
			return DiagnosticType.Reset;
		}
	}
}
