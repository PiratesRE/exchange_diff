using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.DataMining;
using Microsoft.Win32;

namespace Microsoft.Exchange.Inference.Common
{
	public static class HashProvider
	{
		internal static Hookable<IHashProvider> HookableDefaultProvider
		{
			get
			{
				return HashProvider.hookableDefaultProvider;
			}
		}

		internal static IHashProvider Default
		{
			get
			{
				return HashProvider.hookableDefaultProvider.Value;
			}
		}

		internal static IHashProvider MD5
		{
			get
			{
				return HashProvider.MD5HashProvider.Instance;
			}
		}

		internal static IHashProvider Exchange
		{
			get
			{
				return HashProvider.ExchangeDataMiningHashProvider.Instance;
			}
		}

		public static string HashString(string inputString, IHashProvider hashProvider, bool isCaseSensitive)
		{
			string result = null;
			if (inputString != null)
			{
				if (hashProvider != null)
				{
					if (!isCaseSensitive)
					{
						inputString = inputString.ToUpperInvariant();
					}
					return hashProvider.HashString(inputString);
				}
				result = inputString;
			}
			return result;
		}

		private static Hookable<IHashProvider> hookableDefaultProvider = Hookable<IHashProvider>.Create(true, HashProvider.Exchange);

		internal sealed class MD5HashProvider : IHashProvider
		{
			private MD5HashProvider()
			{
				this.hashAlgorithm = System.Security.Cryptography.MD5.Create();
			}

			~MD5HashProvider()
			{
				if (this.hashAlgorithm != null)
				{
					this.hashAlgorithm.Dispose();
				}
			}

			public static HashProvider.MD5HashProvider Instance
			{
				get
				{
					return HashProvider.MD5HashProvider.instance;
				}
			}

			public bool IsInitialized
			{
				get
				{
					return this.hashAlgorithm != null;
				}
			}

			internal MD5 HashAlgorithm
			{
				get
				{
					return this.hashAlgorithm;
				}
			}

			public bool Initialize()
			{
				return this.IsInitialized;
			}

			public string HashString(string input)
			{
				if (input == null)
				{
					return null;
				}
				byte[] array = this.hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				return stringBuilder.ToString();
			}

			private static HashProvider.MD5HashProvider instance = new HashProvider.MD5HashProvider();

			private readonly MD5 hashAlgorithm;
		}

		internal sealed class ExchangeDataMiningHashProvider : IHashProvider
		{
			private ExchangeDataMiningHashProvider()
			{
			}

			~ExchangeDataMiningHashProvider()
			{
				if (this.obfuscationProvider != null)
				{
					this.obfuscationProvider.Dispose();
					this.obfuscationProvider = null;
				}
			}

			public static HashProvider.ExchangeDataMiningHashProvider Instance
			{
				get
				{
					return HashProvider.ExchangeDataMiningHashProvider.instance;
				}
			}

			public bool IsInitialized
			{
				get
				{
					return this.obfuscationProvider != null;
				}
			}

			internal static Hookable<Func<string>> HookableGetExchangeUploaderConfigPathDelegate
			{
				get
				{
					return HashProvider.ExchangeDataMiningHashProvider.hookableGetExchangeUploaderConfigPathDelegate;
				}
			}

			internal static Hookable<Func<string>> HookableGetOfficeDataLoaderConfigPathDelegate
			{
				get
				{
					return HashProvider.ExchangeDataMiningHashProvider.hookableGetOfficeDataLoaderConfigPathDelegate;
				}
			}

			public bool Initialize()
			{
				if (this.IsInitialized)
				{
					return true;
				}
				bool result;
				lock (HashProvider.ExchangeDataMiningHashProvider.singletonLock)
				{
					if (this.IsInitialized)
					{
						result = true;
					}
					else
					{
						this.obfuscationProvider = HashProvider.ExchangeDataMiningHashProvider.TryCreateObfuscationProvider();
						result = (this.obfuscationProvider != null);
					}
				}
				return result;
			}

