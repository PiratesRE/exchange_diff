namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal partial class InvisibleForm : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.backgroundWorker.Dispose();
			}
			base.Dispose(disposing);
		}

		private global::System.ComponentModel.BackgroundWorker backgroundWorker;
	}
}
