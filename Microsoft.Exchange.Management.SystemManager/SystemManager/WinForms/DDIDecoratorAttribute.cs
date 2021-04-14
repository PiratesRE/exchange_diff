using System;
using System.Reflection;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class DDIDecoratorAttribute : DDIValidateAttribute
	{
		public Type AttributeType { get; set; }

		public DDIDecoratorAttribute(string description) : base(description)
		{
		}

		protected DDIValidateAttribute GetDDIAttribute()
		{
			DDIValidateAttribute result = null;
			if (this.AttributeType != null)
			{
				try
				{
					result = (Activator.CreateInstance(this.AttributeType) as DDIValidateAttribute);
				}
				catch (ArgumentException)
				{
				}
				catch (NotSupportedException)
				{
				}
				catch (TargetInvocationException)
				{
				}
				catch (MethodAccessException)
				{
				}
				catch (MemberAccessException)
				{
				}
				catch (TypeLoadException)
				{
				}
			}
			return result;
		}
	}
}
