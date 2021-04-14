using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class TranslationMechanismBase : ITranslationMechanism
	{
		private LocalizedString MessageWithoutDisplayName { get; set; }

		private bool IsLogging { get; set; }

		public TranslationMechanismBase(LocalizedString messageWithoutDisplayName, bool isLogging)
		{
			this.MessageWithoutDisplayName = messageWithoutDisplayName;
			this.IsLogging = isLogging;
		}

		public string Translate(Identity id, Exception ex, string originalMessage)
		{
			string text;
			if (this.HasDisplayName(id))
			{
				text = this.TranslationWithDisplayName(id, originalMessage);
			}
			else
			{
				text = this.MessageWithoutDisplayName;
			}
			if (this.IsLogging)
			{
				EcpEventLogConstants.Tuple_PowershellExceptionTranslated.LogEvent(new object[]
				{
					EcpEventLogExtensions.GetUserNameToLog(),
					ex.GetType(),
					text,
					originalMessage
				});
			}
			return text;
		}

		private bool HasDisplayName(Identity id)
		{
			return null != id && !string.IsNullOrEmpty(id.DisplayName) && !id.DisplayName.Equals(id.RawIdentity);
		}

		protected abstract string TranslationWithDisplayName(Identity id, string originalMessage);
	}
}
