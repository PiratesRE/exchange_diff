using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "FrontendTransportService", DefaultParameterSetName = "Identity")]
	public sealed class GetFrontendTransportService : GetSystemConfigurationObjectTask<FrontendTransportServerIdParameter, FrontendTransportServer>
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				return new BitMaskOrFilter(FrontendTransportServerSchema.CurrentServerRole, 16384UL);
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
			FrontendTransportServer dataObject2 = (FrontendTransportServer)dataObject;
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteResult(new FrontendTransportServerPresentationObject(dataObject2));
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null)
			{
				this.Identity = FrontendTransportServerIdParameter.CreateIdentity(this.Identity);
			}
			base.InternalValidate();
		}
	}
}
