using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.SystemProbeTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewEncryptedSystemProbeGuidCommand : SyntheticCommandWithPipelineInput<SystemProbeData, SystemProbeData>
	{
		private NewEncryptedSystemProbeGuidCommand() : base("New-EncryptedSystemProbeGuid")
		{
		}

		public NewEncryptedSystemProbeGuidCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewEncryptedSystemProbeGuidCommand SetParameters(NewEncryptedSystemProbeGuidCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Guid? Guid
			{
				set
				{
					base.PowerSharpParameters["Guid"] = value;
				}
			}

			public virtual DateTime TimeStamp
			{
				set
				{
					base.PowerSharpParameters["TimeStamp"] = value;
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
		}
	}
}
