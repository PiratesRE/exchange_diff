using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NoOpReplaceMechanism : ITranslationMechanism
	{
		public string Translate(Identity id, Exception ex, string originalMessage)
		{
			return originalMessage;
		}
	}
}
