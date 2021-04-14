using System;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace System.Runtime.InteropServices
{
	[SecurityCritical]
	internal class ComEventsSink : NativeMethods.IDispatch, ICustomQueryInterface
	{
		internal ComEventsSink(object rcw, Guid iid)
		{
			this._iidSourceItf = iid;
			this.Advise(rcw);
		}

		internal static ComEventsSink Find(ComEventsSink sinks, ref Guid iid)
		{
			ComEventsSink comEventsSink = sinks;
			while (comEventsSink != null && comEventsSink._iidSourceItf != iid)
			{
				comEventsSink = comEventsSink._next;
			}
			return comEventsSink;
		}

		internal static ComEventsSink Add(ComEventsSink sinks, ComEventsSink sink)
		{
			sink._next = sinks;
			return sink;
		}

		[SecurityCritical]
		internal static ComEventsSink RemoveAll(ComEventsSink sinks)
		{
			while (sinks != null)
			{
				sinks.Unadvise();
				sinks = sinks._next;
			}
			return null;
		}

		[SecurityCritical]
		internal static ComEventsSink Remove(ComEventsSink sinks, ComEventsSink sink)
		{
			if (sink == sinks)
			{
				sinks = sinks._next;
			}
			else
			{
				ComEventsSink comEventsSink = sinks;
				while (comEventsSink != null && comEventsSink._next != sink)
				{
					comEventsSink = comEventsSink._next;
				}
				if (comEventsSink != null)
				{
					comEventsSink._next = sink._next;
				}
			}
			sink.Unadvise();
			return sinks;
		}

		public ComEventsMethod RemoveMethod(ComEventsMethod method)
		{
			this._methods = ComEventsMethod.Remove(this._methods, method);
			return this._methods;
		}

		public ComEventsMethod FindMethod(int dispid)
		{
			return ComEventsMethod.Find(this._methods, dispid);
		}

		public ComEventsMethod AddMethod(int dispid)
		{
			ComEventsMethod comEventsMethod = new ComEventsMethod(dispid);
			this._methods = ComEventsMethod.Add(this._methods, comEventsMethod);
			return comEventsMethod;
		}

		[SecurityCritical]
		void NativeMethods.IDispatch.GetTypeInfoCount(out uint pctinfo)
		{
			pctinfo = 0U;
		}

		[SecurityCritical]
		void NativeMethods.IDispatch.GetTypeInfo(uint iTInfo, int lcid, out IntPtr info)
		{
			throw new NotImplementedException();
		}

		[SecurityCritical]
		void NativeMethods.IDispatch.GetIDsOfNames(ref Guid iid, string[] names, uint cNames, int lcid, int[] rgDispId)
		{
			throw new NotImplementedException();
		}

		private unsafe static Variant* GetVariant(Variant* pSrc)
		{
			if (pSrc->VariantType == (VarEnum)16396)
			{
				Variant* ptr = (Variant*)((void*)pSrc->AsByRefVariant);
				if ((ptr->VariantType & (VarEnum)20479) == (VarEnum)16396)
				{
					return ptr;
				}
			}
			return pSrc;
		}

		[SecurityCritical]
		unsafe void NativeMethods.IDispatch.Invoke(int dispid, ref Guid riid, int lcid, INVOKEKIND wFlags, ref DISPPARAMS pDispParams, IntPtr pvarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			ComEventsMethod comEventsMethod = this.FindMethod(dispid);
			if (comEventsMethod == null)
			{
				return;
			}
			object[] array = new object[pDispParams.cArgs];
			int[] array2 = new int[pDispParams.cArgs];
			bool[] array3 = new bool[pDispParams.cArgs];
			Variant* ptr = (Variant*)((void*)pDispParams.rgvarg);
			int* ptr2 = (int*)((void*)pDispParams.rgdispidNamedArgs);
			int i;
			int num;
			for (i = 0; i < pDispParams.cNamedArgs; i++)
			{
				num = ptr2[i];
				Variant* variant = ComEventsSink.GetVariant(ptr + i);
				array[num] = variant->ToObject();
				array3[num] = true;
				if (variant->IsByRef)
				{
					array2[num] = i;
				}
				else
				{
					array2[num] = -1;
				}
			}
			num = 0;
			while (i < pDispParams.cArgs)
			{
				while (array3[num])
				{
					num++;
				}
				Variant* variant2 = ComEventsSink.GetVariant(ptr + (pDispParams.cArgs - 1 - i));
				array[num] = variant2->ToObject();
				if (variant2->IsByRef)
				{
					array2[num] = pDispParams.cArgs - 1 - i;
				}
				else
				{
					array2[num] = -1;
				}
				num++;
				i++;
			}
			object obj = comEventsMethod.Invoke(array);
			if (pvarResult != IntPtr.Zero)
			{
				Marshal.GetNativeVariantForObject(obj, pvarResult);
			}
			for (i = 0; i < pDispParams.cArgs; i++)
			{
				int num2 = array2[i];
				if (num2 != -1)
				{
					ComEventsSink.GetVariant(ptr + num2)->CopyFromIndirect(array[i]);
				}
			}
		}

		[SecurityCritical]
		CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out IntPtr ppv)
		{
			ppv = IntPtr.Zero;
			if (iid == this._iidSourceItf || iid == typeof(NativeMethods.IDispatch).GUID)
			{
				ppv = Marshal.GetComInterfaceForObject(this, typeof(NativeMethods.IDispatch), CustomQueryInterfaceMode.Ignore);
				return CustomQueryInterfaceResult.Handled;
			}
			if (iid == ComEventsSink.IID_IManagedObject)
			{
				return CustomQueryInterfaceResult.Failed;
			}
			return CustomQueryInterfaceResult.NotHandled;
		}

		private void Advise(object rcw)
		{
			IConnectionPointContainer connectionPointContainer = (IConnectionPointContainer)rcw;
			IConnectionPoint connectionPoint;
			connectionPointContainer.FindConnectionPoint(ref this._iidSourceItf, out connectionPoint);
			connectionPoint.Advise(this, out this._cookie);
			this._connectionPoint = connectionPoint;
		}

		[SecurityCritical]
		private void Unadvise()
		{
			try
			{
				this._connectionPoint.Unadvise(this._cookie);
				Marshal.ReleaseComObject(this._connectionPoint);
			}
			catch (Exception)
			{
			}
			finally
			{
				this._connectionPoint = null;
			}
		}

		private Guid _iidSourceItf;

		private IConnectionPoint _connectionPoint;

		private int _cookie;

		private ComEventsMethod _methods;

		private ComEventsSink _next;

		private const VarEnum VT_BYREF_VARIANT = (VarEnum)16396;

		private const VarEnum VT_TYPEMASK = (VarEnum)4095;

		private const VarEnum VT_BYREF_TYPEMASK = (VarEnum)20479;

		private static Guid IID_IManagedObject = new Guid("{C3FCC19E-A970-11D2-8B5A-00A0C9B7C9C4}");
	}
}
