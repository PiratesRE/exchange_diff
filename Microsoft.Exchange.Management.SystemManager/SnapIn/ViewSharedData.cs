using System;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class ViewSharedData
	{
		public object SelectionObject
		{
			get
			{
				return this.selectionObject;
			}
			set
			{
				this.selectionObject = value;
			}
		}

		public string SelectedResultPaneName
		{
			get
			{
				return this.selectedResultPaneName;
			}
			set
			{
				this.selectedResultPaneName = value;
			}
		}

		public View CurrentActiveView
		{
			get
			{
				return this.currentActiveView;
			}
			internal set
			{
				this.currentActiveView = value;
			}
		}

		private object selectionObject;

		private string selectedResultPaneName;

		private View currentActiveView;
	}
}
