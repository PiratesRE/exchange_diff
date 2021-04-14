using System;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageExsetdataAtom : Task
	{
		[LocDescription(Strings.IDs.DomainControllerName)]
		[Parameter(Mandatory = true)]
		public string DomainController
		{
			get
			{
				return (string)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[LocDescription(Strings.IDs.ExsetdataOrganizationName)]
		[Parameter(Mandatory = false)]
		public string Organization
		{
			get
			{
				return (string)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[LocDescription(Strings.IDs.ExsetdataLegacyOrganizationName)]
		public string LegacyOrganization
		{
			get
			{
				return (string)base.Fields["LegacyOrganization"];
			}
			set
			{
				base.Fields["LegacyOrganization"] = value;
			}
		}

		private static ManagedLoggerDelegate LoggerDelegate
		{
			get
			{
				if (ManageExsetdataAtom.managedLoggerDelegate == null)
				{
					ManageExsetdataAtom.managedLoggerDelegate = new ManagedLoggerDelegate(TaskLogger.UnmanagedLog);
				}
				return ManageExsetdataAtom.managedLoggerDelegate;
			}
		}

		protected void InstallAtom(AtomID atomID)
		{
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.LogExsetdataInstallingAtom(atomID.ToString()));
			uint scErr = ExsetdataNativeMethods.SetupAtom((uint)atomID, 61953U, ConfigurationContext.Setup.InstallPath, ConfigurationContext.Setup.InstallPath, this.DomainController, this.Organization, this.LegacyOrganization, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, RoutingGroup.DefaultName, ManageExsetdataAtom.LoggerDelegate);
			this.HandleExsetdataReturnCode(scErr);
			TaskLogger.LogExit();
		}

		protected void BuildToBuildUpgradeAtom(AtomID atomID)
		{
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.LogExsetdataReinstallingAtom(atomID.ToString()));
			uint scErr = ExsetdataNativeMethods.SetupAtom((uint)atomID, 61955U, ConfigurationContext.Setup.InstallPath, ConfigurationContext.Setup.InstallPath, this.DomainController, this.Organization, this.LegacyOrganization, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, RoutingGroup.DefaultName, ManageExsetdataAtom.LoggerDelegate);
			this.HandleExsetdataReturnCode(scErr);
			TaskLogger.LogExit();
		}

		protected void DisasterRecoveryAtom(AtomID atomID)
		{
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.LogExsetdataReinstallingAtom(atomID.ToString()));
			uint scErr = ExsetdataNativeMethods.SetupAtom((uint)atomID, 61959U, ConfigurationContext.Setup.InstallPath, ConfigurationContext.Setup.InstallPath, this.DomainController, this.Organization, this.LegacyOrganization, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, RoutingGroup.DefaultName, ManageExsetdataAtom.LoggerDelegate);
			this.HandleExsetdataReturnCode(scErr);
			TaskLogger.LogExit();
		}

		protected void UninstallAtom(AtomID atomID)
		{
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.LogExsetdataUninstallingAtom(atomID.ToString()));
			uint scErr = ExsetdataNativeMethods.SetupAtom((uint)atomID, 61954U, ConfigurationContext.Setup.InstallPath, ConfigurationContext.Setup.InstallPath, this.DomainController, this.Organization, this.LegacyOrganization, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, AdministrativeGroup.DefaultName, RoutingGroup.DefaultName, ManageExsetdataAtom.LoggerDelegate);
			this.HandleExsetdataReturnCode(scErr);
			TaskLogger.LogExit();
		}

		private void HandleExsetdataReturnCode(uint scErr)
		{
			if ((scErr & 2147483648U) == 0U)
			{
				return;
			}
			string text = null;
			string text2 = null;
			uint messageFromExsetdata = ManageExsetdataAtom.GetMessageFromExsetdata(scErr, new CultureInfo("en-US"), ref text);
			if (messageFromExsetdata != 0U)
			{
				base.ThrowTerminatingError(new LocalizedException(Strings.ExceptionExsetdataGetMessageError(scErr, messageFromExsetdata)), ErrorCategory.NotSpecified, null);
			}
			messageFromExsetdata = ManageExsetdataAtom.GetMessageFromExsetdata(scErr, CultureInfo.CurrentUICulture, ref text2);
			if (messageFromExsetdata != 0U)
			{
				base.ThrowTerminatingError(new LocalizedException(Strings.ExceptionExsetdataGetMessageError(scErr, messageFromExsetdata)), ErrorCategory.NotSpecified, null);
			}
			LocalizedString localizedMessage = Strings.ExceptionExsetdataGenericError(scErr, text2.ToString());
			LocalizedString englishMessage = Strings.ExceptionExsetdataGenericError(scErr, text.ToString());
			base.ThrowTerminatingError(new ExsetdataException(scErr, englishMessage, localizedMessage), ErrorCategory.NotSpecified, null);
		}

		private static uint GetMessageFromExsetdata(uint scErr, CultureInfo cultureInfo, ref string errorMessage)
		{
			StringBuilder stringBuilder = new StringBuilder(500);
			int capacity = stringBuilder.Capacity;
			int lcid = cultureInfo.LCID;
			uint num = ExsetdataNativeMethods.ScGetFormattedError(scErr, lcid, stringBuilder, ref capacity);
			if (num == 3221684458U)
			{
				stringBuilder.EnsureCapacity(capacity);
				capacity = stringBuilder.Capacity;
				num = ExsetdataNativeMethods.ScGetFormattedError(scErr, lcid, stringBuilder, ref capacity);
			}
			if (num == 0U)
			{
				errorMessage = stringBuilder.ToString();
			}
			return num;
		}

		private static ManagedLoggerDelegate managedLoggerDelegate;
	}
}
