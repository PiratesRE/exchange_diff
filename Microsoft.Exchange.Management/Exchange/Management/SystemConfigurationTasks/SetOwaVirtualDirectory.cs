using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OwaVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOwaVirtualDirectory : SetWebAppVirtualDirectory<ADOwaVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOwaVirtualDirectory(this.Identity.ToString());
			}
		}

		[Parameter]
		public string DefaultDomain
		{
			get
			{
				return (string)base.Fields["DefaultDomain"];
			}
			set
			{
				base.Fields["DefaultDomain"] = value;
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
		public new bool OAuthAuthentication
		{
			get
			{
				return base.OAuthAuthentication;
			}
			set
			{
				base.OAuthAuthentication = value;
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

		protected override void UpdateDataObject(ADOwaVirtualDirectory dataObject)
		{
			if (dataObject.IsChanged(ADOwaVirtualDirectorySchema.LogonFormat) && (dataObject.LogonFormat == LogonFormats.FullDomain || dataObject.LogonFormat == LogonFormats.PrincipalName))
			{
				dataObject.DefaultDomain = "";
			}
			if (base.Fields.Contains("DefaultDomain"))
			{
				dataObject.DefaultDomain = this.DefaultDomain;
			}
			if (base.Fields.Contains("GzipLevel"))
			{
				if (dataObject.IsExchange2007OrLater || !this.IsMailboxRoleInstalled(dataObject))
				{
					GzipLevel gzipLevel = base.GzipLevel;
					GzipLevel gzipLevel2 = dataObject.GzipLevel;
					dataObject.GzipLevel = base.GzipLevel;
					base.CheckGzipLevelIsNotError(dataObject.GzipLevel);
				}
				else
				{
					base.WriteError(new TaskException(Strings.OwaGzipCannotBeSetToLegacyVirtualDirectoryWhenMailboxRoleInstalled), ErrorCategory.NotSpecified, null);
				}
			}
			base.UpdateDataObject(dataObject);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			ADOwaVirtualDirectory dataObject = this.DataObject;
			if (dataObject.ExchangeVersion.IsOlderThan(ADOwaVirtualDirectory.MinimumSupportedExchangeObjectVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorSetOlderVirtualDirectory(dataObject.Identity.ToString(), dataObject.ExchangeVersion.ToString(), ADOwaVirtualDirectory.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (dataObject.WebReadyFileTypes != null)
			{
				foreach (string text in dataObject.WebReadyFileTypes)
				{
					if (!dataObject.WebReadyDocumentViewingSupportedFileTypes.Contains(text.ToLower()))
					{
						PropertyValidationError error = new PropertyValidationError(DataStrings.ConstraintViolationValueIsNotInGivenStringArray(string.Join(",", dataObject.WebReadyDocumentViewingSupportedFileTypes.ToArray()), text), ADOwaVirtualDirectorySchema.WebReadyFileTypes, dataObject.WebReadyFileTypes);
						DataValidationException exception = new DataValidationException(error);
						base.WriteError(exception, ErrorCategory.InvalidArgument, this.DataObject.Identity);
					}
				}
			}
			if (dataObject.WebReadyMimeTypes != null)
			{
				foreach (string text2 in dataObject.WebReadyMimeTypes)
				{
					if (!dataObject.WebReadyDocumentViewingSupportedMimeTypes.Contains(text2.ToLower()))
					{
						PropertyValidationError error2 = new PropertyValidationError(DataStrings.ConstraintViolationValueIsNotInGivenStringArray(string.Join(",", dataObject.WebReadyDocumentViewingSupportedMimeTypes.ToArray()), text2), ADOwaVirtualDirectorySchema.WebReadyMimeTypes, dataObject.WebReadyMimeTypes);
						DataValidationException exception2 = new DataValidationException(error2);
						base.WriteError(exception2, ErrorCategory.InvalidArgument, this.DataObject.Identity);
					}
				}
			}
			if (dataObject.InstantMessagingType == InstantMessagingTypeOptions.Msn && !Datacenter.IsMultiTenancyEnabled())
			{
				base.WriteError(new TaskException(Strings.ErrorMsnIsNotSupportedInEnterprise), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.Contains("AdfsAuthentication") && this.DataObject.AdfsAuthentication)
			{
				ADEcpVirtualDirectory adecpVirtualDirectory = WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADEcpVirtualDirectory>(this.DataObject, base.DataSession);
				if (adecpVirtualDirectory == null || !adecpVirtualDirectory.AdfsAuthentication)
				{
					base.WriteError(new TaskException(Strings.CannotConfigureAdfsOwaWithoutEcpFirst), ErrorCategory.InvalidOperation, null);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			ADEcpVirtualDirectory adecpVirtualDirectory = WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADEcpVirtualDirectory>(this.DataObject, base.DataSession);
			if (adecpVirtualDirectory != null)
			{
				if (this.DataObject.IsChanged(ExchangeWebAppVirtualDirectorySchema.DefaultDomain))
				{
					adecpVirtualDirectory.DefaultDomain = this.DataObject.DefaultDomain;
					flag = true;
				}
				WebAppVirtualDirectoryHelper.CheckTwoWebAppVirtualDirectories(this.DataObject, adecpVirtualDirectory, new Action<string>(base.WriteWarning), Strings.OwaAuthenticationNotMatchEcpWarning, Strings.OwaUrlNotMatchEcpWarning);
			}
			else
			{
				this.WriteWarning(Strings.CreateEcpForOwaWarning);
			}
			if (this.DataObject.IsChanged(ADOwaVirtualDirectorySchema.AnonymousFeaturesEnabled))
			{
				this.UpdateCalendarMetabase(this.DataObject);
			}
			if (this.DataObject.IsChanged(ADOwaVirtualDirectorySchema.IntegratedFeaturesEnabled))
			{
				this.UpdateIntegratedMetabase(this.DataObject);
			}
			if (this.DataObject.IsChanged(ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication))
			{
				this.UpdateOmaMetabase(this.DataObject);
			}
			base.InternalProcessRecord();
			ADOwaVirtualDirectory dataObject = this.DataObject;
			bool flag2 = ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(this.DataObject);
			if (dataObject.LiveIdAuthentication)
			{
				WebAppVirtualDirectoryHelper.UpdateMetabase(dataObject, dataObject.MetabasePath, true);
			}
			else
			{
				WebAppVirtualDirectoryHelper.UpdateMetabase(dataObject, dataObject.MetabasePath, flag2);
			}
			if (flag)
			{
				WebAppVirtualDirectoryHelper.UpdateMetabase(adecpVirtualDirectory, adecpVirtualDirectory.MetabasePath, true);
			}
			if (flag2)
			{
				ExchangeServiceVDirHelper.EwsAutodiscMWA.OnSetManageWCFEndpoints(this, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.OwaEws, false, this.DataObject);
			}
			else if (base.Fields.Contains("FormsAuthentication"))
			{
				ExchangeServiceVDirHelper.UpdateFrontEndHttpModule(this.DataObject, this.FormsAuthentication);
			}
			if (base.Fields.Contains("OAuthAuthentication"))
			{
				ExchangeServiceVDirHelper.SetOAuthAuthenticationModule(this.DataObject.OAuthAuthentication, false, this.DataObject);
			}
			if (base.Fields.Contains("AdfsAuthentication"))
			{
				ExchangeServiceVDirHelper.SetAdfsAuthenticationModule(this.DataObject.AdfsAuthentication, false, this.DataObject);
			}
			TaskLogger.LogExit();
		}

		private bool IsMailboxRoleInstalled(ADOwaVirtualDirectory dataObject)
		{
			Server server = (Server)base.GetDataObject<Server>(new ServerIdParameter(dataObject.Server), base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(dataObject.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(dataObject.Server.ToString())));
			return server.IsMailboxServer;
		}

		private void UpdateCalendarMetabase(ADOwaVirtualDirectory dataObject)
		{
			try
			{
				using (DirectoryEntry directoryEntry = IisUtility.FindWebDirObject(dataObject.MetabasePath, "Calendar"))
				{
					if (dataObject.AnonymousFeaturesEnabled == true)
					{
						IisUtility.SetAuthenticationMethod(directoryEntry, MetabasePropertyTypes.AuthFlags.Anonymous, true);
					}
					else
					{
						IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.None, true);
					}
					directoryEntry.CommitChanges();
				}
				if (dataObject.AnonymousFeaturesEnabled == false)
				{
					this.WriteWarning(Strings.CalendarVDirDisabledWarning);
				}
			}
			catch (WebObjectNotFoundException)
			{
				base.WriteError(new TaskException(Strings.CalendarVDirNotFound), ErrorCategory.InvalidOperation, null);
			}
		}

		private void UpdateIntegratedMetabase(ADOwaVirtualDirectory dataObject)
		{
			if (Datacenter.IsMultiTenancyEnabled())
			{
				base.WriteError(new TaskException(Strings.IntegratedVDirNotSupported), ErrorCategory.InvalidOperation, null);
				return;
			}
			try
			{
				using (DirectoryEntry directoryEntry = IisUtility.FindWebDirObject(dataObject.MetabasePath, "Integrated"))
				{
					bool flag = false;
					if (dataObject.IntegratedFeaturesEnabled != null && dataObject.IntegratedFeaturesEnabled != null)
					{
						flag = dataObject.IntegratedFeaturesEnabled.Value;
					}
					if (flag)
					{
						IisUtility.SetAuthenticationMethod(directoryEntry, MetabasePropertyTypes.AuthFlags.Ntlm, true);
					}
					else
					{
						IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.None, true);
						this.WriteWarning(Strings.IntegratedVDirDisabledWarning);
					}
					directoryEntry.CommitChanges();
				}
			}
			catch (WebObjectNotFoundException)
			{
				base.WriteError(new TaskException(Strings.IntegratedVDirNotFound), ErrorCategory.InvalidOperation, null);
			}
		}

		private void UpdateOmaMetabase(ADOwaVirtualDirectory dataObject)
		{
			try
			{
				using (DirectoryEntry directoryEntry = IisUtility.FindWebDirObject(dataObject.MetabasePath, "oma"))
				{
					if (dataObject.LiveIdAuthentication)
					{
						IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic, false);
					}
					else
					{
						IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic, true);
					}
					directoryEntry.CommitChanges();
				}
			}
			catch (WebObjectNotFoundException)
			{
				base.WriteError(new TaskException(Strings.OmaVDirNotFound), ErrorCategory.InvalidOperation, null);
			}
		}
	}
}
