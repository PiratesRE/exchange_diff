using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "Contact", DefaultParameterSetName = "Identity")]
	public sealed class GetContact : GetRecipientBase<ContactIdParameter, ADContact>
	{
		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetContact.SortPropertiesArray;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return this.RecipientTypeDetails;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ContactSchema>();
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(GetContact.AllowedRecipientTypeDetails, value);
				base.Fields["RecipientTypeDetails"] = value;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADContact adcontact = (ADContact)dataObject;
			if (adcontact.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailForestContact)
			{
				this.numMailForestContact++;
			}
			return new Contact(adcontact);
		}

		protected override void InternalProcessRecord()
		{
			this.numMailForestContact = 0;
			base.InternalProcessRecord();
			if (this.numMailForestContact > 0)
			{
				this.WriteWarning(Strings.MailForestContactFound(this.numMailForestContact));
			}
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.Contact,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailContact,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailForestContact
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			OrgPersonPresentationObjectSchema.DisplayName,
			OrgPersonPresentationObjectSchema.FirstName,
			OrgPersonPresentationObjectSchema.LastName,
			OrgPersonPresentationObjectSchema.Office,
			OrgPersonPresentationObjectSchema.City
		};

		private int numMailForestContact;
	}
}
