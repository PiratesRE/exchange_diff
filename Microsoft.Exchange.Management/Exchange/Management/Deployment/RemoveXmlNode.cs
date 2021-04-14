using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Cmdlet("Remove", "XmlNode")]
	internal class RemoveXmlNode : Task
	{
		public RemoveXmlNode()
		{
			TaskLogger.LogEnter();
			this.ExchangeInstallPath = ConfigurationContext.Setup.InstallPath;
			TaskLogger.LogExit();
		}

		[Parameter(Mandatory = false)]
		public string ExchangeInstallPath
		{
			get
			{
				return (string)base.Fields["ExchangeInstallPath"];
			}
			set
			{
				base.Fields["ExchangeInstallPath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string XmlFileRelativePath
		{
			get
			{
				return (string)base.Fields["XmlFileRelativePath"];
			}
			set
			{
				base.Fields["XmlFileRelativePath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string XmlFileName
		{
			get
			{
				return (string)base.Fields["XmlFileName"];
			}
			set
			{
				base.Fields["XmlFileName"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string XmlNodeNameToRemove
		{
			get
			{
				return (string)base.Fields["XmlNodeNameToRemove"];
			}
			set
			{
				base.Fields["XmlNodeNameToRemove"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				string text = Path.Combine(Path.Combine(this.ExchangeInstallPath, this.XmlFileRelativePath), this.XmlFileName);
				base.WriteVerbose(Strings.VerboseTaskParameters(this.ExchangeInstallPath, this.XmlFileRelativePath, text, this.XmlNodeNameToRemove));
				SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
				safeXmlDocument.Load(text);
				XmlNode xmlNode = safeXmlDocument.DocumentElement.SelectSingleNode(this.XmlNodeNameToRemove);
				if (xmlNode != null)
				{
					safeXmlDocument.DocumentElement.RemoveChild(xmlNode);
					safeXmlDocument.Save(text);
				}
			}
			catch (XmlException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (XPathException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
			catch (IOException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			catch (UnauthorizedAccessException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
			catch (NotSupportedException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidOperation, null);
			}
			catch (SecurityException exception6)
			{
				base.WriteError(exception6, ErrorCategory.InvalidOperation, null);
			}
			catch (ArgumentException exception7)
			{
				base.WriteError(exception7, ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}
	}
}
