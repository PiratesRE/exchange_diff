using System;
using System.ComponentModel;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class TaskCommand : Command
	{
		public TaskCommand()
		{
			this.SelectionMode = SelectionMode.SupportsMultipleSelection;
			this.IsLongRunning = false;
		}

		[DefaultValue(SelectionMode.SupportsMultipleSelection)]
		public override SelectionMode SelectionMode { get; set; }

		[DefaultValue("")]
		public virtual string ActionName { get; set; }

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[DefaultValue(false)]
		public bool IsLongRunning { get; set; }

		[DefaultValue("")]
		[Localizable(true)]
		public string InProgressDescription { get; set; }

		[Localizable(true)]
		[DefaultValue("")]
		public string StoppedDescription { get; set; }

		[DefaultValue("")]
		[Localizable(true)]
		public string CompletedDescription { get; set; }

		private BindingCollection parameters = new BindingCollection();
	}
}
