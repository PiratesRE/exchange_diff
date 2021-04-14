using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarFolderSchema : Schema
	{
		static CalendarFolderSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				CalendarFolderSchema.SharingEffectiveRights,
				CalendarFolderSchema.PermissionSet
			};
			CalendarFolderSchema.schema = new CalendarFolderSchema(xmlElements);
		}

		private CalendarFolderSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return CalendarFolderSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation SharingEffectiveRights = new PropertyInformation("SharingEffectiveRights", ExchangeVersion.Exchange2010, null, new PropertyUri(PropertyUriEnum.FolderSharingEffectiveRights), new PropertyCommand.CreatePropertyCommand(SharingEffectiveRightsProperty.CreateCommand));

		public static readonly PropertyInformation PermissionSet = new PropertyInformation("PermissionSet", ExchangeVersion.Exchange2007SP1, null, new PropertyUri(PropertyUriEnum.PermissionSet), new PropertyCommand.CreatePropertyCommand(CalendarPermissionSetProperty.CreateCommand));
	}
}
