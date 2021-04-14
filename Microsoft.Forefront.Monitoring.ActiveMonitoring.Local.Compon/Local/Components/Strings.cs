using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Local.Components
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3960730520U, "QueueFull");
		}

		public static LocalizedString QueueFull
		{
			get
			{
				return new LocalizedString("QueueFull", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestMailNotFound(string clientMsgId)
		{
			return new LocalizedString("TestMailNotFound", Strings.ResourceManager, new object[]
			{
				clientMsgId
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Forefront.Monitoring.ActiveMonitoring.Local.Components.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			QueueFull = 3960730520U
		}

		private enum ParamIDs
		{
			TestMailNotFound
		}
	}
}
