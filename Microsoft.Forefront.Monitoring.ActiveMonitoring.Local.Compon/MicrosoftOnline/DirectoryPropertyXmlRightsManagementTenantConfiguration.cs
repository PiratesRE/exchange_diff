﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfigurationSingle))]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class DirectoryPropertyXmlRightsManagementTenantConfiguration : DirectoryPropertyXml
	{
		[XmlElement("Value")]
		public XmlValueRightsManagementTenantConfiguration[] Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private XmlValueRightsManagementTenantConfiguration[] valueField;
	}
}
