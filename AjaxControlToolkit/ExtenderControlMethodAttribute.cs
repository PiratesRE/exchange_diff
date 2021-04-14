using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class ExtenderControlMethodAttribute : Attribute
	{
		public ExtenderControlMethodAttribute() : this(true)
		{
		}

		public ExtenderControlMethodAttribute(bool isScriptMethod)
		{
			this.isScriptMethod = isScriptMethod;
		}

		public bool IsScriptMethod
		{
			get
			{
				return this.isScriptMethod;
			}
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			ExtenderControlMethodAttribute extenderControlMethodAttribute = obj as ExtenderControlMethodAttribute;
			return extenderControlMethodAttribute != null && extenderControlMethodAttribute.isScriptMethod == this.isScriptMethod;
		}

		public override int GetHashCode()
		{
			return this.isScriptMethod.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ExtenderControlMethodAttribute.defaultValue);
		}

		private static ExtenderControlMethodAttribute yes = new ExtenderControlMethodAttribute(true);

		private static ExtenderControlMethodAttribute no = new ExtenderControlMethodAttribute(false);

		private static ExtenderControlMethodAttribute defaultValue = ExtenderControlMethodAttribute.no;

		private bool isScriptMethod;
	}
}
