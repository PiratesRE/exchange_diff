using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class PropertyValidationExceptionMechanism : ITranslationMechanism
	{
		private bool IsLogging { get; set; }

		public PropertyValidationExceptionMechanism(bool isLogging)
		{
			this.IsLogging = isLogging;
		}

		public string Translate(Identity id, Exception ex, string originalMessage)
		{
			string text = null;
			PropertyValidationException ex2 = ex as PropertyValidationException;
			if (ex2 != null)
			{
				StringBuilder stringBuilder = new StringBuilder(128);
				foreach (PropertyValidationError propertyValidationError in ex2.PropertyValidationErrors)
				{
					stringBuilder.Append(propertyValidationError.Description);
					stringBuilder.Append(" ");
				}
				text = stringBuilder.ToString();
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
	}
}
