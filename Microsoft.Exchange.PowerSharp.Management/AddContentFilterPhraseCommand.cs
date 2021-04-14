using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class AddContentFilterPhraseCommand : SyntheticCommandWithPipelineInput<ContentFilterPhrase, ContentFilterPhrase>
	{
		private AddContentFilterPhraseCommand() : base("Add-ContentFilterPhrase")
		{
		}

		public AddContentFilterPhraseCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual AddContentFilterPhraseCommand SetParameters(AddContentFilterPhraseCommand.DefaultParameters parameters)
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

			public virtual string Phrase
			{
				set
				{
					base.PowerSharpParameters["Phrase"] = value;
				}
			}

			public virtual Influence Influence
			{
				set
				{
					base.PowerSharpParameters["Influence"] = value;
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
		}
	}
}
