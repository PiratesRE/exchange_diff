using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PostalAddressAttributedValueConverter : BaseConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return null;
		}

		public override string ConvertToString(object propertyValue)
		{
			return string.Empty;
		}

		protected override object ConvertToServiceObjectValue(object propertyValue)
		{
			if (propertyValue == null)
			{
				return null;
			}
			AttributedValue<Microsoft.Exchange.Data.Storage.PostalAddress> attributedValue = (AttributedValue<Microsoft.Exchange.Data.Storage.PostalAddress>)propertyValue;
			return new PostalAddressAttributedValue
			{
				Attributions = attributedValue.Attributions,
				Value = new Microsoft.Exchange.Services.Core.Types.PostalAddress
				{
					Street = attributedValue.Value.Street,
					City = attributedValue.Value.City,
					State = attributedValue.Value.State,
					Country = attributedValue.Value.Country,
					PostalCode = attributedValue.Value.PostalCode,
					PostOfficeBox = attributedValue.Value.PostOfficeBox,
					LocationSource = (LocationSourceType)attributedValue.Value.LocationSource,
					LocationUri = attributedValue.Value.LocationUri,
					Type = attributedValue.Value.Type,
					Latitude = attributedValue.Value.Latitude,
					Longitude = attributedValue.Value.Longitude,
					Accuracy = attributedValue.Value.Accuracy,
					Altitude = attributedValue.Value.Altitude,
					AltitudeAccuracy = attributedValue.Value.AltitudeAccuracy
				}
			};
		}
	}
}
