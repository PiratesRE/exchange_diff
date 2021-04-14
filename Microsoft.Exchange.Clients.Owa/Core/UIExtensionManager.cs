using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class UIExtensionManager
	{
		private UIExtensionManager()
		{
		}

		internal static List<UIExtensionManager.NewMenuExtensionItem>.Enumerator GetMenuItemEnumerator()
		{
			return UIExtensionManager.menuItemEntries.GetEnumerator();
		}

		internal static List<UIExtensionManager.RightClickMenuExtensionItem>.Enumerator GetMessageContextMenuItemEnumerator()
		{
			return UIExtensionManager.contextMenuItemEntries.GetEnumerator();
		}

		internal static List<UIExtensionManager.NavigationExtensionItem>.Enumerator GetNavigationBarEnumerator()
		{
			return UIExtensionManager.navigationBarEntries.GetEnumerator();
		}

		internal static void Initialize()
		{
			if (!File.Exists(UIExtensionManager.FullExtensionFileName))
			{
				return;
			}
			try
			{
				using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(UIExtensionManager.FullExtensionFileName))
				{
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
					while (xmlTextReader.Read())
					{
						if (xmlTextReader.NodeType == XmlNodeType.Element)
						{
							if (xmlTextReader.Name == "MainNavigationBarExtensions")
							{
								UIExtensionManager.ParseNavigationBarEntries(xmlTextReader, UIExtensionManager.navigationBarEntries);
							}
							else if (xmlTextReader.Name == "NewItemMenuEntries")
							{
								UIExtensionManager.ParseNewItemMenuEntries(xmlTextReader, UIExtensionManager.menuItemEntries);
							}
							else if (xmlTextReader.Name == "RightClickMenuExtensions")
							{
								UIExtensionManager.ParseRightClickMenuItemEntries(xmlTextReader, UIExtensionManager.contextMenuItemEntries);
							}
						}
					}
				}
			}
			catch (Exception)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_CustomizationUIExtensionParseError, string.Empty, new object[]
				{
					UIExtensionManager.FullExtensionFileName
				});
				UIExtensionManager.navigationBarEntries.Clear();
				UIExtensionManager.menuItemEntries.Clear();
				UIExtensionManager.contextMenuItemEntries.Clear();
			}
		}

		private static void ParseNavigationBarEntries(XmlTextReader reader, List<UIExtensionManager.NavigationExtensionItem> entries)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "MainNavigationBarEntry")
				{
					entries.Add(new UIExtensionManager.NavigationExtensionItem(UIExtensionManager.CreateIconPath(reader, "LargeIcon", false), UIExtensionManager.CreateIconPath(reader, "SmallIcon", false), reader.GetAttribute("URL"), UIExtensionManager.ParseMultiLanguageText(reader, "MainNavigationBarEntry")));
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "MainNavigationBarExtensions")
				{
					return;
				}
			}
		}

		private static void ParseNewItemMenuEntries(XmlTextReader reader, List<UIExtensionManager.NewMenuExtensionItem> entries)
		{
			while (reader.Read() && entries.Count < 10)
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "NewItemMenuEntry")
				{
					entries.Add(new UIExtensionManager.NewMenuExtensionItem(UIExtensionManager.CreateIconPath(reader, "Icon", false), reader.GetAttribute("ItemType"), UIExtensionManager.ParseMultiLanguageText(reader, "NewItemMenuEntry")));
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "NewItemMenuEntries")
				{
					return;
				}
			}
		}

		private static void ParseRightClickMenuItemEntries(XmlTextReader reader, List<UIExtensionManager.RightClickMenuExtensionItem> entries)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "RightClickMenuEntry")
				{
					entries.Add(new UIExtensionManager.RightClickMenuExtensionItem(UIExtensionManager.CreateIconPath(reader, "Icon", true), reader.GetAttribute("filter"), reader.GetAttribute("URL"), UIExtensionManager.ParseMultiLanguageText(reader, "RightClickMenuEntry")));
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "RightClickMenuExtensions")
				{
					return;
				}
			}
		}

		private static string CreateIconPath(XmlTextReader reader, string attributeName, bool isOptional)
		{
			string attribute = reader.GetAttribute(attributeName);
			if (isOptional && string.IsNullOrEmpty(attribute))
			{
				return null;
			}
			return OwaUrl.ApplicationRoot.ImplicitUrl + "forms/Customization/" + attribute;
		}

		private static Dictionary<string, string> ParseMultiLanguageText(XmlTextReader reader, string expectedEndTag)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == expectedEndTag)))
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "string")
				{
					string text = reader.GetAttribute("language") ?? string.Empty;
					string attribute = reader.GetAttribute("text");
					if (attribute != null)
					{
						dictionary[text.ToLowerInvariant()] = attribute;
					}
				}
			}
			return dictionary;
		}

		internal const string ExtensionFolderName = "forms/Customization/";

		internal const string ExtensionFileName = "UIExtensions.xml";

		private const int MaxNewMenuItemCount = 10;

		internal static readonly string FullExtensionFileName = Path.Combine(HttpRuntime.AppDomainAppPath + "forms/Customization/", "UIExtensions.xml").Replace('/', '\\');

		private static List<UIExtensionManager.NavigationExtensionItem> navigationBarEntries = new List<UIExtensionManager.NavigationExtensionItem>();

		private static List<UIExtensionManager.NewMenuExtensionItem> menuItemEntries = new List<UIExtensionManager.NewMenuExtensionItem>();

		private static List<UIExtensionManager.RightClickMenuExtensionItem> contextMenuItemEntries = new List<UIExtensionManager.RightClickMenuExtensionItem>();

		internal abstract class MultiLanguageTextBase
		{
			internal string GetTextByLanguage(string language)
			{
				string text = language.ToLowerInvariant();
				for (int i = text.Length; i >= 0; i = text.LastIndexOf('-'))
				{
					text = text.Substring(0, i);
					if (this.multiLanguageText.ContainsKey(text))
					{
						return this.multiLanguageText[text];
					}
				}
				if (this.multiLanguageText.ContainsKey(string.Empty))
				{
					return this.multiLanguageText[string.Empty];
				}
				return string.Empty;
			}

			protected MultiLanguageTextBase(Dictionary<string, string> multiLanguageText)
			{
				this.multiLanguageText = multiLanguageText;
			}

			private readonly Dictionary<string, string> multiLanguageText;
		}

		internal class NavigationExtensionItem : UIExtensionManager.MultiLanguageTextBase
		{
			internal string LargeIcon
			{
				get
				{
					return this.largeIcon;
				}
			}

			internal string SmallIcon
			{
				get
				{
					return this.smallIcon;
				}
			}

			internal string TargetUrl
			{
				get
				{
					return this.targetUrl;
				}
			}

			internal NavigationExtensionItem(string largeIcon, string smallIcon, string targetUrl, Dictionary<string, string> multiLanguageText) : base(multiLanguageText)
			{
				this.largeIcon = largeIcon;
				this.smallIcon = smallIcon;
				this.targetUrl = targetUrl;
			}

			private readonly string largeIcon;

			private readonly string smallIcon;

			private readonly string targetUrl;
		}

		internal class NewMenuExtensionItem : UIExtensionManager.MultiLanguageTextBase
		{
			internal string Icon
			{
				get
				{
					return this.icon;
				}
			}

			internal string CustomType
			{
				get
				{
					return this.customType;
				}
			}

			internal NewMenuExtensionItem(string icon, string customType, Dictionary<string, string> multiLanguageText) : base(multiLanguageText)
			{
				this.icon = icon;
				this.customType = customType;
			}

			private readonly string icon;

			private readonly string customType;
		}

		internal class RightClickMenuExtensionItem : UIExtensionManager.MultiLanguageTextBase
		{
			internal string Icon
			{
				get
				{
					return this.icon;
				}
			}

			internal string CustomType
			{
				get
				{
					return this.customType;
				}
			}

			internal string TargetUrl
			{
				get
				{
					return this.targetUrl;
				}
			}

			internal bool HasQueryString
			{
				get
				{
					return this.hasQueryString;
				}
			}

			internal RightClickMenuExtensionItem(string icon, string customType, string targetUrl, Dictionary<string, string> multiLanguageText) : base(multiLanguageText)
			{
				this.icon = icon;
				this.customType = customType;
				this.targetUrl = targetUrl;
				this.hasQueryString = targetUrl.Contains("?");
			}

			private readonly string icon;

			private readonly string customType;

			private readonly string targetUrl;

			private readonly bool hasQueryString;
		}

		private struct XmlTags
		{
			public const string MainNavigationBarExtensions = "MainNavigationBarExtensions";

			public const string MainNavigationBarEntry = "MainNavigationBarEntry";

			public const string NewItemMenuEntries = "NewItemMenuEntries";

			public const string NewItemMenuEntry = "NewItemMenuEntry";

			public const string Language = "language";

			public const string Icon = "Icon";

			public const string LargeIcon = "LargeIcon";

			public const string SmallIcon = "SmallIcon";

			public const string URL = "URL";

			public const string String = "string";

			public const string Text = "text";

			public const string ItemType = "ItemType";

			public const string Filter = "filter";

			public const string OWAUICustomizations = "OWAUICustomizations";

			public const string RightClickMenuExtensions = "RightClickMenuExtensions";

			public const string RightClickMenuEntry = "RightClickMenuEntry";
		}
	}
}
