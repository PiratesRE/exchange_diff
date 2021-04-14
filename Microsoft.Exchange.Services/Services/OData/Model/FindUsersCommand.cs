using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindUsersCommand : ODataCommand<FindUsersRequest, FindUsersResponse>
	{
		public FindUsersCommand(FindUsersRequest request) : base(request)
		{
		}

		protected override FindUsersResponse InternalExecute()
		{
			UserProvider userProvider = new UserProvider(base.Request.ODataContext.CallContext.ADRecipientSessionContext.GetADRecipientSession());
			IFindEntitiesResult<User> result = userProvider.Find(new UserQueryAdapter(UserSchema.SchemaInstance, base.Request.ODataQueryOptions));
			return new FindUsersResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
