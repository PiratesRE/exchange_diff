using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMIPGateway", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetUMIPGateway : SetSystemConfigurationObjectTask<UMIPGatewayIdParameter, UMIPGateway>
	{
		[Parameter]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMIPGateway(this.Identity.ToString());
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return true;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (CommonConstants.UseDataCenterCallRouting && this.DataObject.Address.IsIPAddress && this.DataObject.GlobalCallRoutingScheme != UMGlobalCallRoutingScheme.E164)
				{
					base.WriteError(new GatewayAddressRequiresFqdnException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
				LocalizedException ex = NewUMIPGateway.ValidateFQDNInTenantAcceptedDomain(this.DataObject, (IConfigurationSession)base.DataSession);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject);
				}
				string text = this.DataObject.Address.ToString();
				this.CheckAndWriteError(new IPGatewayAlreadyExistsException(text), text);
				LocalizedException ex2 = NewUMIPGateway.ValidateIPAddressFamily(this.DataObject);
				if (ex2 != null)
				{
					base.WriteError(ex2, ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ShouldUpgradeObjectVersion("UMIPGateway")))
			{
				base.InternalProcessRecord();
			}
		}

		private void CheckAndWriteError(LocalizedException ex, string addr)
		{
			UMIPGateway[] array = Utility.FindGatewayByIPAddress(addr, this.ConfigurationSession);
			if (array != null && array.Length > 0 && (array.Length != 1 || !array[0].Guid.Equals(this.DataObject.Guid)))
			{
				base.WriteError(ex, ErrorCategory.InvalidData, null);
			}
		}
	}
}
