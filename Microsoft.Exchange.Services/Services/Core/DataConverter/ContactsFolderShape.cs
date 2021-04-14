using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ContactsFolderShape : Shape
	{
		static ContactsFolderShape()
		{
			ContactsFolderShape.defaultProperties.Add(BaseFolderSchema.FolderId);
			ContactsFolderShape.defaultProperties.Add(BaseFolderSchema.DisplayName);
			ContactsFolderShape.defaultProperties.Add(BaseFolderSchema.TotalCount);
			ContactsFolderShape.defaultProperties.Add(BaseFolderSchema.ChildFolderCount);
		}

		public ContactsFolderShape() : base(Schema.ContactsFolder, ContactsFolderSchema.GetSchema(), new BaseFolderShape(), ContactsFolderShape.defaultProperties)
		{
		}

		internal static ContactsFolderShape CreateShape()
		{
			return new ContactsFolderShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
