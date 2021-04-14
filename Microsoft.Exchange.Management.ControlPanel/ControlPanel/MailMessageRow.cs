using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Providers;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailMessageRow : BaseRow
	{
		public MailMessageRow(MailMessage message) : base(new Identity(message.Identity.ToString(), message.Subject), message)
		{
			this.Message = message;
		}

		public MailMessage Message { get; private set; }

		[DataMember]
		public string Subject
		{
			get
			{
				return this.Message.Subject;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Body
		{
			get
			{
				return this.Message.Body;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public BodyFormat BodyFormat
		{
			get
			{
				return (BodyFormat)this.Message.BodyFormat;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
