using System;
using System.Globalization;
using System.Reflection;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal sealed class CustomPropertyImpl : ICustomProperty
	{
		public CustomPropertyImpl(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			this.m_property = propertyInfo;
		}

		public string Name
		{
			get
			{
				return this.m_property.Name;
			}
		}

		public bool CanRead
		{
			get
			{
				return this.m_property.GetGetMethod() != null;
			}
		}

		public bool CanWrite
		{
			get
			{
				return this.m_property.GetSetMethod() != null;
			}
		}

		public object GetValue(object target)
		{
			return this.InvokeInternal(target, null, true);
		}

		public object GetValue(object target, object indexValue)
		{
			return this.InvokeInternal(target, new object[]
			{
				indexValue
			}, true);
		}

		public void SetValue(object target, object value)
		{
			this.InvokeInternal(target, new object[]
			{
				value
			}, false);
		}

		public void SetValue(object target, object value, object indexValue)
		{
			this.InvokeInternal(target, new object[]
			{
				indexValue,
				value
			}, false);
		}

		[SecuritySafeCritical]
		private object InvokeInternal(object target, object[] args, bool getValue)
		{
			IGetProxyTarget getProxyTarget = target as IGetProxyTarget;
			if (getProxyTarget != null)
			{
				target = getProxyTarget.GetTarget();
			}
			MethodInfo methodInfo = getValue ? this.m_property.GetGetMethod(true) : this.m_property.GetSetMethod(true);
			if (methodInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString(getValue ? "Arg_GetMethNotFnd" : "Arg_SetMethNotFnd"));
			}
			if (!methodInfo.IsPublic)
			{
				throw new MethodAccessException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_MethodAccessException_WithMethodName"), methodInfo.ToString(), methodInfo.DeclaringType.FullName));
			}
			RuntimeMethodInfo runtimeMethodInfo = methodInfo as RuntimeMethodInfo;
			if (runtimeMethodInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"));
			}
			return runtimeMethodInfo.UnsafeInvoke(target, BindingFlags.Default, null, args, null);
		}

		public Type Type
		{
			get
			{
				return this.m_property.PropertyType;
			}
		}

		private PropertyInfo m_property;
	}
}
