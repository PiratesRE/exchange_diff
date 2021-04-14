using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxTransportService", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxTransportService : GetSystemConfigurationObjectTask<MailboxTransportServerIdParameter, MailboxTransportServer>
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				return new BitMaskOrFilter(MailboxTransportServerSchema.CurrentServerRole, 2UL);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MailboxTransportServer dataObject2 = (MailboxTransportServer)dataObject;
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteResult(new MailboxTransportServerPresentationObject(dataObject2));
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null)
			{
				this.Identity = MailboxTransportServerIdParameter.CreateIdentity(this.Identity);
			}
			base.InternalValidate();
		}
	}
}
