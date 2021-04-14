using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel.EnumTypes
{
	public static class OwaOptionEnumStrings
	{
		static OwaOptionEnumStrings()
		{
			OwaOptionEnumStrings.stringIDs.Add(996355914U, "Quarantined");
			OwaOptionEnumStrings.stringIDs.Add(3601314308U, "UserAgentsChanges");
			OwaOptionEnumStrings.stringIDs.Add(1975373491U, "SyncCommands");
			OwaOptionEnumStrings.stringIDs.Add(3727590521U, "RoleEditor");
			OwaOptionEnumStrings.stringIDs.Add(3728408622U, "DeviceWipePending");
			OwaOptionEnumStrings.stringIDs.Add(3532057202U, "RolePublishingEditor");
			OwaOptionEnumStrings.stringIDs.Add(141120823U, "RecentCommands");
			OwaOptionEnumStrings.stringIDs.Add(3608358242U, "Upgrade");
			OwaOptionEnumStrings.stringIDs.Add(1068346025U, "OutOfBudgets");
			OwaOptionEnumStrings.stringIDs.Add(3159442548U, "DeviceBlocked");
			OwaOptionEnumStrings.stringIDs.Add(2309238384U, "RolePublishingAuthor");
			OwaOptionEnumStrings.stringIDs.Add(3266435989U, "Individual");
			OwaOptionEnumStrings.stringIDs.Add(1636409600U, "RoleNonEditingAuthor");
			OwaOptionEnumStrings.stringIDs.Add(816661212U, "Policy");
			OwaOptionEnumStrings.stringIDs.Add(1140300925U, "RoleReviewer");
			OwaOptionEnumStrings.stringIDs.Add(3485911895U, "StatusUnsuccessFul");
			OwaOptionEnumStrings.stringIDs.Add(403740404U, "NotApplied");
			OwaOptionEnumStrings.stringIDs.Add(3388973407U, "Organization");
			OwaOptionEnumStrings.stringIDs.Add(2979126483U, "CommandFrequency");
			OwaOptionEnumStrings.stringIDs.Add(2589734627U, "RoleContributor");
			OwaOptionEnumStrings.stringIDs.Add(3937861705U, "RoleAvailabilityOnly");
			OwaOptionEnumStrings.stringIDs.Add(3531789014U, "DeviceRule");
			OwaOptionEnumStrings.stringIDs.Add(1981651471U, "StatusPending");
			OwaOptionEnumStrings.stringIDs.Add(3892568161U, "RoleAuthor");
			OwaOptionEnumStrings.stringIDs.Add(1414246128U, "None");
			OwaOptionEnumStrings.stringIDs.Add(14056478U, "StatusTransferred");
			OwaOptionEnumStrings.stringIDs.Add(102260678U, "EnableNotificationEmail");
			OwaOptionEnumStrings.stringIDs.Add(1010456570U, "DeviceDiscovery");
			OwaOptionEnumStrings.stringIDs.Add(3010978409U, "AppliedInFull");
			OwaOptionEnumStrings.stringIDs.Add(3380639415U, "Watsons");
			OwaOptionEnumStrings.stringIDs.Add(3816006969U, "RoleOwner");
			OwaOptionEnumStrings.stringIDs.Add(3617746388U, "DeviceWipeSucceeded");
			OwaOptionEnumStrings.stringIDs.Add(1656602441U, "ExternallyManaged");
			OwaOptionEnumStrings.stringIDs.Add(3060667906U, "RoleLimitedDetails");
			OwaOptionEnumStrings.stringIDs.Add(3231667300U, "StatusRead");
			OwaOptionEnumStrings.stringIDs.Add(2846264340U, "Unknown");
			OwaOptionEnumStrings.stringIDs.Add(1448003977U, "User");
			OwaOptionEnumStrings.stringIDs.Add(1875295180U, "StatusDelivered");
			OwaOptionEnumStrings.stringIDs.Add(3811183882U, "Allowed");
			OwaOptionEnumStrings.stringIDs.Add(278718718U, "DeviceOk");
			OwaOptionEnumStrings.stringIDs.Add(3184119847U, "PartiallyApplied");
			OwaOptionEnumStrings.stringIDs.Add(4019774802U, "Blocked");
			OwaOptionEnumStrings.stringIDs.Add(3905558735U, "Global");
			OwaOptionEnumStrings.stringIDs.Add(3566263623U, "Default");
		}

		public static LocalizedString Quarantined
		{
			get
			{
				return new LocalizedString("Quarantined", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserAgentsChanges
		{
			get
			{
				return new LocalizedString("UserAgentsChanges", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncCommands
		{
			get
			{
				return new LocalizedString("SyncCommands", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleEditor
		{
			get
			{
				return new LocalizedString("RoleEditor", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceWipePending
		{
			get
			{
				return new LocalizedString("DeviceWipePending", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RolePublishingEditor
		{
			get
			{
				return new LocalizedString("RolePublishingEditor", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecentCommands
		{
			get
			{
				return new LocalizedString("RecentCommands", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Upgrade
		{
			get
			{
				return new LocalizedString("Upgrade", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutOfBudgets
		{
			get
			{
				return new LocalizedString("OutOfBudgets", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceBlocked
		{
			get
			{
				return new LocalizedString("DeviceBlocked", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RolePublishingAuthor
		{
			get
			{
				return new LocalizedString("RolePublishingAuthor", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Individual
		{
			get
			{
				return new LocalizedString("Individual", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleNonEditingAuthor
		{
			get
			{
				return new LocalizedString("RoleNonEditingAuthor", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Policy
		{
			get
			{
				return new LocalizedString("Policy", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleReviewer
		{
			get
			{
				return new LocalizedString("RoleReviewer", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusUnsuccessFul
		{
			get
			{
				return new LocalizedString("StatusUnsuccessFul", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotApplied
		{
			get
			{
				return new LocalizedString("NotApplied", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Organization
		{
			get
			{
				return new LocalizedString("Organization", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommandFrequency
		{
			get
			{
				return new LocalizedString("CommandFrequency", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleContributor
		{
			get
			{
				return new LocalizedString("RoleContributor", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleAvailabilityOnly
		{
			get
			{
				return new LocalizedString("RoleAvailabilityOnly", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceRule
		{
			get
			{
				return new LocalizedString("DeviceRule", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusPending
		{
			get
			{
				return new LocalizedString("StatusPending", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleAuthor
		{
			get
			{
				return new LocalizedString("RoleAuthor", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString None
		{
			get
			{
				return new LocalizedString("None", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusTransferred
		{
			get
			{
				return new LocalizedString("StatusTransferred", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnableNotificationEmail
		{
			get
			{
				return new LocalizedString("EnableNotificationEmail", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceDiscovery
		{
			get
			{
				return new LocalizedString("DeviceDiscovery", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppliedInFull
		{
			get
			{
				return new LocalizedString("AppliedInFull", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Watsons
		{
			get
			{
				return new LocalizedString("Watsons", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleOwner
		{
			get
			{
				return new LocalizedString("RoleOwner", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceWipeSucceeded
		{
			get
			{
				return new LocalizedString("DeviceWipeSucceeded", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternallyManaged
		{
			get
			{
				return new LocalizedString("ExternallyManaged", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleLimitedDetails
		{
			get
			{
				return new LocalizedString("RoleLimitedDetails", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusRead
		{
			get
			{
				return new LocalizedString("StatusRead", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unknown
		{
			get
			{
				return new LocalizedString("Unknown", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString User
		{
			get
			{
				return new LocalizedString("User", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusDelivered
		{
			get
			{
				return new LocalizedString("StatusDelivered", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Allowed
		{
			get
			{
				return new LocalizedString("Allowed", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceOk
		{
			get
			{
				return new LocalizedString("DeviceOk", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PartiallyApplied
		{
			get
			{
				return new LocalizedString("PartiallyApplied", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Blocked
		{
			get
			{
				return new LocalizedString("Blocked", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Global
		{
			get
			{
				return new LocalizedString("Global", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Default
		{
			get
			{
				return new LocalizedString("Default", OwaOptionEnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(OwaOptionEnumStrings.IDs key)
		{
			return new LocalizedString(OwaOptionEnumStrings.stringIDs[(uint)key], OwaOptionEnumStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(44);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.OwaOptionEnumStrings", typeof(OwaOptionEnumStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			Quarantined = 996355914U,
			UserAgentsChanges = 3601314308U,
			SyncCommands = 1975373491U,
			RoleEditor = 3727590521U,
			DeviceWipePending = 3728408622U,
			RolePublishingEditor = 3532057202U,
			RecentCommands = 141120823U,
			Upgrade = 3608358242U,
			OutOfBudgets = 1068346025U,
			DeviceBlocked = 3159442548U,
			RolePublishingAuthor = 2309238384U,
			Individual = 3266435989U,
			RoleNonEditingAuthor = 1636409600U,
			Policy = 816661212U,
			RoleReviewer = 1140300925U,
			StatusUnsuccessFul = 3485911895U,
			NotApplied = 403740404U,
			Organization = 3388973407U,
			CommandFrequency = 2979126483U,
			RoleContributor = 2589734627U,
			RoleAvailabilityOnly = 3937861705U,
			DeviceRule = 3531789014U,
			StatusPending = 1981651471U,
			RoleAuthor = 3892568161U,
			None = 1414246128U,
			StatusTransferred = 14056478U,
			EnableNotificationEmail = 102260678U,
			DeviceDiscovery = 1010456570U,
			AppliedInFull = 3010978409U,
			Watsons = 3380639415U,
			RoleOwner = 3816006969U,
			DeviceWipeSucceeded = 3617746388U,
			ExternallyManaged = 1656602441U,
			RoleLimitedDetails = 3060667906U,
			StatusRead = 3231667300U,
			Unknown = 2846264340U,
			User = 1448003977U,
			StatusDelivered = 1875295180U,
			Allowed = 3811183882U,
			DeviceOk = 278718718U,
			PartiallyApplied = 3184119847U,
			Blocked = 4019774802U,
			Global = 3905558735U,
			Default = 3566263623U
		}
	}
}
