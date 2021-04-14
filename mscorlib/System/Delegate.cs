using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace System
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public abstract class Delegate : ICloneable, ISerializable
	{
		[SecuritySafeCritical]
		protected Delegate(object target, string method)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (!this.BindToMethodName(target, (RuntimeType)target.GetType(), method, (DelegateBindingFlags)10))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
			}
		}

		[SecuritySafeCritical]
		protected Delegate(Type target, string method)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (target.IsGenericType && target.ContainsGenericParameters)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_UnboundGenParam"), "target");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			RuntimeType runtimeType = target as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "target");
			}
			this.BindToMethodName(null, runtimeType, method, (DelegateBindingFlags)37);
		}

		private Delegate()
		{
		}

		[__DynamicallyInvokable]
		public object DynamicInvoke(params object[] args)
		{
			return this.DynamicInvokeImpl(args);
		}

		[SecuritySafeCritical]
		protected virtual object DynamicInvokeImpl(object[] args)
		{
			RuntimeMethodHandleInternal methodHandle = new RuntimeMethodHandleInternal(this.GetInvokeMethod());
			RuntimeMethodInfo runtimeMethodInfo = (RuntimeMethodInfo)RuntimeType.GetMethodBase((RuntimeType)base.GetType(), methodHandle);
			return runtimeMethodInfo.UnsafeInvoke(this, BindingFlags.Default, null, args, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			if (obj == null || !Delegate.InternalEqualTypes(this, obj))
			{
				return false;
			}
			Delegate @delegate = (Delegate)obj;
			if (this._target == @delegate._target && this._methodPtr == @delegate._methodPtr && this._methodPtrAux == @delegate._methodPtrAux)
			{
				return true;
			}
			if (this._methodPtrAux.IsNull())
			{
				if (!@delegate._methodPtrAux.IsNull())
				{
					return false;
				}
				if (this._target != @delegate._target)
				{
					return false;
				}
			}
			else
			{
				if (@delegate._methodPtrAux.IsNull())
				{
					return false;
				}
				if (this._methodPtrAux == @delegate._methodPtrAux)
				{
					return true;
				}
			}
			if (this._methodBase == null || @delegate._methodBase == null || !(this._methodBase is MethodInfo) || !(@delegate._methodBase is MethodInfo))
			{
				return Delegate.InternalEqualMethodHandles(this, @delegate);
			}
			return this._methodBase.Equals(@delegate._methodBase);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return base.GetType().GetHashCode();
		}

		[__DynamicallyInvokable]
		public static Delegate Combine(Delegate a, Delegate b)
		{
			if (a == null)
			{
				return b;
			}
			return a.CombineImpl(b);
		}

		[ComVisible(true)]
		[__DynamicallyInvokable]
		public static Delegate Combine(params Delegate[] delegates)
		{
			if (delegates == null || delegates.Length == 0)
			{
				return null;
			}
			Delegate @delegate = delegates[0];
			for (int i = 1; i < delegates.Length; i++)
			{
				@delegate = Delegate.Combine(@delegate, delegates[i]);
			}
			return @delegate;
		}

		[__DynamicallyInvokable]
		public virtual Delegate[] GetInvocationList()
		{
			return new Delegate[]
			{
				this
			};
		}

		[__DynamicallyInvokable]
		public MethodInfo Method
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetMethodImpl();
			}
		}

		[SecuritySafeCritical]
		protected virtual MethodInfo GetMethodImpl()
		{
			if (this._methodBase == null || !(this._methodBase is MethodInfo))
			{
				IRuntimeMethodInfo runtimeMethodInfo = this.FindMethodHandle();
				RuntimeType runtimeType = RuntimeMethodHandle.GetDeclaringType(runtimeMethodInfo);
				if ((RuntimeTypeHandle.IsGenericTypeDefinition(runtimeType) || RuntimeTypeHandle.HasInstantiation(runtimeType)) && (RuntimeMethodHandle.GetAttributes(runtimeMethodInfo) & MethodAttributes.Static) <= MethodAttributes.PrivateScope)
				{
					if (this._methodPtrAux == (IntPtr)0)
					{
						Type type = this._target.GetType();
						Type genericTypeDefinition = runtimeType.GetGenericTypeDefinition();
						while (type != null)
						{
							if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
							{
								runtimeType = (type as RuntimeType);
								break;
							}
							type = type.BaseType;
						}
					}
					else
					{
						MethodInfo method = base.GetType().GetMethod("Invoke");
						runtimeType = (RuntimeType)method.GetParameters()[0].ParameterType;
					}
				}
				this._methodBase = (MethodInfo)RuntimeType.GetMethodBase(runtimeType, runtimeMethodInfo);
			}
			return (MethodInfo)this._methodBase;
		}

		[__DynamicallyInvokable]
		public object Target
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetTarget();
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static Delegate Remove(Delegate source, Delegate value)
		{
			if (source == null)
			{
				return null;
			}
			if (value == null)
			{
				return source;
			}
			if (!Delegate.InternalEqualTypes(source, value))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTypeMis"));
			}
			return source.RemoveImpl(value);
		}

		[__DynamicallyInvokable]
		public static Delegate RemoveAll(Delegate source, Delegate value)
		{
			Delegate @delegate;
			do
			{
				@delegate = source;
				source = Delegate.Remove(source, value);
			}
			while (@delegate != source);
			return @delegate;
		}

		protected virtual Delegate CombineImpl(Delegate d)
		{
			throw new MulticastNotSupportedException(Environment.GetResourceString("Multicast_Combine"));
		}

		protected virtual Delegate RemoveImpl(Delegate d)
		{
			if (!d.Equals(this))
			{
				return this;
			}
			return null;
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		public static Delegate CreateDelegate(Type type, object target, string method)
		{
			return Delegate.CreateDelegate(type, target, method, false, true);
		}

		public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase)
		{
			return Delegate.CreateDelegate(type, target, method, ignoreCase, true);
		}

		[SecuritySafeCritical]
		public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			if (!runtimeType.IsDelegate())
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			Delegate @delegate = Delegate.InternalAlloc(runtimeType);
			if (!@delegate.BindToMethodName(target, (RuntimeType)target.GetType(), method, (DelegateBindingFlags)26 | (ignoreCase ? DelegateBindingFlags.CaselessMatching : ((DelegateBindingFlags)0))))
			{
				if (throwOnBindFailure)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
				}
				@delegate = null;
			}
			return @delegate;
		}

		public static Delegate CreateDelegate(Type type, Type target, string method)
		{
			return Delegate.CreateDelegate(type, target, method, false, true);
		}

		public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase)
		{
			return Delegate.CreateDelegate(type, target, method, ignoreCase, true);
		}

		[SecuritySafeCritical]
		public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (target.IsGenericType && target.ContainsGenericParameters)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_UnboundGenParam"), "target");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			RuntimeType runtimeType = type as RuntimeType;
			RuntimeType runtimeType2 = target as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			if (runtimeType2 == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "target");
			}
			if (!runtimeType.IsDelegate())
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			Delegate @delegate = Delegate.InternalAlloc(runtimeType);
			if (!@delegate.BindToMethodName(null, runtimeType2, method, (DelegateBindingFlags)5 | (ignoreCase ? DelegateBindingFlags.CaselessMatching : ((DelegateBindingFlags)0))))
			{
				if (throwOnBindFailure)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
				}
				@delegate = null;
			}
			return @delegate;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Delegate CreateDelegate(Type type, MethodInfo method, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
			if (runtimeMethodInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
			}
			if (!runtimeType.IsDelegate())
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			Delegate @delegate = Delegate.CreateDelegateInternal(runtimeType, runtimeMethodInfo, null, (DelegateBindingFlags)132, ref stackCrawlMark);
			if (@delegate == null && throwOnBindFailure)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
			}
			return @delegate;
		}

		[__DynamicallyInvokable]
		public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method)
		{
			return Delegate.CreateDelegate(type, firstArgument, method, true);
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
			if (runtimeMethodInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
			}
			if (!runtimeType.IsDelegate())
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			Delegate @delegate = Delegate.CreateDelegateInternal(runtimeType, runtimeMethodInfo, firstArgument, DelegateBindingFlags.RelaxedSignature, ref stackCrawlMark);
			if (@delegate == null && throwOnBindFailure)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
			}
			return @delegate;
		}

		[__DynamicallyInvokable]
		public static bool operator ==(Delegate d1, Delegate d2)
		{
			if (d1 == null)
			{
				return d2 == null;
			}
			return d1.Equals(d2);
		}

		[__DynamicallyInvokable]
		public static bool operator !=(Delegate d1, Delegate d2)
		{
			if (d1 == null)
			{
				return d2 != null;
			}
			return !d1.Equals(d2);
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException();
		}

		[SecurityCritical]
		internal static Delegate CreateDelegateNoSecurityCheck(Type type, object target, RuntimeMethodHandle method)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (method.IsNullHandle())
			{
				throw new ArgumentNullException("method");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			if (!runtimeType.IsDelegate())
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			Delegate @delegate = Delegate.InternalAlloc(runtimeType);
			if (!@delegate.BindToMethodInfo(target, method.GetMethodInfo(), RuntimeMethodHandle.GetDeclaringType(method.GetMethodInfo()), (DelegateBindingFlags)192))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
			}
			return @delegate;
		}

		[SecurityCritical]
		internal static Delegate CreateDelegateNoSecurityCheck(RuntimeType type, object firstArgument, MethodInfo method)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
			if (runtimeMethodInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
			}
			if (!type.IsDelegate())
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
			}
			Delegate @delegate = Delegate.UnsafeCreateDelegate(type, runtimeMethodInfo, firstArgument, (DelegateBindingFlags)192);
			if (@delegate == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
			}
			return @delegate;
		}

		[__DynamicallyInvokable]
		public static Delegate CreateDelegate(Type type, MethodInfo method)
		{
			return Delegate.CreateDelegate(type, method, true);
		}

		[SecuritySafeCritical]
		internal static Delegate CreateDelegateInternal(RuntimeType rtType, RuntimeMethodInfo rtMethod, object firstArgument, DelegateBindingFlags flags, ref StackCrawlMark stackMark)
		{
			bool flag = (rtMethod.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) > INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN;
			bool flag2 = (rtType.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) > INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN;
			if (flag || flag2)
			{
				RuntimeAssembly executingAssembly = RuntimeAssembly.GetExecutingAssembly(ref stackMark);
				if (executingAssembly != null && !executingAssembly.IsSafeForReflection())
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						flag ? rtMethod.FullName : rtType.FullName
					}));
				}
			}
			return Delegate.UnsafeCreateDelegate(rtType, rtMethod, firstArgument, flags);
		}

		[SecurityCritical]
		internal static Delegate UnsafeCreateDelegate(RuntimeType rtType, RuntimeMethodInfo rtMethod, object firstArgument, DelegateBindingFlags flags)
		{
			Delegate @delegate = Delegate.InternalAlloc(rtType);
			if (@delegate.BindToMethodInfo(firstArgument, rtMethod, rtMethod.GetDeclaringTypeInternal(), flags))
			{
				return @delegate;
			}
			return null;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool BindToMethodName(object target, RuntimeType methodType, string method, DelegateBindingFlags flags);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool BindToMethodInfo(object target, IRuntimeMethodInfo method, RuntimeType methodType, DelegateBindingFlags flags);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MulticastDelegate InternalAlloc(RuntimeType type);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MulticastDelegate InternalAllocLike(Delegate d);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalEqualTypes(object a, object b);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DelegateConstruct(object target, IntPtr slot);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetMulticastInvoke();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetInvokeMethod();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IRuntimeMethodInfo FindMethodHandle();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalEqualMethodHandles(Delegate left, Delegate right);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr AdjustTarget(object target, IntPtr methodPtr);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetCallStub(IntPtr methodPtr);

		[SecuritySafeCritical]
		internal virtual object GetTarget()
		{
			if (!this._methodPtrAux.IsNull())
			{
				return null;
			}
			return this._target;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CompareUnmanagedFunctionPtrs(Delegate d1, Delegate d2);

		[SecurityCritical]
		internal object _target;

		[SecurityCritical]
		internal object _methodBase;

		[SecurityCritical]
		internal IntPtr _methodPtr;

		[SecurityCritical]
		internal IntPtr _methodPtrAux;
	}
}
