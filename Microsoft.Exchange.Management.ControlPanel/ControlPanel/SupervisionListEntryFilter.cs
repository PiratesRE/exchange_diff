using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SupervisionListEntryFilter : WebServiceParameters
	{
		public SupervisionListEntryFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Tag = "allow";
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-SupervisionListEntry";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
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

		public const string RbacParameters = "?Tag&Identity";
	}
}
