using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Update", "RoleGroupMember", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class UpdateRoleGroupMember : RoleGroupMemberTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateRoleGroupMember(this.Identity.ToString(), RoleGroupCommon.NamesFromObjects(this.Members));
			}
		}

		[AllowNull]
		[Parameter]
		public MultiValuedProperty<SecurityPrincipalIdParameter> Members
		{
			get
			{
				return (MultiValuedProperty<SecurityPrincipalIdParameter>)base.Fields[ADGroupSchema.Members];
			}
			set
			{
				base.Fields[ADGroupSchema.Members] = value;
			}
		}

		private new SecurityPrincipalIdParameter Member
		{
			get
			{
				return null;
			}
		}

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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			bool flag = false;
			if (base.Fields.IsModified(ADGroupSchema.Members))
			{
				if (this.DataObject.RoleGroupType == RoleGroupType.Linked)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorLinkedRoleGroupCannotHaveMembers), (ErrorCategory)1000, null);
				}
				List<ADObjectId> list = null;
				SecurityPrincipalIdParameter[] parameters = this.GetChangedValues(true);
				SecurityPrincipalIdParameter[] changedValues = this.GetChangedValues(false);
				if (this.Members == null || !this.Members.IsChangesOnlyCopy)
				{
					flag = true;
					if (this.Members != null)
					{
						parameters = this.Members.ToArray();
					}
					list = this.DataObject.Members.ToList<ADObjectId>();
					this.DataObject.Members = new MultiValuedProperty<ADObjectId>();
				}
				SyncTaskHelper.ResolveModifiedMultiReferenceParameter<SecurityPrincipalIdParameter>("Members", "AddedMembers", parameters, new GetRecipientDelegate<SecurityPrincipalIdParameter>(this.GetRecipient), this.ReferenceErrorReporter, this.recipientIdsDictionary, this.recipientsDictionary, this.parameterDictionary);
				SyncTaskHelper.ValidateModifiedMultiReferenceParameter<ADGroup>("Members", "AddedMembers", this.DataObject, new ValidateRecipientWithBaseObjectDelegate<ADGroup>(MailboxTaskHelper.ValidateGroupMember), this.ReferenceErrorReporter, this.recipientsDictionary, this.parameterDictionary);
				SyncTaskHelper.AddModifiedRecipientIds("AddedMembers", SyncDistributionGroupSchema.Members, this.DataObject, this.recipientIdsDictionary);
				if (flag)
				{
					MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
					foreach (ADObjectId item in list)
					{
						if (!this.DataObject.Members.Contains(item))
						{
							multiValuedProperty.Add(item);
						}
					}
					this.recipientIdsDictionary["RemovedMembers"] = multiValuedProperty;
				}
				else
				{
					SyncTaskHelper.ResolveModifiedMultiReferenceParameter<SecurityPrincipalIdParameter>("Members", "RemovedMembers", changedValues, new GetRecipientDelegate<SecurityPrincipalIdParameter>(this.GetRecipient), this.ReferenceErrorReporter, this.recipientIdsDictionary, this.recipientsDictionary, this.parameterDictionary);
					SyncTaskHelper.RemoveModifiedRecipientIds("RemovedMembers", SyncDistributionGroupSchema.Members, this.DataObject, this.recipientIdsDictionary);
				}
			}
			MailboxTaskHelper.ValidateAddedMembers(base.TenantGlobalCatalogSession, this.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
			if (this.recipientIdsDictionary.ContainsKey("RemovedMembers") && this.recipientIdsDictionary["RemovedMembers"].Count > 0)
			{
				RoleAssignmentsGlobalConstraints roleAssignmentsGlobalConstraints = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
				roleAssignmentsGlobalConstraints.ValidateIsSafeToRemoveRoleGroupMember(this.DataObject, new List<ADObjectId>(this.recipientIdsDictionary["RemovedMembers"]));
			}
			TaskLogger.LogExit();
		}

		protected override void PerformGroupMemberAction()
		{
		}

		internal ADRecipient GetRecipient(SecurityPrincipalIdParameter securityPrincipalIdParameter, Task.ErrorLoggerDelegate writeError)
		{
			return (ADRecipient)base.GetDataObject<ADRecipient>(securityPrincipalIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(securityPrincipalIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(securityPrincipalIdParameter.ToString())));
		}

		private SecurityPrincipalIdParameter[] GetChangedValues(bool added)
		{
			if (this.Members == null)
			{
				return new SecurityPrincipalIdParameter[0];
			}
			object[] array = added ? this.Members.Added : this.Members.Removed;
			SecurityPrincipalIdParameter[] array2 = new SecurityPrincipalIdParameter[array.Length];
			array.CopyTo(array2, 0);
			return array2;
		}

		private static object[] emptyObjectArray = new object[0];

		private Dictionary<object, MultiValuedProperty<ADObjectId>> recipientIdsDictionary = new Dictionary<object, MultiValuedProperty<ADObjectId>>();

		private Dictionary<object, MultiValuedProperty<ADRecipient>> recipientsDictionary = new Dictionary<object, MultiValuedProperty<ADRecipient>>();

		private Dictionary<ADRecipient, IIdentityParameter> parameterDictionary = new Dictionary<ADRecipient, IIdentityParameter>();

		private BatchReferenceErrorReporter batchReferenceErrorReporter;
	}
}
