using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class ImportUMPromptCommand : SyntheticCommandWithPipelineInput<UMDialPlanIdParameter, UMDialPlanIdParameter>
	{
		private ImportUMPromptCommand() : base("Import-UMPrompt")
		{
		}

		public ImportUMPromptCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual ImportUMPromptCommand SetParameters(ImportUMPromptCommand.UploadAutoAttendantPromptsStreamParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportUMPromptCommand SetParameters(ImportUMPromptCommand.UploadAutoAttendantPromptsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportUMPromptCommand SetParameters(ImportUMPromptCommand.UploadDialPlanPromptsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportUMPromptCommand SetParameters(ImportUMPromptCommand.UploadDialPlanPromptsStreamParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportUMPromptCommand SetParameters(ImportUMPromptCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class UploadAutoAttendantPromptsStreamParameters : ParametersBase
		{
			public virtual string UMAutoAttendant
			{
				set
				{
					base.PowerSharpParameters["UMAutoAttendant"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual string PromptFileName
			{
				set
				{
					base.PowerSharpParameters["PromptFileName"] = value;
				}
			}

			public virtual Stream PromptFileStream
			{
				set
				{
					base.PowerSharpParameters["PromptFileStream"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class UploadAutoAttendantPromptsParameters : ParametersBase
		{
			public virtual string UMAutoAttendant
			{
				set
				{
					base.PowerSharpParameters["UMAutoAttendant"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual string PromptFileName
			{
				set
				{
					base.PowerSharpParameters["PromptFileName"] = value;
				}
			}

			public virtual byte PromptFileData
			{
				set
				{
					base.PowerSharpParameters["PromptFileData"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class UploadDialPlanPromptsParameters : ParametersBase
		{
			public virtual string UMDialPlan
			{
				set
				{
					base.PowerSharpParameters["UMDialPlan"] = ((value != null) ? new UMDialPlanIdParameter(value) : null);
				}
			}

			public virtual string PromptFileName
			{
				set
				{
					base.PowerSharpParameters["PromptFileName"] = value;
				}
			}

			public virtual byte PromptFileData
			{
				set
				{
					base.PowerSharpParameters["PromptFileData"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class UploadDialPlanPromptsStreamParameters : ParametersBase
		{
			public virtual string UMDialPlan
			{
				set
				{
					base.PowerSharpParameters["UMDialPlan"] = ((value != null) ? new UMDialPlanIdParameter(value) : null);
				}
			}

			public virtual string PromptFileName
			{
				set
				{
					base.PowerSharpParameters["PromptFileName"] = value;
				}
			}

			public virtual Stream PromptFileStream
			{
				set
				{
					base.PowerSharpParameters["PromptFileStream"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
