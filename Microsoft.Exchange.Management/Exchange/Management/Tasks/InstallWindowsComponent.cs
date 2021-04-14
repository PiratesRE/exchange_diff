using System;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("install", "windowscomponent")]
	public class InstallWindowsComponent : RunProcessBase
	{
		[Parameter(Mandatory = true)]
		public string ShortNameForRole
		{
			get
			{
				return (string)base.Fields["ShortNameForRole"];
			}
			set
			{
				base.Fields["ShortNameForRole"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public bool ADToolsNeeded
		{
			get
			{
				return (bool)base.Fields["ADToolsNeeded"];
			}
			set
			{
				base.Fields["ADToolsNeeded"] = value;
			}
		}

		public InstallWindowsComponent()
		{
			base.ExeName = "ServerManagerCmd.exe";
			base.IgnoreExitCode = new int[]
			{
				1000,
				1001,
				1003,
				3010
			};
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string text = null;
			try
			{
				string text2 = "Exchange-" + this.ShortNameForRole + ".xml";
				string text3 = Path.Combine(ConfigurationContext.Setup.BinPath, text2);
				string text4 = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, this.ShortNameForRole + "Prereqs.log");
				string text5 = text3;
				if (this.ADToolsNeeded)
				{
					SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
					safeXmlDocument.Load(text3);
					XmlNode documentElement = safeXmlDocument.DocumentElement;
					XmlElement xmlElement = safeXmlDocument.CreateElement("Feature", documentElement.NamespaceURI);
					xmlElement.SetAttribute("Id", "RSAT-ADDS");
					documentElement.AppendChild(xmlElement);
					string text6 = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, "temp" + text2);
					safeXmlDocument.Save(text6);
					text5 = text6;
					text = text6;
				}
				base.Args = string.Concat(new string[]
				{
					"-inputPath \"",
					text5,
					"\" -logPath \"",
					text4,
					"\""
				});
				base.InternalProcessRecord();
			}
			catch (IOException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (UnauthorizedAccessException exception2)
			{
				base.WriteError(exception2, ErrorCategory.SecurityError, null);
			}
			finally
			{
				if (text != null)
				{
					try
					{
						File.Delete(text);
					}
					catch
					{
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void HandleProcessOutput(string outputString, string errorString)
		{
			Regex regex = new Regex("<\\d*/\\d*>\\s*");
			outputString = regex.Replace(outputString, "");
			base.HandleProcessOutput(outputString, errorString);
		}
	}
}
