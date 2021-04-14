namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal partial class BackgroundWorkerProgressDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ProgressDialog
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
