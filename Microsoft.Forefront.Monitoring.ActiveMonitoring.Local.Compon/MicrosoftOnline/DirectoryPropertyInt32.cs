using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max65535))]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max2))]
	[XmlInclude(typeof(DirectoryPropertyInt32Single))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMax1))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max3))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max1))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DirectoryPropertyInt32 : DirectoryProperty
	{
		[XmlElement("Value")]
		public int[] Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private int[] valueField;
	}
}
