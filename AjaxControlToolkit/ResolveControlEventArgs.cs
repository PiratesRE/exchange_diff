using System;
using System.Web.UI;

namespace AjaxControlToolkit
{
	public class ResolveControlEventArgs : EventArgs
	{
		public ResolveControlEventArgs(string controlId)
		{
			this.controlID = controlId;
		}

		public string ControlID
		{
			get
			{
				return this.controlID;
			}
		}

		public Control Control
		{
			get
			{
				return this.control;
			}
			set
			{
				this.control = value;
			}
		}

		private string controlID;

		private Control control;
	}
}
