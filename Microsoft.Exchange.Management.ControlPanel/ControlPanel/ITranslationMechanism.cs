using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public interface ITranslationMechanism
	{
		string Translate(Identity id, Exception ex, string originalMessage);
	}
}
