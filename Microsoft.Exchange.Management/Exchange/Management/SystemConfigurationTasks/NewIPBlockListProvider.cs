using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "IPBlockListProvider", SupportsShouldProcess = true)]
	public sealed class NewIPBlockListProvider : NewIPListProvider<IPBlockListProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddIPBlockListProvider(base.Name.ToString(), base.LookupDomain.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public AsciiString RejectionResponse
		{
			get
			{
				return this.DataObject.RejectionResponse;
			}
			set
			{
				this.DataObject.RejectionResponse = value;
			}
		}
	}
}
