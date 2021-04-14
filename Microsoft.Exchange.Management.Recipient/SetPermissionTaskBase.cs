using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetPermissionTaskBase<TIdentity, TPublicObject, TDataObject> : SetObjectWithIdentityTaskBase<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : AcePresentationObject, new() where TDataObject : IConfigurable, new()
	{
		internal ADObjectId DomainControllerDomainId
		{
			get
			{
				return this.domainControllerDomainId;
			}
		}

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

		internal IConfigurationSession ReadOnlyConfigurationSession
		{
			get
			{
				return this.readOnlyConfigurationSession;
			}
		}

		protected virtual int CurrentProcessedObject
		{
			get
			{
				return this.currentProcessedObject;
			}
			set
			{
				this.currentProcessedObject = value;
			}
		}

		protected List<List<ActiveDirectoryAccessRule>> ModifiedAcl
		{
			get
			{
				return this.modifiedAcl;
			}
		}

		protected List<TDataObject> ModifiedObjects
		{
			get
			{
				return this.modifiedObjects;
			}
		}

		protected SecurityIdentifier SecurityPrincipalSid
		{
			get
			{
				if (!(this.securityPrincipal.MasterAccountSid != null) || this.securityPrincipal.MasterAccountSid.IsWellKnown(WellKnownSidType.SelfSid))
				{
					return this.securityPrincipal.Sid;
				}
				return this.securityPrincipal.MasterAccountSid;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return null;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			this.readOnlyRecipientSession = PermissionTaskHelper.GetReadOnlyRecipientSession(this.DomainController);
			if (this.readOnlyRecipientSession.UseGlobalCatalog)
			{
				this.globalCatalogRecipientSession = this.readOnlyRecipientSession;
			}
			else
			{
				this.globalCatalogRecipientSession = PermissionTaskHelper.GetReadOnlyRecipientSession(null);
			}
			this.readOnlyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 207, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\permission\\SetPermissionTaskBase.cs");
			if (this.DomainController != null)
			{
				ADServer adserver = DirectoryUtilities.DomainControllerFromName(this.DomainController);
				if (adserver == null)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.DCWithGivenNameCouldNotBeFound(this.DomainController)), ErrorCategory.ObjectNotFound, null);
				}
				this.domainControllerDomainId = adserver.DomainId;
			}
			this.modifiedAcl.Clear();
			this.modifiedObjects.Clear();
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "AccessRights")]
		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Owner")]
		[Parameter(Mandatory = false, Position = 0, ParameterSetName = "Instance")]
		public override TIdentity Identity
		{
			get
			{
				if (base.Fields[SetPermissionTaskBase<TIdentity, TPublicObject, TDataObject>.paramIdentity] != null)
				{
					return (TIdentity)((object)base.Fields[SetPermissionTaskBase<TIdentity, TPublicObject, TDataObject>.paramIdentity]);
				}
				if (this.Instance != null)
				{
					TIdentity result = (default(TIdentity) == null) ? Activator.CreateInstance<TIdentity>() : default(TIdentity);
					TPublicObject instance = this.Instance;
					result.Initialize((ADObjectId)instance.Identity);
					return result;
				}
				return (TIdentity)((object)null);
			}
			set
			{
				base.Fields[SetPermissionTaskBase<TIdentity, TPublicObject, TDataObject>.paramIdentity] = value;
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

		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Instance")]
		public new TPublicObject Instance
		{
			get
			{
				return base.Instance;
			}
			set
			{
				base.Instance = value;
			}
		}

		protected bool IsInherited
		{
			get
			{
				if (this.Instance != null)
				{
					TPublicObject instance = this.Instance;
					return instance.IsInherited;
				}
				return false;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.IsInherited)
			{
				TPublicObject instance = this.Instance;
				this.WriteWarning(Strings.ErrorWillNotPerformOnInheritedAccessRight(instance.Identity.ToString()));
				return;
			}
			if (base.ParameterSetName == "Owner")
			{
				return;
			}
			TPublicObject instance2 = this.Instance;
			if (instance2.User != null)
			{
				IRecipientSession session = this.GlobalCatalogRecipientSession;
				TPublicObject instance3 = this.Instance;
				this.securityPrincipal = SecurityPrincipalIdParameter.GetSecurityPrincipal(session, instance3.User, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.IsInherited)
			{
				return;
			}
			this.PutEachEntryInList();
			TaskLogger.LogExit();
		}

		protected abstract void ApplyModification(TDataObject modifiedObject, ActiveDirectoryAccessRule[] modifiedAces);

		protected abstract void WriteAces(ADObjectId id, IEnumerable<ActiveDirectoryAccessRule> aces);

		protected void WriteCurrentProgress(LocalizedString activityDesc, LocalizedString status, ProgressRecordType recordType, int percent)
		{
			base.WriteProgress(new ExProgressRecord(0, activityDesc, status)
			{
				RecordType = recordType,
				PercentComplete = percent
			});
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if ("Owner" != base.ParameterSetName)
			{
				this.CurrentProcessedObject = 0;
				while (this.CurrentProcessedObject < this.ModifiedObjects.Count)
				{
					try
					{
						this.ApplyModification(this.ModifiedObjects[this.CurrentProcessedObject], this.ModifiedAcl[this.CurrentProcessedObject].ToArray());
					}
					catch (OverflowException exception)
					{
						this.WriteError(exception, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_180;
					}
					catch (DataValidationException exception2)
					{
						this.WriteError(exception2, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_180;
					}
					catch (DataSourceOperationException exception3)
					{
						this.WriteError(exception3, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_180;
					}
					catch (DataSourceTransientException exception4)
					{
						this.WriteError(exception4, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_180;
					}
					goto IL_C1;
					IL_180:
					this.CurrentProcessedObject++;
					continue;
					IL_C1:
					if (this.CurrentProcessedObject + 1 < this.ModifiedObjects.Count)
					{
						this.WriteCurrentProgress(Strings.ProcessingAceActivity, Strings.ProcessingAceStatus(((ADRawEntry)((object)this.ModifiedObjects[this.CurrentProcessedObject])).Id.ToString()), ProgressRecordType.Processing, this.CurrentProcessedObject * 100 / this.ModifiedObjects.Count);
					}
					this.WriteAces(((ADRawEntry)((object)this.ModifiedObjects[this.CurrentProcessedObject])).Id, this.ModifiedAcl[this.CurrentProcessedObject]);
					if (this.CurrentProcessedObject + 1 == this.ModifiedObjects.Count)
					{
						this.WriteCurrentProgress(Strings.ProcessingAceActivity, Strings.CompletedAceActivity, ProgressRecordType.Completed, 100);
						goto IL_180;
					}
					goto IL_180;
				}
			}
			TaskLogger.LogExit();
		}

		internal virtual void WriteErrorPerObject(LocalizedException exception, ExchangeErrorCategory category, object target)
		{
			if (target == null)
			{
				this.WriteError(exception, category, this.CurrentProcessedObject, false);
				return;
			}
			base.WriteError(exception, category, target);
		}

		protected abstract void ConstructAcl(List<ActiveDirectoryAccessRule> modifiedAcl);

		protected void PutEachEntryInList()
		{
			List<ActiveDirectoryAccessRule> item = null;
			bool flag = false;
			for (int i = 0; i < this.ModifiedObjects.Count; i++)
			{
				if (this.IsEqualEntry(i))
				{
					item = this.ModifiedAcl[i];
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				item = new List<ActiveDirectoryAccessRule>();
			}
			this.ConstructAcl(item);
			if (!flag)
			{
				this.ModifiedObjects.Add(this.DataObject);
				this.ModifiedAcl.Add(item);
			}
		}

		protected abstract bool IsEqualEntry(int index);

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is SecurityDescriptorAccessDeniedException;
		}

		private IRecipientSession readOnlyRecipientSession;

		private IRecipientSession globalCatalogRecipientSession;

		private IConfigurationSession readOnlyConfigurationSession;

		private ADObjectId domainControllerDomainId;

		private IADSecurityPrincipal securityPrincipal;

		private static string paramIdentity = "Identity";

		private List<List<ActiveDirectoryAccessRule>> modifiedAcl = new List<List<ActiveDirectoryAccessRule>>();

		private List<TDataObject> modifiedObjects = new List<TDataObject>();

		private int currentProcessedObject;
	}
}
