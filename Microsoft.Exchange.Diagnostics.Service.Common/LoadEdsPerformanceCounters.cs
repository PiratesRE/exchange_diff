using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	internal static class LoadEdsPerformanceCounters
	{
		internal static void RegisterCounters(string installationPath)
		{
			string filename = Path.Combine(installationPath, "Microsoft.Exchange.Diagnostics.Service.Common.EdsPerformanceCounters.xml");
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.Load(filename);
			string text = LoadEdsPerformanceCounters.ReadStringElement(safeXmlDocument, "Category/Name");
			Logger.LogInformationMessage("Found performance counter category {0}", new object[]
			{
				text
			});
			if (!LoadEdsPerformanceCounters.SafeExists(text))
			{
				Logger.LogInformationMessage("Registering EDS performance counters.", new object[0]);
				CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
				XmlNodeList xmlNodeList = safeXmlDocument.SelectNodes("Category/Counters/Counter");
				using (xmlNodeList)
				{
					for (int i = 0; i < xmlNodeList.Count; i++)
					{
						string text2 = LoadEdsPerformanceCounters.ReadStringElement(xmlNodeList[i], "Name");
						PerformanceCounterType counterType = (PerformanceCounterType)Enum.Parse(typeof(PerformanceCounterType), LoadEdsPerformanceCounters.ReadStringElement(xmlNodeList[i], "Type"), true);
						counterCreationDataCollection.Add(new CounterCreationData(text2, string.Empty, counterType));
						Logger.LogInformationMessage("Loaded counter {0}", new object[]
						{
							text2
						});
					}
				}
				LoadEdsPerformanceCounters.SafeCreate(text, counterCreationDataCollection);
				return;
			}
			Logger.LogInformationMessage("Counters are already registered.", new object[0]);
		}

		private static void LoadHelp()
		{
		}

		private static string ReadStringElement(XmlNode node, string xpath)
		{
			XmlNodeList xmlNodeList = node.SelectNodes(xpath);
			string innerText;
			using (xmlNodeList)
			{
				if (xmlNodeList.Count != 1)
				{
					throw new ArgumentException("Invalid performance counter files");
				}
				innerText = xmlNodeList[0].InnerText;
			}
			return innerText;
		}

		private static bool SafeExists(string categoryName)
		{
			try
			{
				return PerformanceCounterCategory.Exists(categoryName);
			}
			catch (InvalidOperationException ex)
			{
				Logger.LogErrorMessage("Category existence check failed due to exception: '{0}'", new object[]
				{
					ex
				});
			}
			return true;
		}

		private static void SafeCreate(string categoryName, CounterCreationDataCollection edsCounterCollection)
		{
			try
			{
				PerformanceCounterCategory.Create(categoryName, string.Empty, PerformanceCounterCategoryType.SingleInstance, edsCounterCollection);
			}
			catch (InvalidOperationException ex)
			{
				Logger.LogErrorMessage("Counters failed to register due to exception: '{0}'", new object[]
				{
					ex
				});
			}
		}

		private const string CounterTag = "Category/Counters/Counter";

		private const string CounterNameTag = "Name";

		private const string CounterTypeTag = "Type";

		private const string CategoryNameTag = "Category/Name";

		private const string CounterXmlFileName = "Microsoft.Exchange.Diagnostics.Service.Common.EdsPerformanceCounters.xml";
	}
}
