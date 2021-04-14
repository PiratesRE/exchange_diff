using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewStringMechanism : TranslationMechanismBase
	{
		private LocalizedString MessageWithDisplayName { get; set; }

		public NewStringMechanism(LocalizedString messageWithDisplayName, LocalizedString messageWithoutDisplayName, bool isLogging) : base(messageWithoutDisplayName, isLogging)
		{
			this.MessageWithDisplayName = messageWithDisplayName;
		}

		public NewStringMechanism(LocalizedString newMessage, bool isLogging) : this(newMessage, newMessage, isLogging)
		{
		}

		public NewStringMechanism(LocalizedString newMessage) : this(newMessage, newMessage, true)
		{
		}

		protected override string TranslationWithDisplayName(Identity id, string originalMessage)
		{
			return string.Format(this.MessageWithDisplayName, id.DisplayName);
		}
	}
}
