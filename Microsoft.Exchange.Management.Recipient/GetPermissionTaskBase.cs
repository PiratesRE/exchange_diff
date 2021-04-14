using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetPermissionTaskBase<TIdentity, TDataObject> : GetObjectWithIdentityTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : IConfigurable, new()
	{
		internal IRecipientSession ReadOnlyRecipientSession
		{
			get
			{
				return this.readOnlyRecipientSession;
			}
		}

		internal IRecipientSession GlobalCatalogRecipientSession
		{
			get
			{
				return this.globalCatalogRecipientSession;
			}
		}

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

		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
		public override TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AccessRights")]
		public SecurityPrincipalIdParameter User
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["User"];
			}
			set
			{
				base.Fields["User"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Owner")]
		public SwitchParameter Owner
		{
			get
			{
				return (SwitchParameter)base.Fields["Owner"];
			}
			set
			{
				base.Fields["Owner"] = value;
			}
		}

		protected SecurityIdentifier SecurityPrincipal
		{
			get
			{
				return this.securityPrincipal;
			}
		}

		protected bool HasObjectMatchingIdentity
		{
			get
			{
				return this.hasObjectMatchingIdentity;
			}
			set
			{
				this.hasObjectMatchingIdentity = value;
			}
		}

		public GetPermissionTaskBase()
		{
			base.Fields["Owner"] = new SwitchParameter(false);
			base.Fields.ResetChangeTracking();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.readOnlyRecipientSession = PermissionTaskHelper.GetReadOnlyRecipientSession(this.DomainController);
			if (this.readOnlyRecipientSession.UseGlobalCatalog)
			{
				this.globalCatalogRecipientSession = this.readOnlyRecipientSession;
			}
			else
			{
				this.globalCatalogRecipientSession = PermissionTaskHelper.GetReadOnlyRecipientSession(null);
			}
			if (this.User != null)
			{
				this.securityPrincipal = SecurityPrincipalIdParameter.GetUserSid(this.GlobalCatalogRecipientSession, this.User, new Task.TaskErrorLoggingDelegate(base.ThrowTerminatingError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.HasObjectMatchingIdentity = false;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null)
			{
				LocalizedString? localizedString;
				IEnumerable<TDataObject> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
				this.WriteResult<TDataObject>(dataObjects);
				if (!base.HasErrors && !this.HasObjectMatchingIdentity)
				{
					LocalizedString? localizedString2 = localizedString;
					LocalizedString message;
					if (localizedString2 == null)
					{
						TIdentity identity = this.Identity;
						message = base.GetErrorMessageObjectNotFound(identity.ToString(), typeof(TDataObject).ToString(), (base.DataSession != null) ? base.DataSession.Source : null);
					}
					else
					{
						message = localizedString2.GetValueOrDefault();
					}
					base.WriteError(new ManagementObjectNotFoundException(message), ErrorCategory.InvalidData, null);
				}
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private bool hasObjectMatchingIdentity;

		private SecurityIdentifier securityPrincipal;

		private IRecipientSession readOnlyRecipientSession;

		private IRecipientSession globalCatalogRecipientSession;
	}
}
