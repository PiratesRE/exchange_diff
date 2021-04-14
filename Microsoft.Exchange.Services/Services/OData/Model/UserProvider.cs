using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UserProvider
	{
		public UserProvider(IRecipientSession recipientSession)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			this.RecipientSession = recipientSession;
		}

		public IRecipientSession RecipientSession { get; private set; }

		public static User ADUserToEntity(ADRawEntry user, IList<PropertyDefinition> properties)
		{
			ArgumentValidator.ThrowIfNull("user", user);
			ArgumentValidator.ThrowIfNull("properties", properties);
			User user2 = new User();
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				propertyDefinition.ADDriverPropertyProvider.GetPropertyFromDataSource(user2, propertyDefinition, user);
			}
			return user2;
		}

		public User Read(string id, UserQueryAdapter queryAdapter = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("id", id);
			SmtpProxyAddress proxyAddress = null;
			try
			{
				proxyAddress = new SmtpProxyAddress(id, false);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidUserException(id);
			}
			ADUser aduser = this.RecipientSession.FindByProxyAddress<ADUser>(proxyAddress);
			if (aduser == null)
			{
				throw new InvalidUserException(id);
			}
			return UserProvider.ADUserToEntity(aduser, queryAdapter.RequestedProperties);
		}

		public IFindEntitiesResult<User> Find(UserQueryAdapter queryAdapter)
		{
			ArgumentValidator.ThrowIfNull("queryAdapter", queryAdapter);
			QueryFilter queryFilter = queryAdapter.GetQueryFilter();
			ADPagedReader<ADRawEntry> source = this.RecipientSession.FindPagedADRawEntry(null, QueryScope.SubTree, QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
				queryFilter
			}), queryAdapter.GetSortBy(), 0, queryAdapter.GetRequestedADProperties());
			IEnumerable<ADRawEntry> enumerable = source.Skip(queryAdapter.GetSkipCount()).Take(queryAdapter.GetPageSize());
			List<User> list = new List<User>();
			foreach (ADRawEntry user in enumerable)
			{
				User item = UserProvider.ADUserToEntity(user, queryAdapter.RequestedProperties);
				list.Add(item);
			}
			return new FindEntitiesResult<User>(list, -1);
		}
	}
}
