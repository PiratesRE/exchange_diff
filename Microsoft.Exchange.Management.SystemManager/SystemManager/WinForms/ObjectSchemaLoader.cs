using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ObjectSchemaLoader
	{
		static ObjectSchemaLoader()
		{
			ObjectSchemaLoader.unHandledTypeList["System.Net.IPAddress"] = typeof(IPAddress);
			ObjectSchemaLoader.unHandledTypeList["Microsoft.Exchange.Data.Common.LocalizedString, Microsoft.Exchange.Data.Common"] = typeof(LocalizedString);
			ObjectSchemaLoader.unHandledTypeList["Microsoft.Exchange.Data.Common.LocalizedString"] = typeof(LocalizedString);
			ExpressionParser.EnrolPredefinedTypes(typeof(CertificateStatus));
		}

		public ObjectSchemaLoader(DataDrivenCategory dataDrivenCategory, string xPath, string schema)
		{
			Stream manifestResource = WinformsHelper.GetManifestResource(dataDrivenCategory);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.Load(manifestResource);
			foreach (object obj in safeXmlDocument.SelectNodes(xPath))
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNode namedItem = xmlNode.Attributes.GetNamedItem("Name");
				if (namedItem.Value.Equals(schema))
				{
					this.objectDefinition = xmlNode;
					break;
				}
			}
		}

		protected XmlNode ObjectDefinition
		{
			get
			{
				return this.objectDefinition;
			}
		}

		public static Type GetTypeByString(string name)
		{
			Type type = Type.GetType(name);
			if (null == type && ObjectSchemaLoader.unHandledTypeList.ContainsKey(name))
			{
				type = ObjectSchemaLoader.unHandledTypeList[name];
			}
			return type;
		}

		public static object GetStaticField(string typeName, string fieldName)
		{
			FieldInfo field = Type.GetType(typeName).GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (!(field != null))
			{
				return null;
			}
			return field.GetValue(null);
		}

		private static Dictionary<string, Type> unHandledTypeList = new Dictionary<string, Type>();

		private XmlNode objectDefinition;
	}
}
