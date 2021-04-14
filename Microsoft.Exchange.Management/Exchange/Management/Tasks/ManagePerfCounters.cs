using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManagePerfCounters : Task
	{
		[LocDescription(Strings.IDs.PerfCounterDefinitionFileNameDescription)]
		[Parameter(Mandatory = true)]
		public string DefinitionFileName
		{
			get
			{
				return (string)base.Fields["DefinitionFileName"];
			}
			set
			{
				base.Fields["DefinitionFileName"] = value;
			}
		}

		internal void CreatePerfCounterCategory()
		{
			int num = 0;
			int num2 = 15000;
			LoadUnloadPerfCounterLocalizedText loadUnloadPerfCounterLocalizedText = new LoadUnloadPerfCounterLocalizedText();
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.PerfCounterCategoryDefinition(this.CategoryName));
			if (!this.unmanagedCategory)
			{
				bool flag;
				do
				{
					Exception ex = null;
					try
					{
						PerformanceCounterCategory.Create(this.CategoryName, string.Empty, this.categoryType, this.counterDataCollection);
					}
					catch (InvalidOperationException ex2)
					{
						if (!PerformanceCounterCategory.Exists(this.CategoryName))
						{
							ex = ex2;
						}
					}
					catch (Win32Exception ex3)
					{
						ex = ex3;
					}
					if (ex == null)
					{
						break;
					}
					num++;
					flag = (num < 5);
					if (flag)
					{
						base.WriteVerbose(Strings.PerfCounterFileInUse(this.CategoryName, num2 / 1000, 5 - num));
						Thread.Sleep(num2);
					}
					else
					{
						base.WriteError(ex, ErrorCategory.InvalidData, null);
					}
				}
				while (flag);
				if (this.fileMappingSize > 0)
				{
					string name = string.Format(CultureInfo.InvariantCulture, "SYSTEM\\CurrentControlSet\\Services\\{0}\\Performance", new object[]
					{
						this.CategoryName
					});
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, true))
					{
						registryKey.SetValue("FileMappingSize", this.fileMappingSize);
					}
				}
				try
				{
					loadUnloadPerfCounterLocalizedText.UnloadLocalizedText(this.iniFileName, this.CategoryName);
					loadUnloadPerfCounterLocalizedText.LoadLocalizedText(this.iniFileName, this.CategoryName);
					goto IL_251;
				}
				catch (FileNotFoundException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidData, null);
					goto IL_251;
				}
				catch (TaskException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidData, null);
					goto IL_251;
				}
			}
			string subkey = string.Format(CultureInfo.InvariantCulture, "SYSTEM\\CurrentControlSet\\Services\\{0}\\Performance", new object[]
			{
				this.CategoryName
			});
			using (RegistryKey registryKey2 = Registry.LocalMachine.CreateSubKey(subkey))
			{
				if (this.extensionDllInfo.UseExpandedStringForLibrary)
				{
					registryKey2.SetValue("Library", this.extensionDllInfo.Library, RegistryValueKind.ExpandString);
				}
				else
				{
					registryKey2.SetValue("Library", this.extensionDllInfo.Library);
				}
				registryKey2.SetValue("Open", this.extensionDllInfo.Open);
				registryKey2.SetValue("Collect", this.extensionDllInfo.Collect);
				registryKey2.SetValue("Close", this.extensionDllInfo.Close);
			}
			try
			{
				loadUnloadPerfCounterLocalizedText.LoadLocalizedText(this.iniFileName, this.CategoryName);
			}
			catch (FileNotFoundException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidData, null);
			}
			catch (TaskException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidData, null);
			}
			IL_251:
			TaskLogger.LogExit();
		}

		internal void DeletePerfCounterCategory()
		{
			int num = 0;
			bool flag = false;
			int num2 = 15000;
			LoadUnloadPerfCounterLocalizedText loadUnloadPerfCounterLocalizedText = new LoadUnloadPerfCounterLocalizedText();
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.PerfCounterCategoryDefinition(this.CategoryName));
			if (!this.unmanagedCategory)
			{
				this.LogInstalledCategoryNames(Path.GetFullPath(this.iniFileName), "Rem");
				do
				{
					try
					{
						PerformanceCounterCategory.Delete(this.CategoryName);
					}
					catch (Win32Exception exception)
					{
						num++;
						flag = (num < 5);
						if (flag)
						{
							base.WriteVerbose(Strings.PerfCounterFileInUse(this.CategoryName, num2 / 1000, 5 - num));
							Thread.Sleep(num2);
						}
						else
						{
							base.WriteError(exception, ErrorCategory.InvalidData, null);
						}
					}
					catch (InvalidOperationException exception2)
					{
						if (PerformanceCounterCategory.Exists(this.CategoryName))
						{
							base.WriteError(exception2, ErrorCategory.InvalidData, null);
						}
						flag = false;
					}
				}
				while (flag);
			}
			else
			{
				try
				{
					loadUnloadPerfCounterLocalizedText.UnloadLocalizedText(this.iniFileName, this.CategoryName);
				}
				catch (Win32Exception exception3)
				{
					base.WriteError(exception3, ErrorCategory.InvalidData, null);
				}
				catch (TaskException exception4)
				{
					base.WriteError(exception4, ErrorCategory.InvalidData, null);
				}
				string subkey = string.Format(CultureInfo.InvariantCulture, "SYSTEM\\CurrentControlSet\\Services\\{0}", new object[]
				{
					this.CategoryName
				});
				Registry.LocalMachine.DeleteSubKeyTree(subkey);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			string definitionFileName = this.DefinitionFileName;
			if (!File.Exists(this.DefinitionFileName))
			{
				this.DefinitionFileName = Path.Combine(ConfigurationContext.Setup.SetupPerfPath, this.DefinitionFileName);
			}
			if (!File.Exists(this.DefinitionFileName))
			{
				this.DefinitionFileName = definitionFileName;
				this.DefinitionFileName = Path.Combine(ConfigurationContext.Setup.DataPath, this.DefinitionFileName);
			}
			if (!File.Exists(this.DefinitionFileName))
			{
				this.DefinitionFileName = definitionFileName;
				this.DefinitionFileName = Path.Combine(ConfigurationContext.Setup.BinPerfProcessorPath, this.DefinitionFileName);
			}
			if (!File.Exists(this.DefinitionFileName))
			{
				base.WriteError(new ArgumentException(Strings.ExceptionPerfCounterDefinitionFileNotFound(this.DefinitionFileName), "DefinitionFileName"), ErrorCategory.InvalidArgument, null);
			}
			this.ReadDefinitionFile();
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		private void ReadDefinitionFile()
		{
			TaskLogger.LogEnter();
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				base.WriteVerbose(Strings.PerfCounterProcessingFile(this.DefinitionFileName));
				xmlDocument.Load(this.DefinitionFileName);
				this.CategoryName = ManagePerfCounters.ReadStringElement(xmlDocument, "Category/Name");
				this.iniFileName = ManagePerfCounters.ReadStringElement(xmlDocument, "Category/IniFileName");
				string text = ManagePerfCounters.ReadStringElement(xmlDocument, "Category/CategoryType");
				if (!text.Equals("SingleInstance", StringComparison.OrdinalIgnoreCase))
				{
					this.categoryType = PerformanceCounterCategoryType.MultiInstance;
				}
				this.iniFileName = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(this.DefinitionFileName)), this.iniFileName);
				XmlNode xmlNode = xmlDocument.SelectSingleNode("Category/PerfmonExtensionDll");
				if (xmlNode != null)
				{
					this.unmanagedCategory = true;
					this.extensionDllInfo.Library = ManagePerfCounters.ReadStringElement(xmlNode, "Library");
					this.extensionDllInfo.Open = ManagePerfCounters.ReadStringElement(xmlNode, "Open");
					this.extensionDllInfo.Collect = ManagePerfCounters.ReadStringElement(xmlNode, "Collect");
					this.extensionDllInfo.Close = ManagePerfCounters.ReadStringElement(xmlNode, "Close");
					this.extensionDllInfo.UseExpandedStringForLibrary = false;
					string directoryName = Path.GetDirectoryName(Path.GetFullPath(this.DefinitionFileName));
					string text2 = Path.Combine(ConfigurationContext.Setup.BinPerfProcessorPath, this.extensionDllInfo.Library);
					if (File.Exists(text2))
					{
						text2 = string.Format("{0}\\Perf\\{1}\\{2}", ConfigurationContext.Setup.BinPath, "%PROCESSOR_ARCHITECTURE%", this.extensionDllInfo.Library);
						this.extensionDllInfo.UseExpandedStringForLibrary = true;
					}
					else
					{
						text2 = Path.Combine(directoryName, this.extensionDllInfo.Library);
						if (!File.Exists(text2))
						{
							text2 = Path.Combine(Path.Combine(directoryName, "..\\..\\bin"), this.extensionDllInfo.Library);
							if (!File.Exists(text2))
							{
								text2 = Path.Combine(Path.Combine(directoryName, "..\\..\\common"), this.extensionDllInfo.Library);
								if (!File.Exists(text2))
								{
									base.WriteError(new TaskException(Strings.ExceptionPerfCounterInvalidDefinitionFile), ErrorCategory.InvalidData, null);
								}
							}
						}
					}
					this.extensionDllInfo.Library = text2;
				}
				else
				{
					using (XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Category/Counters/Counter"))
					{
						if (xmlNodeList.Count == 0)
						{
							base.WriteError(new TaskException(Strings.ExceptionPerfCounterInvalidDefinitionFile), ErrorCategory.InvalidData, null);
						}
						this.counterDataCollection = new CounterCreationDataCollection();
						for (int i = 0; i < xmlNodeList.Count; i++)
						{
							string counterName = ManagePerfCounters.ReadStringElement(xmlNodeList[i], "Name");
							try
							{
								PerformanceCounterType counterType = (PerformanceCounterType)Enum.Parse(typeof(PerformanceCounterType), ManagePerfCounters.ReadStringElement(xmlNodeList[i], "Type"), true);
								this.counterDataCollection.Add(new CounterCreationData(counterName, string.Empty, counterType));
								base.WriteVerbose(Strings.PerfCounterDefinition(counterName, counterType));
							}
							catch (ArgumentException innerException)
							{
								base.WriteError(new TaskException(Strings.ExceptionPerfCounterInvalidDefinitionFile, innerException), ErrorCategory.InvalidData, null);
							}
						}
					}
				}
			}
			catch (XmlException innerException2)
			{
				base.WriteError(new TaskException(Strings.ExceptionPerfCounterInvalidDefinitionFile, innerException2), ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
		}

		private static string ReadStringElement(XmlNode node, string xpath)
		{
			string innerText;
			using (XmlNodeList xmlNodeList = node.SelectNodes(xpath))
			{
				if (xmlNodeList.Count != 1)
				{
					throw new TaskException(Strings.ExceptionPerfCounterInvalidDefinitionFile);
				}
				innerText = xmlNodeList[0].InnerText;
			}
			return innerText;
		}

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
		protected internal static extern int GetPrivateProfileString(string applicationName, string keyName, string defaultString, StringBuilder returnedString, int size, string fileName);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
		protected internal static extern int GetPrivateProfileStringSpecial(string applicationName, string keyName, string defaultString, SafeHandle lpBuffer, int size, string fileName);

		private void LogInstalledCategoryNames(string iniFileName, string action_prefix)
		{
			int num = 40000;
			SafeHGlobalHandle safeHGlobalHandle2;
			SafeHGlobalHandle safeHGlobalHandle = safeHGlobalHandle2 = new SafeHGlobalHandle(Marshal.AllocHGlobal(num * 2));
			try
			{
				int privateProfileStringSpecial = ManagePerfCounters.GetPrivateProfileStringSpecial("objects", null, null, safeHGlobalHandle, num, iniFileName);
				if (privateProfileStringSpecial > 0 && privateProfileStringSpecial < num - 2)
				{
					int num2 = 160000;
					SafeHGlobalHandle safeHGlobalHandle4;
					SafeHGlobalHandle safeHGlobalHandle3 = safeHGlobalHandle4 = new SafeHGlobalHandle(Marshal.AllocHGlobal(num2 * 2));
					try
					{
						int privateProfileStringSpecial2 = ManagePerfCounters.GetPrivateProfileStringSpecial("text", null, null, safeHGlobalHandle3, num2, iniFileName);
						if (privateProfileStringSpecial2 > 0 && privateProfileStringSpecial2 < num2 - 2)
						{
							StringBuilder stringBuilder = new StringBuilder(10000);
							string path = Path.Combine(ConfigurationContext.Setup.LoggingPath, "lodctr_backups\\InstalledPerfCategories.log");
							IntPtr ptr = safeHGlobalHandle.DangerousGetHandle();
							for (;;)
							{
								string text = Marshal.PtrToStringUni(ptr);
								if (string.IsNullOrEmpty(text))
								{
									break;
								}
								int num3 = text.LastIndexOf('_');
								if (num3 > 0)
								{
									int num4 = text.LastIndexOf('_', num3 - 1, num3);
									if (num4 > 0)
									{
										string strA = text.Substring(0, num4);
										IntPtr ptr2 = safeHGlobalHandle3.DangerousGetHandle();
										for (;;)
										{
											string text2 = Marshal.PtrToStringUni(ptr2);
											if (string.IsNullOrEmpty(text2))
											{
												break;
											}
											int num5 = text2.LastIndexOf('_');
											if (num5 > 0)
											{
												int num6 = text2.LastIndexOf('_', num5 - 1, num5);
												if (num6 > 0)
												{
													string strB = text2.Substring(0, num6);
													if (string.Compare(strA, strB) == 0 && !text2.EndsWith("_HELP") && ManagePerfCounters.GetPrivateProfileString("text", text2, null, stringBuilder, stringBuilder.Capacity, iniFileName) != 0)
													{
														File.AppendAllText(path, string.Format("{0}-{1}-{2}={3}\r\n", new object[]
														{
															action_prefix,
															Path.GetFileName(iniFileName),
															text2,
															stringBuilder
														}), new UnicodeEncoding());
													}
												}
											}
											ptr2 = (IntPtr)(ptr2.ToInt64() + (long)((text2.Length + 1) * 2));
										}
									}
								}
								ptr = (IntPtr)(ptr.ToInt64() + (long)((text.Length + 1) * 2));
							}
						}
					}
					finally
					{
						if (safeHGlobalHandle4 != null)
						{
							((IDisposable)safeHGlobalHandle4).Dispose();
						}
					}
				}
			}
			finally
			{
				if (safeHGlobalHandle2 != null)
				{
					((IDisposable)safeHGlobalHandle2).Dispose();
				}
			}
		}

		private const string CategoryTypeTag = "Category/CategoryType";

		private const string IniFileNameTag = "Category/IniFileName";

		private const string CategoryNameTag = "Category/Name";

		private const string SingleInstanceValue = "SingleInstance";

		private const string ExtensionDllTag = "Category/PerfmonExtensionDll";

		private const string CounterTag = "Category/Counters/Counter";

		private const string CounterNameTag = "Name";

		private const string CounterTypeTag = "Type";

		private const string DefinitionFileNameField = "DefinitionFileName";

		private const string LodctrBackupsSubdir = "lodctr_backups";

		internal string CategoryName;

		internal int fileMappingSize;

		private string iniFileName;

		private bool unmanagedCategory;

		private ManagePerfCounters.PerfmonExtensionDllInfo extensionDllInfo;

		private CounterCreationDataCollection counterDataCollection;

		private PerformanceCounterCategoryType categoryType;

		private struct PerfmonExtensionDllInfo
		{
			public string Library;

			public string Open;

			public string Collect;

			public string Close;

			public bool UseExpandedStringForLibrary;
		}
	}
}
