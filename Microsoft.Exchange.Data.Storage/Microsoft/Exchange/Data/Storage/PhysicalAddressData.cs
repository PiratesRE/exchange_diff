using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PhysicalAddressData
	{
		internal PhysicalAddressData()
		{
			this.PopulateMaps();
		}

		internal Dictionary<string, FormattedSentence> FormatMap
		{
			get
			{
				return this.formatMap;
			}
		}

		internal Dictionary<string, string> RegionMap
		{
			get
			{
				return this.regionMap;
			}
		}

		private static string FixupNewlinesInFormat(string s)
		{
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '\\')
				{
					if (i + 1 < s.Length && s[i + 1] == 'r')
					{
						stringBuilder.Append('\r');
						i++;
					}
					else if (i + 1 < s.Length && s[i + 1] == 'n')
					{
						stringBuilder.Append('\n');
						i++;
					}
					else
					{
						stringBuilder.Append(s[i]);
					}
				}
				else
				{
					stringBuilder.Append(s[i]);
				}
			}
			return stringBuilder.ToString();
		}

		private void PopulateMaps()
		{
			try
			{
				Stream manifestResourceStream;
				Stream input = manifestResourceStream = base.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("AddressFormats.xml");
				Dictionary<string, FormattedSentence> dictionary;
				Dictionary<string, string> dictionary2;
				try
				{
					XmlReader xmlReader2;
					XmlReader xmlReader = xmlReader2 = XmlReader.Create(input);
					try
					{
						dictionary = new Dictionary<string, FormattedSentence>(200, StringComparer.OrdinalIgnoreCase);
						dictionary2 = new Dictionary<string, string>(4000, StringComparer.OrdinalIgnoreCase);
						string text = null;
						bool flag = false;
						xmlReader.Read();
						while (!xmlReader.EOF)
						{
							if (xmlReader.NodeType != XmlNodeType.Element)
							{
								xmlReader.Read();
							}
							else
							{
								if (string.Compare(xmlReader.LocalName, "Format", StringComparison.OrdinalIgnoreCase) == 0)
								{
									text = xmlReader.GetAttribute("region", string.Empty);
									if (xmlReader.AttributeCount != 1 || text == null || text.Trim() == string.Empty)
									{
										throw new InvalidOperationException("Bogus Format element (no region code) in RegionToFormat.xml.");
									}
									text = Util.InternString(text.Trim().ToUpperInvariant());
									string text2 = xmlReader.ReadElementContentAsString();
									if (text2 == null || text2.Trim() == string.Empty)
									{
										throw new InvalidOperationException(string.Format("Bogus FormattedSentence in RegionToFormat.xml: {0}", text));
									}
									text2 = text2.Trim();
									FormattedSentence value;
									try
									{
										string format = PhysicalAddressData.FixupNewlinesInFormat(text2);
										value = new FormattedSentence(format);
									}
									catch (FormatException innerException)
									{
										throw new InvalidOperationException(string.Format("Bogus FormattedSentence in RegionToFormat.xml: {0}: {1}", text, text2), innerException);
									}
									catch (NotSupportedException innerException2)
									{
										throw new InvalidOperationException(string.Format("Bogus FormattedSentence in RegionToFormat.xml: {0}: {1}", text, text2), innerException2);
									}
									FormattedSentence formattedSentence;
									if (dictionary.TryGetValue(text, out formattedSentence))
									{
										throw new InvalidOperationException(string.Format("Duplicate region in RegionToFormat.xml: {0}.", text));
									}
									dictionary.Add(text, value);
								}
								else if (string.Compare(xmlReader.LocalName, "Region", StringComparison.OrdinalIgnoreCase) == 0)
								{
									text = xmlReader.GetAttribute("code", string.Empty);
									if (xmlReader.AttributeCount != 1 || text == null || text.Trim() == string.Empty)
									{
										throw new InvalidOperationException("Bogus Region element (no code) in CountryToRegion.xml.");
									}
									text = text.Trim().ToUpperInvariant();
									FormattedSentence formattedSentence2 = null;
									flag = !dictionary.TryGetValue(text, out formattedSentence2);
								}
								else if (string.Compare(xmlReader.LocalName, "Name", StringComparison.OrdinalIgnoreCase) == 0 && !flag)
								{
									if (string.IsNullOrEmpty(text))
									{
										throw new InvalidOperationException("No region defined for Name element in CountryToRegion.xml.");
									}
									string text3 = xmlReader.ReadElementContentAsString();
									if (text3 == null || text3.Trim() == string.Empty)
									{
										throw new InvalidOperationException(string.Format("Bogus Name element (empty) in CountryToRegion.xml: {0}.", text));
									}
									string[] array = text3.Trim().Split(PhysicalAddressData.CountryDelimiter, StringSplitOptions.RemoveEmptyEntries);
									if (array.Length == 0)
									{
										throw new InvalidOperationException(string.Format("Bogus Name element (nearly empty) in CountryToRegion.xml: {0}.", text));
									}
									foreach (string text4 in array)
									{
										string key = Util.InternString(text4.Trim());
										string text5;
										if (!dictionary2.TryGetValue(key, out text5))
										{
											dictionary2.Add(key, text);
										}
									}
								}
								xmlReader.Read();
							}
						}
					}
					finally
					{
						if (xmlReader2 != null)
						{
							((IDisposable)xmlReader2).Dispose();
						}
					}
				}
				finally
				{
					if (manifestResourceStream != null)
					{
						((IDisposable)manifestResourceStream).Dispose();
					}
				}
				this.formatMap = dictionary;
				this.regionMap = dictionary2;
			}
			catch (XmlException innerException3)
			{
				throw new InvalidOperationException("XmlException in RegionToFormat.xml.", innerException3);
			}
		}

		private const string ResourceName = "AddressFormats.xml";

		private static readonly char[] CountryDelimiter = new char[]
		{
			';'
		};

		private Dictionary<string, FormattedSentence> formatMap;

		private Dictionary<string, string> regionMap;
	}
}
