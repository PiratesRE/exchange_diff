using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlInclude(typeof(MSExchBypassModerationFromDLMembersLink))]
	[XmlInclude(typeof(DLMemSubmitPerms))]
	[XmlInclude(typeof(DLMemRejectPerms))]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public abstract class DirectoryLinkAddressListObjectToGroup : DirectoryLink
	{
		public DirectoryLinkAddressListObjectToGroup()
		{
			this.targetClassField = DirectoryObjectClass.Group;
		}

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
		public DirectoryObjectClass TargetClass
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

		[XmlIgnore]
		public bool TargetClassSpecified
		{
			get
			{
				return this.targetClassFieldSpecified;
			}
			set
			{
				this.targetClassFieldSpecified = value;
			}
		}

		private DirectoryObjectClassAddressList sourceClassField;

		private DirectoryObjectClass targetClassField;

		private bool targetClassFieldSpecified;
	}
}
