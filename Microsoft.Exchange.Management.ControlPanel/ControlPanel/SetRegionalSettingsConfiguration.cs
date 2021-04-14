using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetRegionalSettingsConfiguration : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxRegionalConfiguration";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string DateFormat
		{
			get
			{
				return (string)base["DateFormat"];
			}
			set
			{
				base["DateFormat"] = value;
			}
		}

		[DataMember]
		public int Language
		{
			get
			{
				return (int)(base["Language"] ?? 0);
			}
			set
			{
				base["Language"] = value;
			}
		}

		[DataMember]
		public bool LocalizeDefaultFolderName
		{
			get
			{
				return (bool)(base["LocalizeDefaultFolderName"] ?? false);
			}
			set
			{
				base["LocalizeDefaultFolderName"] = value;
			}
		}

		[DataMember]
		public string TimeFormat
		{
			get
			{
				return (string)base["TimeFormat"];
			}
			set
			{
				base["TimeFormat"] = value;
			}
		}

		[DataMember]
		public string TimeZone { get; set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (!string.IsNullOrEmpty(this.TimeZone))
			{
				base["TimeZone"] = this.TimeZone;
			}
		}
	}
}
