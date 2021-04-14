using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public sealed class ExtenderControlEventAttribute : Attribute
	{
		public ExtenderControlEventAttribute() : this(true)
		{
		}

		public ExtenderControlEventAttribute(bool isScriptEvent)
		{
			this.isScriptEvent = isScriptEvent;
		}

		public bool IsScriptEvent
		{
			get
			{
				return this.isScriptEvent;
			}
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			ExtenderControlEventAttribute extenderControlEventAttribute = obj as ExtenderControlEventAttribute;
			return extenderControlEventAttribute != null && extenderControlEventAttribute.isScriptEvent == this.isScriptEvent;
		}

		public override int GetHashCode()
		{
			return this.isScriptEvent.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ExtenderControlEventAttribute.defaultValue);
		}

		private static ExtenderControlEventAttribute yes = new ExtenderControlEventAttribute(true);

		private static ExtenderControlEventAttribute no = new ExtenderControlEventAttribute(false);

		private static ExtenderControlEventAttribute defaultValue = ExtenderControlEventAttribute.no;

		private bool isScriptEvent;
	}
}
