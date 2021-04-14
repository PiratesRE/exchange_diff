using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class RegisteredUsers : DirectoryLinkDeviceToUser
	{
		public override DirectoryObjectClass GetSourceClass()
		{
			return DirectoryObjectClass.Account;
		}

		public override void SetSourceClass(DirectoryObjectClass objectClass)
		{
		}

		public override DirectoryObjectClass GetTargetClass()
		{
			return DirectoryObjectClass.Company;
		}

		public override void SetTargetClass(DirectoryObjectClass objectClass)
		{
		}
	}
}
