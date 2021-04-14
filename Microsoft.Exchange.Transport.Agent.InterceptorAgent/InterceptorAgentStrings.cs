using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal static class InterceptorAgentStrings
	{
		static InterceptorAgentStrings()
		{
			InterceptorAgentStrings.stringIDs.Add(658946777U, "ConditionTypeValueCannotBeNullOrEmpty");
			InterceptorAgentStrings.stringIDs.Add(2490668003U, "ConditionTypeValueInvalidTenantIdGuid");
		}

		public static LocalizedString ConditionTypeValueInvalidDirectionalityType(string values)
		{
			return new LocalizedString("ConditionTypeValueInvalidDirectionalityType", InterceptorAgentStrings.ResourceManager, new object[]
			{
				values
			});
		}

		public static LocalizedString ConditionTypeValueCannotBeNullOrEmpty
		{
			get
			{
				return new LocalizedString("ConditionTypeValueCannotBeNullOrEmpty", InterceptorAgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConditionTypeValueInvalidTenantIdGuid
		{
			get
			{
				return new LocalizedString("ConditionTypeValueInvalidTenantIdGuid", InterceptorAgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(InterceptorAgentStrings.IDs key)
		{
			return new LocalizedString(InterceptorAgentStrings.stringIDs[(uint)key], InterceptorAgentStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Agent.InterceptorAgent.InterceptorAgentStrings", typeof(InterceptorAgentStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ConditionTypeValueCannotBeNullOrEmpty = 658946777U,
			ConditionTypeValueInvalidTenantIdGuid = 2490668003U
		}

		private enum ParamIDs
		{
			ConditionTypeValueInvalidDirectionalityType
		}
	}
}
