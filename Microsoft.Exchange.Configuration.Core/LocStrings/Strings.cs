using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Core.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3249200815U, "MissingPartitionId");
			Strings.stringIDs.Add(2408588661U, "InvalidFlighting");
			Strings.stringIDs.Add(1505454489U, "MissingDelegatedPrincipal");
			Strings.stringIDs.Add(2738351686U, "MissingVersion");
			Strings.stringIDs.Add(945651915U, "MissingUserSid");
			Strings.stringIDs.Add(2311412808U, "MissingWindowsLiveId");
			Strings.stringIDs.Add(3093790777U, "MissingAppPasswordUsed");
			Strings.stringIDs.Add(3582203768U, "MissingManagedOrganization");
			Strings.stringIDs.Add(1519135930U, "MissingUserName");
			Strings.stringIDs.Add(1121806018U, "MissingAuthenticationType");
			Strings.stringIDs.Add(481556215U, "MissingOrganization");
		}

		public static LocalizedString FailedToReceiveWinRMData(string identity)
		{
			return new LocalizedString("FailedToReceiveWinRMData", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InvalidPartitionId(string value)
		{
			return new LocalizedString("InvalidPartitionId", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidDelegatedPrincipal(string value)
		{
			return new LocalizedString("InvalidDelegatedPrincipal", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString WinRMDataKeyNotFound(string identity, string key)
		{
			return new LocalizedString("WinRMDataKeyNotFound", Strings.ResourceManager, new object[]
			{
				identity,
				key
			});
		}

		public static LocalizedString MissingPartitionId
		{
			get
			{
				return new LocalizedString("MissingPartitionId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFlighting
		{
			get
			{
				return new LocalizedString("InvalidFlighting", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingDelegatedPrincipal
		{
			get
			{
				return new LocalizedString("MissingDelegatedPrincipal", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingVersion
		{
			get
			{
				return new LocalizedString("MissingVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAuthenticationType(string value)
		{
			return new LocalizedString("InvalidAuthenticationType", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MissingUserSid
		{
			get
			{
				return new LocalizedString("MissingUserSid", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingWindowsLiveId
		{
			get
			{
				return new LocalizedString("MissingWindowsLiveId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingAppPasswordUsed
		{
			get
			{
				return new LocalizedString("MissingAppPasswordUsed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingManagedOrganization
		{
			get
			{
				return new LocalizedString("MissingManagedOrganization", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOrganization(string value)
		{
			return new LocalizedString("InvalidOrganization", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MissingUserName
		{
			get
			{
				return new LocalizedString("MissingUserName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserTokenException(string reason)
		{
			return new LocalizedString("UserTokenException", Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString MissingAuthenticationType
		{
			get
			{
				return new LocalizedString("MissingAuthenticationType", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingOrganization
		{
			get
			{
				return new LocalizedString("MissingOrganization", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUserSid(string value)
		{
			return new LocalizedString("InvalidUserSid", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString IllegalItemValue(string value)
		{
			return new LocalizedString("IllegalItemValue", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(11);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Configuration.Core.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MissingPartitionId = 3249200815U,
			InvalidFlighting = 2408588661U,
			MissingDelegatedPrincipal = 1505454489U,
			MissingVersion = 2738351686U,
			MissingUserSid = 945651915U,
			MissingWindowsLiveId = 2311412808U,
			MissingAppPasswordUsed = 3093790777U,
			MissingManagedOrganization = 3582203768U,
			MissingUserName = 1519135930U,
			MissingAuthenticationType = 1121806018U,
			MissingOrganization = 481556215U
		}

		private enum ParamIDs
		{
			FailedToReceiveWinRMData,
			InvalidPartitionId,
			InvalidDelegatedPrincipal,
			WinRMDataKeyNotFound,
			InvalidAuthenticationType,
			InvalidOrganization,
			UserTokenException,
			InvalidUserSid,
			IllegalItemValue
		}
	}
}
