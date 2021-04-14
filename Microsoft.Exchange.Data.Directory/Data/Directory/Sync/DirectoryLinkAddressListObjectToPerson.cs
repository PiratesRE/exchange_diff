using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlInclude(typeof(AuthOrig))]
	[XmlInclude(typeof(UnauthOrig))]
	[XmlInclude(typeof(MSExchModeratedByLink))]
	[XmlInclude(typeof(MSExchBypassModerationLink))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public abstract class DirectoryLinkAddressListObjectToPerson : DirectoryLink
	{
		[XmlAttribute]
		public DirectoryObjectClassAddressList SourceClass
		{
			get
			{
				return this.sourceClassField;
			}
			set
			{
				this.sourceClassField = value;
			}
		}

		[XmlAttribute]
		public DirectoryObjectClassPerson TargetClass
		{
			get
			{
				return this.targetClassField;
			}
			set
			{
				this.targetClassField = value;
			}
		}

		private DirectoryObjectClassAddressList sourceClassField;

		private DirectoryObjectClassPerson targetClassField;
	}
}
