using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ManageEventManifest
	{
		private static void DoManifestAction(string manifestName, string actionString)
		{
			string path = Path.Combine(Environment.GetEnvironmentVariable("windir"), "system32");
			string executableFilename = Path.Combine(path, "wevtutil.exe");
			string arguments = actionString + " \"" + manifestName + "\"";
			string text;
			string errors;
			int num = ProcessRunner.Run(executableFilename, arguments, -1, null, out text, out errors);
			if (num != 0)
			{
				throw new InvalidOperationException(Strings.EventManifestActionFailed(manifestName, actionString, num, errors));
			}
		}

		private static void DoWevtutilAction(string cmdlineArguments)
		{
			string path = Path.Combine(Environment.GetEnvironmentVariable("windir"), "system32");
			string executableFilename = Path.Combine(path, "wevtutil.exe");
			string text;
			string errors;
			int num = ProcessRunner.Run(executableFilename, cmdlineArguments, -1, null, out text, out errors);
			if (num != 0)
			{
				throw new InvalidOperationException(Strings.EventOtherActionFailed(cmdlineArguments, num, errors));
			}
		}

		internal static void Install(string manifestName)
		{
			ManageEventManifest.DoManifestAction(manifestName, "install-manifest");
		}

		internal static void Uninstall(string manifestName)
		{
			ManageEventManifest.DoManifestAction(manifestName, "uninstall-manifest");
		}

		internal static void SetChannelAttribute(string verb, string channelName, string arguments)
		{
			ManageEventManifest.DoWevtutilAction(string.Format("{0} {1} {2}", verb, channelName, arguments));
		}

		internal static bool UpdateMessageDllPath(string manifestName, string msgDll, string providerName)
		{
			bool flag = false;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(manifestName);
			using (XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("provider"))
			{
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlAttribute xmlAttribute = (XmlAttribute)xmlNode.Attributes.GetNamedItem("name");
					if (string.Equals(xmlAttribute.Value, providerName, StringComparison.OrdinalIgnoreCase))
					{
						xmlAttribute = (XmlAttribute)xmlNode.Attributes.GetNamedItem("resourceFileName");
						xmlAttribute.Value = msgDll;
						xmlAttribute = (XmlAttribute)xmlNode.Attributes.GetNamedItem("messageFileName");
						xmlAttribute.Value = msgDll;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				xmlDocument.Save(manifestName);
			}
			return flag;
		}
	}
}
