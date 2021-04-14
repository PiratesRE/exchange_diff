using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.DataModel
{
	public class Location
	{
		public string DisplayName { get; set; }

		public string Annotation { get; set; }

		public PostalAddress Address { get; set; }
	}
}
