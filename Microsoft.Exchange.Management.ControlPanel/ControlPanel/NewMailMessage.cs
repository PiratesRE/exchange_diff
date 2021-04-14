using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Providers;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewMailMessage : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-MailMessage";
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
		public string Subject
		{
			get
			{
				return (string)base[MailMessageSchema.Subject];
			}
			set
			{
				base[MailMessageSchema.Subject] = value;
			}
		}

		[DataMember]
		public string Body
		{
			get
			{
				return (string)base[MailMessageSchema.Body];
			}
			set
			{
				base[MailMessageSchema.Body] = value;
			}
		}

		[DataMember]
		public BodyFormat BodyFormat
		{
			get
			{
				return (BodyFormat)base[MailMessageSchema.BodyFormat];
			}
			set
			{
				base[MailMessageSchema.BodyFormat] = (MailBodyFormat)value;
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.BodyFormat = BodyFormat.Html;
		}

		public const string RbacParameters = "?Subject&Body&BodyFormat";
	}
}
