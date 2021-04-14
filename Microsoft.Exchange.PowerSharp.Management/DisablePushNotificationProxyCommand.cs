﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.PushNotifications;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class DisablePushNotificationProxyCommand : SyntheticCommandWithPipelineInput<PushNotificationProxyPresentationObject, PushNotificationProxyPresentationObject>
	{
		private DisablePushNotificationProxyCommand() : base("Disable-PushNotificationProxy")
		{
		}

		public DisablePushNotificationProxyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual DisablePushNotificationProxyCommand SetParameters(DisablePushNotificationProxyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
