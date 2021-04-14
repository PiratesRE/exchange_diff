using System;
using System.Configuration;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewAuditLogSearchBase<TDataObject> : NewTenantADTaskBase<TDataObject> where TDataObject : AuditLogSearchBase, new()
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ExDateTime StartDate
		{
			get
			{
				return (ExDateTime)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ExDateTime EndDate
		{
			get
			{
				return (ExDateTime)base.Fields["EndDate"];
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ExternalAccess
		{
			get
			{
				return (bool?)base.Fields["ExternalAccess"];
			}
			set
			{
				base.Fields["ExternalAccess"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MultiValuedProperty<SmtpAddress> StatusMailRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)base.Fields["StatusMailRecipients"];
			}
			set
			{
				base.Fields["StatusMailRecipients"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (this.StartDate >= this.EndDate)
			{
				base.WriteError(new ArgumentException(Strings.InvalidTimeRange, "StartDate"), ErrorCategory.InvalidArgument, null);
			}
			foreach (SmtpAddress smtpAddress in this.StatusMailRecipients)
			{
				if (!smtpAddress.IsValidAddress)
				{
					base.WriteError(new ArgumentException(Strings.InvalidSmtpAddressOrAlias(smtpAddress.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
			base.InternalValidate();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TDataObject dataObject = this.DataObject;
			dataObject.OrganizationId = base.CurrentOrganizationId;
			TDataObject dataObject2 = this.DataObject;
			dataObject2.SetId(new AuditLogSearchId(Guid.NewGuid()));
			if (string.IsNullOrEmpty(this.Name))
			{
				TDataObject dataObject3 = this.DataObject;
				dataObject3.Name = string.Format(CultureInfo.InvariantCulture, "{0}{1:yyyyMMdd}{{{2}}}", new object[]
				{
					Strings.AuditLogSearchNamePrefix,
					DateTime.UtcNow,
					Guid.NewGuid()
				});
			}
			else
			{
				TDataObject dataObject4 = this.DataObject;
				dataObject4.Name = this.Name;
			}
			TDataObject dataObject5 = this.DataObject;
			ADObjectId adobjectId;
			dataObject5.CreatedByEx = (base.TryGetExecutingUserId(out adobjectId) ? adobjectId : NewAuditLogSearchBase<TDataObject>.dummyUserADId);
			TDataObject dataObject6 = this.DataObject;
			dataObject6.CreatedBy = this.GetExecutingUserDisplayName();
			TDataObject dataObject7 = this.DataObject;
			dataObject7.StatusMailRecipients = this.StatusMailRecipients;
			TDataObject dataObject8 = this.DataObject;
			dataObject8.ExternalAccess = this.ExternalAccess;
			if (!this.StartDate.HasTimeZone)
			{
				ExDateTime exDateTime = ExDateTime.Create(ExTimeZone.CurrentTimeZone, this.StartDate.UniversalTime)[0];
				TDataObject dataObject9 = this.DataObject;
				dataObject9.StartDateUtc = new DateTime?(exDateTime.UniversalTime);
			}
			else
			{
				TDataObject dataObject10 = this.DataObject;
				dataObject10.StartDateUtc = new DateTime?(this.StartDate.UniversalTime);
			}
			if (!this.EndDate.HasTimeZone)
			{
				ExDateTime exDateTime2 = ExDateTime.Create(ExTimeZone.CurrentTimeZone, this.EndDate.UniversalTime)[0];
				TDataObject dataObject11 = this.DataObject;
				dataObject11.EndDateUtc = new DateTime?(exDateTime2.UniversalTime);
			}
			else
			{
				TDataObject dataObject12 = this.DataObject;
				dataObject12.EndDateUtc = new DateTime?(this.EndDate.UniversalTime);
			}
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (LocalizedException)
			{
				base.WriteError(new FailedToCreateAuditLogSearchException(), ErrorCategory.ResourceUnavailable, null);
			}
		}

		private string GetExecutingUserDisplayName()
		{
			ADObjectId adobjectId;
			if (base.TryGetExecutingUserId(out adobjectId))
			{
				return adobjectId.ToString();
			}
			if (base.ExchangeRunspaceConfig != null && !string.IsNullOrEmpty(base.ExchangeRunspaceConfig.ExecutingUserDisplayName))
			{
				return base.ExchangeRunspaceConfig.ExecutingUserDisplayName;
			}
			if (base.ExchangeRunspaceConfig != null && !string.IsNullOrEmpty(base.ExchangeRunspaceConfig.IdentityName))
			{
				return base.ExchangeRunspaceConfig.IdentityName;
			}
			return string.Empty;
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 255, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AuditLogSearch\\NewAuditLogSearch.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}

		protected override IConfigDataProvider CreateSession()
		{
			OrganizationId organizationId = this.ResolveCurrentOrganization();
			ADUser tenantArbitrationMailbox;
			try
			{
				tenantArbitrationMailbox = AdminAuditLogHelper.GetTenantArbitrationMailbox(organizationId);
			}
			catch (ObjectNotFoundException innerException)
			{
				TaskLogger.Trace("ObjectNotFoundException occurred when getting Exchange principal from the arbitration mailbox.", new object[0]);
				throw new AuditLogSearchArbitrationMailboxNotFoundException(organizationId.ToString(), innerException);
			}
			catch (NonUniqueRecipientException innerException2)
			{
				TaskLogger.Trace("More than one tenant arbitration mailbox found for the current organization.", new object[0]);
				throw new AuditLogSearchNonUniqueArbitrationMailboxFoundException(organizationId.ToString(), innerException2);
			}
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), tenantArbitrationMailbox, RemotingOptions.AllowCrossSite);
			ADSessionSettings sessionSettings = base.CurrentOrganizationId.ToADSessionSettings();
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.FullyConsistent, sessionSettings, 310, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AuditLogSearch\\NewAuditLogSearch.cs");
			return this.InternalCreateSearchDataProvider(principal, organizationId);
		}

		internal abstract IConfigDataProvider InternalCreateSearchDataProvider(ExchangePrincipal principal, OrganizationId organizationId);

		protected override void InternalEndProcessing()
		{
			this.DisposeSession();
			base.InternalEndProcessing();
		}

		protected override void InternalStopProcessing()
		{
			this.DisposeSession();
			base.InternalStopProcessing();
		}

		protected override void InternalStateReset()
		{
			this.DisposeSession();
			base.InternalStateReset();
		}

		private void DisposeSession()
		{
			if (base.DataSession is IDisposable)
			{
				((IDisposable)base.DataSession).Dispose();
			}
		}

		internal static MultiValuedProperty<string> GetMultiValuedSmptAddressAsStrings(MultiValuedProperty<SmtpAddress> values)
		{
			string[] array = new string[values.Count];
			for (int i = 0; i < values.Count; i++)
			{
				array[i] = values[i].ToString();
			}
			return new MultiValuedProperty<string>(array);
		}

		internal static MultiValuedProperty<SmtpAddress> GetMultiValuedStringsAsSmptAddresses(MultiValuedProperty<string> values)
		{
			SmtpAddress[] array = new SmtpAddress[values.Count];
			for (int i = 0; i < values.Count; i++)
			{
				array[i] = new SmtpAddress(values[i]);
			}
			return new MultiValuedProperty<SmtpAddress>(array);
		}

		internal static int? ReadIntegerAppSetting(string appSettingKey)
		{
			int? result;
			try
			{
				AppSettingsReader appSettingsReader = new AppSettingsReader();
				result = new int?((int)appSettingsReader.GetValue(appSettingKey, typeof(int)));
			}
			catch (InvalidOperationException)
			{
				TaskLogger.Trace("Failed to read {0}, either it does not exist, or it is misconfigured.", new object[]
				{
					appSettingKey
				});
				result = null;
			}
			return result;
		}

		private static ADObjectId dummyUserADId = new ADObjectId();

		internal IRecipientSession recipientSession;
	}
}
