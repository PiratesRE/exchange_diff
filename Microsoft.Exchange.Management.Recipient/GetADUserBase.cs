using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetADUserBase<TIdentity> : GetRecipientBase<TIdentity, ADUser> where TIdentity : RecipientIdParameter, new()
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter SoftDeletedUser
		{
			get
			{
				return base.SoftDeletedObject;
			}
			set
			{
				base.SoftDeletedObject = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			ADObjectId searchRoot = recipientSession.SearchRoot;
			if (this.SoftDeletedUser.IsPresent && base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null)
			{
				searchRoot = new ADObjectId("OU=Soft Deleted Objects," + base.CurrentOrganizationId.OrganizationalUnit.DistinguishedName);
			}
			if (this.SoftDeletedUser.IsPresent)
			{
				recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(recipientSession.DomainController, searchRoot, recipientSession.Lcid, recipientSession.ReadOnly, recipientSession.ConsistencyMode, recipientSession.NetworkCredential, recipientSession.SessionSettings, 74, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\user\\GetADUserBase.cs");
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = recipientSession.EnforceDefaultScope;
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = recipientSession.UseGlobalCatalog;
				tenantOrRootOrgRecipientSession.LinkResolutionServer = recipientSession.LinkResolutionServer;
				recipientSession = tenantOrRootOrgRecipientSession;
			}
			return recipientSession;
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetADUserBase<UserIdParameter>.SortPropertiesArray;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return User.FromDataObject((ADUser)dataObject);
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<UserSchema>();
			}
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			OrgPersonPresentationObjectSchema.DisplayName,
			OrgPersonPresentationObjectSchema.FirstName,
			OrgPersonPresentationObjectSchema.LastName,
			OrgPersonPresentationObjectSchema.Office,
			OrgPersonPresentationObjectSchema.City
		};
	}
}
