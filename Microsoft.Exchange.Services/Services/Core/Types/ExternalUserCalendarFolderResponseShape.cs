using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExternalUserCalendarFolderResponseShape : ExternalUserResponseShape
	{
		protected override List<PropertyPath> PropertiesAllowedForReadAccess
		{
			get
			{
				return ExternalUserCalendarFolderResponseShape.properties;
			}
		}

		private static List<PropertyPath> properties = new List<PropertyPath>
		{
			ItemSchema.ItemId.PropertyPath,
			CalendarFolderSchema.SharingEffectiveRights.PropertyPath
		};
	}
}
