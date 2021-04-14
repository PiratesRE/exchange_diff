﻿using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum PermissionLevelType
	{
		None,
		Owner,
		PublishingEditor,
		Editor,
		PublishingAuthor,
		Author,
		NoneditingAuthor,
		Reviewer,
		Contributor,
		Custom
	}
}
