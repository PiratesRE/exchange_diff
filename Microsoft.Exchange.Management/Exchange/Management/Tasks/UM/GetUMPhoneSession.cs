using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.UM.UMPhoneSession;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMPhoneSession", SupportsShouldProcess = false, DefaultParameterSetName = "Identity")]
	public sealed class GetUMPhoneSession : GetTenantADObjectWithIdentityTaskBase<UMPhoneSessionIdentityParameter, UMPhoneSession>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public new UMPhoneSessionIdentityParameter Identity
		{
			get
			{
				return (UMPhoneSessionIdentityParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new UMPlayOnPhoneDataProvider(null, TypeOfPlayOnPhoneGreetingCall.Unknown);
		}
	}
}
