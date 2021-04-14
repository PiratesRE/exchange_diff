using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ContactConverter
	{
		internal enum PostalAddressIndexType
		{
			None,
			Home,
			Business,
			Other
		}

		internal class PostalAddressIndex : BaseConverter
		{
			public static ContactConverter.PostalAddressIndexType Parse(string value)
			{
				if (value != null)
				{
					if (value == "None")
					{
						return ContactConverter.PostalAddressIndexType.None;
					}
					if (value == "Business")
					{
						return ContactConverter.PostalAddressIndexType.Business;
					}
					if (value == "Home")
					{
						return ContactConverter.PostalAddressIndexType.Home;
					}
					if (value == "Other")
					{
						return ContactConverter.PostalAddressIndexType.Other;
					}
				}
				throw new InvalidValueForPropertyException(new PropertyUri(PropertyUriEnum.PostalAddressIndex), null);
			}

			public static string ToString(ContactConverter.PostalAddressIndexType postalAddressIndex)
			{
				switch (postalAddressIndex)
				{
				case ContactConverter.PostalAddressIndexType.None:
					return "None";
				case ContactConverter.PostalAddressIndexType.Home:
					return "Home";
				case ContactConverter.PostalAddressIndexType.Business:
					return "Business";
				case ContactConverter.PostalAddressIndexType.Other:
					return "Other";
				default:
					throw new InvalidValueForPropertyException(new PropertyUri(PropertyUriEnum.PostalAddressIndex), null);
				}
			}

			public override object ConvertToObject(string propertyString)
			{
				return ContactConverter.PostalAddressIndex.Parse(propertyString);
			}

			public override string ConvertToString(object propertyValue)
			{
				return ContactConverter.PostalAddressIndex.ToString((ContactConverter.PostalAddressIndexType)propertyValue);
			}

			private const string None = "None";

			private const string Business = "Business";

			private const string Home = "Home";

			private const string Other = "Other";
		}

		internal class FileAsMapping : EnumConverter
		{
			public static Microsoft.Exchange.Data.Storage.FileAsMapping Parse(string value)
			{
				switch (value)
				{
				case "Company":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.Company;
				case "CompanyLastCommaFirst":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.CompanyLastCommaFirst;
				case "CompanyLastFirst":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.CompanyLastFirst;
				case "CompanyLastSpaceFirst":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.CompanyLastSpaceFirst;
				case "FirstSpaceLast":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.FirstSpaceLast;
				case "LastCommaFirst":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastCommaFirst;
				case "LastCommaFirstCompany":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastCommaFirstCompany;
				case "LastFirst":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirst;
				case "LastFirstCompany":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirstCompany;
				case "LastFirstSuffix":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirstSuffix;
				case "LastSpaceFirst":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastSpaceFirst;
				case "LastSpaceFirstCompany":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastSpaceFirstCompany;
				case "None":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.None;
				case "DisplayName":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.DisplayName;
				case "Empty":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.Empty;
				case "FirstName":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.GivenName;
				case "LastFirstMiddleSuffix":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirstMiddleSuffix;
				case "LastName":
					return Microsoft.Exchange.Data.Storage.FileAsMapping.LastName;
				}
				throw new InvalidValueForPropertyException(new PropertyUri(PropertyUriEnum.FileAsMapping), null);
			}

			public static string ToString(Microsoft.Exchange.Data.Storage.FileAsMapping fileAsMapping)
			{
				if (fileAsMapping <= Microsoft.Exchange.Data.Storage.FileAsMapping.Company)
				{
					if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.None)
					{
						return "None";
					}
					if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.Company)
					{
						return "Company";
					}
				}
				else
				{
					switch (fileAsMapping)
					{
					case Microsoft.Exchange.Data.Storage.FileAsMapping.LastCommaFirst:
						return "LastCommaFirst";
					case Microsoft.Exchange.Data.Storage.FileAsMapping.CompanyLastCommaFirst:
						return "CompanyLastCommaFirst";
					case Microsoft.Exchange.Data.Storage.FileAsMapping.LastCommaFirstCompany:
						return "LastCommaFirstCompany";
					default:
						switch (fileAsMapping)
						{
						case Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirst:
							return "LastFirst";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.LastSpaceFirst:
							return "LastSpaceFirst";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.CompanyLastFirst:
							return "CompanyLastFirst";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.CompanyLastSpaceFirst:
							return "CompanyLastSpaceFirst";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirstCompany:
							return "LastFirstCompany";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.LastSpaceFirstCompany:
							return "LastSpaceFirstCompany";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirstSuffix:
							return "LastFirstSuffix";
						case Microsoft.Exchange.Data.Storage.FileAsMapping.FirstSpaceLast:
							return "FirstSpaceLast";
						}
						break;
					}
				}
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
				{
					if (fileAsMapping <= Microsoft.Exchange.Data.Storage.FileAsMapping.DisplayName)
					{
						if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.Empty)
						{
							return "Empty";
						}
						if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.DisplayName)
						{
							return "DisplayName";
						}
					}
					else
					{
						if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.GivenName)
						{
							return "FirstName";
						}
						if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.LastName)
						{
							return "LastName";
						}
						if (fileAsMapping == Microsoft.Exchange.Data.Storage.FileAsMapping.LastFirstMiddleSuffix)
						{
							return "LastFirstMiddleSuffix";
						}
					}
					return null;
				}
				return null;
			}

			public override object ConvertToObject(string propertyString)
			{
				return ContactConverter.FileAsMapping.Parse(propertyString);
			}

			public override string ConvertToString(object propertyValue)
			{
				return ContactConverter.FileAsMapping.ToString((Microsoft.Exchange.Data.Storage.FileAsMapping)propertyValue);
			}

			private const string Company = "Company";

			private const string CompanyLastCommaFirst = "CompanyLastCommaFirst";

			private const string CompanyLastFirst = "CompanyLastFirst";

			private const string CompanyLastSpaceFirst = "CompanyLastSpaceFirst";

			private const string Empty = "Empty";

			private const string DisplayName = "DisplayName";

			private const string FirstName = "FirstName";

			private const string FirstSpaceLast = "FirstSpaceLast";

			private const string LastCommaFirst = "LastCommaFirst";

			private const string LastCommaFirstCompany = "LastCommaFirstCompany";

			private const string LastFirst = "LastFirst";

			private const string LastFirstCompany = "LastFirstCompany";

			private const string LastFirstMiddleSuffix = "LastFirstMiddleSuffix";

			private const string LastFirstSuffix = "LastFirstSuffix";

			private const string LastName = "LastName";

			private const string LastSpaceFirst = "LastSpaceFirst";

			private const string LastSpaceFirstCompany = "LastSpaceFirstCompany";

			private const string None = "None";
		}
	}
}
