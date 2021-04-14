using System;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class LocationDataEntityConverter
	{
		internal static Location ToLocation(this Location dataEntityLocation)
		{
			if (dataEntityLocation == null)
			{
				return null;
			}
			return new Location
			{
				DisplayName = dataEntityLocation.DisplayName
			};
		}

		internal static Location ToDataEntityLocation(this Location location)
		{
			if (location == null)
			{
				return null;
			}
			return new Location
			{
				DisplayName = location.DisplayName
			};
		}
	}
}
