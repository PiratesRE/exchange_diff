using System;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IRecipient
	{
		string EmailAddress { get; set; }

		string Name { get; set; }

		string RoutingType { get; set; }
	}
}