			public string HashString(string input)
			{
				if (!this.Initialize())
				{
					throw new InvalidOperationException("Obfuscation provider could not be initialized.");
				}
				if (input == null)
				{
					return input;
				}
				return this.obfuscationProvider.ProtectGenericData(input, 5);
			}

			internal static KeyConfiguration GetExchangeUploaderKeyConfiguration()
			{
				KeyConfiguration result = null;
				string text = null;
				string text2 = HashProvider.ExchangeDataMiningHashProvider.hookableGetExchangeUploaderConfigPathDelegate.Value();
				if (string.IsNullOrEmpty(text2) || !File.Exists(text = Path.Combine(text2, "Uploader.xml")))
				{
					text2 = HashProvider.ExchangeDataMiningHashProvider.hookableGetOfficeDataLoaderConfigPathDelegate.Value();
					if (!string.IsNullOrEmpty(text2))
					{
						text = Path.Combine(text2, "Uploader.xml");
					}
				}
				if (File.Exists(text))
				{
					XElement xelement = XElement.Load(text);
					XElement xelement2 = xelement.Element("KeyConfiguration");
					result = ConfigurationParser.ParseKeyConfiguration(xelement2);
				}
				return result;
			}

			private static ObfuscationProvider TryCreateObfuscationProvider()
			{
				ObfuscationProvider result = null;
				KeyConfiguration exchangeUploaderKeyConfiguration = HashProvider.ExchangeDataMiningHashProvider.GetExchangeUploaderKeyConfiguration();
				if (exchangeUploaderKeyConfiguration != null && exchangeUploaderKeyConfiguration.KeySettings != null && exchangeUploaderKeyConfiguration.KeySettings.Length > 0)
				{
					result = ObfuscationProvider.CreateHashOnlyObfuscationProvider(exchangeUploaderKeyConfiguration, null);
				}
				return result;
			}

			private static string GetExchangeUploaderConfigurationPath()
			{
				string result = null;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format("System\\CurrentControlSet\\Services\\{0}", HashProvider.ExchangeDataMiningHashProvider.ExchangeFileUploaderServiceName)))
				{
					if (registryKey != null)
					{
						string executablePath = InferenceCommonUtility.GetExecutablePath(registryKey.GetValue("ImagePath") as string);
						if (!string.IsNullOrEmpty(executablePath))
						{
							result = Path.GetDirectoryName(executablePath);
						}
					}
				}
				return result;
			}

			private static string GetOfficeDataLoaderConfigurationPath()
			{
				string result = null;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format("System\\CurrentControlSet\\Services\\{0}", HashProvider.ExchangeDataMiningHashProvider.OfficeDataLoaderServiceName)))
				{
					if (registryKey != null)
					{
						string executablePath = InferenceCommonUtility.GetExecutablePath(registryKey.GetValue("ImagePath") as string);
						if (!string.IsNullOrEmpty(executablePath))
						{
							result = Path.GetDirectoryName(executablePath);
						}
					}
				}
				return result;
			}

			private static readonly object singletonLock = new object();

			private static readonly HashProvider.ExchangeDataMiningHashProvider instance = new HashProvider.ExchangeDataMiningHashProvider();

			private static readonly Hookable<Func<string>> hookableGetExchangeUploaderConfigPathDelegate = Hookable<Func<string>>.Create(true, new Func<string>(HashProvider.ExchangeDataMiningHashProvider.GetExchangeUploaderConfigurationPath));

			private static readonly Hookable<Func<string>> hookableGetOfficeDataLoaderConfigPathDelegate = Hookable<Func<string>>.Create(true, new Func<string>(HashProvider.ExchangeDataMiningHashProvider.GetOfficeDataLoaderConfigurationPath));

			private static readonly string ExchangeFileUploaderServiceName = "MSExchangeFileUpload";

			private static readonly string OfficeDataLoaderServiceName = "MSOfficeDataLoader";

			private ObfuscationProvider obfuscationProvider;
		}
	}
}
