using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(DirectoryPropertyStringLength2To500))]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To2048))]
	[XmlInclude(typeof(DirectoryPropertyServicePrincipalName))]
	[XmlInclude(typeof(DirectoryPropertyProxyAddresses))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To1024))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To1024))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To512))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To256))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To64))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To40))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To3))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To2048))]
	[XmlInclude(typeof(DirectoryPropertyStringSingle))]
	[XmlInclude(typeof(DirectoryPropertyTargetAddress))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleMailNickname))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To128))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To1123))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To1123))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To512))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To500))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To454))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To256))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To16))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To40))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To20))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To64))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To6))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To3))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public abstract class DirectoryPropertyString : DirectoryProperty
	{
		[XmlElement("Value")]
		public string[] Value
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

		private string[] valueField;
	}
}
