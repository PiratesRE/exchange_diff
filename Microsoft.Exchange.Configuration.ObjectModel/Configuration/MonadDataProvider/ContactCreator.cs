using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class ContactCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"FirstName",
				"LastName",
				"Initials",
				"Name",
				"SimpleDisplayName",
				"WebPage",
				"Notes",
				"StreetAddress",
				"City",
				"StateOrProvince",
				"PostalCode",
				"CountryOrRegion",
				"Phone",
				"Pager",
				"Fax",
				"HomePhone",
				"MobilePhone",
				"Title",
				"Company",
				"Department",
				"Office",
				"Manager"
			};
		}
	}
}
