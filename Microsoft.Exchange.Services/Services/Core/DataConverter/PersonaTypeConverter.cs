using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PersonaTypeConverter : EnumConverter
	{
		public static PersonType Parse(string propertyString)
		{
			PersonType result;
			if (!EnumValidator.TryParse<PersonType>(propertyString, EnumParseOptions.Default, out result))
			{
				result = PersonType.Unknown;
			}
			return result;
		}

		public static string ToString(PersonType propertyValue)
		{
			string result = null;
			switch (propertyValue)
			{
			case PersonType.Unknown:
				result = PersonaTypeConverter.UnknownString;
				break;
			case PersonType.Person:
				result = PersonaTypeConverter.PersonString;
				break;
			case PersonType.DistributionList:
				result = PersonaTypeConverter.DistributionListString;
				break;
			case PersonType.Room:
				result = PersonaTypeConverter.RoomString;
				break;
			case PersonType.Place:
				result = PersonaTypeConverter.PlaceString;
				break;
			case PersonType.ModernGroup:
				if (CallContext.Current != null && (CallContext.Current.WorkloadType == WorkloadType.Owa || CallContext.Current.WorkloadType == WorkloadType.OwaVoice))
				{
					result = PersonaTypeConverter.ModernGroupString;
				}
				else
				{
					result = PersonaTypeConverter.PersonString;
				}
				break;
			}
			return result;
		}

		public override object ConvertToObject(string propertyString)
		{
			return PersonaTypeConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return PersonaTypeConverter.ToString((PersonType)propertyValue);
		}

		private static readonly string PersonString = PersonType.Person.ToString();

		private static readonly string DistributionListString = PersonType.DistributionList.ToString();

		private static readonly string RoomString = PersonType.Room.ToString();

		private static readonly string PlaceString = PersonType.Place.ToString();

		private static readonly string ModernGroupString = PersonType.ModernGroup.ToString();

		private static readonly string UnknownString = PersonType.Unknown.ToString();
	}
}
