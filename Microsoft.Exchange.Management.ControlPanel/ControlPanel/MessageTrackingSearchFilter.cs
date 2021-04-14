using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageTrackingSearchFilter : ResultSizeFilter
	{
		public MessageTrackingSearchFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Search-MessageTrackingReport";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}

		[DataMember]
		public string Subject
		{
			get
			{
				return (string)base["Subject"];
			}
			set
			{
				base["Subject"] = value;
			}
		}

		[DataMember]
		public string Sender
		{
			get
			{
				return (string)base["Sender"];
			}
			set
			{
				base["Sender"] = value.Trim();
			}
		}

		[DataMember]
		public string MessageEntryId
		{
			get
			{
				return (string)base["MessageEntryId"];
			}
			set
			{
				base["MessageEntryId"] = value;
			}
		}

		[DataMember]
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
		public string Recipients
		{
			get
			{
				SmtpAddress[] addresses = base["Recipients"] as SmtpAddress[];
				return addresses.ToSmtpAddressesString();
			}
			set
			{
				base["Recipients"] = value.ToSmtpAddressArray();
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext streamingContext)
		{
			this.Identity = ((RbacPrincipal.Current.ExecutingUserId != null) ? Identity.FromExecutingUserId() : null);
			if (base.CanSetParameter("ByPassDelegateChecking"))
			{
				base["BypassDelegateChecking"] = true;
			}
		}

		public new const string RbacParameters = "?ResultSize&Identity&MessageId&Subject&Sender&Recipients";
	}
}
