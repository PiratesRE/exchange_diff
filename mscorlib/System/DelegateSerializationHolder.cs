using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System
{
	[Serializable]
	internal sealed class DelegateSerializationHolder : IObjectReference, ISerializable
	{
		[SecurityCritical]
		internal static DelegateSerializationHolder.DelegateEntry GetDelegateSerializationInfo(SerializationInfo info, Type delegateType, object target, MethodInfo method, int targetIndex)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (!method.IsPublic || (method.DeclaringType != null && !method.DeclaringType.IsVisible))
			{
				new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
			}
			Type baseType = delegateType.BaseType;
			if (baseType == null || (baseType != typeof(Delegate) && baseType != typeof(MulticastDelegate)))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			if (method.DeclaringType == null)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_GlobalMethodSerialization"));
			}
			DelegateSerializationHolder.DelegateEntry delegateEntry = new DelegateSerializationHolder.DelegateEntry(delegateType.FullName, delegateType.Module.Assembly.FullName, target, method.ReflectedType.Module.Assembly.FullName, method.ReflectedType.FullName, method.Name);
			if (info.MemberCount == 0)
			{
				info.SetType(typeof(DelegateSerializationHolder));
				info.AddValue("Delegate", delegateEntry, typeof(DelegateSerializationHolder.DelegateEntry));
			}
			if (target != null)
			{
				string text = "target" + targetIndex;
				info.AddValue(text, delegateEntry.target);
				delegateEntry.target = text;
			}
			string name = "method" + targetIndex;
			info.AddValue(name, method);
			return delegateEntry;
		}

		[SecurityCritical]
		private DelegateSerializationHolder(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			bool flag = true;
			try
			{
				this.m_delegateEntry = (DelegateSerializationHolder.DelegateEntry)info.GetValue("Delegate", typeof(DelegateSerializationHolder.DelegateEntry));
			}
			catch
			{
				this.m_delegateEntry = this.OldDelegateWireFormat(info, context);
				flag = false;
			}
			if (flag)
			{
				DelegateSerializationHolder.DelegateEntry delegateEntry = this.m_delegateEntry;
				int num = 0;
				while (delegateEntry != null)
				{
					if (delegateEntry.target != null)
					{
						string text = delegateEntry.target as string;
						if (text != null)
						{
							delegateEntry.target = info.GetValue(text, typeof(object));
						}
					}
					num++;
					delegateEntry = delegateEntry.delegateEntry;
				}
				MethodInfo[] array = new MethodInfo[num];
				int i;
				for (i = 0; i < num; i++)
				{
					string name = "method" + i;
					array[i] = (MethodInfo)info.GetValueNoThrow(name, typeof(MethodInfo));
					if (array[i] == null)
					{
						break;
					}
				}
				if (i == num)
				{
					this.m_methods = array;
				}
			}
		}

		private void ThrowInsufficientState(string field)
		{
			throw new SerializationException(Environment.GetResourceString("Serialization_InsufficientDeserializationState", new object[]
			{
				field
			}));
		}

		private DelegateSerializationHolder.DelegateEntry OldDelegateWireFormat(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			string @string = info.GetString("DelegateType");
			string string2 = info.GetString("DelegateAssembly");
			object value = info.GetValue("Target", typeof(object));
			string string3 = info.GetString("TargetTypeAssembly");
			string string4 = info.GetString("TargetTypeName");
			string string5 = info.GetString("MethodName");
			return new DelegateSerializationHolder.DelegateEntry(@string, string2, value, string3, string4, string5);
		}

		[SecurityCritical]
		private Delegate GetDelegate(DelegateSerializationHolder.DelegateEntry de, int index)
		{
			Delegate @delegate;
			try
			{
				if (de.methodName == null || de.methodName.Length == 0)
				{
					this.ThrowInsufficientState("MethodName");
				}
				if (de.assembly == null || de.assembly.Length == 0)
				{
					this.ThrowInsufficientState("DelegateAssembly");
				}
				if (de.targetTypeName == null || de.targetTypeName.Length == 0)
				{
					this.ThrowInsufficientState("TargetTypeName");
				}
				RuntimeType type = (RuntimeType)Assembly.GetType_Compat(de.assembly, de.type);
				RuntimeType runtimeType = (RuntimeType)Assembly.GetType_Compat(de.targetTypeAssembly, de.targetTypeName);
				if (this.m_methods != null)
				{
					object firstArgument = (de.target != null) ? RemotingServices.CheckCast(de.target, runtimeType) : null;
					@delegate = Delegate.CreateDelegateNoSecurityCheck(type, firstArgument, this.m_methods[index]);
				}
				else if (de.target != null)
				{
					@delegate = Delegate.CreateDelegate(type, RemotingServices.CheckCast(de.target, runtimeType), de.methodName);
				}
				else
				{
					@delegate = Delegate.CreateDelegate(type, runtimeType, de.methodName);
				}
				if ((@delegate.Method != null && !@delegate.Method.IsPublic) || (@delegate.Method.DeclaringType != null && !@delegate.Method.DeclaringType.IsVisible))
				{
					new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
				}
			}
			catch (Exception ex)
			{
				if (ex is SerializationException)
				{
					throw ex;
				}
				throw new SerializationException(ex.Message, ex);
			}
			return @delegate;
		}

		[SecurityCritical]
		public object GetRealObject(StreamingContext context)
		{
			int num = 0;
			for (DelegateSerializationHolder.DelegateEntry delegateEntry = this.m_delegateEntry; delegateEntry != null; delegateEntry = delegateEntry.Entry)
			{
				num++;
			}
			int num2 = num - 1;
			if (num == 1)
			{
				return this.GetDelegate(this.m_delegateEntry, 0);
			}
			object[] array = new object[num];
			for (DelegateSerializationHolder.DelegateEntry delegateEntry2 = this.m_delegateEntry; delegateEntry2 != null; delegateEntry2 = delegateEntry2.Entry)
			{
				num--;
				array[num] = this.GetDelegate(delegateEntry2, num2 - num);
			}
			return ((MulticastDelegate)array[0]).NewMulticastDelegate(array, array.Length);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_DelegateSerHolderSerial"));
		}

		private DelegateSerializationHolder.DelegateEntry m_delegateEntry;

		private MethodInfo[] m_methods;

		[Serializable]
		internal class DelegateEntry
		{
			internal DelegateEntry(string type, string assembly, object target, string targetTypeAssembly, string targetTypeName, string methodName)
			{
				this.type = type;
				this.assembly = assembly;
				this.target = target;
				this.targetTypeAssembly = targetTypeAssembly;
				this.targetTypeName = targetTypeName;
				this.methodName = methodName;
			}

			internal DelegateSerializationHolder.DelegateEntry Entry
			{
				get
				{
					return this.delegateEntry;
				}
				set
				{
					this.delegateEntry = value;
				}
			}

			internal string type;

			internal string assembly;

			internal object target;

			internal string targetTypeAssembly;

			internal string targetTypeName;

			internal string methodName;

			internal DelegateSerializationHolder.DelegateEntry delegateEntry;
		}
	}
}
