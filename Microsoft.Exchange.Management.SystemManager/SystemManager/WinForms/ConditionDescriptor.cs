using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ConditionDescriptor
	{
		public ConditionDescriptor(int index, string description)
		{
			this.index = index;
			this.Description = description;
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		private int index;

		private string description;
	}
}
