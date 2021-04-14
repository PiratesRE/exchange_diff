using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UploadExtensionParameter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-App";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		public Stream FileStream
		{
			get
			{
				return (Stream)base["FileStream"];
			}
			set
			{
				base["FileStream"] = value;
			}
		}

		public SwitchParameter AllowReadWriteMailbox
		{
			get
			{
				object obj = base["AllowReadWriteMailbox"];
				if (obj != null)
				{
					return (SwitchParameter)obj;
				}
				return new SwitchParameter(false);
			}
			set
			{
				base["AllowReadWriteMailbox"] = value;
			}
		}
	}
}
