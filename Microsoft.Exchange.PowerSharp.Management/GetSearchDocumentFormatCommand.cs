using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Search;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetSearchDocumentFormatCommand : SyntheticCommandWithPipelineInput<Server, Server>
	{
		private GetSearchDocumentFormatCommand() : base("Get-SearchDocumentFormat")
		{
		}

		public GetSearchDocumentFormatCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetSearchDocumentFormatCommand SetParameters(GetSearchDocumentFormatCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SearchDocumentFormatId Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
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
