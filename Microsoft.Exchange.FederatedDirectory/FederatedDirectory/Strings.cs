using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal static class Strings
	{
		public static LocalizedString FailedToAddPendingMember(string smtpAddress, string error)
		{
			return new LocalizedString("FailedToAddPendingMember", Strings.ResourceManager, new object[]
			{
				smtpAddress,
				error
			});
		}

		public static LocalizedString FailedToRemoveMember(string smtpAddress, string error)
		{
			return new LocalizedString("FailedToRemoveMember", Strings.ResourceManager, new object[]
			{
				smtpAddress,
				error
			});
		}

		public static LocalizedString GroupMailboxFailedUpdate(string group, string error)
		{
			return new LocalizedString("GroupMailboxFailedUpdate", Strings.ResourceManager, new object[]
			{
				group,
				error
			});
		}

		public static LocalizedString FailedToAddOwner(string smtpAddress, string error)
		{
			return new LocalizedString("FailedToAddOwner", Strings.ResourceManager, new object[]
			{
				smtpAddress,
				error
			});
		}

		public static LocalizedString FailedToRemoveOwner(string smtpAddress, string error)
		{
			return new LocalizedString("FailedToRemoveOwner", Strings.ResourceManager, new object[]
			{
				smtpAddress,
				error
			});
		}

		public static LocalizedString FailedToAddMember(string smtpAddress, string error)
		{
			return new LocalizedString("FailedToAddMember", Strings.ResourceManager, new object[]
			{
				smtpAddress,
				error
			});
		}

		public static LocalizedString FailedToRemovePendingMember(string smtpAddress, string error)
		{
			return new LocalizedString("FailedToRemovePendingMember", Strings.ResourceManager, new object[]
			{
				smtpAddress,
				error
			});
		}

		public static LocalizedString GroupMailboxFailedCreate(string group, string error)
		{
			return new LocalizedString("GroupMailboxFailedCreate", Strings.ResourceManager, new object[]
			{
				group,
				error
			});
		}

		public static LocalizedString PartiallyFailedToUpdateGroup(string group)
		{
			return new LocalizedString("PartiallyFailedToUpdateGroup", Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString GroupMailboxFailedDelete(string group, string error)
		{
			return new LocalizedString("GroupMailboxFailedDelete", Strings.ResourceManager, new object[]
			{
				group,
				error
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.FederatedDirectory.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			FailedToAddPendingMember,
			FailedToRemoveMember,
			GroupMailboxFailedUpdate,
			FailedToAddOwner,
			FailedToRemoveOwner,
			FailedToAddMember,
			FailedToRemovePendingMember,
			GroupMailboxFailedCreate,
			PartiallyFailedToUpdateGroup,
			GroupMailboxFailedDelete
		}
	}
}
