using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "SyncDistributionGroupMember", SupportsShouldProcess = true, DefaultParameterSetName = "AddOrRemove")]
	public sealed class UpdateSyncDistributionGroupMember : UpdateDistributionGroupMemberBase
	{
		internal override IReferenceErrorReporter ReferenceErrorReporter
		{
			get
			{
				if (this.batchReferenceErrorReporter == null)
				{
					this.batchReferenceErrorReporter = new BatchReferenceErrorReporter();
				}
				return this.batchReferenceErrorReporter;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override DistributionGroupIdParameter Identity
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

		[Parameter(Mandatory = false, ParameterSetName = "Replace")]
		public new MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> Members
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>)base.Fields["Members"];
			}
			set
			{
				base.Fields["Members"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddOrRemove")]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> AddedMembers
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>)base.Fields["AddedMembers"];
			}
			set
			{
				base.Fields["AddedMembers"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddOrRemove")]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> RemovedMembers
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>)base.Fields["RemovedMembers"];
			}
			set
			{
				base.Fields["RemovedMembers"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RawMembers")]
		public MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> RawMembers
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>)base.Fields["RawMembers"];
			}
			set
			{
				base.Fields["RawMembers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedObject"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SoftDeletedObject"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.IncludeSoftDeletedObjects.IsPresent)
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = true;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("AddedMembers") && this.AddedMembers != null && this.AddedMembers.Count > 0)
			{
				foreach (RecipientWithAdUserGroupIdParameter<RecipientIdParameter> recipientWithAdUserGroupIdParameter in this.AddedMembers)
				{
					ADObjectId memberid;
					if (!ADObjectId.TryParseDnOrGuid(recipientWithAdUserGroupIdParameter.RawIdentity, out memberid) || !MailboxTaskHelper.GroupContainsMember(this.DataObject, memberid, base.TenantGlobalCatalogSession))
					{
						MailboxTaskHelper.ValidateAndAddMember(base.TenantGlobalCatalogSession, this.DataObject, recipientWithAdUserGroupIdParameter, false, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
					}
				}
			}
			if (base.Fields.IsModified("RemovedMembers") && this.RemovedMembers != null && this.RemovedMembers.Count > 0)
			{
				foreach (RecipientWithAdUserGroupIdParameter<RecipientIdParameter> recipientWithAdUserGroupIdParameter2 in this.RemovedMembers)
				{
					ADObjectId memberid2;
					if (!ADObjectId.TryParseDnOrGuid(recipientWithAdUserGroupIdParameter2.RawIdentity, out memberid2) || MailboxTaskHelper.GroupContainsMember(this.DataObject, memberid2, base.TenantGlobalCatalogSession))
					{
						MailboxTaskHelper.ValidateAndRemoveMember(base.TenantGlobalCatalogSession, this.DataObject, recipientWithAdUserGroupIdParameter2, this.Identity.RawIdentity, false, new Task.TaskErrorLoggingDelegate(base.WriteError), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>));
					}
				}
			}
			if (this.RawMembers != null && this.RawMembers.IsChangesOnlyCopy)
			{
				this.UpdateMembersWhenRawMembersChanged();
			}
			TaskLogger.LogExit();
		}

		protected override bool ShouldSkipRangedAttributes()
		{
			int num = 0;
			if (base.Fields.IsModified("AddedMembers") && this.AddedMembers != null)
			{
				num += this.AddedMembers.Count;
			}
			if (base.Fields.IsModified("RemovedMembers") && this.RemovedMembers != null)
			{
				num += this.RemovedMembers.Count;
			}
			if (base.Fields.IsModified("RawMembers") && this.RawMembers != null && this.RawMembers.IsChangesOnlyCopy)
			{
				this.addedValues = this.GetChangedValues(true);
				this.removedValues = this.GetChangedValues(false);
				num += this.addedValues.Length;
				num += this.removedValues.Length;
			}
			return base.Fields.IsModified("Members") || num < 100;
		}

		private void UpdateMembersWhenRawMembersChanged()
		{
			if (this.addedValues.Length > 0)
			{
				SyncTaskHelper.ResolveModifiedMultiReferenceParameter<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>("RawMembers", "AddedMembers", this.addedValues, new GetRecipientDelegate<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>(this.GetRecipient), this.ReferenceErrorReporter, this.recipientIdsDictionary, this.recipientsDictionary, this.parameterDictionary);
				SyncTaskHelper.ValidateModifiedMultiReferenceParameter<ADGroup>("RawMembers", "AddedMembers", this.DataObject, SyncTaskHelper.ValidateWithBaseObjectBypassADUser<ADGroup>(new ValidateRecipientWithBaseObjectDelegate<ADGroup>(MailboxTaskHelper.ValidateGroupMember)), this.ReferenceErrorReporter, this.recipientsDictionary, this.parameterDictionary);
				SyncTaskHelper.AddModifiedRecipientIds("AddedMembers", SyncDistributionGroupSchema.Members, this.DataObject, this.recipientIdsDictionary, new Func<ADGroup, ADObjectId, IConfigDataProvider, bool>(MailboxTaskHelper.GroupContainsMember), base.TenantGlobalCatalogSession);
			}
			if (this.removedValues.Length > 0)
			{
				SyncTaskHelper.ResolveModifiedMultiReferenceParameter<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>("RawMembers", "RemovedMembers", this.removedValues, new GetRecipientDelegate<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>(this.GetRecipient), this.ReferenceErrorReporter, this.recipientIdsDictionary, this.recipientsDictionary, this.parameterDictionary);
				SyncTaskHelper.RemoveModifiedRecipientIds("RemovedMembers", SyncDistributionGroupSchema.Members, this.DataObject, this.recipientIdsDictionary, new Func<ADGroup, ADObjectId, IConfigDataProvider, bool>(MailboxTaskHelper.GroupContainsMember), base.TenantGlobalCatalogSession);
			}
		}

		private ADRecipient GetRecipient(RecipientWithAdUserGroupIdParameter<RecipientIdParameter> IdParameter, Task.ErrorLoggerDelegate writeError)
		{
			return (ADRecipient)base.GetDataObject<ADRecipient>(IdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(IdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(IdParameter.ToString())));
		}

		private RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[] GetChangedValues(bool added)
		{
			if (this.RawMembers == null)
			{
				return new RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[0];
			}
			object[] array = added ? this.RawMembers.Added : this.RawMembers.Removed;
			RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[] array2 = new RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[array.Length];
			array.CopyTo(array2, 0);
			return array2;
		}

		private Dictionary<object, MultiValuedProperty<ADObjectId>> recipientIdsDictionary = new Dictionary<object, MultiValuedProperty<ADObjectId>>();

		private Dictionary<object, MultiValuedProperty<ADRecipient>> recipientsDictionary = new Dictionary<object, MultiValuedProperty<ADRecipient>>();

		private Dictionary<ADRecipient, IIdentityParameter> parameterDictionary = new Dictionary<ADRecipient, IIdentityParameter>();

		private BatchReferenceErrorReporter batchReferenceErrorReporter;

		private RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[] addedValues = new RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[0];

		private RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[] removedValues = new RecipientWithAdUserGroupIdParameter<RecipientIdParameter>[0];
	}
}
