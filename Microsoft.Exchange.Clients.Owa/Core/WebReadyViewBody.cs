using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class WebReadyViewBody : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.utilities = new WebReadyViewUtilities(base.OwaContext);
			this.utilities.InvokeTaskManager();
			this.previousButtonState = (this.utilities.CurrentPageNumber != 1);
			this.nextButtonState = (this.utilities.CurrentPageNumber != this.utilities.TotalPageNumber);
		}

		protected bool PreviousButtonState
		{
			get
			{
				return this.previousButtonState;
			}
		}

		protected bool NextButtonState
		{
			get
			{
				return this.nextButtonState;
			}
		}

		protected bool IsSupportPaging
		{
			get
			{
				return this.utilities.IsSupportPaging;
			}
		}

		protected bool IsCopyRestricted
		{
			get
			{
				return !this.utilities.HasError && this.utilities.IsCopyRestricted;
			}
		}

		protected bool IsPrintRestricted
		{
			get
			{
				return !this.utilities.HasError && this.utilities.IsPrintRestricted;
			}
		}

		protected bool IsRestricted
		{
			get
			{
				return !this.utilities.HasError && this.utilities.IsIrmProtected;
			}
		}

		protected SanitizedHtmlString IrmTemplateDescription
		{
			get
			{
				return this.utilities.IrmInfobarMessage;
			}
		}

		protected bool HasError
		{
			get
			{
				return this.utilities.HasError;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.utilities.ErrorMessage;
			}
		}

		protected int CurrentPageNumber
		{
			get
			{
				return this.utilities.CurrentPageNumber;
			}
		}

		protected int TotalPageNumber
		{
			get
			{
				return this.utilities.TotalPageNumber;
			}
		}

		private WebReadyViewUtilities utilities;

		private bool previousButtonState;

		private bool nextButtonState;
	}
}
