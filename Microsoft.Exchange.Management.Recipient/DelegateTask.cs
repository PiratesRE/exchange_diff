using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class DelegateTask : RecipientObjectActionTask<SecurityPrincipalIdParameter, ADGroup>
	{
		protected bool Add
		{
			get
			{
				return this.add;
			}
			set
			{
				this.add = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public DelegateRoleType Role
		{
			get
			{
				return (DelegateRoleType)base.Fields["Role"];
			}
			set
			{
				base.Fields["Role"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string Scope
		{
			get
			{
				return (string)base.Fields["Scope"];
			}
			set
			{
				base.Fields["Scope"] = value;
			}
		}

		public DelegateTask()
		{
			base.Fields["Role"] = DelegateRoleType.OrgAdmin;
			base.Fields.ResetChangeTracking();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			recipientSession.EnforceDefaultScope = false;
			return recipientSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Role == DelegateRoleType.ServerAdmin && string.IsNullOrEmpty(this.Scope))
			{
				base.WriteError(new ArgumentException(Strings.ErrorNeedToSpecifyScopeParameter, "Scope"), ErrorCategory.InvalidArgument, this.Role);
				return;
			}
			if (this.Role != DelegateRoleType.ServerAdmin && !string.IsNullOrEmpty(this.Scope) && string.Compare(this.Scope, Strings.OrganizationWide.ToString(), true, CultureInfo.InvariantCulture) != 0 && string.Compare(this.Scope, "Organization Wide", true, CultureInfo.InvariantCulture) != 0)
			{
				base.WriteError(new ArgumentException(Strings.ErrorCannotSpecifyScopeParameter, "Scope"), ErrorCategory.InvalidArgument, this.Role);
			}
			IEnumerable<ADRecipient> objects = this.Identity.GetObjects<ADRecipient>(this.RootId, base.TenantGlobalCatalogSession);
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					this.user = enumerator.Current;
					if (enumerator.MoveNext())
					{
						base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorUserNotUnique(this.Identity.ToString())), ErrorCategory.InvalidData, null);
					}
				}
				else if (this.Add)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotFound(this.Identity.ToString())), ErrorCategory.ObjectNotFound, null);
				}
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADGroup adgroup = null;
			this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 197, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\permission\\DelegateBaseTask.cs");
			if (this.Role == DelegateRoleType.ServerAdmin)
			{
				ServerIdParameter serverIdParameter = null;
				try
				{
					serverIdParameter = ServerIdParameter.Parse(this.Scope);
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidData, null);
				}
				this.server = (Server)base.GetDataObject<Server>(serverIdParameter, this.configSession, null, new LocalizedString?(Strings.ErrorServerNotFound((string)serverIdParameter)), new LocalizedString?(Strings.ErrorServerNotUnique((string)serverIdParameter)));
			}
			IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
			bool useGlobalCatalog = recipientSession.UseGlobalCatalog;
			recipientSession.UseGlobalCatalog = true;
			try
			{
				if (this.Role == DelegateRoleType.OrgAdmin)
				{
					adgroup = ((IDirectorySession)base.DataSession).ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EoaWkGuid, this.ConfigurationSession.ConfigurationNamingContext.ToDNString());
				}
				else if (this.Role == DelegateRoleType.RecipientAdmin)
				{
					adgroup = ((IDirectorySession)base.DataSession).ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EmaWkGuid, this.ConfigurationSession.ConfigurationNamingContext.ToDNString());
				}
				else if (this.Role == DelegateRoleType.PublicFolderAdmin)
				{
					adgroup = ((IDirectorySession)base.DataSession).ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EpaWkGuid, this.ConfigurationSession.ConfigurationNamingContext.ToDNString());
				}
				else if (this.Role == DelegateRoleType.ViewOnlyAdmin || this.Role == DelegateRoleType.ServerAdmin)
				{
					adgroup = ((IDirectorySession)base.DataSession).ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EraWkGuid, this.ConfigurationSession.ConfigurationNamingContext.ToDNString());
				}
			}
			finally
			{
				recipientSession.UseGlobalCatalog = useGlobalCatalog;
			}
			if (adgroup == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorExchangeAdministratorsGroupNotFound(this.Role.ToString(), this.Identity.ToString())), ErrorCategory.InvalidData, this.Role);
			}
			return adgroup;
		}

		private bool add = true;

		protected ADRecipient user;

		protected Server server;

		internal IConfigurationSession configSession;
	}
}
