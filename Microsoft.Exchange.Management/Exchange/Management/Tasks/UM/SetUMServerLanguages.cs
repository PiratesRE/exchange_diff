using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMServerLanguages", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetUMServerLanguages : SystemConfigurationObjectActionTask<UMServerIdParameter, Server>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<UMLanguage> Languages
		{
			get
			{
				return (MultiValuedProperty<UMLanguage>)base.Fields["Languages"];
			}
			set
			{
				base.Fields["Languages"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUmServer(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (!base.Fields.IsModified("Languages"))
				{
					LanguagesNotPassed exception = new LanguagesNotPassed();
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				}
				if (this.Languages == null)
				{
					this.DataObject.Languages = new MultiValuedProperty<UMLanguage>();
				}
				else
				{
					this.DataObject.Languages = this.Languages;
				}
			}
			TaskLogger.LogExit();
		}
	}
}
