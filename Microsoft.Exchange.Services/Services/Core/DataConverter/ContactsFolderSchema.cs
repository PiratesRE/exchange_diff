using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ContactsFolderSchema : Schema
	{
		static ContactsFolderSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				ContactsFolderSchema.SharingEffectiveRights,
				FolderSchema.PermissionSet
			};
			ContactsFolderSchema.schema = new ContactsFolderSchema(xmlElements);
		}

		private ContactsFolderSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return ContactsFolderSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation SharingEffectiveRights = new PropertyInformation("SharingEffectiveRights", ExchangeVersion.Exchange2010, null, new PropertyUri(PropertyUriEnum.FolderSharingEffectiveRights), new PropertyCommand.CreatePropertyCommand(SharingEffectiveRightsProperty.CreateCommand));
	}
}
