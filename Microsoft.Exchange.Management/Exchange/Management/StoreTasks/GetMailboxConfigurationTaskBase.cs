using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	public abstract class GetMailboxConfigurationTaskBase<TDataObject> : GetTenantADObjectWithIdentityTaskBase<MailboxIdParameter, TDataObject> where TDataObject : IConfigurable, new()
	{
		public GetMailboxConfigurationTaskBase()
		{
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 68, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\GetMailboxConfigurationTaskBase.cs");
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Identity, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.Identity.ToString())));
			StoreTasksHelper.CheckUserVersion(aduser, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.ReadUserFromDC)
			{
				IRecipientSession tenantOrRootOrgRecipientSession2 = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 85, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\GetMailboxConfigurationTaskBase.cs");
				tenantOrRootOrgRecipientSession2.UseGlobalCatalog = false;
				if (aduser.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					tenantOrRootOrgRecipientSession2.EnforceDefaultScope = false;
				}
				ADUser aduser2 = (ADUser)tenantOrRootOrgRecipientSession2.Read<ADUser>(aduser.Identity);
				if (aduser2 != null)
				{
					aduser = aduser2;
				}
			}
			this.mailboxStoreIdParameter = new MailboxStoreIdParameter(new MailboxStoreIdentity(aduser.Id));
			return this.CreateMailboxDataProvider(aduser);
		}

		protected virtual bool ReadUserFromDC
		{
			get
			{
				return false;
			}
		}

		protected virtual IConfigDataProvider CreateMailboxDataProvider(ADUser adUser)
		{
			return new MailboxStoreTypeProvider(adUser)
			{
				MailboxSession = StoreTasksHelper.OpenMailboxSession(ExchangePrincipal.FromADUser(base.SessionSettings, adUser, RemotingOptions.AllowCrossSite), "GetMailboxConfigurationTaskBase")
			};
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			try
			{
				LocalizedString? localizedString;
				IEnumerable<TDataObject> dataObjects = base.GetDataObjects(this.mailboxStoreIdParameter, base.OptionalIdentityData, out localizedString);
				this.WriteResult<TDataObject>(dataObjects);
				if (!base.HasErrors && base.WriteObjectCount == 0U)
				{
					base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(TDataObject).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, null);
				}
			}
			catch (FormatException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			StoreTasksHelper.CleanupMailboxStoreTypeProvider(base.DataSession);
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				StoreTasksHelper.CleanupMailboxStoreTypeProvider(base.DataSession);
			}
		}

		private MailboxStoreIdParameter mailboxStoreIdParameter;
	}
}
