using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UserComparer : IStringComparer
	{
		private UserComparer()
		{
		}

		public static UserComparer CreateInstance()
		{
			return UserComparer.intance;
		}

		public bool Equals(string userX, string userY)
		{
			if (string.IsNullOrEmpty(userX))
			{
				throw new ArgumentNullException("userX");
			}
			if (string.IsNullOrEmpty(userY))
			{
				throw new ArgumentNullException("userY");
			}
			return userX.Equals(userY, StringComparison.InvariantCultureIgnoreCase);
		}

		private static UserComparer intance = new UserComparer();
	}
}
