using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "MailboxCalendarFolder", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxCalendarFolder : SetTenantXsoObjectWithFolderIdentityTaskBase<MailboxCalendarFolder>
	{
		private static string PreferredHostname
		{
			get
			{
				if (SetMailboxCalendarFolder.preferredHostname == null)
				{
					ServiceEndpoint serviceEndpoint = null;
					try
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 56, "PreferredHostname", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxCalendarFolder\\SetMailboxCalendarFolder.cs");
						ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
						serviceEndpoint = endpointContainer.GetEndpoint(ServiceEndpointId.AnonymousCalendarHostUrl);
					}
					catch (EndpointContainerNotFoundException)
					{
					}
					catch (ServiceEndpointNotFoundException)
					{
					}
					SetMailboxCalendarFolder.preferredHostname = ((serviceEndpoint != null) ? serviceEndpoint.Uri.Host : string.Empty);
				}
				return SetMailboxCalendarFolder.preferredHostname;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ResetUrl
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResetUrl"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ResetUrl"] = value;
				SetMailboxCalendarFolder.preferredHostname = null;
			}
		}

		private bool IsMultitenancyEnabled
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMailboxCalendarFolder(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.user = this.PrepareMailboxUser();
			base.InnerMailboxFolderDataProvider = new MailboxCalendarFolderDataProvider(base.SessionSettings, this.user, "Set-MailboxCalendarFolder");
			return base.InnerMailboxFolderDataProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			MailboxCalendarFolder mailboxCalendarFolder = (MailboxCalendarFolder)base.PrepareDataObject();
			mailboxCalendarFolder.MailboxFolderId = this.Identity.InternalMailboxFolderId;
			return mailboxCalendarFolder;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject.PublishEnabled)
			{
				if (this.DataObject.PublishedCalendarUrl == null || this.DataObject.PublishedICalUrl == null || this.ResetUrl || this.DataObject.IsChanged(MailboxCalendarFolderSchema.SearchableUrlEnabled))
				{
					string owaServiceUrl = this.GetOwaServiceUrl(base.InnerMailboxFolderDataProvider.MailboxSession.MailboxOwner);
					UriBuilder uriBuilder = new UriBuilder(owaServiceUrl);
					uriBuilder.Scheme = Uri.UriSchemeHttp;
					uriBuilder.Port = -1;
					if (!string.IsNullOrEmpty(SetMailboxCalendarFolder.PreferredHostname))
					{
						uriBuilder.Host = SetMailboxCalendarFolder.PreferredHostname;
					}
					string owaHttpExternalUrl = uriBuilder.Uri.ToString();
					if (this.DataObject.SearchableUrlEnabled)
					{
						StoreObjectId folderId = this.DataObject.MailboxFolderId.StoreObjectIdValue ?? base.InnerMailboxFolderDataProvider.ResolveStoreObjectIdFromFolderPath(this.DataObject.MailboxFolderId.MailboxFolderPath);
						this.GeneratePublicUrls(owaHttpExternalUrl, folderId);
					}
					else
					{
						this.GenerateObscureUrls(owaHttpExternalUrl);
					}
				}
			}
			else if (!this.DataObject.IsChanged(MailboxCalendarFolderSchema.PublishEnabled) && (this.ResetUrl || this.DataObject.IsModified(MailboxCalendarFolderSchema.SearchableUrlEnabled) || this.DataObject.IsModified(MailboxCalendarFolderSchema.PublishDateRangeFrom) || this.DataObject.IsModified(MailboxCalendarFolderSchema.PublishDateRangeTo) || this.DataObject.IsModified(MailboxCalendarFolderSchema.DetailLevel)))
			{
				base.WriteError(new PublishingNotEnabledException(), ExchangeErrorCategory.Client, this.Identity);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private void GeneratePublicUrls(string owaHttpExternalUrl, StoreObjectId folderId)
		{
			SmtpAddress primarySmtpAddress = this.user.PrimarySmtpAddress;
			string folderName = null;
			using (Folder folder = Folder.Bind(base.InnerMailboxFolderDataProvider.MailboxSession, folderId))
			{
				folderName = folder.DisplayName;
			}
			PublicUrl publicUrl = PublicUrl.Create(owaHttpExternalUrl, SharingDataType.Calendar, primarySmtpAddress, folderName, this.user.SharingAnonymousIdentities);
			this.DataObject.PublishedCalendarUrl = publicUrl.ToString() + ".html";
			this.DataObject.PublishedICalUrl = publicUrl.ToString() + ".ics";
		}

		private void GenerateObscureUrls(string owaHttpExternalUrl)
		{
			string domain = this.user.PrimarySmtpAddress.Domain;
			Guid mailboxGuid = base.InnerMailboxFolderDataProvider.MailboxSession.MailboxGuid;
			ObscureUrl obscureUrl = ObscureUrl.CreatePublishCalendarUrl(owaHttpExternalUrl, mailboxGuid, domain);
			this.DataObject.PublishedCalendarUrl = obscureUrl.ToString() + ".html";
			this.DataObject.PublishedICalUrl = obscureUrl.ToString() + ".ics";
		}

		private string GetOwaServiceUrl(IExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && this.IsMultitenancyEnabled)
			{
				return this.GetE15MultitenancyOwaServiceUrl(exchangePrincipal);
			}
			return this.GetEnterpriseOrE14OwaServiceUrl(exchangePrincipal);
		}

		private string GetE15MultitenancyOwaServiceUrl(IExchangePrincipal exchangePrincipal)
		{
			Uri uri = null;
			Exception ex = null;
			try
			{
				uri = FrontEndLocator.GetFrontEndOwaUrl(exchangePrincipal);
			}
			catch (ServerNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex != null)
				{
					throw new NoExternalOwaAvailableException(ex);
				}
			}
			return uri.ToString();
		}

		private string GetEnterpriseOrE14OwaServiceUrl(IExchangePrincipal exchangePrincipal)
		{
			ServiceTopology serviceTopology = this.IsMultitenancyEnabled ? ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxCalendarFolder\\SetMailboxCalendarFolder.cs", "GetEnterpriseOrE14OwaServiceUrl", 335) : ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxCalendarFolder\\SetMailboxCalendarFolder.cs", "GetEnterpriseOrE14OwaServiceUrl", 335);
			IList<OwaService> list = serviceTopology.FindAll<OwaService>(exchangePrincipal, ClientAccessType.External, SetMailboxCalendarFolder.serviceFilter, "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxCalendarFolder\\SetMailboxCalendarFolder.cs", "GetEnterpriseOrE14OwaServiceUrl", 339);
			if (list.Count != 0)
			{
				return list[0].Url.ToString();
			}
			OwaService owaService = serviceTopology.FindAny<OwaService>(ClientAccessType.External, SetMailboxCalendarFolder.serviceFilter, "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxCalendarFolder\\SetMailboxCalendarFolder.cs", "GetEnterpriseOrE14OwaServiceUrl", 348);
			if (owaService == null)
			{
				throw new NoExternalOwaAvailableException();
			}
			return owaService.Url.ToString();
		}

		private const string BrowseUrlType = ".html";

		private const string ICalUrlType = ".ics";

		private static Predicate<OwaService> serviceFilter = (OwaService service) => service.AnonymousFeaturesEnabled;

		private static string preferredHostname;

		private ADUser user;
	}
}
