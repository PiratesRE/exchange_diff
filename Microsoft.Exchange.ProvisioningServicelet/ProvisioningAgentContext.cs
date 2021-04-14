using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Servicelets.Provisioning.Messages;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal class ProvisioningAgentContext : DisposeTrackableBase
	{
		public ProvisioningAgentContext(Guid jobId, CultureInfo cultureInfo, Guid ownerExchangeObjectId, ADObjectId ownerId, DelegatedPrincipal delegatedAdminOwner, SubmittedByUserAdminType submittedByUserAdminType, string tenantOrganization, OrganizationId organizationId, ExEventLog eventLog)
		{
			this.JobId = jobId;
			this.CultureInfo = cultureInfo;
			this.OwnerExchangeObjectId = ownerExchangeObjectId;
			this.OwnerId = ownerId;
			this.DelegatedAdminOwner = delegatedAdminOwner;
			this.SubmittedByUserAdminType = submittedByUserAdminType;
			this.TenantOrganization = tenantOrganization;
			this.OrganizationId = organizationId;
			this.MigrationRunspace = null;
			this.DatacenterMigrationRunspace = null;
			this.EventLog = eventLog;
		}

		public ExDateTime LastModified
		{
			get
			{
				return this.lastModified;
			}
			internal set
			{
				this.lastModified = value;
			}
		}

		public RunspaceProxy Runspace
		{
			get
			{
				return this.MigrationRunspace.RunspaceProxy;
			}
		}

		public RunspaceProxy DatacenterRunspace
		{
			get
			{
				return this.DatacenterMigrationRunspace.RunspaceProxy;
			}
		}

		private MigrationRunspaceProxy MigrationRunspace
		{
			get
			{
				return this.runspaceProxy;
			}
			set
			{
				this.runspaceProxy = value;
				ExDateTime now = ExDateTime.Now;
				this.lastModified = now;
			}
		}

		private MigrationRunspaceProxy DatacenterMigrationRunspace
		{
			get
			{
				if (this.datacenterRunspaceProxy == null)
				{
					if (this.Owner == null && this.OrganizationId == null)
					{
						throw new InvalidOperationException("need to have an owner or org id before trying to create the proxy.");
					}
					ExTraceGlobals.WorkerTracer.TraceInformation(0, (long)this.GetHashCode(), "creating datacenter runspace proxy");
					this.datacenterRunspaceProxy = MigrationRunspaceProxy.CreateRunspaceForDatacenterAdmin((this.Owner != null) ? this.Owner.OrganizationId : this.OrganizationId);
					this.lastModified = ExDateTime.Now;
				}
				return this.datacenterRunspaceProxy;
			}
			set
			{
				this.datacenterRunspaceProxy = value;
				this.lastModified = ExDateTime.Now;
			}
		}

		private ADUser Owner { get; set; }

		public bool Initialize()
		{
			if (this.Owner == null || (this.Owner.ExchangeObjectId == Guid.Empty && this.Owner.Id != this.OwnerId) || this.Owner.ExchangeObjectId != this.OwnerExchangeObjectId)
			{
				this.Owner = null;
				if (this.OwnerExchangeObjectId != Guid.Empty)
				{
					IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId), 243, "Initialize", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\Provisioning\\Program\\ProvisioningAgentContext.cs");
					ADNotificationAdapter.RunADOperation(delegate()
					{
						this.Owner = (ADUser)recipientSession.FindByExchangeObjectId(this.OwnerExchangeObjectId);
					});
				}
				else if (this.OwnerId != null)
				{
					IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(this.OwnerId), 258, "Initialize", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\Provisioning\\Program\\ProvisioningAgentContext.cs");
					ADNotificationAdapter.RunADOperation(delegate()
					{
						this.Owner = (ADUser)recipientSession.Read(this.OwnerId);
					});
				}
			}
			if (this.Owner == null && this.DelegatedAdminOwner == null)
			{
				this.EventLog.LogEvent(MSExchangeProvisioningEventLogConstants.Tuple_TenantAdminNotFound, string.Empty, new object[]
				{
					this.TenantOrganization
				});
				ExTraceGlobals.WorkerTracer.TraceInformation<ADObjectId>(0, (long)this.GetHashCode(), "couldn't find owner {0}", this.OwnerId);
				return false;
			}
			return this.InitializeRunspaceProxy();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.runspaceProxy != null)
				{
					this.runspaceProxy.Dispose();
				}
				this.runspaceProxy = null;
				if (this.datacenterRunspaceProxy != null)
				{
					this.datacenterRunspaceProxy.Dispose();
				}
				this.datacenterRunspaceProxy = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProvisioningAgentContext>(this);
		}

		private bool InitializeRunspaceProxy()
		{
			if (this.runspaceProxy == null)
			{
				if (this.Owner == null && this.DelegatedAdminOwner == null)
				{
					throw new InvalidOperationException("need to have an owner before trying to create the proxy.");
				}
				ExTraceGlobals.WorkerTracer.TraceInformation<SubmittedByUserAdminType>(0, (long)this.GetHashCode(), "creating runspace proxy for {0}", this.SubmittedByUserAdminType);
				try
				{
					if (this.SubmittedByUserAdminType == SubmittedByUserAdminType.Partner)
					{
						if (this.Owner != null)
						{
							this.runspaceProxy = MigrationRunspaceProxy.CreateRunspaceForPartner(this.Owner.Id, this.Owner, this.TenantOrganization);
						}
						else
						{
							this.runspaceProxy = MigrationRunspaceProxy.CreateRunspaceForDelegatedPartner(this.DelegatedAdminOwner, this.TenantOrganization);
						}
					}
					else if (this.Owner != null)
					{
						this.runspaceProxy = MigrationRunspaceProxy.CreateRunspaceForTenantAdmin(this.Owner.Id, this.Owner);
					}
					else
					{
						this.runspaceProxy = MigrationRunspaceProxy.CreateRunspaceForDelegatedTenantAdmin(this.DelegatedAdminOwner);
					}
				}
				catch (MigrationPermanentException arg)
				{
					this.EventLog.LogEvent(MSExchangeProvisioningEventLogConstants.Tuple_TenantAdminNotFound, string.Empty, new object[]
					{
						this.TenantOrganization
					});
					ExTraceGlobals.WorkerTracer.TraceInformation<ADObjectId, MigrationPermanentException>(0, (long)this.GetHashCode(), "owner couldn't create runspace{0}", this.OwnerId, arg);
					this.runspaceProxy = null;
				}
			}
			return this.runspaceProxy != null;
		}

		public readonly Guid JobId;

		public readonly CultureInfo CultureInfo;

		public readonly Guid OwnerExchangeObjectId;

		public readonly ADObjectId OwnerId;

		public readonly DelegatedPrincipal DelegatedAdminOwner;

		public readonly SubmittedByUserAdminType SubmittedByUserAdminType;

		public readonly string TenantOrganization;

		public readonly OrganizationId OrganizationId;

		public readonly ExEventLog EventLog;

		private ExDateTime lastModified;

		private MigrationRunspaceProxy runspaceProxy;

		private MigrationRunspaceProxy datacenterRunspaceProxy;
	}
}
