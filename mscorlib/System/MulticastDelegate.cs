using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public abstract class MulticastDelegate : Delegate
	{
		protected MulticastDelegate(object target, string method) : base(target, method)
		{
		}

		protected MulticastDelegate(Type target, string method) : base(target, method)
		{
		}

		[SecuritySafeCritical]
		internal bool IsUnmanagedFunctionPtr()
		{
			return this._invocationCount == (IntPtr)(-1);
		}

		[SecuritySafeCritical]
		internal bool InvocationListLogicallyNull()
		{
			return this._invocationList == null || this._invocationList is LoaderAllocator || this._invocationList is DynamicResolver;
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			int targetIndex = 0;
			object[] array = this._invocationList as object[];
			if (array == null)
			{
				MethodInfo method = base.Method;
				if (!(method is RuntimeMethodInfo) || this.IsUnmanagedFunctionPtr())
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidDelegateType"));
				}
				if (!this.InvocationListLogicallyNull() && !this._invocationCount.IsNull() && !this._methodPtrAux.IsNull())
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidDelegateType"));
				}
				DelegateSerializationHolder.GetDelegateSerializationInfo(info, base.GetType(), base.Target, method, targetIndex);
				return;
			}
			else
			{
				DelegateSerializationHolder.DelegateEntry delegateEntry = null;
				int num = (int)this._invocationCount;
				int num2 = num;
				while (--num2 >= 0)
				{
					MulticastDelegate multicastDelegate = (MulticastDelegate)array[num2];
					MethodInfo method2 = multicastDelegate.Method;
					if (method2 is RuntimeMethodInfo && !this.IsUnmanagedFunctionPtr() && (multicastDelegate.InvocationListLogicallyNull() || multicastDelegate._invocationCount.IsNull() || multicastDelegate._methodPtrAux.IsNull()))
					{
						DelegateSerializationHolder.DelegateEntry delegateSerializationInfo = DelegateSerializationHolder.GetDelegateSerializationInfo(info, multicastDelegate.GetType(), multicastDelegate.Target, method2, targetIndex++);
						if (delegateEntry != null)
						{
							delegateEntry.Entry = delegateSerializationInfo;
						}
						delegateEntry = delegateSerializationInfo;
					}
				}
				if (delegateEntry == null)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidDelegateType"));
				}
				return;
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public sealed override bool Equals(object obj)
		{
			if (obj == null || !Delegate.InternalEqualTypes(this, obj))
			{
				return false;
			}
			MulticastDelegate multicastDelegate = obj as MulticastDelegate;
			if (multicastDelegate == null)
			{
				return false;
			}
			if (this._invocationCount != (IntPtr)0)
			{
				if (this.InvocationListLogicallyNull())
				{
					if (this.IsUnmanagedFunctionPtr())
					{
						return multicastDelegate.IsUnmanagedFunctionPtr() && Delegate.CompareUnmanagedFunctionPtrs(this, multicastDelegate);
					}
					if (multicastDelegate._invocationList is Delegate)
					{
						return this.Equals(multicastDelegate._invocationList);
					}
					return base.Equals(obj);
				}
				else
				{
					if (this._invocationList is Delegate)
					{
						return this._invocationList.Equals(obj);
					}
					return this.InvocationListEquals(multicastDelegate);
				}
			}
			else
			{
				if (!this.InvocationListLogicallyNull())
				{
					return this._invocationList.Equals(multicastDelegate._invocationList) && base.Equals(multicastDelegate);
				}
				if (multicastDelegate._invocationList is Delegate)
				{
					return this.Equals(multicastDelegate._invocationList);
				}
				return base.Equals(multicastDelegate);
			}
		}

		[SecuritySafeCritical]
		private bool InvocationListEquals(MulticastDelegate d)
		{
			object[] array = this._invocationList as object[];
			if (d._invocationCount != this._invocationCount)
			{
				return false;
			}
			int num = (int)this._invocationCount;
			for (int i = 0; i < num; i++)
			{
				Delegate @delegate = (Delegate)array[i];
				object[] array2 = d._invocationList as object[];
				if (!@delegate.Equals(array2[i]))
				{
					return false;
				}
			}
			return true;
		}

		[SecurityCritical]
		private bool TrySetSlot(object[] a, int index, object o)
		{
			if (a[index] == null && Interlocked.CompareExchange<object>(ref a[index], o, null) == null)
			{
				return true;
			}
			if (a[index] != null)
			{
				MulticastDelegate multicastDelegate = (MulticastDelegate)o;
				MulticastDelegate multicastDelegate2 = (MulticastDelegate)a[index];
				if (multicastDelegate2._methodPtr == multicastDelegate._methodPtr && multicastDelegate2._target == multicastDelegate._target && multicastDelegate2._methodPtrAux == multicastDelegate._methodPtrAux)
				{
					return true;
				}
			}
			return false;
		}

		[SecurityCritical]
		private MulticastDelegate NewMulticastDelegate(object[] invocationList, int invocationCount, bool thisIsMultiCastAlready)
		{
			MulticastDelegate multicastDelegate = Delegate.InternalAllocLike(this);
			if (thisIsMultiCastAlready)
			{
				multicastDelegate._methodPtr = this._methodPtr;
				multicastDelegate._methodPtrAux = this._methodPtrAux;
			}
			else
			{
				multicastDelegate._methodPtr = base.GetMulticastInvoke();
				multicastDelegate._methodPtrAux = base.GetInvokeMethod();
			}
			multicastDelegate._target = multicastDelegate;
			multicastDelegate._invocationList = invocationList;
			multicastDelegate._invocationCount = (IntPtr)invocationCount;
			return multicastDelegate;
		}

		[SecurityCritical]
		internal MulticastDelegate NewMulticastDelegate(object[] invocationList, int invocationCount)
		{
			return this.NewMulticastDelegate(invocationList, invocationCount, false);
		}

		[SecurityCritical]
		internal void StoreDynamicMethod(MethodInfo dynamicMethod)
		{
			if (this._invocationCount != (IntPtr)0)
			{
				MulticastDelegate multicastDelegate = (MulticastDelegate)this._invocationList;
				multicastDelegate._methodBase = dynamicMethod;
				return;
			}
			this._methodBase = dynamicMethod;
		}

		[SecuritySafeCritical]
		protected sealed override Delegate CombineImpl(Delegate follow)
		{
			if (follow == null)
			{
				return this;
			}
			if (!Delegate.InternalEqualTypes(this, follow))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTypeMis"));
			}
			MulticastDelegate multicastDelegate = (MulticastDelegate)follow;
			int num = 1;
			object[] array = multicastDelegate._invocationList as object[];
			if (array != null)
			{
				num = (int)multicastDelegate._invocationCount;
			}
			object[] array2 = this._invocationList as object[];
			int num2;
			object[] array3;
			if (array2 == null)
			{
				num2 = 1 + num;
				array3 = new object[num2];
				array3[0] = this;
				if (array == null)
				{
					array3[1] = multicastDelegate;
				}
				else
				{
					for (int i = 0; i < num; i++)
					{
						array3[1 + i] = array[i];
					}
				}
				return this.NewMulticastDelegate(array3, num2);
			}
			int num3 = (int)this._invocationCount;
			num2 = num3 + num;
			array3 = null;
			if (num2 <= array2.Length)
			{
				array3 = array2;
				if (array == null)
				{
					if (!this.TrySetSlot(array3, num3, multicastDelegate))
					{
						array3 = null;
					}
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						if (!this.TrySetSlot(array3, num3 + j, array[j]))
						{
							array3 = null;
							break;
						}
					}
				}
			}
			if (array3 == null)
			{
				int k;
				for (k = array2.Length; k < num2; k *= 2)
				{
				}
				array3 = new object[k];
				for (int l = 0; l < num3; l++)
				{
					array3[l] = array2[l];
				}
				if (array == null)
				{
					array3[num3] = multicastDelegate;
				}
				else
				{
					for (int m = 0; m < num; m++)
					{
						array3[num3 + m] = array[m];
					}
				}
			}
			return this.NewMulticastDelegate(array3, num2, true);
		}

		[SecurityCritical]
		private object[] DeleteFromInvocationList(object[] invocationList, int invocationCount, int deleteIndex, int deleteCount)
		{
			object[] array = this._invocationList as object[];
			int num = array.Length;
			while (num / 2 >= invocationCount - deleteCount)
			{
				num /= 2;
			}
			object[] array2 = new object[num];
			for (int i = 0; i < deleteIndex; i++)
			{
				array2[i] = invocationList[i];
			}
			for (int j = deleteIndex + deleteCount; j < invocationCount; j++)
			{
				array2[j - deleteCount] = invocationList[j];
			}
			return array2;
		}

		private bool EqualInvocationLists(object[] a, object[] b, int start, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (!a[start + i].Equals(b[i]))
				{
					return false;
				}
			}
			return true;
		}

		[SecuritySafeCritical]
		protected sealed override Delegate RemoveImpl(Delegate value)
		{
			MulticastDelegate multicastDelegate = value as MulticastDelegate;
			if (multicastDelegate == null)
			{
				return this;
			}
			if (!(multicastDelegate._invocationList is object[]))
			{
				object[] array = this._invocationList as object[];
				if (array == null)
				{
					if (this.Equals(value))
					{
						return null;
					}
				}
				else
				{
					int num = (int)this._invocationCount;
					int num2 = num;
					while (--num2 >= 0)
					{
						if (value.Equals(array[num2]))
						{
							if (num == 2)
							{
								return (Delegate)array[1 - num2];
							}
							object[] invocationList = this.DeleteFromInvocationList(array, num, num2, 1);
							return this.NewMulticastDelegate(invocationList, num - 1, true);
						}
					}
				}
			}
			else
			{
				object[] array2 = this._invocationList as object[];
				if (array2 != null)
				{
					int num3 = (int)this._invocationCount;
					int num4 = (int)multicastDelegate._invocationCount;
					int i = num3 - num4;
					while (i >= 0)
					{
						if (this.EqualInvocationLists(array2, multicastDelegate._invocationList as object[], i, num4))
						{
							if (num3 - num4 == 0)
							{
								return null;
							}
							if (num3 - num4 == 1)
							{
								return (Delegate)array2[(i != 0) ? 0 : (num3 - 1)];
							}
							object[] invocationList2 = this.DeleteFromInvocationList(array2, num3, i, num4);
							return this.NewMulticastDelegate(invocationList2, num3 - num4, true);
						}
						else
						{
							i--;
						}
					}
				}
			}
			return this;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public sealed override Delegate[] GetInvocationList()
		{
			object[] array = this._invocationList as object[];
			Delegate[] array2;
			if (array == null)
			{
				array2 = new Delegate[]
				{
					this
				};
			}
			else
			{
				int num = (int)this._invocationCount;
				array2 = new Delegate[num];
				for (int i = 0; i < num; i++)
				{
					array2[i] = (Delegate)array[i];
				}
			}
			return array2;
		}

		[__DynamicallyInvokable]
		public static bool operator ==(MulticastDelegate d1, MulticastDelegate d2)
		{
			if (d1 == null)
			{
				return d2 == null;
			}
			return d1.Equals(d2);
		}

		[__DynamicallyInvokable]
		public static bool operator !=(MulticastDelegate d1, MulticastDelegate d2)
		{
			if (d1 == null)
			{
				return d2 != null;
			}
			return !d1.Equals(d2);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public sealed override int GetHashCode()
		{
			if (this.IsUnmanagedFunctionPtr())
			{
				return ValueType.GetHashCodeOfPtr(this._methodPtr) ^ ValueType.GetHashCodeOfPtr(this._methodPtrAux);
			}
			object[] array = this._invocationList as object[];
			if (array == null)
			{
				return base.GetHashCode();
			}
			int num = 0;
			for (int i = 0; i < (int)this._invocationCount; i++)
			{
				num = num * 33 + array[i].GetHashCode();
			}
			return num;
		}

		[SecuritySafeCritical]
		internal override object GetTarget()
		{
			if (this._invocationCount != (IntPtr)0)
			{
				if (this.InvocationListLogicallyNull())
				{
					return null;
				}
				object[] array = this._invocationList as object[];
				if (array != null)
				{
					int num = (int)this._invocationCount;
					return ((Delegate)array[num - 1]).GetTarget();
				}
				Delegate @delegate = this._invocationList as Delegate;
				if (@delegate != null)
				{
					return @delegate.GetTarget();
				}
			}
			return base.GetTarget();
		}

		[SecuritySafeCritical]
		protected override MethodInfo GetMethodImpl()
		{
			if (this._invocationCount != (IntPtr)0 && this._invocationList != null)
			{
				object[] array = this._invocationList as object[];
				if (array != null)
				{
					int num = (int)this._invocationCount - 1;
					return ((Delegate)array[num]).Method;
				}
				MulticastDelegate multicastDelegate = this._invocationList as MulticastDelegate;
				if (multicastDelegate != null)
				{
					return multicastDelegate.GetMethodImpl();
				}
			}
			else if (this.IsUnmanagedFunctionPtr())
			{
				if (this._methodBase == null || !(this._methodBase is MethodInfo))
				{
					IRuntimeMethodInfo runtimeMethodInfo = base.FindMethodHandle();
					RuntimeType runtimeType = RuntimeMethodHandle.GetDeclaringType(runtimeMethodInfo);
					if (RuntimeTypeHandle.IsGenericTypeDefinition(runtimeType) || RuntimeTypeHandle.HasInstantiation(runtimeType))
					{
						RuntimeType runtimeType2 = base.GetType() as RuntimeType;
						runtimeType = runtimeType2;
					}
					this._methodBase = (MethodInfo)RuntimeType.GetMethodBase(runtimeType, runtimeMethodInfo);
				}
				return (MethodInfo)this._methodBase;
			}
			return base.GetMethodImpl();
		}

		[DebuggerNonUserCode]
		private void ThrowNullThisInDelegateToInstance()
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_DlgtNullInst"));
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorClosed(object target, IntPtr methodPtr)
		{
			if (target == null)
			{
				this.ThrowNullThisInDelegateToInstance();
			}
			this._target = target;
			this._methodPtr = methodPtr;
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorClosedStatic(object target, IntPtr methodPtr)
		{
			this._target = target;
			this._methodPtr = methodPtr;
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorRTClosed(object target, IntPtr methodPtr)
		{
			this._target = target;
			this._methodPtr = base.AdjustTarget(target, methodPtr);
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorOpened(object target, IntPtr methodPtr, IntPtr shuffleThunk)
		{
			this._target = this;
			this._methodPtr = shuffleThunk;
			this._methodPtrAux = methodPtr;
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorSecureClosed(object target, IntPtr methodPtr, IntPtr callThunk, IntPtr creatorMethod)
		{
			MulticastDelegate multicastDelegate = Delegate.InternalAllocLike(this);
			multicastDelegate.CtorClosed(target, methodPtr);
			this._invocationList = multicastDelegate;
			this._target = this;
			this._methodPtr = callThunk;
			this._methodPtrAux = creatorMethod;
			this._invocationCount = base.GetInvokeMethod();
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorSecureClosedStatic(object target, IntPtr methodPtr, IntPtr callThunk, IntPtr creatorMethod)
		{
			MulticastDelegate multicastDelegate = Delegate.InternalAllocLike(this);
			multicastDelegate.CtorClosedStatic(target, methodPtr);
			this._invocationList = multicastDelegate;
			this._target = this;
			this._methodPtr = callThunk;
			this._methodPtrAux = creatorMethod;
			this._invocationCount = base.GetInvokeMethod();
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorSecureRTClosed(object target, IntPtr methodPtr, IntPtr callThunk, IntPtr creatorMethod)
		{
			MulticastDelegate multicastDelegate = Delegate.InternalAllocLike(this);
			multicastDelegate.CtorRTClosed(target, methodPtr);
			this._invocationList = multicastDelegate;
			this._target = this;
			this._methodPtr = callThunk;
			this._methodPtrAux = creatorMethod;
			this._invocationCount = base.GetInvokeMethod();
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorSecureOpened(object target, IntPtr methodPtr, IntPtr shuffleThunk, IntPtr callThunk, IntPtr creatorMethod)
		{
			MulticastDelegate multicastDelegate = Delegate.InternalAllocLike(this);
			multicastDelegate.CtorOpened(target, methodPtr, shuffleThunk);
			this._invocationList = multicastDelegate;
			this._target = this;
			this._methodPtr = callThunk;
			this._methodPtrAux = creatorMethod;
			this._invocationCount = base.GetInvokeMethod();
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorVirtualDispatch(object target, IntPtr methodPtr, IntPtr shuffleThunk)
		{
			this._target = this;
			this._methodPtr = shuffleThunk;
			this._methodPtrAux = base.GetCallStub(methodPtr);
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorSecureVirtualDispatch(object target, IntPtr methodPtr, IntPtr shuffleThunk, IntPtr callThunk, IntPtr creatorMethod)
		{
			MulticastDelegate multicastDelegate = Delegate.InternalAllocLike(this);
			multicastDelegate.CtorVirtualDispatch(target, methodPtr, shuffleThunk);
			this._invocationList = multicastDelegate;
			this._target = this;
			this._methodPtr = callThunk;
			this._methodPtrAux = creatorMethod;
			this._invocationCount = base.GetInvokeMethod();
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorCollectibleClosedStatic(object target, IntPtr methodPtr, IntPtr gchandle)
		{
			this._target = target;
			this._methodPtr = methodPtr;
			this._methodBase = GCHandle.InternalGet(gchandle);
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorCollectibleOpened(object target, IntPtr methodPtr, IntPtr shuffleThunk, IntPtr gchandle)
		{
			this._target = this;
			this._methodPtr = shuffleThunk;
			this._methodPtrAux = methodPtr;
			this._methodBase = GCHandle.InternalGet(gchandle);
		}

		[SecurityCritical]
		[DebuggerNonUserCode]
		private void CtorCollectibleVirtualDispatch(object target, IntPtr methodPtr, IntPtr shuffleThunk, IntPtr gchandle)
		{
			this._target = this;
			this._methodPtr = shuffleThunk;
			this._methodPtrAux = base.GetCallStub(methodPtr);
			this._methodBase = GCHandle.InternalGet(gchandle);
		}

		[SecurityCritical]
		private object _invocationList;

		[SecurityCritical]
		private IntPtr _invocationCount;
	}
}
