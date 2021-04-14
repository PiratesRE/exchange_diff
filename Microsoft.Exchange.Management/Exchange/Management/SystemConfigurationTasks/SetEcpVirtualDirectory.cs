using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "EcpVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetEcpVirtualDirectory : SetWebAppVirtualDirectory<ADEcpVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetEcpVirtualDirectory(this.Identity.ToString());
			}
		}

		[Parameter]
		public new bool DigestAuthentication
		{
			get
			{
				return base.DigestAuthentication;
			}
			set
			{
				base.DigestAuthentication = value;
			}
		}

		[Parameter]
		public new bool FormsAuthentication
		{
			get
			{
				return base.FormsAuthentication;
			}
			set
			{
				base.FormsAuthentication = value;
			}
		}

		[Parameter]
		public new bool AdfsAuthentication
		{
			get
			{
				return base.AdfsAuthentication;
			}
			set
			{
				base.AdfsAuthentication = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADOwaVirtualDirectory adowaVirtualDirectory = WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADOwaVirtualDirectory>(this.DataObject, base.DataSession);
			if (adowaVirtualDirectory != null)
			{
				WebAppVirtualDirectoryHelper.CheckTwoWebAppVirtualDirectories(this.DataObject, adowaVirtualDirectory, new Action<string>(base.WriteWarning), Strings.EcpAuthenticationNotMatchOwaWarning, Strings.EcpUrlNotMatchOwaWarning);
			}
			else
			{
				this.WriteWarning(Strings.CreateOwaForEcpWarning);
			}
			if (this.DataObject.IsChanged(ADEcpVirtualDirectorySchema.AdminEnabled) || this.DataObject.IsChanged(ADEcpVirtualDirectorySchema.OwaOptionsEnabled))
			{
				this.WriteWarning(Strings.NeedIisRestartForChangingECPFeatureWarning);
			}
			base.InternalProcessRecord();
			ADEcpVirtualDirectory dataObject = this.DataObject;
			WebAppVirtualDirectoryHelper.UpdateMetabase(dataObject, dataObject.MetabasePath, true);
			if (!ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(this.DataObject) && base.Fields.Contains("FormsAuthentication"))
			{
				ExchangeServiceVDirHelper.UpdateFrontEndHttpModule(this.DataObject, this.FormsAuthentication);
			}
			if (base.Fields.Contains("AdfsAuthentication"))
			{
				ExchangeServiceVDirHelper.SetAdfsAuthenticationModule(this.DataObject.AdfsAuthentication, false, this.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.Contains("AdfsAuthentication") && !this.DataObject.AdfsAuthentication)
			{
				ADOwaVirtualDirectory adowaVirtualDirectory = WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADOwaVirtualDirectory>(this.DataObject, base.DataSession);
				if (adowaVirtualDirectory != null && adowaVirtualDirectory.AdfsAuthentication)
				{
					base.WriteError(new TaskException(Strings.CannotDisableAdfsEcpWithoutOwaFirst), ErrorCategory.InvalidOperation, null);
				}
			}
		}

		protected override void UpdateDataObject(ADEcpVirtualDirectory dataObject)
		{
			if (base.Fields.Contains("GzipLevel") && base.GzipLevel != dataObject.GzipLevel)
			{
				dataObject.GzipLevel = base.GzipLevel;
				base.CheckGzipLevelIsNotError(dataObject.GzipLevel);
			}
			base.UpdateDataObject(dataObject);
		}
	}
}
