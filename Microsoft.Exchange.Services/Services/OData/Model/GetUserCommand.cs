using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetUserCommand : ODataCommand<GetUserRequest, GetUserResponse>
	{
		public GetUserCommand(GetUserRequest request) : base(request)
		{
		}

		protected override GetUserResponse InternalExecute()
		{
			UserQueryAdapter userQueryAdapter = new UserQueryAdapter(UserSchema.SchemaInstance, base.Request.ODataQueryOptions);
			User result;
			if (base.Request.Id.Equals("Me", StringComparison.OrdinalIgnoreCase))
			{
				ADUser accessingADUser = base.Request.ODataContext.CallContext.AccessingADUser;
				result = UserProvider.ADUserToEntity(accessingADUser, userQueryAdapter.RequestedProperties);
			}
			else
			{
				UserProvider userProvider = new UserProvider(base.Request.ODataContext.CallContext.ADRecipientSessionContext.GetADRecipientSession());
				result = userProvider.Read(base.Request.Id, userQueryAdapter);
			}
			return new GetUserResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
