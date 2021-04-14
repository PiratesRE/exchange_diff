using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class SecondaryDomainTaskBase : ComponentInfoBasedTask
	{
		internal IConfigurationSession Session { get; set; }

		private protected ADObjectId RootOrgContainerId { protected get; private set; }

		protected SecondaryDomainTaskBase()
		{
			base.ImplementsResume = false;
			base.IsTenantOrganization = true;
			base.IsDatacenter = true;
			base.ShouldLoadDatacenterConfigFile = false;
			base.ComponentInfoFileNames = new List<string>();
			base.ComponentInfoFileNames.Add("setup\\data\\DatacenterSecondaryDomainConfig.xml");
		}

		public new LongPath UpdatesDir
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected override bool IsInnerRunspaceThrottlingEnabled
		{
			get
			{
				return true;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new ManageOrganizationTaskModuleFactory();
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.RootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
		}

		internal virtual IConfigurationSession CreateSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, this.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.RescopeToSubtree(sessionSettings), 110, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\SecondaryDomainTaskBase.cs");
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.Session = this.CreateSession();
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is DataSourceOperationException || e is DataSourceTransientException || e is DataValidationException || e is ManagementObjectNotFoundException || e is ManagementObjectAmbiguousException || base.IsKnownException(e);
		}
	}
}
