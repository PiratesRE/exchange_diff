using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ExchangeServerAccessLicenseUser", DefaultParameterSetName = "LicenseName")]
	public sealed class GetExchangeServerAccessLicenseUser : Task
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "LicenseName")]
		public string LicenseName
		{
			get
			{
				return (string)base.Fields["LicenseName"];
			}
			set
			{
				base.Fields["LicenseName"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!ExchangeServerAccessLicense.TryParse(this.LicenseName, out this.license))
			{
				base.WriteError(new ArgumentException(Strings.InvalidLicenseName(this.LicenseName ?? "null")), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.license.UnitLabel == ExchangeServerAccessLicense.UnitLabelType.Server)
			{
				this.InternalProcessServer();
			}
			else
			{
				if (this.license.UnitLabel != ExchangeServerAccessLicense.UnitLabelType.CAL)
				{
					throw new NotImplementedException();
				}
				this.InternalProcessCAL();
			}
			TaskLogger.LogExit();
		}

		private void InternalProcessServer()
		{
			this.InternalWriteResult(base.InvokeCommand.InvokeScript(string.Format("Get-ExchangeServer | Where {{$_.AdminDisplayVersion.Major -eq {0}}} | Where {{$_.Edition -eq '{1}'}} | Select {2}", (int)this.license.VersionMajor, this.license.AccessLicense, ExchangeServerSchema.Fqdn.Name)), ExchangeServerSchema.Fqdn);
		}

		private void InternalProcessCAL()
		{
			this.InternalWriteResult(base.InvokeCommand.InvokeScript(string.Format("CalCalculation.ps1 {0} {1}", (int)this.license.VersionMajor, this.license.AccessLicense)), MailEnabledRecipientSchema.PrimarySmtpAddress);
		}

		private void InternalWriteResult(Collection<PSObject> psObjects, ADPropertyDefinition propertyDef)
		{
			if (psObjects == null)
			{
				throw new ArgumentNullException(Strings.ErrorNullParameter("psObjects"));
			}
			if (propertyDef == null)
			{
				throw new ArgumentNullException(Strings.ErrorNullParameter("propertyDef"));
			}
			foreach (PSObject psobject in psObjects)
			{
				foreach (PSPropertyInfo pspropertyInfo in psobject.Properties)
				{
					if (pspropertyInfo == null)
					{
						throw new ArgumentNullException(Strings.ErrorNullParameter("property"));
					}
					if (pspropertyInfo.Value == null)
					{
						throw new ArgumentNullException(Strings.ErrorNullParameter("property value"));
					}
					base.WriteObject(new ExchangeServerAccessLicenseUser(this.license.LicenseName, pspropertyInfo.Value.ToString()));
				}
			}
		}

		private ExchangeServerAccessLicense license;
	}
}
