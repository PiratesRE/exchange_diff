using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeLimits
	{
		internal MimeLimits(int partDepth, int embeddedDepth, int size, int headerBytes, int parts, int headers, int addressItemsPerHeader, int textValueBytesPerValue, int parametersPerHeader, int encodedWordLength)
		{
			this.partDepth = partDepth;
			this.embeddedDepth = embeddedDepth;
			this.size = size;
			this.headerBytes = headerBytes;
			this.parts = parts;
			this.headers = headers;
			this.addressItemsPerHeader = addressItemsPerHeader;
			this.textValueBytesPerValue = textValueBytesPerValue;
			this.parametersPerHeader = parametersPerHeader;
			this.encodedWordLength = encodedWordLength;
		}

		private MimeLimits()
		{
		}

		public static MimeLimits Default
		{
			get
			{
				return MimeLimits.GetDefaultMimeDocumentLimits();
			}
		}

		public static MimeLimits Unlimited
		{
			get
			{
				return MimeLimits.unlimitedLimits;
			}
		}

		public int MaxPartDepth
		{
			get
			{
				return this.partDepth;
			}
		}

		public int MaxEmbeddedDepth
		{
			get
			{
				return this.embeddedDepth;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.size;
			}
		}

		public int MaxHeaderBytes
		{
			get
			{
				return this.headerBytes;
			}
		}

		public int MaxParts
		{
			get
			{
				return this.parts;
			}
		}

		public int MaxHeaders
		{
			get
			{
				return this.headers;
			}
		}

		public int MaxAddressItemsPerHeader
		{
			get
			{
				return this.addressItemsPerHeader;
			}
		}

		public int MaxTextValueBytesPerValue
		{
			get
			{
				return this.textValueBytesPerValue;
			}
		}

		public int MaxParametersPerHeader
		{
			get
			{
				return this.parametersPerHeader;
			}
		}

		internal int MaxEncodedWordLength
		{
			get
			{
				return this.encodedWordLength;
			}
		}

		internal static void RefreshConfiguration()
		{
			MimeLimits.defaultLimits = null;
		}

		private static MimeLimits GetDefaultMimeDocumentLimits()
		{
			if (MimeLimits.defaultLimits == null)
			{
				lock (MimeLimits.configurationLockObject)
				{
					if (MimeLimits.defaultLimits == null)
					{
						IList<CtsConfigurationSetting> configuration = ApplicationServices.Provider.GetConfiguration("MimeLimits");
						int defaultValue = 10;
						int defaultValue2 = 100;
						int defaultValue3 = int.MaxValue;
						int defaultValue4 = int.MaxValue;
						int defaultValue5 = 10000;
						int defaultValue6 = 100000;
						int defaultValue7 = int.MaxValue;
						int defaultValue8 = 32768;
						int defaultValue9 = int.MaxValue;
						int num = 256;
						foreach (CtsConfigurationSetting ctsConfigurationSetting in configuration)
						{
							string key;
							switch (key = ctsConfigurationSetting.Name.ToLower())
							{
							case "maximumpartdepth":
								defaultValue = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue, 5, false);
								break;
							case "maximumembeddeddepth":
								defaultValue2 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue2, 10, false);
								break;
							case "maximumsize":
								defaultValue3 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue3, 100, true);
								break;
							case "maximumtotalheaderssize":
								defaultValue4 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue4, 100, true);
								break;
							case "maximumparts":
								defaultValue5 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue5, 100, false);
								break;
							case "maximumheaders":
								defaultValue6 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue6, 100, false);
								break;
							case "maximumaddressitemsperheader":
								defaultValue7 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue7, 100, false);
								break;
							case "maximumparametersperheader":
								defaultValue9 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue9, 10, false);
								break;
							case "maximumtextvaluesize":
								defaultValue8 = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, defaultValue8, 10, true);
								break;
							}
						}
						MimeLimits.defaultLimits = new MimeLimits(defaultValue, defaultValue2, defaultValue3, defaultValue4, defaultValue5, defaultValue6, defaultValue7, defaultValue8, defaultValue9, num);
					}
				}
			}
			return MimeLimits.defaultLimits;
		}

		private static object configurationLockObject = new object();

		private static volatile MimeLimits defaultLimits;

		private static MimeLimits unlimitedLimits = new MimeLimits();

		private int partDepth = int.MaxValue;

		private int embeddedDepth = int.MaxValue;

		private int size = int.MaxValue;

		private int headerBytes = int.MaxValue;

		private int parts = int.MaxValue;

		private int headers = int.MaxValue;

		private int addressItemsPerHeader = int.MaxValue;

		private int textValueBytesPerValue = int.MaxValue;

		private int parametersPerHeader = int.MaxValue;

		private int encodedWordLength;
	}
}
