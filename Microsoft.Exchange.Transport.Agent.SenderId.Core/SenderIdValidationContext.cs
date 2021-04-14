using System;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SenderIdValidationContext
	{
		internal SenderIdValidationContext(SenderIdValidationBaseContext baseContext, string purportedResponsibleDomain, bool processExpModifier, AsyncCallback asyncCallback, object asyncState)
		{
			this.baseContext = baseContext;
			this.purportedResponsibleDomain = purportedResponsibleDomain.ToLowerInvariant();
			this.processExpModifier = processExpModifier;
			this.clientAR = new SenderIdAsyncResult(asyncCallback, asyncState);
			this.isValid = true;
		}

		public SenderIdValidationBaseContext BaseContext
		{
			get
			{
				return this.baseContext;
			}
		}

		public string PurportedResponsibleDomain
		{
			get
			{
				return this.purportedResponsibleDomain;
			}
		}

		public bool ProcessExpModifier
		{
			get
			{
				return this.processExpModifier;
			}
		}

		public ExpSpfModifier Exp
		{
			get
			{
				return this.exp;
			}
		}

		public SenderIdAsyncResult ClientAR
		{
			get
			{
				return this.clientAR;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		public void AddExpModifier(ExpSpfModifier modifier)
		{
			if (this.exp == null)
			{
				this.exp = modifier;
				return;
			}
			this.SetInvalid();
		}

		public void ValidationCompleted(SenderIdStatus status)
		{
			this.ValidationCompleted(new SenderIdResult(status));
		}

		public void ValidationCompleted(SenderIdStatus status, SenderIdFailReason failReason)
		{
			this.ValidationCompleted(new SenderIdResult(status, failReason));
		}

		public void ValidationCompleted(SenderIdStatus status, SenderIdFailReason failReason, string explanation)
		{
			this.ValidationCompleted(new SenderIdResult(status, failReason, explanation));
		}

		public void ValidationCompleted(SenderIdResult result)
		{
			this.BaseContext.SenderIdValidator.ValidationCompleted(this, result);
			this.clientAR.InvokeCompleted(result);
		}

		public void SetInvalid()
		{
			this.isValid = false;
		}

		private SenderIdValidationBaseContext baseContext;

		private string purportedResponsibleDomain;

		private bool processExpModifier;

		private ExpSpfModifier exp;

		private SenderIdAsyncResult clientAR;

		private bool isValid;
	}
}
