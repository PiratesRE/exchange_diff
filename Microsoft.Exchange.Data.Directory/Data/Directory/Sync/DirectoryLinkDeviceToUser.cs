using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(RegisteredUsers))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(RegisteredOwner))]
	[DebuggerStepThrough]
	[Serializable]
	public abstract class DirectoryLinkDeviceToUser : DirectoryLink
	{
		public DirectoryLinkDeviceToUser()
		{
			this.sourceClassField = DirectoryObjectClass.Device;
			this.targetClassField = DirectoryObjectClass.User;
		}

		[XmlAttribute]
		public DirectoryObjectClass SourceClass
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

		[XmlIgnore]
		public bool SourceClassSpecified
		{
			get
			{
				return this.sourceClassFieldSpecified;
			}
			set
			{
				this.sourceClassFieldSpecified = value;
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

		private DirectoryObjectClass sourceClassField;

		private bool sourceClassFieldSpecified;

		private DirectoryObjectClass targetClassField;

		private bool targetClassFieldSpecified;
	}
}
