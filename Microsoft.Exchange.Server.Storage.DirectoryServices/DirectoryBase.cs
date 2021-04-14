using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public abstract class DirectoryBase : IDirectory
	{
		public bool BypassContextADAccessValidation
		{
			get
			{
				return this.bypassContextADAccessValidation;
			}
			set
			{
				this.bypassContextADAccessValidation = value;
			}
		}

		public ErrorCode PrimeDirectoryCaches(IExecutionContext context)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.PrimeDirectoryCachesImpl(context).Propagate((LID)29856U);
		}

		public ServerInfo GetServerInfo(IExecutionContext context)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetServerInfoImpl(context);
		}

		public void RefreshServerInfo(IExecutionContext context)
		{
			this.ValidateADAccessIsAllowed(context);
			this.RefreshServerInfoImpl(context);
		}

		public void RefreshDatabaseInfo(IExecutionContext context, Guid databaseGuid)
		{
			this.ValidateADAccessIsAllowed(context);
			this.RefreshDatabaseInfoImpl(context, databaseGuid);
		}

		public void RefreshMailboxInfo(IExecutionContext context, Guid mailboxGuid)
		{
			this.ValidateADAccessIsAllowed(context);
			this.RefreshMailboxInfoImpl(context, mailboxGuid);
		}

		public void RefreshOrganizationContainer(IExecutionContext context, Guid organizationGuid)
		{
			this.ValidateADAccessIsAllowed(context);
			this.RefreshOrganizationContainerImpl(context, organizationGuid);
		}

		public DatabaseInfo GetDatabaseInfo(IExecutionContext context, Guid databaseGuid)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetDatabaseInfoImpl(context, databaseGuid);
		}

		public MailboxInfo GetMailboxInfo(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetMailboxInfoFlags flags)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetMailboxInfoImpl(context, tenantHint, mailboxGuid, flags);
		}

		public MailboxInfo GetMailboxInfo(IExecutionContext context, TenantHint tenantHint, string legacyDN)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetMailboxInfoImpl(context, tenantHint, legacyDN);
		}

		public AddressInfo GetAddressInfoByMailboxGuid(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetAddressInfoFlags flags)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetAddressInfoByMailboxGuidImpl(context, tenantHint, mailboxGuid, flags);
		}

		public AddressInfo GetAddressInfoByObjectId(IExecutionContext context, TenantHint tenantHint, Guid objectId)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetAddressInfoByObjectIdImpl(context, tenantHint, objectId);
		}

		public AddressInfo GetAddressInfo(IExecutionContext context, TenantHint tenantHint, string legacyDN, bool loadPublicDelegates)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.GetAddressInfoImpl(context, tenantHint, legacyDN, loadPublicDelegates);
		}

		public TenantHint ResolveTenantHint(IExecutionContext context, byte[] tenantHintBlob)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.ResolveTenantHintImpl(context, tenantHintBlob);
		}

		public void PrePopulateCachesForMailbox(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, string domainController)
		{
			this.ValidateADAccessIsAllowed(context);
			this.PrePopulateCachesForMailboxImpl(context, tenantHint, mailboxGuid, domainController);
		}

		public bool IsMemberOfDistributionList(IExecutionContext context, TenantHint tenantHint, AddressInfo addressInfo, Guid distributionListObjectId)
		{
			this.ValidateADAccessIsAllowed(context);
			return this.IsMemberOfDistributionListImpl(context, tenantHint, addressInfo, distributionListObjectId);
		}

		protected abstract ErrorCode PrimeDirectoryCachesImpl(IExecutionContext context);

		protected abstract ServerInfo GetServerInfoImpl(IExecutionContext context);

		protected abstract void RefreshServerInfoImpl(IExecutionContext context);

		protected abstract void RefreshDatabaseInfoImpl(IExecutionContext context, Guid databaseGuid);

		protected abstract void RefreshMailboxInfoImpl(IExecutionContext context, Guid mailboxGuid);

		protected abstract void RefreshOrganizationContainerImpl(IExecutionContext context, Guid organizationGuid);

		protected abstract DatabaseInfo GetDatabaseInfoImpl(IExecutionContext context, Guid databaseGuid);

		protected abstract MailboxInfo GetMailboxInfoImpl(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetMailboxInfoFlags flags);

		protected abstract MailboxInfo GetMailboxInfoImpl(IExecutionContext context, TenantHint tenantHint, string legacyDN);

		protected abstract AddressInfo GetAddressInfoByMailboxGuidImpl(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetAddressInfoFlags flags);

		protected abstract AddressInfo GetAddressInfoByObjectIdImpl(IExecutionContext context, TenantHint tenantHint, Guid objectId);

		protected abstract AddressInfo GetAddressInfoImpl(IExecutionContext context, TenantHint tenantHint, string legacyDN, bool loadPublicDelegates);

		protected abstract TenantHint ResolveTenantHintImpl(IExecutionContext context, byte[] tenantHintBlob);

		protected abstract void PrePopulateCachesForMailboxImpl(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, string domainController);

		protected abstract bool IsMemberOfDistributionListImpl(IExecutionContext context, TenantHint tenantHint, AddressInfo addressInfo, Guid distributionListObjectId);

		private void ValidateADAccessIsAllowed(IExecutionContext context)
		{
			if (this.bypassContextADAccessValidation)
			{
				return;
			}
			if (context.IsMailboxOperationStarted)
			{
				throw new InvalidOperationException("AD access is not allowed when we are in the middle of MAPI protocol request.");
			}
		}

		private bool bypassContextADAccessValidation;
	}
}
