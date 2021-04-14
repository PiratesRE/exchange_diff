﻿using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[Flags]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum GroupMailboxConfigurationActionType
	{
		SetRegionalSettings = 1,
		CreateDefaultFolders = 2,
		SetInitialFolderPermissions = 4,
		SetAllFolderPermissions = 8,
		ConfigureCalendar = 16,
		SendWelcomeMessage = 32,
		GenerateGroupPhoto = 64
	}
}
