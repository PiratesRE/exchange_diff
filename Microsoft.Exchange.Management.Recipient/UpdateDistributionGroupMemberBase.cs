using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class UpdateDistributionGroupMemberBase : RecipientObjectActionTask<DistributionGroupIdParameter, ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateDistributionGroupMember(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> Members
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

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassSecurityGroupManagerCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassSecurityGroupManagerCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassSecurityGroupManagerCheck"] = value;
			}
		}

		internal IRecipientSession GlobalCatalogRBACSession
		{
			get
			{
				return this.globalCatalogRBACSession;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.SessionSettings.IncludeSoftDeletedObjectLinks = true;
			this.globalCatalogRBACSession = DistributionGroupMemberTaskBase<RecipientIdParameter>.CreateGlobalCatalogRBACSession(base.DomainController, base.SessionSettings);
			return base.CreateSession();
		}

		protected override IConfigurable ResolveDataObject()
		{
			bool skipRangedAttributes = ((IDirectorySession)base.DataSession).SkipRangedAttributes;
			bool skipRangedAttributes2 = base.TenantGlobalCatalogSession.SkipRangedAttributes;
			((IDirectorySession)base.DataSession).SkipRangedAttributes = this.ShouldSkipRangedAttributes();
			base.TenantGlobalCatalogSession.SkipRangedAttributes = this.ShouldSkipRangedAttributes();
			base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjectLinks = true;
			IConfigurable result = base.ResolveDataObject();
			((IDirectorySession)base.DataSession).SkipRangedAttributes = skipRangedAttributes;
			base.TenantGlobalCatalogSession.SkipRangedAttributes = skipRangedAttributes;
			return result;
		}

		protected virtual bool ShouldSkipRangedAttributes()
		{
			return true;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || this.DataObject.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.NotAValidDistributionGroup), ExchangeErrorCategory.Client, this.Identity.ToString());
			}
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("Members"))
			{
				DistributionGroupMemberTaskBase<DistributionGroupIdParameter>.GetExecutingUserAndCheckGroupOwnership(this, (IDirectorySession)base.DataSession, base.TenantGlobalCatalogSession, this.DataObject, this.BypassSecurityGroupManagerCheck);
				MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
				foreach (ADObjectId adobjectId in this.DataObject.Members)
				{
					if (-1 != adobjectId.DistinguishedName.IndexOf(",OU=Soft Deleted Objects,", StringComparison.OrdinalIgnoreCase))
					{
						multiValuedProperty.Add(adobjectId);
					}
				}
				this.DataObject.Members = multiValuedProperty;
				this.DataObject.Members.IsCompletelyRead = true;
				if (this.Members != null && this.Members.Count > 0)
				{
					foreach (RecipientWithAdUserGroupIdParameter<RecipientIdParameter> member in this.Members)
					{
						MailboxTaskHelper.ValidateAndAddMember(base.TenantGlobalCatalogSession, this.DataObject, member, false, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
					}
				}
			}
			TaskLogger.LogExit();
		}

		private IRecipientSession globalCatalogRBACSession;
	}
}
