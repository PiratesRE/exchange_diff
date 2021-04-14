using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class SmallIconManager
	{
		private static Dictionary<FolderSharingFlag, Dictionary<string, SmallIconManager.SmallIcon>> CreateSharingFolderIconMapping()
		{
			Dictionary<FolderSharingFlag, Dictionary<string, SmallIconManager.SmallIcon>> dictionary = new Dictionary<FolderSharingFlag, Dictionary<string, SmallIconManager.SmallIcon>>();
			dictionary[FolderSharingFlag.SharedIn] = new Dictionary<string, SmallIconManager.SmallIcon>();
			dictionary[FolderSharingFlag.SharedOut] = new Dictionary<string, SmallIconManager.SmallIcon>();
			dictionary[FolderSharingFlag.WebCalendar] = new Dictionary<string, SmallIconManager.SmallIcon>();
			dictionary[FolderSharingFlag.SharedIn]["IPF.Note"] = new SmallIconManager.SmallIcon(355, -1018465893);
			dictionary[FolderSharingFlag.SharedOut]["IPF.Note"] = new SmallIconManager.SmallIcon(359, -1018465893);
			dictionary[FolderSharingFlag.SharedIn]["IPF.Appointment"] = new SmallIconManager.SmallIcon(356, -1018465893);
			dictionary[FolderSharingFlag.SharedOut]["IPF.Appointment"] = new SmallIconManager.SmallIcon(360, -1018465893);
			dictionary[FolderSharingFlag.SharedIn]["IPF.Contact"] = new SmallIconManager.SmallIcon(357, -1018465893);
			dictionary[FolderSharingFlag.SharedOut]["IPF.Contact"] = new SmallIconManager.SmallIcon(361, -1018465893);
			dictionary[FolderSharingFlag.SharedIn]["IPF.Task"] = new SmallIconManager.SmallIcon(358, -1018465893);
			dictionary[FolderSharingFlag.SharedOut]["IPF.Task"] = new SmallIconManager.SmallIcon(362, -1018465893);
			dictionary[FolderSharingFlag.WebCalendar]["IPF.Appointment"] = new SmallIconManager.SmallIcon(458, -1018465893);
			return dictionary;
		}

		internal static void Initialize()
		{
			SmallIconManager.smallIconTable = new Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>();
			SmallIconManager.prefixMatchSmallIconTable = new Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>();
			string xmlFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "SmallIcons.xml");
			SmallIconManager.LoadXmlData(xmlFilePath, null, SmallIconManager.prefixMatchSmallIconTable, SmallIconManager.smallIconTable);
			string fullExtensionFileName = UIExtensionManager.FullExtensionFileName;
			if (!File.Exists(fullExtensionFileName))
			{
				return;
			}
			try
			{
				Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> dictionary = new Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>();
				Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> dictionary2 = new Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>();
				SmallIconManager.LoadXmlData(fullExtensionFileName, "forms/Customization/", dictionary2, dictionary);
				SmallIconManager.MergeSmallIconTable(SmallIconManager.smallIconTable, dictionary);
				SmallIconManager.MergeSmallIconTable(SmallIconManager.prefixMatchSmallIconTable, dictionary2);
			}
			catch (Exception)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_CustomizationUIExtensionParseError, string.Empty, new object[]
				{
					fullExtensionFileName
				});
			}
		}

		internal static void RenderItemIconUrl(TextWriter writer, UserContext userContext, string itemClass)
		{
			SmallIconManager.RenderItemIconUrl(writer, userContext, itemClass, false, false);
		}

		internal static void RenderItemIconUrl(TextWriter writer, UserContext userContext, string itemClass, string defaultItemClass)
		{
			SmallIconManager.RenderItemIconUrl(writer, userContext, itemClass, defaultItemClass, false, false, -1);
		}

		internal static void RenderItemIconUrl(TextWriter writer, UserContext userContext, string itemClass, int iconFlag)
		{
			SmallIconManager.RenderItemIconUrl(writer, userContext, itemClass, null, false, false, iconFlag);
		}

		internal static void RenderItemIconUrl(TextWriter writer, UserContext userContext, string itemClass, bool isInConflict, bool isRead)
		{
			SmallIconManager.RenderItemIconUrl(writer, userContext, itemClass, null, isInConflict, isRead, -1);
		}

		private static void RenderIconUrl(TextWriter writer, UserContext userContext, SmallIconManager.SmallIcon smallIcon)
		{
			if (smallIcon.IsCustom)
			{
				writer.Write(smallIcon.CustomUrl);
				return;
			}
			userContext.RenderThemeFileUrl(writer, smallIcon.ThemeId);
		}

		internal static void RenderIcon(TextWriter writer, UserContext userContext, SmallIconManager.SmallIcon smallIcon, bool showTooltip, string styleClass, params string[] extraAttributes)
		{
			if (smallIcon.IsCustom)
			{
				writer.Write("<img src=\"");
				writer.Write(smallIcon.CustomUrl);
				writer.Write("\"");
				foreach (string value in extraAttributes)
				{
					writer.Write(" ");
					writer.Write(value);
				}
				writer.Write(">");
				return;
			}
			if (showTooltip)
			{
				userContext.RenderThemeImageWithToolTip(writer, (ThemeFileId)smallIcon.ThemeId, null, smallIcon.AltId, extraAttributes);
				return;
			}
			userContext.RenderThemeImage(writer, (ThemeFileId)smallIcon.ThemeId, null, extraAttributes);
		}

		internal static void RenderItemIconUrl(TextWriter writer, UserContext userContext, string itemClass, string defaultItemClass, bool isInConflict, bool isRead, int iconFlag)
		{
			SmallIconManager.SmallIcon itemSmallIcon = SmallIconManager.GetItemSmallIcon(itemClass, defaultItemClass, isInConflict, isRead, iconFlag);
			SmallIconManager.RenderIconUrl(writer, userContext, itemSmallIcon);
		}

		internal static void RenderItemIcon(TextWriter writer, UserContext userContext, string itemClass, params string[] extraAttributes)
		{
			SmallIconManager.RenderItemIcon(writer, userContext, itemClass, false, string.Empty, extraAttributes);
		}

		internal static void RenderItemIcon(TextWriter writer, UserContext userContext, string itemClass, bool showTooltip, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.RenderItemIcon(writer, userContext, itemClass, showTooltip, null, styleClass, extraAttributes);
		}

		internal static void RenderItemIcon(TextWriter writer, UserContext userContext, string itemClass, bool showTooltip, string defaultItemClass, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.RenderItemIcon(writer, userContext, itemClass, showTooltip, defaultItemClass, false, false, -1, styleClass, extraAttributes);
		}

		internal static void RenderItemIcon(TextWriter writer, UserContext userContext, string itemClass, bool showTooltip, int iconFlag, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.RenderItemIcon(writer, userContext, itemClass, showTooltip, null, false, false, iconFlag, styleClass, extraAttributes);
		}

		internal static void RenderItemIcon(TextWriter writer, UserContext userContext, string itemClass, bool showTooltip, bool isInConflict, bool isRead, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.RenderItemIcon(writer, userContext, itemClass, showTooltip, null, isInConflict, isRead, -1, styleClass, extraAttributes);
		}

		internal static void RenderItemIcon(TextWriter writer, UserContext userContext, string itemClass, bool showTooltip, string defaultItemClass, bool isInConflict, bool isRead, int iconFlag, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.SmallIcon itemSmallIcon = SmallIconManager.GetItemSmallIcon(itemClass, defaultItemClass, isInConflict, isRead, iconFlag);
			SmallIconManager.RenderIcon(writer, userContext, itemSmallIcon, showTooltip, styleClass, extraAttributes);
		}

		public static SmallIconManager.SmallIcon GetItemSmallIcon(string itemClass, string defaultItemClass, bool isInConflict, bool isRead, int iconFlag)
		{
			if (isInConflict)
			{
				return new SmallIconManager.SmallIcon(82, -1240042979);
			}
			if (!EnumValidator.IsValidValue<IconIndex>((IconIndex)iconFlag))
			{
				iconFlag = -1;
			}
			if (iconFlag == -1 && isRead)
			{
				iconFlag = 256;
			}
			else if (iconFlag == 306 && isRead)
			{
				iconFlag = 256;
			}
			SmallIconManager.SmallIcon smallIcon = SmallIconManager.LookupSmallIcon(itemClass, iconFlag);
			if (smallIcon == null && iconFlag == 256)
			{
				iconFlag = -1;
				smallIcon = SmallIconManager.LookupSmallIcon(itemClass, iconFlag);
			}
			if (iconFlag == -1 && isRead && smallIcon == null)
			{
				smallIcon = SmallIconManager.PrefixMatchLookupSmallIcon(itemClass, 256);
			}
			if (smallIcon == null)
			{
				smallIcon = SmallIconManager.PrefixMatchLookupSmallIcon(itemClass, iconFlag);
			}
			if (smallIcon == null)
			{
				smallIcon = SmallIconManager.LookupSmallIcon(itemClass, -1);
			}
			if (smallIcon == null && defaultItemClass != null)
			{
				smallIcon = SmallIconManager.LookupSmallIcon(defaultItemClass, iconFlag);
			}
			if (smallIcon == null && defaultItemClass != null)
			{
				smallIcon = SmallIconManager.PrefixMatchLookupSmallIcon(defaultItemClass, iconFlag);
			}
			if (smallIcon == null && (iconFlag == 261 || iconFlag == 262))
			{
				smallIcon = SmallIconManager.LookupSmallIcon("IPM.Note", iconFlag);
			}
			if (smallIcon == null)
			{
				smallIcon = (isRead ? new SmallIconManager.SmallIcon(132, -1075414859) : new SmallIconManager.SmallIcon(133, -1679159840));
			}
			return smallIcon;
		}

		internal static void RenderFolderIconUrl(TextWriter writer, UserContext userContext, string containerClass)
		{
			SmallIconManager.RenderFolderIconUrl(writer, userContext, containerClass, FolderSharingFlag.None);
		}

		private static SmallIconManager.SmallIcon GetFolderSmallIcon(UserContext userContext, string containerClass, FolderSharingFlag sharingFlag)
		{
			SmallIconManager.SmallIcon smallIcon;
			if (containerClass == null)
			{
				smallIcon = SmallIconManager.normalFolderIcon;
			}
			else
			{
				smallIcon = SmallIconManager.LookupSmallIcon(containerClass, -1);
				if (smallIcon == null)
				{
					smallIcon = SmallIconManager.PrefixMatchLookupSmallIcon(containerClass, -1);
					if (smallIcon == null)
					{
						smallIcon = SmallIconManager.normalFolderIcon;
					}
				}
			}
			if (!smallIcon.IsCustom && sharingFlag != FolderSharingFlag.None)
			{
				smallIcon = (SmallIconManager.GetIconForSharedFolder(containerClass, sharingFlag) ?? smallIcon);
			}
			return smallIcon;
		}

		internal static void RenderFolderIconUrl(TextWriter writer, UserContext userContext, string containerClass, FolderSharingFlag sharingFlag)
		{
			SmallIconManager.SmallIcon folderSmallIcon = SmallIconManager.GetFolderSmallIcon(userContext, containerClass, sharingFlag);
			SmallIconManager.RenderIconUrl(writer, userContext, folderSmallIcon);
		}

		internal static void RenderFolderIcon(TextWriter writer, UserContext userContext, string containerClass, FolderSharingFlag sharingFlag, bool showTooltip, params string[] extraAttributes)
		{
			SmallIconManager.SmallIcon folderSmallIcon = SmallIconManager.GetFolderSmallIcon(userContext, containerClass, sharingFlag);
			SmallIconManager.RenderIcon(writer, userContext, folderSmallIcon, showTooltip, string.Empty, extraAttributes);
		}

		private static SmallIconManager.SmallIcon GetIconForSharedFolder(string containerClass, FolderSharingFlag sharingFlag)
		{
			if (sharingFlag == FolderSharingFlag.None)
			{
				return null;
			}
			if (string.IsNullOrEmpty(containerClass))
			{
				containerClass = "IPF.Note";
			}
			foreach (KeyValuePair<string, SmallIconManager.SmallIcon> keyValuePair in SmallIconManager.sharingFolderIconMapping[sharingFlag])
			{
				if (ObjectClass.IsOfClass(containerClass, keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		private static SmallIconManager.SmallIcon GetFileSmallIcon(UserContext userContext, string fileExtension, int iconFlag)
		{
			SmallIconManager.SmallIcon smallIcon = SmallIconManager.LookupSmallIcon(fileExtension, iconFlag);
			if (smallIcon == null)
			{
				smallIcon = new SmallIconManager.SmallIcon(129, -1018465893);
			}
			return smallIcon;
		}

		internal static void RenderFileIcon(TextWriter writer, UserContext userContext, string fileExtension, params string[] extraAttributes)
		{
			SmallIconManager.RenderFileIcon(writer, userContext, fileExtension, -1, false, string.Empty, extraAttributes);
		}

		internal static void RenderFileIcon(TextWriter writer, UserContext userContext, string fileExtension, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.RenderFileIcon(writer, userContext, fileExtension, -1, false, styleClass, extraAttributes);
		}

		internal static void RenderFileIcon(TextWriter writer, UserContext userContext, string fileExtension, int iconFlag, bool showTooltip, string styleClass, params string[] extraAttributes)
		{
			SmallIconManager.SmallIcon fileSmallIcon = SmallIconManager.GetFileSmallIcon(userContext, fileExtension, iconFlag);
			SmallIconManager.RenderIcon(writer, userContext, fileSmallIcon, showTooltip, styleClass, extraAttributes);
		}

		internal static void RenderFileIconUrl(TextWriter writer, UserContext userContext, string fileExtension)
		{
			SmallIconManager.RenderFileIconUrl(writer, userContext, fileExtension, -1);
		}

		internal static void RenderFileIconUrl(TextWriter writer, UserContext userContext, string fileExtension, int iconFlag)
		{
			SmallIconManager.SmallIcon fileSmallIcon = SmallIconManager.GetFileSmallIcon(userContext, fileExtension, iconFlag);
			SmallIconManager.RenderIconUrl(writer, userContext, fileSmallIcon);
		}

		private static XmlTextReader InitializeXmlTextReader(string xmlFilePath)
		{
			ExTraceGlobals.SmallIconCallTracer.TraceDebug<string>(0L, "InitializeXmlTextReader: XmlFilePath = '{0}'", xmlFilePath);
			if (!File.Exists(xmlFilePath))
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_SmallIconsFileNotFound, string.Empty, new object[]
				{
					xmlFilePath
				});
				throw new OwaSmallIconManagerInitializationException("SmallIcon XML file is not found: '" + xmlFilePath + "'");
			}
			XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(xmlFilePath);
			xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
			xmlTextReader.NameTable.Add("SmallIconMappings");
			xmlTextReader.NameTable.Add("Mapping");
			xmlTextReader.NameTable.Add("ItemClass");
			xmlTextReader.NameTable.Add("IconFlag");
			xmlTextReader.NameTable.Add("SmallIcon");
			xmlTextReader.NameTable.Add("PrefixMatch");
			xmlTextReader.NameTable.Add("Alt");
			return xmlTextReader;
		}

		private static void LoadXmlData(string xmlFilePath, string folderName, Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> prefixIconTable, Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> iconTable)
		{
			ExTraceGlobals.SmallIconCallTracer.TraceDebug<string>(0L, "LoadXmlData: XmlFilePath = '{0}'", xmlFilePath);
			using (XmlTextReader xmlTextReader = SmallIconManager.InitializeXmlTextReader(xmlFilePath))
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder();
				while (xmlTextReader.Read())
				{
					XmlNodeType nodeType = xmlTextReader.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType == XmlNodeType.EndElement)
						{
							if (xmlTextReader.Name == xmlTextReader.NameTable.Get("SmallIconMappings"))
							{
								flag = false;
							}
						}
					}
					else if (xmlTextReader.Name == xmlTextReader.NameTable.Get("SmallIconMappings"))
					{
						flag = true;
					}
					else if (flag && xmlTextReader.Name == xmlTextReader.NameTable.Get("Mapping"))
					{
						SmallIconManager.ParseMappingElement(xmlTextReader, folderName, stringBuilder, prefixIconTable, iconTable);
					}
				}
				if (stringBuilder.Length != 0)
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_SmallIconsAltReferenceInvalid, string.Empty, new object[]
					{
						stringBuilder.ToString()
					});
				}
			}
		}

		private static void ParseMappingElement(XmlTextReader xmlTextReader, string folderName, StringBuilder invalidAltIds, Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> prefixIconTable, Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> iconTable)
		{
			ExTraceGlobals.SmallIconCallTracer.TraceDebug(0L, "ParseMappingElement");
			string text = null;
			int num = -1;
			string text2 = null;
			bool flag = false;
			Strings.IDs ds = -1018465893;
			if (xmlTextReader.MoveToAttribute(xmlTextReader.NameTable.Get("ItemClass")))
			{
				text = xmlTextReader.Value;
			}
			if (xmlTextReader.MoveToAttribute(xmlTextReader.NameTable.Get("SmallIcon")))
			{
				text2 = xmlTextReader.Value;
			}
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				ExTraceGlobals.SmallIconTracer.TraceDebug<string, string>(0L, "Either ItemClass:'{0}' or SmallIcon:'{1}' is not valid.", text, text2);
				return;
			}
			if (xmlTextReader.MoveToAttribute(xmlTextReader.NameTable.Get("IconFlag")))
			{
				string value = xmlTextReader.Value;
				try
				{
					num = (int)Enum.Parse(typeof(IconIndex), value, true);
					goto IL_C9;
				}
				catch (ArgumentException)
				{
					num = value.GetHashCode();
					goto IL_C9;
				}
			}
			num = -1;
			IL_C9:
			flag = (xmlTextReader.MoveToAttribute(xmlTextReader.NameTable.Get("PrefixMatch")) && string.Equals(xmlTextReader.Value, "true", StringComparison.OrdinalIgnoreCase));
			if (xmlTextReader.MoveToAttribute(xmlTextReader.NameTable.Get("Alt")))
			{
				string value2 = xmlTextReader.Value;
				try
				{
					ds = (Strings.IDs)Enum.Parse(typeof(Strings.IDs), value2, true);
					goto IL_160;
				}
				catch (ArgumentException)
				{
					if (invalidAltIds.Length != 0)
					{
						invalidAltIds.Append(",");
					}
					invalidAltIds.Append(value2);
					ds = -1018465893;
					goto IL_160;
				}
			}
			ds = -1018465893;
			IL_160:
			SmallIconManager.SmallIcon smallIcon = (folderName == null) ? new SmallIconManager.SmallIcon(ThemeFileList.Add(text2, true), ds) : new SmallIconManager.SmallIcon(OwaUrl.ApplicationRoot.ImplicitUrl + folderName + text2, ds);
			if (flag)
			{
				if (!prefixIconTable.ContainsKey(num))
				{
					prefixIconTable[num] = new Dictionary<string, SmallIconManager.SmallIcon>(StringComparer.OrdinalIgnoreCase);
				}
				prefixIconTable[num][text] = smallIcon;
			}
			else
			{
				if (!iconTable.ContainsKey(num))
				{
					iconTable[num] = new Dictionary<string, SmallIconManager.SmallIcon>(StringComparer.OrdinalIgnoreCase);
				}
				iconTable[num][text] = smallIcon;
			}
			ExTraceGlobals.SmallIconDataTracer.TraceDebug(0L, "Add {0}PrefixMatch mapping: IconFlag = '{1}', ItemClass = '{2}' or SmallIcon:'{3}', Alt:'{4}'.", new object[]
			{
				flag ? string.Empty : "Non-",
				num,
				text,
				(folderName == null) ? ("Index " + smallIcon.ThemeId) : text2,
				ds
			});
		}

		private static SmallIconManager.SmallIcon PrefixMatchLookupSmallIcon(string itemClass, int iconFlag)
		{
			ExTraceGlobals.SmallIconCallTracer.TraceDebug<string, int>(0L, "PrefixMatchLookupSmallIcon: ItemClass = '{0}', IconFlag = '{1}'", itemClass, iconFlag);
			Dictionary<string, SmallIconManager.SmallIcon> dictionary = null;
			if (!SmallIconManager.prefixMatchSmallIconTable.TryGetValue(iconFlag, out dictionary))
			{
				ExTraceGlobals.SmallIconTracer.TraceDebug<int>(0L, "Can not find IconFlag from the prefixMatchSmallIconTable: IconFlag = '{0}'", iconFlag);
				return null;
			}
			if (dictionary.ContainsKey(itemClass))
			{
				SmallIconManager.SmallIcon smallIcon = dictionary[itemClass];
				ExTraceGlobals.SmallIconTracer.TraceDebug<string, int, Strings.IDs>(0L, "Found exact match of ItemClass: ItemClass = '{0}', ThemeFileIndex = '{1}', Alt = '{2}'", itemClass, smallIcon.ThemeId, smallIcon.AltId);
				return smallIcon;
			}
			foreach (string text in dictionary.Keys)
			{
				if (string.Compare(text, 0, itemClass, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					SmallIconManager.SmallIcon smallIcon2 = dictionary[text];
					ExTraceGlobals.SmallIconTracer.TraceDebug<string, int, Strings.IDs>(0L, "Found prefix match of ItemClass: ItemClass = '{0}', ThemeFileIndex = '{1}', Alt = '{2}'", itemClass, smallIcon2.ThemeId, smallIcon2.AltId);
					return smallIcon2;
				}
			}
			return null;
		}

		private static SmallIconManager.SmallIcon LookupSmallIcon(string itemClass, int iconFlag)
		{
			ExTraceGlobals.SmallIconCallTracer.TraceDebug<string, int>(0L, "LookupSmallIcon: ItemClass = '{0}', IconFlag = '{1}'", itemClass, iconFlag);
			Dictionary<string, SmallIconManager.SmallIcon> dictionary = null;
			if (SmallIconManager.smallIconTable.TryGetValue(iconFlag, out dictionary) && dictionary.ContainsKey(itemClass))
			{
				SmallIconManager.SmallIcon smallIcon = dictionary[itemClass];
				ExTraceGlobals.SmallIconTracer.TraceDebug<string, int, Strings.IDs>(0L, "Found exact match of ItemClass: ItemClass = '{0}', ThemeFileIndex = '{1}', Alt = {2}", itemClass, smallIcon.ThemeId, smallIcon.AltId);
				return smallIcon;
			}
			return null;
		}

		private static void MergeSmallIconTable(Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> iconTable, Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> tempIconTable)
		{
			foreach (KeyValuePair<int, Dictionary<string, SmallIconManager.SmallIcon>> keyValuePair in tempIconTable)
			{
				Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>.Enumerator enumerator;
				if (iconTable.ContainsKey(keyValuePair.Key))
				{
					KeyValuePair<int, Dictionary<string, SmallIconManager.SmallIcon>> keyValuePair2 = enumerator.Current;
					using (Dictionary<string, SmallIconManager.SmallIcon>.Enumerator enumerator2 = keyValuePair2.Value.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<int, Dictionary<string, SmallIconManager.SmallIcon>> keyValuePair3 = enumerator.Current;
							Dictionary<string, SmallIconManager.SmallIcon> dictionary = iconTable[keyValuePair3.Key];
							KeyValuePair<string, SmallIconManager.SmallIcon> keyValuePair4 = enumerator2.Current;
							string key = keyValuePair4.Key;
							KeyValuePair<string, SmallIconManager.SmallIcon> keyValuePair5 = enumerator2.Current;
							dictionary[key] = keyValuePair5.Value;
						}
						continue;
					}
				}
				KeyValuePair<int, Dictionary<string, SmallIconManager.SmallIcon>> keyValuePair6 = enumerator.Current;
				int key2 = keyValuePair6.Key;
				KeyValuePair<int, Dictionary<string, SmallIconManager.SmallIcon>> keyValuePair7 = enumerator.Current;
				iconTable[key2] = keyValuePair7.Value;
			}
		}

		public static Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>.Enumerator SmallIconTable
		{
			get
			{
				return SmallIconManager.smallIconTable.GetEnumerator();
			}
		}

		private const string SmallIconXmlFile = "SmallIcons.xml";

		private static Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> smallIconTable;

		private static Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>> prefixMatchSmallIconTable;

		private static readonly SmallIconManager.SmallIcon normalFolderIcon = new SmallIconManager.SmallIcon(118, -1018465893);

		private static readonly Dictionary<FolderSharingFlag, Dictionary<string, SmallIconManager.SmallIcon>> sharingFolderIconMapping = SmallIconManager.CreateSharingFolderIconMapping();

		private struct XmlTags
		{
			public const string Mapping = "Mapping";

			public const string ItemClass = "ItemClass";

			public const string IconFlag = "IconFlag";

			public const string SmallIcon = "SmallIcon";

			public const string PrefixMatch = "PrefixMatch";

			public const string SmallIconMappings = "SmallIconMappings";

			public const string Alt = "Alt";
		}

		public class SmallIcon
		{
			public SmallIcon(int themeId, Strings.IDs altId)
			{
				this.themeId = themeId;
				this.altId = altId;
				this.customUrl = null;
			}

			public SmallIcon(string customUrl, Strings.IDs altId)
			{
				this.themeId = 0;
				this.altId = altId;
				this.customUrl = customUrl;
			}

			public int ThemeId
			{
				get
				{
					return this.themeId;
				}
			}

			public Strings.IDs AltId
			{
				get
				{
					return this.altId;
				}
			}

			public string CustomUrl
			{
				get
				{
					return this.customUrl;
				}
			}

			public bool IsCustom
			{
				get
				{
					return this.customUrl != null;
				}
			}

			private int themeId;

			private Strings.IDs altId;

			private string customUrl;
		}
	}
}
