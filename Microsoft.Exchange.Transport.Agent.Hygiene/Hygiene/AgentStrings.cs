using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.Hygiene
{
	public static class AgentStrings
	{
		static AgentStrings()
		{
			AgentStrings.stringIDs.Add(1295055262U, "WinlenShrinkFactorRange");
			AgentStrings.stringIDs.Add(4279554728U, "InvalidProxyChain");
			AgentStrings.stringIDs.Add(4193024477U, "BlockSenderRemoteOP");
			AgentStrings.stringIDs.Add(1988599787U, "WinlenExpandFactorRange");
			AgentStrings.stringIDs.Add(708167962U, "InitWinLenRange");
			AgentStrings.stringIDs.Add(893210808U, "MinWinLenRange");
			AgentStrings.stringIDs.Add(3745363330U, "InvalidArgument");
			AgentStrings.stringIDs.Add(1999162800U, "BlockSenderLocalSRL");
			AgentStrings.stringIDs.Add(3696302047U, "WritingDisallowedOnClosedConnection");
			AgentStrings.stringIDs.Add(553826582U, "BlockSenderLocalOP");
			AgentStrings.stringIDs.Add(3589472089U, "InvalidProxyType");
			AgentStrings.stringIDs.Add(672016929U, "InvalidOpenProxyType");
			AgentStrings.stringIDs.Add(4259117195U, "BlockSenderRemoteSRL");
			AgentStrings.stringIDs.Add(2034608252U, "FailedToFindInsertionPoint");
			AgentStrings.stringIDs.Add(1497417765U, "GoodBehaviorPeriodRange");
			AgentStrings.stringIDs.Add(589634914U, "MaxWinLenRange");
		}

		public static LocalizedString WinlenShrinkFactorRange
		{
			get
			{
				return new LocalizedString("WinlenShrinkFactorRange", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidProxyChain
		{
			get
			{
				return new LocalizedString("InvalidProxyChain", "Ex3D991D", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockSenderRemoteOP
		{
			get
			{
				return new LocalizedString("BlockSenderRemoteOP", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WinlenExpandFactorRange
		{
			get
			{
				return new LocalizedString("WinlenExpandFactorRange", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InitWinLenRange
		{
			get
			{
				return new LocalizedString("InitWinLenRange", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidScl(int s)
		{
			return new LocalizedString("InvalidScl", "ExA11912", false, true, AgentStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidLogLength(int length)
		{
			return new LocalizedString("InvalidLogLength", "Ex7D9896", false, true, AgentStrings.ResourceManager, new object[]
			{
				length
			});
		}

		public static LocalizedString MinWinLenRange
		{
			get
			{
				return new LocalizedString("MinWinLenRange", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidArgument
		{
			get
			{
				return new LocalizedString("InvalidArgument", "Ex01E420", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockSenderLocalSRL
		{
			get
			{
				return new LocalizedString("BlockSenderLocalSRL", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WritingDisallowedOnClosedConnection
		{
			get
			{
				return new LocalizedString("WritingDisallowedOnClosedConnection", "Ex91C974", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockSenderLocalOP
		{
			get
			{
				return new LocalizedString("BlockSenderLocalOP", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidInsertValue(int val)
		{
			return new LocalizedString("InvalidInsertValue", "ExC1E84F", false, true, AgentStrings.ResourceManager, new object[]
			{
				val
			});
		}

		public static LocalizedString InvalidProxyType
		{
			get
			{
				return new LocalizedString("InvalidProxyType", "Ex328029", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOpenProxyType
		{
			get
			{
				return new LocalizedString("InvalidOpenProxyType", "ExBC6118", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockSenderRemoteSRL
		{
			get
			{
				return new LocalizedString("BlockSenderRemoteSRL", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSclLevel(int scl)
		{
			return new LocalizedString("InvalidSclLevel", "ExFAEE6D", false, true, AgentStrings.ResourceManager, new object[]
			{
				scl
			});
		}

		public static LocalizedString FailedToFindInsertionPoint
		{
			get
			{
				return new LocalizedString("FailedToFindInsertionPoint", "Ex1B0A54", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GoodBehaviorPeriodRange
		{
			get
			{
				return new LocalizedString("GoodBehaviorPeriodRange", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxWinLenRange
		{
			get
			{
				return new LocalizedString("MaxWinLenRange", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFeatureThreshold(int srl)
		{
			return new LocalizedString("InvalidFeatureThreshold", "ExA7B6B6", false, true, AgentStrings.ResourceManager, new object[]
			{
				srl
			});
		}

		public static LocalizedString GetLocalizedString(AgentStrings.IDs key)
		{
			return new LocalizedString(AgentStrings.stringIDs[(uint)key], AgentStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(16);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Agent.Hygiene.AgentStrings", typeof(AgentStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			WinlenShrinkFactorRange = 1295055262U,
			InvalidProxyChain = 4279554728U,
			BlockSenderRemoteOP = 4193024477U,
			WinlenExpandFactorRange = 1988599787U,
			InitWinLenRange = 708167962U,
			MinWinLenRange = 893210808U,
			InvalidArgument = 3745363330U,
			BlockSenderLocalSRL = 1999162800U,
			WritingDisallowedOnClosedConnection = 3696302047U,
			BlockSenderLocalOP = 553826582U,
			InvalidProxyType = 3589472089U,
			InvalidOpenProxyType = 672016929U,
			BlockSenderRemoteSRL = 4259117195U,
			FailedToFindInsertionPoint = 2034608252U,
			GoodBehaviorPeriodRange = 1497417765U,
			MaxWinLenRange = 589634914U
		}

		private enum ParamIDs
		{
			InvalidScl,
			InvalidLogLength,
			InvalidInsertValue,
			InvalidSclLevel,
			InvalidFeatureThreshold
		}
	}
}
