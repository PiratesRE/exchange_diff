using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class PublicDatabasePresentationObject : MailEnabledOrgPerson
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PublicDatabasePresentationObject.schema;
			}
		}

		public PublicDatabasePresentationObject()
		{
		}

		internal PublicDatabasePresentationObject(ADPublicDatabase dataObject) : base(dataObject)
		{
		}

		private static PublicDatabasePresentationObjectSchema schema = ObjectSchema.GetInstance<PublicDatabasePresentationObjectSchema>();
	}
}
