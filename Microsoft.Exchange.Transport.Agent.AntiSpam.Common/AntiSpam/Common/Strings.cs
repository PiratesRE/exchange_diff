using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(84245057U, "OptInNotConfigured");
			Strings.stringIDs.Add(19418672U, "OptInConfigured");
			Strings.stringIDs.Add(1349797536U, "UpdateModeDisabled");
			Strings.stringIDs.Add(1359024969U, "OptInRequestDisabled");
			Strings.stringIDs.Add(620496255U, "UpdateModeEnabled");
			Strings.stringIDs.Add(264826011U, "OptInRequestNotifyInstall");
			Strings.stringIDs.Add(1549663132U, "OptInRequestScheduled");
			Strings.stringIDs.Add(1731110810U, "UpdateModeManual");
			Strings.stringIDs.Add(4029010872U, "OptInRequestNotifyDownload");
		}

		public static LocalizedString OptInNotConfigured
		{
			get
			{
				return new LocalizedString("OptInNotConfigured", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToRegisterForConfigChangeNotification(string agentName)
		{
			return new LocalizedString("FailedToRegisterForConfigChangeNotification", "ExF907B1", false, true, Strings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString OptInConfigured
		{
			get
			{
				return new LocalizedString("OptInConfigured", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadConfiguration(string agentName)
		{
			return new LocalizedString("FailedToReadConfiguration", "Ex05145E", false, true, Strings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString UpdateModeDisabled
		{
			get
			{
				return new LocalizedString("UpdateModeDisabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OptInRequestDisabled
		{
			get
			{
				return new LocalizedString("OptInRequestDisabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateModeEnabled
		{
			get
			{
				return new LocalizedString("UpdateModeEnabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OptInRequestNotifyInstall
		{
			get
			{
				return new LocalizedString("OptInRequestNotifyInstall", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OptInRequestScheduled
		{
			get
			{
				return new LocalizedString("OptInRequestScheduled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateModeManual
		{
			get
			{
				return new LocalizedString("UpdateModeManual", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OptInRequestNotifyDownload
		{
			get
			{
				return new LocalizedString("OptInRequestNotifyDownload", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(9);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Agent.AntiSpam.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			OptInNotConfigured = 84245057U,
			OptInConfigured = 19418672U,
			UpdateModeDisabled = 1349797536U,
			OptInRequestDisabled = 1359024969U,
			UpdateModeEnabled = 620496255U,
			OptInRequestNotifyInstall = 264826011U,
			OptInRequestScheduled = 1549663132U,
			UpdateModeManual = 1731110810U,
			OptInRequestNotifyDownload = 4029010872U
		}

		private enum ParamIDs
		{
			FailedToRegisterForConfigChangeNotification,
			FailedToReadConfiguration
		}
	}
}
