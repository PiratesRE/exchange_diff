using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ABContactSchema : ABObjectSchema
	{
		public static readonly ABPropertyDefinition BusinessPhoneNumber = new ABPropertyDefinition("BusinessPhoneNumber", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition CompanyName = new ABPropertyDefinition("CompanyName", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition DepartmentName = new ABPropertyDefinition("DepartmentName", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition BusinessFaxNumber = new ABPropertyDefinition("BusinessFax", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition GivenName = new ABPropertyDefinition("GivenName", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition HomePhoneNumber = new ABPropertyDefinition("HomePhoneNumber", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition Initials = new ABPropertyDefinition("Initials", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition MobilePhoneNumber = new ABPropertyDefinition("MobilePhoneNumber", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition OfficeLocation = new ABPropertyDefinition("OfficeLocation", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition Surname = new ABPropertyDefinition("Surname", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WebPage = new ABPropertyDefinition("WebPage", typeof(Uri), PropertyDefinitionFlags.ReadOnly, null);

		public static readonly ABPropertyDefinition Title = new ABPropertyDefinition("Title", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WorkAddressPostOfficeBox = new ABPropertyDefinition("WorkAddressPostOfficeBox", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WorkAddressStreet = new ABPropertyDefinition("WorkAddressStreet", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WorkAddressCity = new ABPropertyDefinition("WorkAddressCity", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WorkAddressState = new ABPropertyDefinition("WorkAddressState", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WorkAddressPostalCode = new ABPropertyDefinition("WorkAddressPostalCode", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition WorkAddressCountry = new ABPropertyDefinition("WorkAddressCountry", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition Picture = new ABPropertyDefinition("ThumbnailPhoto", typeof(byte[]), PropertyDefinitionFlags.ReadOnly, null);
	}
}
