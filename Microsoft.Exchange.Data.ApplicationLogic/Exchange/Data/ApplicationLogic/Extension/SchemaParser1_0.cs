using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class SchemaParser1_0 : SchemaParser
	{
		public SchemaParser1_0(SafeXmlDocument xmlDoc, ExtensionInstallScope extensionInstallScope) : base(xmlDoc, extensionInstallScope)
		{
		}

		public override Version SchemaVersion
		{
			get
			{
				return SchemaConstants.SchemaVersion1_0;
			}
		}

		public override Version GetMinApiVersion()
		{
			return SchemaConstants.Exchange2013RtmApiVersion;
		}

		protected override string GetOweNamespacePrefix()
		{
			return "owe1_0";
		}

		protected override string GetOweNamespaceUri()
		{
			return "http://schemas.microsoft.com/office/appforoffice/1.0";
		}

		protected override XmlNode GetFormSettingsParentNode(FormSettings.FormSettingsType formSettingsType)
		{
			if (formSettingsType == FormSettings.FormSettingsType.ItemRead)
			{
				return this.xmlDoc.ChildNodes[0];
			}
			return null;
		}
	}
}
