using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMailboxCalendarFolder : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxCalendarFolder";
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
		public bool PublishEnabled
		{
			get
			{
				return (bool)(base["PublishEnabled"] ?? false);
			}
			set
			{
				base["PublishEnabled"] = value;
			}
		}

		[DataMember]
		public string DetailLevel
		{
			get
			{
				return (string)base["DetailLevel"];
			}
			set
			{
				base["DetailLevel"] = value;
			}
		}

		[DataMember]
		public string PublishDateRangeFrom
		{
			get
			{
				return (string)base["PublishDateRangeFrom"];
			}
			set
			{
				base["PublishDateRangeFrom"] = value;
			}
		}

		[DataMember]
		public string PublishDateRangeTo
		{
			get
			{
				return (string)base["PublishDateRangeTo"];
			}
			set
			{
				base["PublishDateRangeTo"] = value;
			}
		}

		[DataMember]
		public bool SearchableUrlEnabled
		{
			get
			{
				return (bool)(base["SearchableUrlEnabled"] ?? false);
			}
			set
			{
				base["SearchableUrlEnabled"] = value;
			}
		}
	}
}
