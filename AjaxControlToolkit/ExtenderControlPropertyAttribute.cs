using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ExtenderControlPropertyAttribute : Attribute
	{
		public ExtenderControlPropertyAttribute() : this(true)
		{
		}

		public ExtenderControlPropertyAttribute(bool isScriptProperty)
		{
			this.isScriptProperty = isScriptProperty;
		}

		public bool IsScriptProperty
		{
			get
			{
				return this.isScriptProperty;
			}
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			ExtenderControlPropertyAttribute extenderControlPropertyAttribute = obj as ExtenderControlPropertyAttribute;
			return extenderControlPropertyAttribute != null && extenderControlPropertyAttribute.isScriptProperty == this.isScriptProperty;
		}

		public override int GetHashCode()
		{
			return this.isScriptProperty.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ExtenderControlPropertyAttribute.defaultValue);
		}

		private static ExtenderControlPropertyAttribute yes = new ExtenderControlPropertyAttribute(true);

		private static ExtenderControlPropertyAttribute no = new ExtenderControlPropertyAttribute(false);

		private static ExtenderControlPropertyAttribute defaultValue = ExtenderControlPropertyAttribute.no;

		private bool isScriptProperty;
	}
}
