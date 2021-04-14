using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMPhoneSession;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Stop", "UMPhoneSession", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class StopUMPhoneSession : RemoveTenantADTaskBase<UMPhoneSessionIdentityParameter, UMPhoneSession>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStopUMPhoneSession;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new UMPlayOnPhoneDataProvider(null, TypeOfPlayOnPhoneGreetingCall.Unknown);
		}
	}
}
