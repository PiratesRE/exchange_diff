using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class FolderSchema : Schema
	{
		static FolderSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				FolderSchema.PermissionSet,
				FolderSchema.UnreadCount
			};
			FolderSchema.schema = new FolderSchema(xmlElements);
		}

		private FolderSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
			IList<PropertyInformation> propertyInformationListByShapeEnum = base.GetPropertyInformationListByShapeEnum(ShapeEnum.AllProperties);
			propertyInformationListByShapeEnum.Remove(FolderSchema.PermissionSet);
		}

		public static Schema GetSchema()
		{
			return FolderSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation UnreadCount = new PropertyInformation("UnreadCount", ExchangeVersion.Exchange2007, FolderSchema.UnreadCount, new PropertyUri(PropertyUriEnum.UnreadCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation PermissionSet = new PropertyInformation("PermissionSet", ExchangeVersion.Exchange2007SP1, null, new PropertyUri(PropertyUriEnum.PermissionSet), new PropertyCommand.CreatePropertyCommand(PermissionSetProperty.CreateCommand));
	}
}
