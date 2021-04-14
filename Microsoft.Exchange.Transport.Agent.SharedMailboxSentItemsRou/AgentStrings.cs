using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal static class AgentStrings
	{
		static AgentStrings()
		{
			AgentStrings.stringIDs.Add(2339319564U, "WrapperMessageBody");
		}

		public static LocalizedString WrapperMessageSubjectFormat(string originalSubject)
		{
			return new LocalizedString("WrapperMessageSubjectFormat", AgentStrings.ResourceManager, new object[]
			{
				originalSubject
			});
		}

		public static LocalizedString WrapperMessageBody
		{
			get
			{
				return new LocalizedString("WrapperMessageBody", AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(AgentStrings.IDs key)
		{
			return new LocalizedString(AgentStrings.stringIDs[(uint)key], AgentStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent.AgentStrings", typeof(AgentStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			WrapperMessageBody = 2339319564U
		}

		private enum ParamIDs
		{
			WrapperMessageSubjectFormat
		}
	}
}
