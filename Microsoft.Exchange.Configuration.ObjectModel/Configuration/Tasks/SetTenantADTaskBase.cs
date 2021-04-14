﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetTenantADTaskBase<TIdentity, TPublicObject, TDataObject> : SetObjectWithIdentityTaskBase<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new() where TDataObject : IConfigurable, new()
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		internal ADSessionSettings SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.ResolveCurrentOrgIdBasedOnIdentity(this.Identity);
			this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.ResolveCurrentOrgIdBasedOnIdentity(this.Identity);
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			this.sessionSettings = null;
		}

		private ADSessionSettings sessionSettings;
	}
}
