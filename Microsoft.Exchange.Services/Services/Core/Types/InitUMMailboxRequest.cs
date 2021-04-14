﻿using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("InitUMMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class InitUMMailboxRequest : BaseRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new InitUMMailbox(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
