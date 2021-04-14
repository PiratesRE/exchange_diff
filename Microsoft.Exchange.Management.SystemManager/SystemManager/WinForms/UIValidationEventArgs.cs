using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class UIValidationEventArgs : EventArgs
	{
		public UIValidationEventArgs(ICollection<UIValidationError> errors)
		{
			this.errors = errors;
		}

		public ICollection<UIValidationError> Errors
		{
			get
			{
				return this.errors;
			}
		}

		private ICollection<UIValidationError> errors;
	}
}
