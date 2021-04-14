using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.GroupMailbox.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(676830401U, "CannotEscalateForNonMember");
			Strings.stringIDs.Add(2693957296U, "MaixmumNumberOfMailboxAssociationsForUserReached");
			Strings.stringIDs.Add(1226541993U, "CannotPinGroupForNonMember");
			Strings.stringIDs.Add(2475272174U, "MaxSubscriptionsForGroupReached");
			Strings.stringIDs.Add(1872174711U, "JoinRequestMessageNoAttachedBodyPrefix");
			Strings.stringIDs.Add(3131285730U, "JoinRequestMessageAttachedBodyPrefix");
		}

		public static LocalizedString ErrorUnableToAddExternalUser(string externalUser)
		{
			return new LocalizedString("ErrorUnableToAddExternalUser", Strings.ResourceManager, new object[]
			{
				externalUser
			});
		}

		public static LocalizedString CannotEscalateForNonMember
		{
			get
			{
				return new LocalizedString("CannotEscalateForNonMember", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRequestMessageFooterTextWithLink(string link)
		{
			return new LocalizedString("JoinRequestMessageFooterTextWithLink", Strings.ResourceManager, new object[]
			{
				link
			});
		}

		public static LocalizedString RpcReplicationCallFailed(int error)
		{
			return new LocalizedString("RpcReplicationCallFailed", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString JoinRequestMessageSubject(string user, string group)
		{
			return new LocalizedString("JoinRequestMessageSubject", Strings.ResourceManager, new object[]
			{
				user,
				group
			});
		}

		public static LocalizedString MaixmumNumberOfMailboxAssociationsForUserReached
		{
			get
			{
				return new LocalizedString("MaixmumNumberOfMailboxAssociationsForUserReached", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotPinGroupForNonMember
		{
			get
			{
				return new LocalizedString("CannotPinGroupForNonMember", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EwsUrlDiscoveryFailed(string user)
		{
			return new LocalizedString("EwsUrlDiscoveryFailed", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString MaxSubscriptionsForGroupReached
		{
			get
			{
				return new LocalizedString("MaxSubscriptionsForGroupReached", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningUnableToSendWelcomeMessage(string exception)
		{
			return new LocalizedString("WarningUnableToSendWelcomeMessage", Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString JoinRequestMessageNoAttachedBodyPrefix
		{
			get
			{
				return new LocalizedString("JoinRequestMessageNoAttachedBodyPrefix", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnableToConfigureMailbox(string folder, string groupMailbox)
		{
			return new LocalizedString("ErrorUnableToConfigureMailbox", Strings.ResourceManager, new object[]
			{
				folder,
				groupMailbox
			});
		}

		public static LocalizedString JoinRequestMessageHeading(string user, string group)
		{
			return new LocalizedString("JoinRequestMessageHeading", Strings.ResourceManager, new object[]
			{
				user,
				group
			});
		}

		public static LocalizedString JoinRequestMessageAttachedBodyPrefix
		{
			get
			{
				return new LocalizedString("JoinRequestMessageAttachedBodyPrefix", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.GroupMailbox.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			CannotEscalateForNonMember = 676830401U,
			MaixmumNumberOfMailboxAssociationsForUserReached = 2693957296U,
			CannotPinGroupForNonMember = 1226541993U,
			MaxSubscriptionsForGroupReached = 2475272174U,
			JoinRequestMessageNoAttachedBodyPrefix = 1872174711U,
			JoinRequestMessageAttachedBodyPrefix = 3131285730U
		}

		private enum ParamIDs
		{
			ErrorUnableToAddExternalUser,
			JoinRequestMessageFooterTextWithLink,
			RpcReplicationCallFailed,
			JoinRequestMessageSubject,
			EwsUrlDiscoveryFailed,
			WarningUnableToSendWelcomeMessage,
			ErrorUnableToConfigureMailbox,
			JoinRequestMessageHeading
		}
	}
}
