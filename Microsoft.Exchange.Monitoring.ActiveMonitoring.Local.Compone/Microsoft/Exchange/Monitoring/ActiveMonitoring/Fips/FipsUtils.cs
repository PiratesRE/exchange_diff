using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	public class FipsUtils
	{
		public static Collection<PSObject> RunFipsCmdlet<T>(string cmdletName, Dictionary<string, T> parameters)
		{
			if (string.IsNullOrEmpty(cmdletName))
			{
				throw new ArgumentNullException("cmdletName", "Cmdlet name cannot be null or empty.");
			}
			Collection<PSObject> result = null;
			try
			{
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				PSSnapInException ex = null;
				runspaceConfiguration.AddPSSnapIn("Microsoft.Forefront.Filtering.Management.PowerShell", out ex);
				if (ex != null)
				{
					WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.FIPSTracer, FipsUtils.traceContext, "Non-fatal error occurred while adding the powerShell snap-in - {0}. Warning: {1}", "Microsoft.Forefront.Filtering.Management.PowerShell", ex.Message, null, "RunFipsCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\Utils.cs", 57);
				}
				using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
				{
					runspace.Open();
					Pipeline pipeline = runspace.CreatePipeline();
					Command command = new Command(cmdletName);
					if (parameters != null)
					{
						foreach (KeyValuePair<string, T> keyValuePair in parameters)
						{
							command.Parameters.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					pipeline.Commands.Add(command);
					result = pipeline.Invoke();
				}
			}
			catch (Exception ex2)
			{
				WTFDiagnostics.TraceDebug<Exception, string>(ExTraceGlobals.FIPSTracer, FipsUtils.traceContext, "Failed to run the FIPS cmdlet {0}. Exception: {1}", ex2, ex2.Message, null, "RunFipsCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\Utils.cs", 86);
				throw;
			}
			return result;
		}

		private static XmlElement CheckXmlElement(XmlNode node, string elementName)
		{
			if (node == null)
			{
				throw new ArgumentException(string.Format("Work definition error - node is null.", elementName));
			}
			XmlElement xmlElement = node as XmlElement;
			if (xmlElement == null)
			{
				throw new ArgumentException(string.Format("Work definition error - node '{0}' is not an XML element.", elementName));
			}
			return xmlElement;
		}

		private static int GetInteger(string strValue, string elementName, int defaultValue, int minValue, int maxValue)
		{
			if (string.IsNullOrWhiteSpace(strValue))
			{
				return defaultValue;
			}
			int num;
			if (int.TryParse(strValue, out num) && num >= minValue && num <= maxValue)
			{
				return num;
			}
			throw new ArgumentException(string.Format("Work definition error - the attribute or element '{0}' has an invalid value '{1}' of type '{2}'. It should be greater than or equal to {3} and less than or equal to {4}", new object[]
			{
				elementName,
				strValue,
				typeof(int).Name,
				minValue,
				maxValue
			}));
		}

		public static FailedUpdatesDefinition LoadFromContext(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
			{
				throw new ArgumentException("Work Definition XML is null");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xml);
			if (xmlDocument.HasChildNodes && xmlDocument.FirstChild.Name == "ExtensionAttributes")
			{
				XmlDocument xmlDocument2 = new SafeXmlDocument();
				xmlDocument2.LoadXml(xmlDocument.FirstChild.InnerXml.ToString());
				xmlDocument = xmlDocument2;
			}
			XmlElement xmlElement = FipsUtils.CheckXmlElement(xmlDocument.SelectSingleNode("//FailedUpdatesDefinition"), "FailedUpdatesDefinition");
			return new FailedUpdatesDefinition
			{
				ConsecutiveFailures = FipsUtils.GetInteger(xmlElement.GetAttribute("ConsecutiveFailures"), "ConsecutiveFailures", 8, 2, 24),
				NumberOfFailedEngine = FipsUtils.GetInteger(xmlElement.GetAttribute("NumberOfFailedEngine"), "NumberOfFailedEngine", 1, 1, 3)
			};
		}

		private const string SnapIn = "Microsoft.Forefront.Filtering.Management.PowerShell";

		private static TracingContext traceContext = TracingContext.Default;
	}
}
