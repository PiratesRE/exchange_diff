using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1769372998U, "OperationAborted");
		}

		public static LocalizedString MinimizedProperty(string propertyName)
		{
			return new LocalizedString("MinimizedProperty", Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString OperationAborted
		{
			get
			{
				return new LocalizedString("OperationAborted", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationTimedOut(string timeout)
		{
			return new LocalizedString("OperationTimedOut", Strings.ResourceManager, new object[]
			{
				timeout
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.HA.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			OperationAborted = 1769372998U
		}

		private enum ParamIDs
		{
			MinimizedProperty,
			OperationTimedOut
		}
	}
}
