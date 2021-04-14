using System;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Serializable]
	public class UIValidationError
	{
		public static UIValidationError[] None
		{
			get
			{
				return UIValidationError.none;
			}
		}

		public UIValidationError(LocalizedString description, Control errorProviderAnchor)
		{
			this.description = description;
			this.errorProviderAnchor = errorProviderAnchor;
		}

		public LocalizedString Description
		{
			get
			{
				return this.description;
			}
		}

		public Control ErrorProviderAnchor
		{
			get
			{
				return this.errorProviderAnchor;
			}
		}

		public override bool Equals(object right)
		{
			if (right == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, right))
			{
				return true;
			}
			if (base.GetType() != right.GetType())
			{
				return false;
			}
			UIValidationError uivalidationError = right as UIValidationError;
			return string.Compare(this.Description, uivalidationError.Description) == 0 && this.ErrorProviderAnchor == uivalidationError.ErrorProviderAnchor;
		}

		public override int GetHashCode()
		{
			return this.Description.GetHashCode() ^ this.ErrorProviderAnchor.GetHashCode();
		}

		private LocalizedString description;

		private Control errorProviderAnchor;

		private static readonly UIValidationError[] none = new UIValidationError[0];
	}
}
