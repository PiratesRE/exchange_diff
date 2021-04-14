using System;

namespace AjaxControlToolkit.Design
{
	internal sealed class ExtenderVisiblePropertyAttribute : Attribute
	{
		public ExtenderVisiblePropertyAttribute(bool value)
		{
			this.value = value;
		}

		public bool Value
		{
			get
			{
				return this.value;
			}
		}

		public override bool IsDefaultAttribute()
		{
			return !this.value;
		}

		private bool value;

		public static ExtenderVisiblePropertyAttribute Yes = new ExtenderVisiblePropertyAttribute(true);

		public static ExtenderVisiblePropertyAttribute No = new ExtenderVisiblePropertyAttribute(false);

		public static ExtenderVisiblePropertyAttribute Default = ExtenderVisiblePropertyAttribute.No;
	}
}
