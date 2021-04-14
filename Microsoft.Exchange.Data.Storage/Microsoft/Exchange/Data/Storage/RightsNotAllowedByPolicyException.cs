using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class RightsNotAllowedByPolicyException : StoragePermanentException
	{
		internal RightsNotAllowedByPolicyException(RightsNotAllowedRecipient[] rightsNotAllowedRecipients, StoreObjectType storeObjectType, string folderName) : base(ServerStrings.RightsNotAllowedByPolicy(storeObjectType.ToString(), folderName))
		{
			if (rightsNotAllowedRecipients == null)
			{
				throw new ArgumentNullException("rightsNotAllowedRecipients");
			}
			if (rightsNotAllowedRecipients.Length == 0)
			{
				throw new ArgumentException("rightsNotAllowedRecipients");
			}
			this.rightsNotAllowedRecipients = rightsNotAllowedRecipients;
		}

		public RightsNotAllowedRecipient[] RightsNotAllowedRecipients
		{
			get
			{
				return this.rightsNotAllowedRecipients;
			}
		}

		private RightsNotAllowedRecipient[] rightsNotAllowedRecipients;
	}
}
