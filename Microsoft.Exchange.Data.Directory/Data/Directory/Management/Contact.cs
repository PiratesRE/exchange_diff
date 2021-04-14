using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class Contact : OrgPersonPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return Contact.schema;
			}
		}

		public Contact()
		{
		}

		public Contact(ADContact dataObject) : base(dataObject)
		{
		}

		internal static Contact FromDataObject(ADContact dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new Contact(dataObject);
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[ContactSchema.OrganizationalUnit];
			}
		}

		private static ContactSchema schema = ObjectSchema.GetInstance<ContactSchema>();
	}
}
