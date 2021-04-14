using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewJournalRule : JournalRuleObjectProperties
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "New-JournalRule";
			}
		}

		[DataMember]
		public bool? Enabled
		{
			get
			{
				return (bool?)base["Enabled"];
			}
			set
			{
				base["Enabled"] = value;
			}
		}
	}
}
