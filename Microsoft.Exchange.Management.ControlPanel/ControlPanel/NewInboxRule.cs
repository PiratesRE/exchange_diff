using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewInboxRule : InboxRuleParameters
	{
		public NewInboxRule()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
		}

		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "New-InboxRule";
			}
		}

		public Identity FromMessageId
		{
			get
			{
				return Identity.FromIdParameter(base["FromMessageId"]);
			}
			set
			{
				base["FromMessageId"] = value.ToIdParameter();
			}
		}

		public bool? ValidateOnly
		{
			get
			{
				return (bool?)base["ValidateOnly"];
			}
			set
			{
				base["ValidateOnly"] = (value ?? false);
			}
		}
	}
}
