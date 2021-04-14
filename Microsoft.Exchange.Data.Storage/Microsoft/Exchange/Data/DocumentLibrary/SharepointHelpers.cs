using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharepointHelpers
	{
		private SharepointHelpers()
		{
		}

		static SharepointHelpers()
		{
			SharepointHelpers.SharepointNamespaceManager = new XmlNamespaceManager(new NameTable());
			SharepointHelpers.SharepointNamespaceManager.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/soap/");
			SharepointHelpers.SharepointNamespaceManager.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
			SharepointHelpers.SharepointNamespaceManager.AddNamespace("z", "#RowsetSchema");
			XmlDocument xmlDocument = new SafeXmlDocument();
			SharepointHelpers.DefaultQueryOptions = xmlDocument.CreateElement("QueryOptions");
			XmlNode xmlNode = xmlDocument.CreateElement("DateInUtc");
			xmlNode.InnerText = "TRUE";
			SharepointHelpers.DefaultQueryOptions.AppendChild(xmlNode);
			xmlNode = xmlDocument.CreateElement("IncludeMandatoryColumns");
			xmlNode.InnerText = "TRUE";
			SharepointHelpers.DefaultQueryOptions.AppendChild(xmlNode);
		}

		internal static object[] GetValuesFromCAMLView(Schema schema, XmlNode xmlNode, CultureInfo cultureInfo, params PropertyDefinition[] propertyDefinitions)
		{
			return SharepointHelpers.GetValuesFromCAMLView(schema, xmlNode, cultureInfo, (IList<PropertyDefinition>)propertyDefinitions);
		}

		internal static object[] GetValuesFromCAMLView(Schema schema, XmlNode xmlNode, CultureInfo cultureInfo, IList<PropertyDefinition> propertyDefinitions)
		{
			object[] array = new object[propertyDefinitions.Count];
			string str = string.Empty;
			if (xmlNode.LocalName == "row")
			{
				str = "ows_";
			}
			int i = 0;
			while (i < propertyDefinitions.Count)
			{
				SharepointPropertyDefinition sharepointPropertyDefinition = SharepointPropertyDefinition.PropertyDefinitionToSharepointPropertyDefinition(schema, propertyDefinitions[i]);
				if (sharepointPropertyDefinition != null)
				{
					string name = str + sharepointPropertyDefinition.SharepointName;
					try
					{
						if (xmlNode.Attributes[name] == null || (array[i] = sharepointPropertyDefinition.FromSharepoint(xmlNode.Attributes[name].Value, cultureInfo)) == null)
						{
							array[i] = new PropertyError(propertyDefinitions[i], PropertyErrorCode.NotFound);
						}
						goto IL_DC;
					}
					catch (FormatException)
					{
						array[i] = new PropertyError(propertyDefinitions[i], PropertyErrorCode.CorruptData);
						goto IL_DC;
					}
					goto IL_AC;
				}
				goto IL_AC;
				IL_DC:
				i++;
				continue;
				IL_AC:
				if (propertyDefinitions[i] is DocumentLibraryPropertyDefinition)
				{
					array[i] = new PropertyError(propertyDefinitions[i], PropertyErrorCode.NotFound);
					goto IL_DC;
				}
				array[i] = new PropertyError(propertyDefinitions[i], PropertyErrorCode.NotSupported);
				goto IL_DC;
			}
			return array;
		}

		internal static XmlNode GenerateViewFieldCAML(Schema schema, ICollection<PropertyDefinition> propertyDefinitions)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("ViewFields");
			foreach (SharepointPropertyDefinition sharepointPropertyDefinition in SharepointPropertyDefinition.PropertyDefinitionsToSharepointPropertyDefinitions(schema, propertyDefinitions))
			{
				if (sharepointPropertyDefinition != null)
				{
					XmlNode xmlNode2 = xmlDocument.CreateElement("FieldRef");
					xmlNode2.Attributes.Append(xmlDocument.CreateAttribute("Name"));
					xmlNode2.Attributes["Name"].Value = sharepointPropertyDefinition.SharepointName;
					xmlNode.AppendChild(xmlNode2);
				}
			}
			return xmlNode;
		}

		private static XmlNode GenerateQueryCAMLHelper(QueryFilter query, XmlDocument xmlDoc, int depth)
		{
			if (depth >= SharepointHelpers.MaxFilterHierarchyDepth)
			{
				throw new ArgumentException("filter");
			}
			XmlNode xmlNode = null;
			if (query is AndFilter)
			{
				AndFilter andFilter = (AndFilter)query;
				using (ReadOnlyCollection<QueryFilter>.Enumerator enumerator = andFilter.Filters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						QueryFilter query2 = enumerator.Current;
						if (xmlNode == null)
						{
							xmlNode = SharepointHelpers.GenerateQueryCAMLHelper(query2, xmlDoc, depth + 1);
						}
						else
						{
							XmlNode newChild = xmlNode;
							xmlNode = xmlDoc.CreateElement("And");
							xmlNode.AppendChild(newChild);
							xmlNode.AppendChild(SharepointHelpers.GenerateQueryCAMLHelper(query2, xmlDoc, depth + 1));
						}
					}
					return xmlNode;
				}
			}
			if (query is OrFilter)
			{
				OrFilter orFilter = (OrFilter)query;
				using (ReadOnlyCollection<QueryFilter>.Enumerator enumerator2 = orFilter.Filters.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						QueryFilter query3 = enumerator2.Current;
						if (xmlNode == null)
						{
							xmlNode = SharepointHelpers.GenerateQueryCAMLHelper(query3, xmlDoc, depth + 1);
						}
						else
						{
							XmlNode newChild2 = xmlNode;
							xmlNode = xmlDoc.CreateElement("Or");
							xmlNode.AppendChild(newChild2);
							xmlNode.AppendChild(SharepointHelpers.GenerateQueryCAMLHelper(query3, xmlDoc, depth + 1));
						}
					}
					return xmlNode;
				}
			}
			if (!(query is ComparisonFilter))
			{
				throw new NotSupportedException(Strings.ExFilterNotSupported(query.GetType()));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)query;
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				xmlNode = xmlDoc.CreateElement("Eq");
				break;
			case ComparisonOperator.NotEqual:
				xmlNode = xmlDoc.CreateElement("Neq");
				break;
			case ComparisonOperator.LessThan:
				xmlNode = xmlDoc.CreateElement("Lt");
				break;
			case ComparisonOperator.LessThanOrEqual:
				xmlNode = xmlDoc.CreateElement("Leq");
				break;
			case ComparisonOperator.GreaterThan:
				xmlNode = xmlDoc.CreateElement("Gt");
				break;
			case ComparisonOperator.GreaterThanOrEqual:
				xmlNode = xmlDoc.CreateElement("Geq");
				break;
			default:
				throw new InvalidOperationException("Invalid comparison operator");
			}
			SharepointPropertyDefinition sharepointPropertyDefinition = (SharepointPropertyDefinition)comparisonFilter.Property;
			xmlNode.InnerXml = string.Format("<FieldRef Name=\"{0}\" /><Value Type=\"{1}\">{2}</Value>", sharepointPropertyDefinition.SharepointName, sharepointPropertyDefinition.FieldTypeAsString, sharepointPropertyDefinition.ToSharepoint(comparisonFilter.PropertyValue, null));
			return xmlNode;
		}

		internal static XmlNode GenerateQueryCAML(QueryFilter query)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("Query");
			if (query != null)
			{
				xmlNode.AppendChild(xmlDocument.CreateElement("Where"));
				xmlNode.ChildNodes[0].AppendChild(SharepointHelpers.GenerateQueryCAMLHelper(query, xmlDocument, 0));
			}
			return xmlNode;
		}

		internal static XmlNode GenerateQueryOptionsXml(IList<string> itemHierarchy)
		{
			string folderPath = null;
			if (itemHierarchy != null && itemHierarchy.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < itemHierarchy.Count; i++)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("/");
					}
					stringBuilder.Append(itemHierarchy[i]);
				}
				folderPath = stringBuilder.ToString();
			}
			return SharepointHelpers.GenerateQueryOptionsXml(folderPath);
		}

		internal static XmlNode GenerateQueryOptionsXml(string folderPath)
		{
			XmlNode xmlNode = SharepointHelpers.DefaultQueryOptions.CloneNode(true);
			if (folderPath != null)
			{
				XmlNode xmlNode2 = xmlNode.OwnerDocument.CreateElement("Folder");
				xmlNode2.InnerText = folderPath;
				xmlNode.AppendChild(xmlNode2);
			}
			return xmlNode;
		}

		internal static object NoOp(string obj, CultureInfo cultureInfo)
		{
			return obj;
		}

		internal static string ObjectToString(object obj, CultureInfo cultureInfo)
		{
			return obj.ToString();
		}

		internal static object ExtensionToContentType(string str, CultureInfo cultureInfo)
		{
			return ExtensionToContentTypeMapper.Instance.GetContentTypeByExtension(str);
		}

		internal static object SecondJoinedValue(string str, CultureInfo cultureInfo)
		{
			int num = str.IndexOf(";#");
			if (num == -1)
			{
				return null;
			}
			return str.Substring(num + ";#".Length);
		}

		internal static string DateTimeToSharepoint(object obj, CultureInfo cultureInfo)
		{
			return string.Format("{0:s}Z", (DateTime)((ExDateTime)obj).ToUtc());
		}

		internal static object SharepointToDateTime(string obj, CultureInfo cultureInfo)
		{
			return ExDateTime.Parse(obj, cultureInfo.DateTimeFormat);
		}

		internal static object SharepoinToListDateTime(string obj, CultureInfo cultureInfo)
		{
			DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
			return ExDateTime.ParseExact(obj, new string[]
			{
				"M/d/yyyy h:mm:ss tt",
				"yyyyMMdd HH:mm:ss"
			}, currentInfo, DateTimeStyles.None);
		}

		internal static object SharepointJoinedToDateTime(string obj, CultureInfo cultureInfo)
		{
			return SharepointHelpers.SharepointToDateTime((string)SharepointHelpers.SecondJoinedValue(obj, cultureInfo), cultureInfo);
		}

		internal static object SharepointJoinedToInt(string obj, CultureInfo cultureInfo)
		{
			string value = (string)SharepointHelpers.SecondJoinedValue(obj, cultureInfo);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			return int.Parse((string)SharepointHelpers.SecondJoinedValue(obj, cultureInfo));
		}

		internal static object SharepointJoinedToLong(string obj, CultureInfo cultureInfo)
		{
			string value = (string)SharepointHelpers.SecondJoinedValue(obj, cultureInfo);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			return long.Parse((string)SharepointHelpers.SecondJoinedValue(obj, cultureInfo));
		}

		internal static object SharepointIsFolderToBool(string obj, CultureInfo cultureInfo)
		{
			return (int)SharepointHelpers.SharepointJoinedToInt(obj, cultureInfo) != 0;
		}

		public static object SharepointToInt(string obj, CultureInfo cultureInfo)
		{
			return int.Parse(obj);
		}

		public static object SharepointToAbsoluteUri(string obj, CultureInfo cultureInfo)
		{
			return new Uri(obj, UriKind.Absolute);
		}

		public static object SharepointToRelateiveUri(string obj, CultureInfo cultureInfo)
		{
			return new Uri(obj, UriKind.Relative);
		}

		public static object SharepointToBool(string obj, CultureInfo cultureInfo)
		{
			return bool.Parse(obj);
		}

		internal static XmlNamespaceManager SharepointNamespaceManager;

		internal static XmlNode DefaultQueryOptions;

		internal static int MaxFilterHierarchyDepth = Utils.MaxFilterDepth;
	}
}
