using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class RemoveRecipientObjectTask<TIdentity, TDataObject> : RemoveADTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ADObject, new()
	{
		protected SwitchParameter ForReconciliation
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForReconciliation"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForReconciliation"] = value;
			}
		}

		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreDefaultScope"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreDefaultScope"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 162, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RemoveAdObjectTask.cs");
			if (this.IgnoreDefaultScope)
			{
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
			}
			return tenantOrRootOrgRecipientSession;
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADObject adobject = (ADObject)RecipientTaskHelper.ResolveDataObject<TDataObject>(base.DataSession, base.TenantGlobalCatalogSession, base.ServerSettings, this.Identity, this.RootId, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<TDataObject>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, adobject))
			{
				base.UnderscopeDataSession(adobject.OrganizationId);
				base.CurrentOrganizationId = adobject.OrganizationId;
			}
			ADRecipient adrecipient = adobject as ADRecipient;
			if (adrecipient != null)
			{
				adrecipient.BypassModerationCheck = true;
			}
			return adobject;
		}

		protected override void InternalValidate()
		{
			ADObjectId adobjectId;
			if (this.IgnoreDefaultScope && !RecipientTaskHelper.IsValidDistinguishedName(this.Identity, out adobjectId))
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorOnlyDNSupportedWithIgnoreDefaultScope), ExchangeErrorCategory.Client, this.Identity);
			}
			base.InternalValidate();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.IgnoreDefaultScope && base.DomainController != null)
			{
				base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorIgnoreDefaultScopeAndDCSetTogether), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			ADRecipient adrecipient = base.DataObject as ADRecipient;
			bool flag = this.ShouldSoftDeleteObject();
			if (!flag)
			{
				this.LoadRoleAssignments();
			}
			if (this.ForReconciliation && adrecipient != null && !string.IsNullOrEmpty(adrecipient.ExternalDirectoryObjectId))
			{
				adrecipient.ExternalDirectoryObjectId = string.Empty;
				try
				{
					if (!flag)
					{
						(base.DataSession as IRecipientSession).Save(base.DataObject as ADRecipient, true);
					}
					else
					{
						base.DataSession.Save(base.DataObject);
					}
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.ServerTransient, null);
				}
			}
			base.InternalProcessRecord();
			if (!flag)
			{
				this.RemoveRolesAssigments();
			}
		}

		protected override void SaveSoftDeletedObject()
		{
			TDataObject dataObject = base.DataObject;
			RecipientTaskHelper.CreateSoftDeletedObjectsContainerIfNecessary(dataObject.Id.Parent, base.DomainController);
			base.DataSession.Save(base.DataObject);
		}

		private void LoadRoleAssignments()
		{
			if (!typeof(TDataObject).IsAssignableFrom(typeof(ADUser)))
			{
				this.roleAssignments = null;
				return;
			}
			TDataObject dataObject = base.DataObject;
			ADObjectId adobjectId;
			if (!dataObject.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				TDataObject dataObject2 = base.DataObject;
				adobjectId = dataObject2.OrganizationId.ConfigurationUnit;
			}
			else
			{
				adobjectId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			}
			ADObjectId adobjectId2 = adobjectId;
			ADObjectId rootOrgId = adobjectId2;
			TDataObject dataObject3 = base.DataObject;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgId, dataObject3.OrganizationId, base.ExecutingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.TenantGlobalCatalogSession.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 340, "LoadRoleAssignments", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RemoveAdObjectTask.cs");
			IConfigurationSession configurationSession = tenantOrTopologyConfigurationSession;
			ADObjectId[] array = new ADObjectId[1];
			ADObjectId[] array2 = array;
			int num = 0;
			TDataObject dataObject4 = base.DataObject;
			array2[num] = dataObject4.Id;
			this.roleAssignments = configurationSession.FindRoleAssignmentsByUserIds(array, false);
		}

		private void RemoveRolesAssigments()
		{
			if (this.roleAssignments == null)
			{
				return;
			}
			foreach (Result<ExchangeRoleAssignment> result in this.roleAssignments)
			{
				string id = result.Data.Id.ToString();
				try
				{
					base.WriteVerbose(Strings.VerboseRemovingRoleAssignment(id));
					result.Data.Session.Delete(result.Data);
					base.WriteVerbose(Strings.VerboseRemovedRoleAssignment(id));
				}
				catch (Exception ex)
				{
					if (!this.IsKnownException(ex))
					{
						throw;
					}
					this.WriteWarning(Strings.WarningCouldNotRemoveRoleAssignment(id, ex.Message));
				}
			}
			this.roleAssignments = null;
		}

		[Obsolete("Use GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) instead")]
		protected new IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new()
		{
			return base.GetDataObject<TObject>(id, session, rootID, optionalData, notFoundError, multipleFoundError);
		}

		[Obsolete("Use GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) instead")]
		protected new IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new()
		{
			return base.GetDataObject<TObject>(id, session, rootID, null, notFoundError, multipleFoundError);
		}

		[Obsolete("Use ThrowTerminatingError(Exception exception, ExchangeErrorCategory category, object target) instead.")]
		protected new void ThrowTerminatingError(Exception exception, ErrorCategory category, object target)
		{
			base.ThrowTerminatingError(exception, category, target);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target, bool reThrow) instead.")]
		protected new void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow)
		{
			base.WriteError(exception, category, target, reThrow);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target) instead.")]
		internal new void WriteError(Exception exception, ErrorCategory category, object target)
		{
			base.WriteError(exception, category, target, true);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target, bool reThrow, string helpUrl) instead.")]
		protected new void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow, string helpUrl)
		{
			base.WriteError(exception, category, target, reThrow, helpUrl);
		}

		private Result<ExchangeRoleAssignment>[] roleAssignments;
	}
}
