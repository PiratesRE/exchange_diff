using System;
using System.Text;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	internal class DeleteMapiMailDefinition
	{
		private DeleteMapiMailDefinition()
		{
		}

		public string SenderEmailAddress { get; internal set; }

		public string MessageClass { get; private set; }

		public string InternetMessageId { get; private set; }

		public static DeleteMapiMailDefinition CreateInstance(string messageClass, string senderEmail, string internetMessageId)
		{
			if (string.IsNullOrEmpty(messageClass))
			{
				throw new ArgumentException("messageClass is null or empty");
			}
			if (string.IsNullOrEmpty(senderEmail))
			{
				throw new ArgumentException("senderEmail is null or empty");
			}
			if (string.IsNullOrEmpty(internetMessageId))
			{
				throw new ArgumentException("internetMessageId is null or empty");
			}
			return new DeleteMapiMailDefinition
			{
				MessageClass = messageClass,
				SenderEmailAddress = senderEmail,
				InternetMessageId = internetMessageId
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Message Class: " + this.MessageClass);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Sender Email Address: " + this.SenderEmailAddress);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("InternetMessageId: " + this.InternetMessageId);
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}
	}
}
