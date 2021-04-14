using System;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class SetupWizardEventArgs : EventArgs
	{
		public static Exception ErrorException
		{
			get
			{
				return SetupWizardEventArgs.errorException;
			}
			set
			{
				SetupWizardEventArgs.errorException = value;
			}
		}

		private static Exception errorException;
	}
}
