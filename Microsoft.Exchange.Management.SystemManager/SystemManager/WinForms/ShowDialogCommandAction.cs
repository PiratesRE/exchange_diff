using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ShowDialogCommandAction : ResultsCommandAction
	{
		public ShowDialogCommandAction()
		{
		}

		protected override void OnExecute()
		{
			base.OnExecute();
			ExchangePropertyPageControl exchangePropertyPageControl = this.CreateDialogControl();
			DataContext context = exchangePropertyPageControl.Context;
			if (context != null)
			{
				context.DataSaved += delegate(object param0, EventArgs param1)
				{
					this.RefreshResultsThreadSafely(context);
				};
				context.RefreshOnSave = (context.RefreshOnSave ?? base.GetDefaultRefreshObject());
			}
			base.ResultPane.ShowDialog(exchangePropertyPageControl);
		}

		protected abstract ExchangePropertyPageControl CreateDialogControl();
	}
}
