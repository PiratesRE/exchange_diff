using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class Theme : IComparable<Theme>
	{
		private Theme(string folderPath)
		{
			this.Load(folderPath);
		}

		public bool IsBase
		{
			get
			{
				return this.isBase;
			}
		}

		[DataMember]
		public string FolderName
		{
			get
			{
				return this.folderName;
			}
			set
			{
				this.folderName = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
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

		public static Theme Create(string folderPath)
		{
			if (folderPath == null)
			{
				throw new ArgumentNullException("folderPath");
			}
			return new Theme(folderPath);
		}

		public int CompareTo(Theme otherTheme)
		{
			if (otherTheme == null)
			{
				return 1;
			}
			if (this.SortOrder == otherTheme.SortOrder)
			{
				return this.StorageId.CompareTo(otherTheme.StorageId);
			}
			return this.SortOrder.CompareTo(otherTheme.SortOrder);
		}

		private static void ParseThemeInfoFile(string themeInfoFilePath, string folderName, out string displayName, out int sortOrder)
		{
			XmlTextReader xmlTextReader = null;
			displayName = null;
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
								Theme.ThrowParserException(xmlTextReader, folderName, string.Format("Attribute '{0}' exceeds the maximum limit of {1} characters.", "DisplayName", 512), ClientsEventLogConstants.Tuple_ThemeInfoAttributeExceededMaximumLength, new object[]
								{
									"themeinfo.xml",
									folderName,
									"DisplayName",
									512,
									xmlTextReader.LineNumber.ToString(CultureInfo.InvariantCulture),
									xmlTextReader.LinePosition.ToString(CultureInfo.InvariantCulture)
								});
							}
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
			throw new OwaThemeManagerInitializationException(string.Format(CultureInfo.InvariantCulture, "Invalid theme info file in folder '{0}'. Line {1} Position {2}.{3}", new object[]
			{
				folderName,
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(description != null) ? (" " + description) : string.Empty
			}), null, null);
		}

		private void Load(string folderPath)
		{
			ExTraceGlobals.ThemesCallTracer.TraceDebug<string>(0L, "Theme.Load. folderPath={0}", folderPath);
			this.folderName = Path.GetFileNameWithoutExtension(folderPath);
			this.displayName = this.folderName;
			string text = Path.Combine(folderPath, "themeinfo.xml");
			string text2 = null;
			int maxValue = int.MaxValue;
			if (File.Exists(text))
			{
				Theme.ParseThemeInfoFile(text, this.folderName, out text2, out maxValue);
			}
			if (text2 != null)
			{
				this.displayName = text2;
			}
			else
			{
				this.displayName = this.folderName;
			}
			this.sortOrder = maxValue;
			if (string.Equals(this.folderName, ThemeManager.BaseThemeFolderName, StringComparison.OrdinalIgnoreCase))
			{
				this.isBase = true;
			}
		}

		private const string ThemeInfoFileName = "themeinfo.xml";

		private const string ThemeDisplayNameAttribute = "DisplayName";

		private const string ThemeSortOrderAttribute = "SortOrder";

		private const string ThemeRootElement = "Theme";

		private const int MaxThemeDisplayNameLength = 512;

		private bool isBase;

		private string folderName;

		private string displayName;

		private int sortOrder = int.MaxValue;
	}
}
