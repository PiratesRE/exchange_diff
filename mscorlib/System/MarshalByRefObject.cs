using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public abstract class MarshalByRefObject
	{
		private object Identity
		{
			get
			{
				return this.__identity;
			}
			set
			{
				this.__identity = value;
			}
		}

		[SecuritySafeCritical]
		internal IntPtr GetComIUnknown(bool fIsBeingMarshalled)
		{
			IntPtr result;
			if (RemotingServices.IsTransparentProxy(this))
			{
				result = RemotingServices.GetRealProxy(this).GetCOMIUnknown(fIsBeingMarshalled);
			}
			else
			{
				result = Marshal.GetIUnknownForObject(this);
			}
			return result;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetComIUnknown(MarshalByRefObject o);

		internal bool IsInstanceOfType(Type T)
		{
			return T.IsInstanceOfType(this);
		}

		internal object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			Type type = base.GetType();
			if (!type.IsCOMObject)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_InvokeMember"));
			}
			return type.InvokeMember(name, invokeAttr, binder, this, args, modifiers, culture, namedParameters);
		}

		protected MarshalByRefObject MemberwiseClone(bool cloneIdentity)
		{
			MarshalByRefObject marshalByRefObject = (MarshalByRefObject)base.MemberwiseClone();
			if (!cloneIdentity)
			{
				marshalByRefObject.Identity = null;
			}
			return marshalByRefObject;
		}

		[SecuritySafeCritical]
		internal static Identity GetIdentity(MarshalByRefObject obj, out bool fServer)
		{
			fServer = true;
			Identity result = null;
			if (obj != null)
			{
				if (!RemotingServices.IsTransparentProxy(obj))
				{
					result = (Identity)obj.Identity;
				}
				else
				{
					fServer = false;
					result = RemotingServices.GetRealProxy(obj).IdentityObject;
				}
			}
			return result;
		}

		internal static Identity GetIdentity(MarshalByRefObject obj)
		{
			bool flag;
			return MarshalByRefObject.GetIdentity(obj, out flag);
		}

		internal ServerIdentity __RaceSetServerIdentity(ServerIdentity id)
		{
			if (this.__identity == null)
			{
				if (!id.IsContextBound)
				{
					id.RaceSetTransparentProxy(this);
				}
				Interlocked.CompareExchange(ref this.__identity, id, null);
			}
			return (ServerIdentity)this.__identity;
		}

		internal void __ResetServerIdentity()
		{
			this.__identity = null;
		}

		[SecurityCritical]
		public object GetLifetimeService()
		{
			return LifetimeServices.GetLease(this);
		}

		[SecurityCritical]
		public virtual object InitializeLifetimeService()
		{
			return LifetimeServices.GetLeaseInitial(this);
		}

		[SecurityCritical]
		public virtual ObjRef CreateObjRef(Type requestedType)
		{
			if (this.__identity == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_NoIdentityEntry"));
			}
			return new ObjRef(this, requestedType);
		}

		[SecuritySafeCritical]
		internal bool CanCastToXmlType(string xmlTypeName, string xmlTypeNamespace)
		{
			Type type = SoapServices.GetInteropTypeFromXmlType(xmlTypeName, xmlTypeNamespace);
			if (type == null)
			{
				string text;
				string assemblyString;
				if (!SoapServices.DecodeXmlNamespaceForClrTypeNamespace(xmlTypeNamespace, out text, out assemblyString))
				{
					return false;
				}
				string name;
				if (text != null && text.Length > 0)
				{
					name = text + "." + xmlTypeName;
				}
				else
				{
					name = xmlTypeName;
				}
				try
				{
					Assembly assembly = Assembly.Load(assemblyString);
					type = assembly.GetType(name, false, false);
				}
				catch
				{
					return false;
				}
			}
			return type != null && type.IsAssignableFrom(base.GetType());
		}

		[SecuritySafeCritical]
		internal static bool CanCastToXmlTypeHelper(RuntimeType castType, MarshalByRefObject o)
		{
			if (castType == null)
			{
				throw new ArgumentNullException("castType");
			}
			if (!castType.IsInterface && !castType.IsMarshalByRef)
			{
				return false;
			}
			string xmlTypeName = null;
			string xmlTypeNamespace = null;
			if (!SoapServices.GetXmlTypeForInteropType(castType, out xmlTypeName, out xmlTypeNamespace))
			{
				xmlTypeName = castType.Name;
				xmlTypeNamespace = SoapServices.CodeXmlNamespaceForClrTypeNamespace(castType.Namespace, castType.GetRuntimeAssembly().GetSimpleName());
			}
			return o.CanCastToXmlType(xmlTypeName, xmlTypeNamespace);
		}

		private object __identity;
	}
}
