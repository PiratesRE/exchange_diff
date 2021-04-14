using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class Theme
	{
		private Theme(string folderPath)
		{
			this.Load(folderPath);
			this.InitializeForCssSprites();
		}

		private static bool IsValidThemeFolder(string folderPath)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Theme.RequiredFiles.Length; i++)
			{
				string path = Path.Combine(folderPath, Theme.RequiredFiles[i]);
				if (!File.Exists(path))
				{
					list.Add(Theme.RequiredFiles[i]);
				}
			}
			if (list.Count > 0)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_InvalidThemeFolder, string.Empty, new object[]
				{
					folderPath,
					string.Join(", ", list.ToArray())
				});
				return false;
			}
			return true;
		}

		public static Theme Create(string folderPath)
		{
			if (folderPath == null)
			{
				throw new ArgumentNullException("folderPath");
			}
			if (!Theme.IsValidThemeFolder(folderPath))
			{
				return null;
			}
			return new Theme(folderPath);
		}

		private static void ParseThemeInfoFile(string themeInfoFilePath, string folderName, out string displayName, out Strings.IDs localizedDisplayName, out int sortOrder)
		{
			XmlTextReader xmlTextReader = null;
			displayName = null;
			localizedDisplayName = -1018465893;
			sortOrder = int.MaxValue;
			try
			{
				xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(themeInfoFilePath);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				if (!xmlTextReader.Read() || xmlTextReader.NodeType != XmlNodeType.Element || !string.Equals("Theme", xmlTextReader.Name, StringComparison.OrdinalIgnoreCase))
				{
					Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Expected root element '{0}' not found.", "Theme"), ClientsEventLogConstants.Tuple_ThemeInfoExpectedElement, new object[]
					{
						"themeinfo.xml",
						folderName,
						"Theme"
					});
				}
				if (xmlTextReader.MoveToFirstAttribute())
				{
					do
					{
						if (string.Equals("DisplayName", xmlTextReader.Name, StringComparison.OrdinalIgnoreCase))
						{
							if (displayName != null)
							{
								Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Duplicated attribute '{0}' found.", "DisplayName"), ClientsEventLogConstants.Tuple_ThemeInfoDuplicatedAttribute, new object[]
								{
									"themeinfo.xml",
									folderName,
									"DisplayName",
									xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
									xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
								});
							}
							displayName = xmlTextReader.Value;
							if (string.IsNullOrEmpty(displayName))
							{
								Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Empty attribute '{0}' not allowed.", "DisplayName"), ClientsEventLogConstants.Tuple_ThemeInfoEmptyAttribute, new object[]
								{
									"themeinfo.xml",
									folderName,
									"DisplayName",
									xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
									xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
								});
							}
							if (displayName.Length > 512)
							{
								Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Attribute '{0}' exceedes the maximum limit of {1} characters.", "DisplayName", 512), ClientsEventLogConstants.Tuple_ThemeInfoAttributeExceededMaximumLength, new object[]
								{
									"themeinfo.xml",
									folderName,
									"DisplayName",
									512,
									xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
									xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
								});
							}
							localizedDisplayName = ThemeManager.GetLocalizedThemeName(displayName);
						}
						if (string.Equals("SortOrder", xmlTextReader.Name, StringComparison.OrdinalIgnoreCase))
						{
							try
							{
								sortOrder = int.Parse(xmlTextReader.Value);
							}
							catch
							{
								Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Attribute '{0}' is not a valid integer.", "SortOrder"), ClientsEventLogConstants.Tuple_ThemeInfoErrorParsingXml, new object[]
								{
									"themeinfo.xml",
									folderName,
									"SortOrder",
									xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
									xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
								});
							}
						}
					}
					while (xmlTextReader.MoveToNextAttribute());
					if (displayName == null)
					{
						Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Attribute '{0}' was not found.", "DisplayName"), ClientsEventLogConstants.Tuple_ThemeInfoMissingAttribute, new object[]
						{
							"themeinfo.xml",
							folderName,
							"DisplayName",
							xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
							xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
						});
					}
				}
			}
			catch (XmlException ex)
			{
				Theme.ThrowParserException(xmlTextReader, folderName, string.Format("XML parser error. {0}", ex.Message), ClientsEventLogConstants.Tuple_ThemeInfoErrorParsingXml, new object[]
				{
					"themeinfo.xml",
					folderName,
					xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
					xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
				});
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		private static void ThrowParserException(XmlTextReader reader, string folderName, string description, ExEventLog.EventTuple tuple, params object[] eventMessageArgs)
		{
			OwaDiagnostics.LogEvent(tuple, string.Empty, eventMessageArgs);
			throw new OwaThemeManagerInitializationException(string.Format(CultureInfo.InvariantCulture, "Invalid theme info file in folder '{0}'. Line  {1} Position {2}.{3}", new object[]
			{
				folderName,
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(description != null) ? (" " + description) : string.Empty
			}), null, null);
		}

		private void InitializeForCssSprites()
		{
			this.classNameForCssSpritesTable = new Dictionary<int, string>(ThemeFileList.Count);
			this.shouldUseCssSpritesTable = new Dictionary<int, bool>(ThemeFileList.Count);
			for (int i = 1; i < ThemeFileList.Count; i++)
			{
				this.classNameForCssSpritesTable[i] = this.InternalGetThemeFileClass((ThemeFileId)i);
				this.shouldUseCssSpritesTable[i] = this.InternalShouldUseCssSprites((ThemeFileId)i);
			}
		}

		private void Load(string folderPath)
		{
			ExTraceGlobals.ThemesCallTracer.TraceDebug<string>(0L, "Theme.Load. folderPath={0}", folderPath);
			string text = Path.Combine(folderPath, "themeinfo.xml");
			this.folderName = Path.GetFileNameWithoutExtension(folderPath);
			this.folderPath = folderPath;
			string text2 = null;
			Strings.IDs ds = -1018465893;
			int maxValue = int.MaxValue;
			if (File.Exists(text))
			{
				Theme.ParseThemeInfoFile(text, this.folderName, out text2, out ds, out maxValue);
			}
			if (text2 != null)
			{
				this.displayName = text2;
			}
			else
			{
				this.displayName = this.folderName;
			}
			this.localizedDisplayName = ds;
			this.sortOrder = maxValue;
			if (string.Equals(this.folderName, ThemeManager.BaseThemeFolderName, StringComparison.OrdinalIgnoreCase))
			{
				this.isBase = true;
			}
			this.themeFileTable = new Dictionary<int, bool>(ThemeFileList.Count);
			for (int i = 0; i < ThemeFileList.Count; i++)
			{
				this.themeFileTable[i] = false;
			}
			string[] array = null;
			try
			{
				array = Directory.GetFiles(folderPath);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ThemesTracer.TraceDebug<Exception, string>(0L, "Exception thrown by Directory.GetFiles. {0}. Callstack = {1}", ex, ex.StackTrace);
				throw;
			}
			ExTraceGlobals.ThemesTracer.TraceDebug<int, string>(0L, "Inspecting {0} files in theme folder '{1}'", array.Length, this.folderName);
			for (int j = 0; j < array.Length; j++)
			{
				string fileName = Path.GetFileName(array[j]);
				if (!string.IsNullOrEmpty(fileName))
				{
					int idFromName = ThemeFileList.GetIdFromName(fileName);
					if (idFromName == 0)
					{
						ExTraceGlobals.ThemesTracer.TraceDebug<string>(0L, "Skipping unknown file '{0}'", fileName);
					}
					else
					{
						ExTraceGlobals.ThemesTracer.TraceDebug<string>(0L, "Succesfully added theme file '{0}'", fileName);
						this.themeFileTable[idFromName] = true;
					}
				}
			}
			this.url = ThemeManager.ThemesFolderPath + this.folderName + "/";
		}

		public bool ShouldUseCssSprites(ThemeFileId themeFileId)
		{
			if (this.shouldUseCssSpritesTable.ContainsKey((int)themeFileId))
			{
				return this.shouldUseCssSpritesTable[(int)themeFileId];
			}
			bool flag = this.InternalShouldUseCssSprites(themeFileId);
			this.shouldUseCssSpritesTable[(int)themeFileId] = flag;
			return flag;
		}

		private bool InternalShouldUseCssSprites(ThemeFileId themeFileId)
		{
			return ThemeFileList.CanUseCssSprites(themeFileId) && (this.IsFileInTheme(ThemeFileId.CssSpritesCss) || !this.IsFileInTheme(themeFileId));
		}

		public string GetThemeFileClass(ThemeFileId themeFileId)
		{
			if (this.classNameForCssSpritesTable.ContainsKey((int)themeFileId))
			{
				return this.classNameForCssSpritesTable[(int)themeFileId];
			}
			string text = this.InternalGetThemeFileClass(themeFileId);
			this.classNameForCssSpritesTable[(int)themeFileId] = text;
			return text;
		}

		private string InternalGetThemeFileClass(ThemeFileId themeFileId)
		{
			string classNameFromId = ThemeFileList.GetClassNameFromId((int)themeFileId);
			StringBuilder stringBuilder = new StringBuilder(classNameFromId.Length + 16);
			stringBuilder.Append("csimgbg");
			stringBuilder.Append(" ");
			stringBuilder.Append(classNameFromId);
			return stringBuilder.ToString();
		}

		internal bool IsFileInTheme(ThemeFileId themeFileId)
		{
			return this.IsFileInTheme((int)themeFileId);
		}

		internal bool IsFileInTheme(int themeFileIndex)
		{
			if (this.themeFileTable.ContainsKey(themeFileIndex))
			{
				return this.themeFileTable[themeFileIndex];
			}
			string nameFromId = ThemeFileList.GetNameFromId(themeFileIndex);
			bool flag = File.Exists(Path.Combine(this.folderPath, nameFromId));
			this.themeFileTable[themeFileIndex] = flag;
			return flag;
		}

		public bool IsBase
		{
			get
			{
				return this.isBase;
			}
		}

		public string DisplayName
		{
			get
			{
				if (this.localizedDisplayName != -1018465893)
				{
					return LocalizedStrings.GetNonEncoded(this.localizedDisplayName);
				}
				return this.displayName;
			}
		}

		public int SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		public string StorageId
		{
			get
			{
				return this.folderName;
			}
		}

		public uint Id
		{
			get
			{
				return ThemeManager.GetIdFromStorageId(this.StorageId);
			}
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		private const string ThemeInfoFileName = "themeinfo.xml";

		private const string ThemeDisplayNameAttribute = "DisplayName";

		private const string ThemeSortOrderAttribute = "SortOrder";

		private const string ThemeRootElement = "Theme";

		private const int MaxThemeDisplayNameLength = 512;

		private static readonly string[] RequiredFiles = new string[]
		{
			"premium.css",
			"gradienth.png",
			"gradientv.png",
			"csssprites.css",
			"csssprites.png",
			"csssprites2.css",
			"csssprites2.png"
		};

		private string url;

		private bool isBase;

		private string folderName;

		private string displayName;

		private int sortOrder = int.MaxValue;

		private Strings.IDs localizedDisplayName = -1018465893;

		private Dictionary<int, bool> themeFileTable;

		private Dictionary<int, bool> shouldUseCssSpritesTable;

		private Dictionary<int, string> classNameForCssSpritesTable;

		private string folderPath;
	}
}
