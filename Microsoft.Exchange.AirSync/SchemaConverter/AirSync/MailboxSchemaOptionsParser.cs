using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class MailboxSchemaOptionsParser
	{
		public MIMESupportValue MIMESupport
		{
			get
			{
				return this.mimeSupport;
			}
		}

		public bool RightsManagementSupport
		{
			get
			{
				return this.rightsManagementSupport;
			}
		}

		internal List<BodyPreference> BodyPreferences
		{
			get
			{
				return this.bodyPreferences;
			}
		}

		internal List<BodyPartPreference> BodyPartPreferences
		{
			get
			{
				return this.bodyPartPreferences;
			}
		}

		internal Dictionary<string, bool> SchemaTags
		{
			get
			{
				return this.schemaTags;
			}
		}

		internal bool HasBodyPreferences
		{
			get
			{
				return this.BodyPreferences != null && this.BodyPreferences.Count > 0;
			}
		}

		internal bool HasBodyPartPreferences
		{
			get
			{
				return this.BodyPartPreferences != null && this.BodyPartPreferences.Count > 0;
			}
		}

		public IDictionary BuildOptionsCollection(string deviceType)
		{
			return this.PopulateOptionsCollection(deviceType, new Hashtable(8));
		}

		public void Parse(XmlNode parentNode)
		{
			bool[] bodyTypeSet = new bool[256];
			bool[] bodyTypeSet2 = new bool[256];
			foreach (object obj in parentNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string namespaceURI;
				if ("Schema" == xmlNode.Name)
				{
					this.ParseSchemaTags(xmlNode);
				}
				else if ((namespaceURI = xmlNode.NamespaceURI) != null)
				{
					string localName3;
					if (!(namespaceURI == "AirSync:"))
					{
						string localName2;
						if (!(namespaceURI == "AirSyncBase:"))
						{
							if (namespaceURI == "RightsManagement:")
							{
								string localName;
								if ((localName = xmlNode.LocalName) != null && localName == "RightsManagementSupport")
								{
									string innerText;
									if ((innerText = xmlNode.InnerText) != null)
									{
										if (innerText == "0")
										{
											this.rightsManagementSupport = false;
											continue;
										}
										if (innerText == "1")
										{
											this.rightsManagementSupport = true;
											continue;
										}
									}
									throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
									{
										ErrorStringForProtocolLogger = "InvalidValueRightsManagementSupport"
									};
								}
							}
						}
						else if ((localName2 = xmlNode.LocalName) != null)
						{
							if (!(localName2 == "BodyPreference"))
							{
								if (localName2 == "BodyPartPreference")
								{
									BodyPartPreference bodyPartPreference = MailboxSchemaOptionsParser.ParsePreference<BodyPartPreference>(xmlNode, bodyTypeSet2, () => new BodyPartPreference());
									if (bodyPartPreference != null)
									{
										this.bodyPartPreferences.Add(bodyPartPreference);
									}
								}
							}
							else
							{
								BodyPreference bodyPreference = MailboxSchemaOptionsParser.ParsePreference<BodyPreference>(xmlNode, bodyTypeSet, () => new BodyPreference());
								if (bodyPreference != null)
								{
									this.bodyPreferences.Add(bodyPreference);
								}
							}
						}
					}
					else if ((localName3 = xmlNode.LocalName) != null)
					{
						if (!(localName3 == "Truncation"))
						{
							if (!(localName3 == "RTFTruncation"))
							{
								if (!(localName3 == "MIMETruncation"))
								{
									if (localName3 == "MIMESupport")
									{
										this.ParseMIMESupport(xmlNode.InnerText);
									}
								}
								else
								{
									this.ParseMIMETruncation(xmlNode.InnerText);
								}
							}
							else
							{
								this.ParseRtfTruncationSetting(xmlNode.InnerText);
							}
						}
						else
						{
							this.ParseTextTruncationSetting(xmlNode.InnerText);
						}
					}
				}
			}
		}

		public IDictionary PopulateOptionsCollection(string deviceType, IDictionary options)
		{
			if ("PocketPC" == deviceType)
			{
				options["ClientSupportsRtf"] = true;
				if (this.truncateRtfBody)
				{
					options["MaxRtfBodySize"] = this.maxRtfBodySize;
				}
			}
			if (this.truncateTextBody)
			{
				options["MaxTextBodySize"] = this.maxTextBodySize;
			}
			options["BodyPreference"] = this.bodyPreferences;
			options["BodyPartPreference"] = this.bodyPartPreferences;
			options["MIMESupport"] = this.mimeSupport;
			if (this.mimeTruncation != null)
			{
				options["MIMETruncation"] = this.mimeTruncation.Value;
			}
			else
			{
				foreach (BodyPreference bodyPreference in this.bodyPreferences)
				{
					if (bodyPreference.Type == BodyType.Mime)
					{
						options["MIMETruncation"] = (int)bodyPreference.TruncationSize;
					}
				}
			}
			options["RightsManagementSupport"] = this.rightsManagementSupport;
			return options;
		}

		private static T ParsePreference<T>(XmlNode prefNode, bool[] bodyTypeSet, Func<T> creator) where T : BodyPreference
		{
			T result = creator();
			foreach (object obj in prefNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string localName;
				if ((localName = xmlNode.LocalName) != null)
				{
					if (!(localName == "Type"))
					{
						if (!(localName == "TruncationSize"))
						{
							if (!(localName == "AllOrNone"))
							{
								if (localName == "Restriction")
								{
									continue;
								}
								if (localName == "Preview")
								{
									int preview;
									if (!int.TryParse(xmlNode.InnerText, out preview))
									{
										throw new ConversionException(4, "Invalid Preview");
									}
									result.Preview = preview;
									continue;
								}
							}
							else
							{
								if (xmlNode.InnerText.Equals("1"))
								{
									result.AllOrNone = true;
									continue;
								}
								continue;
							}
						}
						else
						{
							long truncationSize;
							if (!long.TryParse(xmlNode.InnerText, out truncationSize))
							{
								throw new ConversionException(4, "Invalid Truncation Size");
							}
							result.TruncationSize = truncationSize;
							continue;
						}
					}
					else
					{
						byte b;
						if (!byte.TryParse(xmlNode.InnerText, out b))
						{
							throw new ConversionException(4, "Invalid Body Type");
						}
						if (bodyTypeSet[(int)b])
						{
							throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
							{
								ErrorStringForProtocolLogger = "InvalidNode(Type)InMbxSchemaOptionsParser"
							};
						}
						bodyTypeSet[(int)b] = true;
						switch (b)
						{
						case 1:
							result.Type = BodyType.PlainText;
							continue;
						case 2:
							result.Type = BodyType.Html;
							continue;
						case 3:
							result.Type = BodyType.Rtf;
							continue;
						case 4:
							result.Type = BodyType.Mime;
							continue;
						default:
							return default(T);
						}
					}
				}
				throw new ConversionException(4, "Invalid Element");
			}
			return result;
		}

		private void ParseMIMESupport(string nodeText)
		{
			int num = -1;
			if (!int.TryParse(nodeText, out num))
			{
				throw new ConversionException(4, "MIMESupport is in bad format: " + nodeText);
			}
			switch (num)
			{
			case 0:
				this.mimeSupport = MIMESupportValue.NeverSendMimeData;
				return;
			case 1:
				this.mimeSupport = MIMESupportValue.SendMimeDataForSmimeMessagesOnly;
				return;
			case 2:
				this.mimeSupport = MIMESupportValue.SendMimeDataForAllMessages;
				return;
			default:
				throw new ConversionException(4, "MIMESupport is in bad format: " + nodeText);
			}
		}

		private void ParseMIMETruncation(string nodeText)
		{
			int num = -1;
			if (!int.TryParse(nodeText, out num))
			{
				throw new ConversionException(4, "MIMETruncation is in bad format: " + nodeText);
			}
			switch (num)
			{
			case 0:
				this.mimeTruncation = new int?(0);
				return;
			case 1:
				this.mimeTruncation = new int?(4096);
				return;
			case 2:
				this.mimeTruncation = new int?(5120);
				return;
			case 3:
				this.mimeTruncation = new int?(7168);
				return;
			case 4:
				this.mimeTruncation = new int?(10240);
				return;
			case 5:
				this.mimeTruncation = new int?(20480);
				return;
			case 6:
				this.mimeTruncation = new int?(51200);
				return;
			case 7:
				this.mimeTruncation = new int?(102400);
				return;
			case 8:
				this.mimeTruncation = null;
				return;
			default:
				throw new ConversionException(4, "MIMETruncation is in bad format: " + nodeText);
			}
		}

		private void ParseRtfTruncationSetting(string rtfTruncationSetting)
		{
			this.truncateRtfBody = true;
			int num = 0;
			if (!int.TryParse(rtfTruncationSetting, out num))
			{
				throw new ConversionException(4, "RTFTruncation bad format");
			}
			switch (num)
			{
			case 0:
				this.maxRtfBodySize = 0;
				return;
			case 1:
				this.maxRtfBodySize = 512;
				return;
			case 2:
				this.maxRtfBodySize = 1024;
				return;
			case 3:
				this.maxRtfBodySize = 2048;
				return;
			case 4:
				this.maxRtfBodySize = 5120;
				return;
			case 5:
				this.maxRtfBodySize = 10240;
				return;
			case 6:
				this.maxRtfBodySize = 20480;
				return;
			case 7:
				this.maxRtfBodySize = 51200;
				return;
			case 8:
				this.maxRtfBodySize = 102400;
				return;
			case 9:
				this.truncateTextBody = false;
				return;
			default:
				throw new ConversionException(4, "RTFTruncation, bad value: " + num.ToString(CultureInfo.InvariantCulture));
			}
		}

		private void ParseSchemaTags(XmlNode schemaNode)
		{
			foreach (object obj in schemaNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (this.schemaTags == null)
				{
					this.schemaTags = new Dictionary<string, bool>(schemaNode.ChildNodes.Count);
				}
				this.schemaTags.Add(xmlNode.NamespaceURI + xmlNode.LocalName, true);
			}
		}

		private void ParseTextTruncationSetting(string textTruncationSetting)
		{
			this.truncateTextBody = true;
			int num = 0;
			if (!int.TryParse(textTruncationSetting, out num))
			{
				throw new ConversionException(4, "Truncation bad format");
			}
			switch (num)
			{
			case 0:
				this.maxTextBodySize = 0;
				return;
			case 1:
				this.maxTextBodySize = 512;
				return;
			case 2:
				this.maxTextBodySize = 1024;
				return;
			case 3:
				this.maxTextBodySize = 2048;
				return;
			case 4:
				this.maxTextBodySize = 5120;
				return;
			case 5:
				this.maxTextBodySize = 10240;
				return;
			case 6:
				this.maxTextBodySize = 20480;
				return;
			case 7:
				this.maxTextBodySize = 51200;
				return;
			case 8:
				this.maxTextBodySize = 102400;
				return;
			case 9:
				this.truncateTextBody = false;
				return;
			default:
				throw new ConversionException(4, "Truncation, bad value: " + num.ToString(CultureInfo.InvariantCulture));
			}
		}

		private List<BodyPreference> bodyPreferences = new List<BodyPreference>(5);

		private List<BodyPartPreference> bodyPartPreferences = new List<BodyPartPreference>(1);

		private Dictionary<string, bool> schemaTags;

		private int maxRtfBodySize;

		private int maxTextBodySize;

		private MIMESupportValue mimeSupport;

		private int? mimeTruncation;

		private bool truncateRtfBody;

		private bool truncateTextBody;

		private bool rightsManagementSupport;

		private enum AirSyncV25RtfTruncationSetting
		{
			Invalid = -1,
			TruncAlways,
			TruncHalfk,
			Trunc1k,
			Trunc2k,
			Trunc5k,
			Trunc10k,
			Trunc20k,
			Trunc50k,
			Trunc100k,
			TruncNone
		}

		private enum AirSyncV25TextTruncationSetting
		{
			Invalid = -1,
			TruncAlways,
			TruncHalfk,
			Trunc1k,
			Trunc2k,
			Trunc5k,
			Trunc10k,
			Trunc20k,
			Trunc50k,
			Trunc100k,
			TruncNone
		}
	}
}
