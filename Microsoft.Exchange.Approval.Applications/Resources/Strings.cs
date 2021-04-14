using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Approval.Applications.Resources
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2918013799U, "Semicolon");
			Strings.stringIDs.Add(816183154U, "AutoGroupRequestApprovedBody");
		}

		public static LocalizedString AutoGroupRequestFailedSubject(string group)
		{
			return new LocalizedString("AutoGroupRequestFailedSubject", "Ex5FA695", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString AutoGroupRequestFailedBodyBadRequester(string group, string requester)
		{
			return new LocalizedString("AutoGroupRequestFailedBodyBadRequester", "ExF5FD46", false, true, Strings.ResourceManager, new object[]
			{
				group,
				requester
			});
		}

		public static LocalizedString Semicolon
		{
			get
			{
				return new LocalizedString("Semicolon", "ExC51658", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoGroupRequestExpiredSubject(string group)
		{
			return new LocalizedString("AutoGroupRequestExpiredSubject", "ExFF6103", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString AutoGroupRequestFailedBodyTaskError(string error)
		{
			return new LocalizedString("AutoGroupRequestFailedBodyTaskError", "Ex3FE882", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString AutoGroupRequestRejectedSubject(string group)
		{
			return new LocalizedString("AutoGroupRequestRejectedSubject", "Ex8AA8DB", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString AutoGroupRequestRejectedBody(string owners)
		{
			return new LocalizedString("AutoGroupRequestRejectedBody", "Ex377538", false, true, Strings.ResourceManager, new object[]
			{
				owners
			});
		}

		public static LocalizedString AutoGroupRequestFailedHeader(string group)
		{
			return new LocalizedString("AutoGroupRequestFailedHeader", "Ex0B9BBB", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString AutoGroupRequestApprovedHeader(string approver, string group)
		{
			return new LocalizedString("AutoGroupRequestApprovedHeader", "Ex6D1B7D", false, true, Strings.ResourceManager, new object[]
			{
				approver,
				group
			});
		}

		public static LocalizedString AutoGroupRequestApprovedBody
		{
			get
			{
				return new LocalizedString("AutoGroupRequestApprovedBody", "Ex876BEB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoGroupRequestApprovedSubject(string group)
		{
			return new LocalizedString("AutoGroupRequestApprovedSubject", "ExA17329", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString ErrorTaskInvocationFailed(string user)
		{
			return new LocalizedString("ErrorTaskInvocationFailed", "ExFBD273", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString AutoGroupRequestRejectedHeader(string group)
		{
			return new LocalizedString("AutoGroupRequestRejectedHeader", "Ex4F0764", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString ErrorNoRBACRoleAssignment(string user)
		{
			return new LocalizedString("ErrorNoRBACRoleAssignment", "ExC8B4DD", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString AutoGroupRequestFailedBodyBadApprover(string group, string requester, string approver)
		{
			return new LocalizedString("AutoGroupRequestFailedBodyBadApprover", "ExC11FBB", false, true, Strings.ResourceManager, new object[]
			{
				group,
				requester,
				approver
			});
		}

		public static LocalizedString ErrorUserNotFound(string proxy)
		{
			return new LocalizedString("ErrorUserNotFound", "ExDDCC17", false, true, Strings.ResourceManager, new object[]
			{
				proxy
			});
		}

		public static LocalizedString AutoGroupRequestExpiredBody(string group)
		{
			return new LocalizedString("AutoGroupRequestExpiredBody", "Ex4CD025", false, true, Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Approval.Applications.Resources.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			Semicolon = 2918013799U,
			AutoGroupRequestApprovedBody = 816183154U
		}

		private enum ParamIDs
		{
			AutoGroupRequestFailedSubject,
			AutoGroupRequestFailedBodyBadRequester,
			AutoGroupRequestExpiredSubject,
			AutoGroupRequestFailedBodyTaskError,
			AutoGroupRequestRejectedSubject,
			AutoGroupRequestRejectedBody,
			AutoGroupRequestFailedHeader,
			AutoGroupRequestApprovedHeader,
			AutoGroupRequestApprovedSubject,
			ErrorTaskInvocationFailed,
			AutoGroupRequestRejectedHeader,
			ErrorNoRBACRoleAssignment,
			AutoGroupRequestFailedBodyBadApprover,
			ErrorUserNotFound,
			AutoGroupRequestExpiredBody
		}
	}
}
