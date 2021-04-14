namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed partial class ObjectPickerForm : global::Microsoft.Exchange.Management.SystemManager.WinForms.SearchDialog, global::Microsoft.Exchange.Management.SystemManager.WinForms.ISelectedObjectsProvider
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ObjectPicker = null;
			}
			base.Dispose(disposing);
		}
	}
}
