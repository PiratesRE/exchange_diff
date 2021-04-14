using System;
using System.Text;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	internal class SendMapiMailDefinition
	{
		private SendMapiMailDefinition()
		{
		}

		public string SenderEmailAddress
		{
			get
			{
				return this.senderEmailAddress;
			}
			internal set
			{
				this.senderEmailAddress = value;
			}
		}

		public Guid SenderMbxGuid
		{
			get
			{
				return this.senderMbxGuid;
			}
			internal set
			{
				this.senderMbxGuid = value;
			}
		}

		public Guid SenderMdbGuid
		{
			get
			{
				return this.senderMdbGuid;
			}
			internal set
			{
				this.senderMdbGuid = value;
			}
		}

		public string RecipientEmailAddress
		{
			get
			{
				return this.recipientEmailAddress;
			}
			internal set
			{
				this.recipientEmailAddress = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return this.messageSubject;
			}
			private set
			{
				this.messageSubject = value;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
			private set
			{
				this.messageClass = value;
			}
		}

		public string MessageBody
		{
			get
			{
				return this.messageBody;
			}
			private set
			{
				this.messageBody = value;
			}
		}

		public bool DoNotDeliver
		{
			get
			{
				return this.doNotDeliver;
			}
			private set
			{
				this.doNotDeliver = value;
			}
		}

		public bool DropMessageInHub
		{
			get
			{
				return this.dropMessageInHub;
			}
			private set
			{
				this.dropMessageInHub = value;
			}
		}

		public bool DeleteAfterSubmit
		{
			get
			{
				return this.deleteAfterSubmit;
			}
			private set
			{
				this.deleteAfterSubmit = value;
			}
		}

		public static SendMapiMailDefinition CreateInstance(string subject, string body, string messageClass, bool doNotDeliver, bool dropMessageInHub, bool deleteAfterSubmit, string senderEmail, Guid senderMbx, Guid senderMdb, string recipientEmail, bool skipSenderValidation)
		{
			if (string.IsNullOrEmpty(subject))
			{
				throw new ArgumentNullException("subject");
			}
			if (string.IsNullOrEmpty(body))
			{
				throw new ArgumentNullException("body");
			}
			if (string.IsNullOrEmpty(messageClass))
			{
				throw new ArgumentNullException("messageClass");
			}
			if (!skipSenderValidation)
			{
				if (string.IsNullOrEmpty(senderEmail))
				{
					throw new ArgumentNullException("senderEmail");
				}
				if (string.IsNullOrEmpty(recipientEmail))
				{
					throw new ArgumentNullException("recipientEmail");
				}
			}
			return new SendMapiMailDefinition
			{
				MessageSubject = subject,
				MessageBody = body,
				MessageClass = messageClass,
				DoNotDeliver = doNotDeliver,
				DropMessageInHub = dropMessageInHub,
				DeleteAfterSubmit = deleteAfterSubmit,
				SenderEmailAddress = senderEmail,
				SenderMbxGuid = senderMbx,
				SenderMdbGuid = senderMdb,
				RecipientEmailAddress = recipientEmail
			};
		}

		public static SendMapiMailDefinition CreateInstance(string subject, string body, string messageClass, bool doNotDeliver, bool deleteAfterSubmit, string senderEmail, string recipientEmail)
		{
			return SendMapiMailDefinition.CreateInstance(subject, body, messageClass, doNotDeliver, false, deleteAfterSubmit, senderEmail, Guid.Empty, Guid.Empty, recipientEmail, false);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Message Subject: " + this.MessageSubject);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Message Body: " + this.MessageBody);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Message Class: " + this.MessageClass);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Do Not Deliver Message: " + this.DoNotDeliver);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Drop Message in Hub: " + this.DropMessageInHub);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Delete Message From SentItems After Submit: " + this.DeleteAfterSubmit);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Sender Email Address: " + this.SenderEmailAddress);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Recipient Email Address: " + this.RecipientEmailAddress);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Sender Mbx Guid: " + this.SenderMbxGuid);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Sender Mdb Guid: " + this.SenderMdbGuid);
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		private string senderEmailAddress;

		private Guid senderMbxGuid;

		private Guid senderMdbGuid;

		private string recipientEmailAddress;

		private string messageSubject;

		private string messageClass;

		private string messageBody;

		private bool doNotDeliver;

		private bool dropMessageInHub;

		private bool deleteAfterSubmit;
	}
}
