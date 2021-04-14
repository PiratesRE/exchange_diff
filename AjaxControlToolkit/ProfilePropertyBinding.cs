using System;
using System.ComponentModel;

namespace AjaxControlToolkit
{
	public class ProfilePropertyBinding
	{
		[NotifyParentProperty(true)]
		public string ExtenderPropertyName
		{
			get
			{
				return this.extenderPropertyName;
			}
			set
			{
				this.extenderPropertyName = value;
			}
		}

		[NotifyParentProperty(true)]
		public string ProfilePropertyName
		{
			get
			{
				return this.profilePropertyName;
			}
			set
			{
				this.profilePropertyName = value;
			}
		}

		private string extenderPropertyName;

		private string profilePropertyName;
	}
}
