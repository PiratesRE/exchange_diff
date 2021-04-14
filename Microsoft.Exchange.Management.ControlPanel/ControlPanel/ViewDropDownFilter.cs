using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class ViewDropDownFilter : SearchTextFilter
	{
		[DataMember]
		public string SelectedView
		{
			get
			{
				return this.selectedView;
			}
			set
			{
				this.selectedView = value;
			}
		}

		private string selectedView;
	}
}
