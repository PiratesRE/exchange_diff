using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver.Configuration
{
	internal class StoreDriverParameters
	{
		internal void Load(Dictionary<string, StoreDriverParameterHandler> keyHandlerCollection)
		{
			if (keyHandlerCollection == null)
			{
				return;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string text = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "StoreDriver.config");
			if (!File.Exists(text))
			{
				StoreDriverParameters.diag.TraceError<string>(0L, "The config file {0} does not exist.", text);
				return;
			}
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			try
			{
				using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("StoreDriverParameters.xsd"))
				{
					if (manifestResourceStream == null)
					{
						StoreDriverParameters.diag.TraceDebug<string>(0L, "The schema resource {0} does not exist.", "StoreDriverParameters.xsd");
						return;
					}
					xmlSchemaSet.Add(null, SafeXmlFactory.CreateSafeXmlTextReader(manifestResourceStream));
				}
			}
			catch (FileLoadException arg)
			{
				StoreDriverParameters.diag.TraceError<string, FileLoadException>(0L, "Failed to load the manifest resource stream {0}. Error {1}", "StoreDriverParameters.xsd", arg);
				return;
			}
			catch (FileNotFoundException arg2)
			{
				StoreDriverParameters.diag.TraceError<string, FileNotFoundException>(0L, "Failed to find the manifest resource stream {0}.Error: {1}", text, arg2);
				return;
			}
			Exception ex = null;
			try
			{
				xmlSchemaSet.Compile();
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.Schemas = xmlSchemaSet;
				using (FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					xmlDocument.Load(fileStream);
					XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/configuration/storeDriver/parameters/add");
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (xmlNode != null && xmlNode is XmlElement)
						{
							this.currentNodeValid = true;
							xmlDocument.Validate(delegate(object sender, ValidationEventArgs args)
							{
								StoreDriverParameters.diag.TraceDebug<string>(0L, "The parameter or value is invalid: {0}.", args.Message);
								this.currentNodeValid = false;
							}, xmlNode);
							if (this.currentNodeValid)
							{
								string value = xmlNode.Attributes.GetNamedItem("key").Value;
								string value2 = xmlNode.Attributes.GetNamedItem("value").Value;
								if (value != null && value2 != null && keyHandlerCollection.ContainsKey(value))
								{
									keyHandlerCollection[value](value, value2);
								}
							}
						}
					}
				}
			}
			catch (XmlSchemaException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (IOException ex4)
			{
				ex = ex4;
			}
			catch (XmlException ex5)
			{
				ex = ex5;
			}
			catch (XPathException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				StoreDriverParameters.diag.TraceError<Exception>(0L, "Failed to load parameters from storedriver config. Will use default values. Error: {0}", ex);
			}
		}

		private const string ConfigXSDName = "StoreDriverParameters.xsd";

		private const string ConfigFileName = "StoreDriver.config";

		private const string ParametersNodePath = "/configuration/storeDriver/parameters/add";

		private static readonly Trace diag = ExTraceGlobals.StoreDriverTracer;

		private bool currentNodeValid = true;
	}
}
