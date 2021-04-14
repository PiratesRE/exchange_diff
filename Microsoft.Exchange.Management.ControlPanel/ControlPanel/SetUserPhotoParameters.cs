using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUserPhotoParameters : WebServiceParameters
	{
		[DataMember]
		public string Identity
		{
			get
			{
				object obj = base["Identity"];
				if (obj != null)
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				base["Identity"] = value;
			}
		}

		public Stream PhotoStream
		{
			get
			{
				object obj = base["PictureStream"];
				if (obj != null)
				{
					return (Stream)obj;
				}
				return null;
			}
			set
			{
				base["PictureStream"] = value;
			}
		}

		public SwitchParameter Preview
		{
			get
			{
				object obj = base["Preview"];
				if (obj != null)
				{
					return (SwitchParameter)obj;
				}
				return new SwitchParameter(false);
			}
			set
			{
				base["Preview"] = value;
			}
		}

		public SwitchParameter Save
		{
			get
			{
				object obj = base["Save"];
				if (obj != null)
				{
					return (SwitchParameter)obj;
				}
				return new SwitchParameter(false);
			}
			set
			{
				base["Save"] = value;
			}
		}

		public SwitchParameter Cancel
		{
			get
			{
				object obj = base["Cancel"];
				if (obj != null)
				{
					return (SwitchParameter)obj;
				}
				return new SwitchParameter(false);
			}
			set
			{
				base["Cancel"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UserPhoto";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}
	}
}
