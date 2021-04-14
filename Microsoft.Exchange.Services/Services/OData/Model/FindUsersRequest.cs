using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindUsersRequest : FindEntitiesRequest<User>
	{
		public FindUsersRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new FindUsersCommand(this);
		}
	}
}
