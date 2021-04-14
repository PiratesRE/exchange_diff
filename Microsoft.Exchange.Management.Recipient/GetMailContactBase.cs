using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetMailContactBase : GetRecipientWithAddressListBase<MailContactIdParameter, ADContact>
	{
		public GetMailContactBase()
		{
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetMailContactBase.SortPropertiesArray;
			}
		}

		protected override string SystemAddressListRdn
		{
			get
			{
				return "All Contacts(VLV)";
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
				return ObjectSchema.GetInstance<MailContactSchema>();
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(GetMailContactBase.AllowedRecipientTypeDetails, value);
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
			return new MailContact((ADContact)dataObject);
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
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailContact,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailForestContact
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			MailEnabledRecipientSchema.Alias,
			MailEnabledRecipientSchema.DisplayName
		};

		private int numMailForestContact;
	}
}
