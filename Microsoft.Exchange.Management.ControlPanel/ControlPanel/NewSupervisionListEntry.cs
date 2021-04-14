using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewSupervisionListEntry : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Add-SupervisionListEntry";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public Identity Identity
		{
			get
			{
				return (Identity)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}

		[DataMember]
		public string EntryName
		{
			get
			{
				return (string)base["Entry"];
			}
			set
			{
				base["Entry"] = value;
			}
		}

		[DataMember]
		public string RecipientType
		{
			get
			{
				return (string)base[SupervisionListEntrySchema.RecipientType.Name];
			}
			set
			{
				base[SupervisionListEntrySchema.RecipientType.Name] = value;
			}
		}

		[DataMember]
		public string Tag
		{
			get
			{
				return (string)base[SupervisionListEntrySchema.Tag.Name];
			}
			set
			{
				base[SupervisionListEntrySchema.Tag.Name] = value;
			}
		}
	}
}
