using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class UserNotFoundException : StoreException
	{
		public UserNotFoundException(LID lid, Guid userMailboxGuid) : base(lid, ErrorCodeValue.UnknownUser, string.Format("User not found: {0}", userMailboxGuid))
		{
		}

		public UserNotFoundException(LID lid, string userId) : base(lid, ErrorCodeValue.UnknownUser, string.Format("User not found: {0}", userId))
		{
		}

		public UserNotFoundException(LID lid, Guid userMailboxGuid, Exception innerException) : base(lid, ErrorCodeValue.UnknownUser, string.Format("User not found: {0}", userMailboxGuid), innerException)
		{
		}

		public UserNotFoundException(LID lid, string userId, Exception innerException) : base(lid, ErrorCodeValue.UnknownUser, string.Format("User not found: {0}", userId), innerException)
		{
		}

		private const string UserNotFoundTemplate = "User not found: {0}";
	}
}
