using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class Message : IMethodCallMessage, IMethodMessage, IMessage, IInternalMessage, ISerializable
	{
		public virtual Exception GetFault()
		{
			return this._Fault;
		}

		public virtual void SetFault(Exception e)
		{
			this._Fault = e;
		}

		internal virtual void SetOneWay()
		{
			this._flags |= 8;
		}

		public virtual int GetCallType()
		{
			this.InitIfNecessary();
			return this._flags;
		}

		internal IntPtr GetFramePtr()
		{
			return this._frame;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetAsyncBeginInfo(out AsyncCallback acbd, out object state);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object GetThisPtr();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IAsyncResult GetAsyncResult();

		public void Init()
		{
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object GetReturnValue();

		internal Message()
		{
		}

		[SecurityCritical]
		internal void InitFields(MessageData msgData)
		{
			this._frame = msgData.pFrame;
			this._delegateMD = msgData.pDelegateMD;
			this._methodDesc = msgData.pMethodDesc;
			this._flags = msgData.iFlags;
			this._initDone = true;
			this._metaSigHolder = msgData.pSig;
			this._governingType = msgData.thGoverningType;
			this._MethodName = null;
			this._MethodSignature = null;
			this._MethodBase = null;
			this._URI = null;
			this._Fault = null;
			this._ID = null;
			this._srvID = null;
			this._callContext = null;
			if (this._properties != null)
			{
				((IDictionary)this._properties).Clear();
			}
		}

		private void InitIfNecessary()
		{
			if (!this._initDone)
			{
				this.Init();
				this._initDone = true;
			}
		}

		ServerIdentity IInternalMessage.ServerIdentityObject
		{
			[SecurityCritical]
			get
			{
				return this._srvID;
			}
			[SecurityCritical]
			set
			{
				this._srvID = value;
			}
		}

		Identity IInternalMessage.IdentityObject
		{
			[SecurityCritical]
			get
			{
				return this._ID;
			}
			[SecurityCritical]
			set
			{
				this._ID = value;
			}
		}

		[SecurityCritical]
		void IInternalMessage.SetURI(string URI)
		{
			this._URI = URI;
		}

		[SecurityCritical]
		void IInternalMessage.SetCallContext(LogicalCallContext callContext)
		{
			this._callContext = callContext;
		}

		[SecurityCritical]
		bool IInternalMessage.HasProperties()
		{
			return this._properties != null;
		}

		public IDictionary Properties
		{
			[SecurityCritical]
			get
			{
				if (this._properties == null)
				{
					Interlocked.CompareExchange(ref this._properties, new MCMDictionary(this, null), null);
				}
				return (IDictionary)this._properties;
			}
		}

		public string Uri
		{
			[SecurityCritical]
			get
			{
				return this._URI;
			}
			set
			{
				this._URI = value;
			}
		}

		public bool HasVarArgs
		{
			[SecurityCritical]
			get
			{
				if ((this._flags & 16) == 0 && (this._flags & 32) == 0)
				{
					if (!this.InternalHasVarArgs())
					{
						this._flags |= 16;
					}
					else
					{
						this._flags |= 32;
					}
				}
				return 1 == (this._flags & 32);
			}
		}

		public int ArgCount
		{
			[SecurityCritical]
			get
			{
				return this.InternalGetArgCount();
			}
		}

		[SecurityCritical]
		public object GetArg(int argNum)
		{
			return this.InternalGetArg(argNum);
		}

		[SecurityCritical]
		public string GetArgName(int index)
		{
			if (index >= this.ArgCount)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(this.GetMethodBase());
			ParameterInfo[] parameters = reflectionCachedData.Parameters;
			if (index < parameters.Length)
			{
				return parameters[index].Name;
			}
			return "VarArg" + (index - parameters.Length);
		}

		public object[] Args
		{
			[SecurityCritical]
			get
			{
				return this.InternalGetArgs();
			}
		}

		public int InArgCount
		{
			[SecurityCritical]
			get
			{
				if (this._argMapper == null)
				{
					this._argMapper = new ArgMapper(this, false);
				}
				return this._argMapper.ArgCount;
			}
		}

		[SecurityCritical]
		public object GetInArg(int argNum)
		{
			if (this._argMapper == null)
			{
				this._argMapper = new ArgMapper(this, false);
			}
			return this._argMapper.GetArg(argNum);
		}

		[SecurityCritical]
		public string GetInArgName(int index)
		{
			if (this._argMapper == null)
			{
				this._argMapper = new ArgMapper(this, false);
			}
			return this._argMapper.GetArgName(index);
		}

		public object[] InArgs
		{
			[SecurityCritical]
			get
			{
				if (this._argMapper == null)
				{
					this._argMapper = new ArgMapper(this, false);
				}
				return this._argMapper.Args;
			}
		}

		[SecurityCritical]
		private void UpdateNames()
		{
			RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(this.GetMethodBase());
			this._typeName = reflectionCachedData.TypeAndAssemblyName;
			this._MethodName = reflectionCachedData.MethodName;
		}

		public string MethodName
		{
			[SecurityCritical]
			get
			{
				if (this._MethodName == null)
				{
					this.UpdateNames();
				}
				return this._MethodName;
			}
		}

		public string TypeName
		{
			[SecurityCritical]
			get
			{
				if (this._typeName == null)
				{
					this.UpdateNames();
				}
				return this._typeName;
			}
		}

		public object MethodSignature
		{
			[SecurityCritical]
			get
			{
				if (this._MethodSignature == null)
				{
					this._MethodSignature = Message.GenerateMethodSignature(this.GetMethodBase());
				}
				return this._MethodSignature;
			}
		}

		public LogicalCallContext LogicalCallContext
		{
			[SecurityCritical]
			get
			{
				return this.GetLogicalCallContext();
			}
		}

		public MethodBase MethodBase
		{
			[SecurityCritical]
			get
			{
				return this.GetMethodBase();
			}
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
		}

		[SecurityCritical]
		internal MethodBase GetMethodBase()
		{
			if (null == this._MethodBase)
			{
				IRuntimeMethodInfo methodHandle = new RuntimeMethodInfoStub(this._methodDesc, null);
				this._MethodBase = RuntimeType.GetMethodBase(Type.GetTypeFromHandleUnsafe(this._governingType), methodHandle);
			}
			return this._MethodBase;
		}

		[SecurityCritical]
		internal LogicalCallContext SetLogicalCallContext(LogicalCallContext callCtx)
		{
			LogicalCallContext callContext = this._callContext;
			this._callContext = callCtx;
			return callContext;
		}

		[SecurityCritical]
		internal LogicalCallContext GetLogicalCallContext()
		{
			if (this._callContext == null)
			{
				this._callContext = new LogicalCallContext();
			}
			return this._callContext;
		}

		[SecurityCritical]
		internal static Type[] GenerateMethodSignature(MethodBase mb)
		{
			RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(mb);
			ParameterInfo[] parameters = reflectionCachedData.Parameters;
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			return array;
		}

		[SecurityCritical]
		internal static object[] CoerceArgs(IMethodMessage m)
		{
			MethodBase methodBase = m.MethodBase;
			RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(methodBase);
			return Message.CoerceArgs(m, reflectionCachedData.Parameters);
		}

		[SecurityCritical]
		internal static object[] CoerceArgs(IMethodMessage m, ParameterInfo[] pi)
		{
			return Message.CoerceArgs(m.MethodBase, m.Args, pi);
		}

		[SecurityCritical]
		internal static object[] CoerceArgs(MethodBase mb, object[] args, ParameterInfo[] pi)
		{
			if (pi == null)
			{
				throw new ArgumentNullException("pi");
			}
			if (pi.Length != args.Length)
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_ArgMismatch"), new object[]
				{
					mb.DeclaringType.FullName,
					mb.Name,
					args.Length,
					pi.Length
				}));
			}
			for (int i = 0; i < pi.Length; i++)
			{
				ParameterInfo parameterInfo = pi[i];
				Type parameterType = parameterInfo.ParameterType;
				object obj = args[i];
				if (obj != null)
				{
					args[i] = Message.CoerceArg(obj, parameterType);
				}
				else if (parameterType.IsByRef)
				{
					Type elementType = parameterType.GetElementType();
					if (elementType.IsValueType)
					{
						if (parameterInfo.IsOut)
						{
							args[i] = Activator.CreateInstance(elementType, true);
						}
						else if (!elementType.IsGenericType || !(elementType.GetGenericTypeDefinition() == typeof(Nullable<>)))
						{
							throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_MissingArgValue"), elementType.FullName, i));
						}
					}
				}
				else if (parameterType.IsValueType && (!parameterType.IsGenericType || !(parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_MissingArgValue"), parameterType.FullName, i));
				}
			}
			return args;
		}

		[SecurityCritical]
		internal static object CoerceArg(object value, Type pt)
		{
			object obj = null;
			if (value != null)
			{
				Exception innerException = null;
				try
				{
					if (pt.IsByRef)
					{
						pt = pt.GetElementType();
					}
					if (pt.IsInstanceOfType(value))
					{
						obj = value;
					}
					else
					{
						obj = Convert.ChangeType(value, pt, CultureInfo.InvariantCulture);
					}
				}
				catch (Exception ex)
				{
					innerException = ex;
				}
				if (obj == null)
				{
					string arg;
					if (RemotingServices.IsTransparentProxy(value))
					{
						arg = typeof(MarshalByRefObject).ToString();
					}
					else
					{
						arg = value.ToString();
					}
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_CoercionFailed"), arg, pt), innerException);
				}
			}
			return obj;
		}

		[SecurityCritical]
		internal static object SoapCoerceArg(object value, Type pt, Hashtable keyToNamespaceTable)
		{
			object obj = null;
			if (value != null)
			{
				try
				{
					if (pt.IsByRef)
					{
						pt = pt.GetElementType();
					}
					if (pt.IsInstanceOfType(value))
					{
						obj = value;
					}
					else
					{
						string text = value as string;
						if (text != null)
						{
							if (pt == typeof(double))
							{
								if (text == "INF")
								{
									obj = double.PositiveInfinity;
								}
								else if (text == "-INF")
								{
									obj = double.NegativeInfinity;
								}
								else
								{
									obj = double.Parse(text, CultureInfo.InvariantCulture);
								}
							}
							else if (pt == typeof(float))
							{
								if (text == "INF")
								{
									obj = float.PositiveInfinity;
								}
								else if (text == "-INF")
								{
									obj = float.NegativeInfinity;
								}
								else
								{
									obj = float.Parse(text, CultureInfo.InvariantCulture);
								}
							}
							else if (SoapType.typeofISoapXsd.IsAssignableFrom(pt))
							{
								if (pt == SoapType.typeofSoapTime)
								{
									obj = SoapTime.Parse(text);
								}
								else if (pt == SoapType.typeofSoapDate)
								{
									obj = SoapDate.Parse(text);
								}
								else if (pt == SoapType.typeofSoapYearMonth)
								{
									obj = SoapYearMonth.Parse(text);
								}
								else if (pt == SoapType.typeofSoapYear)
								{
									obj = SoapYear.Parse(text);
								}
								else if (pt == SoapType.typeofSoapMonthDay)
								{
									obj = SoapMonthDay.Parse(text);
								}
								else if (pt == SoapType.typeofSoapDay)
								{
									obj = SoapDay.Parse(text);
								}
								else if (pt == SoapType.typeofSoapMonth)
								{
									obj = SoapMonth.Parse(text);
								}
								else if (pt == SoapType.typeofSoapHexBinary)
								{
									obj = SoapHexBinary.Parse(text);
								}
								else if (pt == SoapType.typeofSoapBase64Binary)
								{
									obj = SoapBase64Binary.Parse(text);
								}
								else if (pt == SoapType.typeofSoapInteger)
								{
									obj = SoapInteger.Parse(text);
								}
								else if (pt == SoapType.typeofSoapPositiveInteger)
								{
									obj = SoapPositiveInteger.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNonPositiveInteger)
								{
									obj = SoapNonPositiveInteger.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNonNegativeInteger)
								{
									obj = SoapNonNegativeInteger.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNegativeInteger)
								{
									obj = SoapNegativeInteger.Parse(text);
								}
								else if (pt == SoapType.typeofSoapAnyUri)
								{
									obj = SoapAnyUri.Parse(text);
								}
								else if (pt == SoapType.typeofSoapQName)
								{
									obj = SoapQName.Parse(text);
									SoapQName soapQName = (SoapQName)obj;
									if (soapQName.Key.Length == 0)
									{
										soapQName.Namespace = (string)keyToNamespaceTable["xmlns"];
									}
									else
									{
										soapQName.Namespace = (string)keyToNamespaceTable["xmlns:" + soapQName.Key];
									}
								}
								else if (pt == SoapType.typeofSoapNotation)
								{
									obj = SoapNotation.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNormalizedString)
								{
									obj = SoapNormalizedString.Parse(text);
								}
								else if (pt == SoapType.typeofSoapToken)
								{
									obj = SoapToken.Parse(text);
								}
								else if (pt == SoapType.typeofSoapLanguage)
								{
									obj = SoapLanguage.Parse(text);
								}
								else if (pt == SoapType.typeofSoapName)
								{
									obj = SoapName.Parse(text);
								}
								else if (pt == SoapType.typeofSoapIdrefs)
								{
									obj = SoapIdrefs.Parse(text);
								}
								else if (pt == SoapType.typeofSoapEntities)
								{
									obj = SoapEntities.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNmtoken)
								{
									obj = SoapNmtoken.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNmtokens)
								{
									obj = SoapNmtokens.Parse(text);
								}
								else if (pt == SoapType.typeofSoapNcName)
								{
									obj = SoapNcName.Parse(text);
								}
								else if (pt == SoapType.typeofSoapId)
								{
									obj = SoapId.Parse(text);
								}
								else if (pt == SoapType.typeofSoapIdref)
								{
									obj = SoapIdref.Parse(text);
								}
								else if (pt == SoapType.typeofSoapEntity)
								{
									obj = SoapEntity.Parse(text);
								}
							}
							else if (pt == typeof(bool))
							{
								if (text == "1" || text == "true")
								{
									obj = true;
								}
								else
								{
									if (!(text == "0") && !(text == "false"))
									{
										throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_CoercionFailed"), text, pt));
									}
									obj = false;
								}
							}
							else if (pt == typeof(DateTime))
							{
								obj = SoapDateTime.Parse(text);
							}
							else if (pt.IsPrimitive)
							{
								obj = Convert.ChangeType(value, pt, CultureInfo.InvariantCulture);
							}
							else if (pt == typeof(TimeSpan))
							{
								obj = SoapDuration.Parse(text);
							}
							else if (pt == typeof(char))
							{
								obj = text[0];
							}
							else
							{
								obj = Convert.ChangeType(value, pt, CultureInfo.InvariantCulture);
							}
						}
						else
						{
							obj = Convert.ChangeType(value, pt, CultureInfo.InvariantCulture);
						}
					}
				}
				catch (Exception)
				{
				}
				if (obj == null)
				{
					string arg;
					if (RemotingServices.IsTransparentProxy(value))
					{
						arg = typeof(MarshalByRefObject).ToString();
					}
					else
					{
						arg = value.ToString();
					}
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_CoercionFailed"), arg, pt));
				}
			}
			return obj;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool InternalHasVarArgs();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int InternalGetArgCount();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object InternalGetArg(int argNum);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object[] InternalGetArgs();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PropagateOutParameters(object[] OutArgs, object retVal);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Dispatch(object target);

		[SecurityCritical]
		[Conditional("_REMOTING_DEBUG")]
		public static void DebugOut(string s)
		{
			Message.OutToUnmanagedDebugger(string.Concat(new object[]
			{
				"\nRMTING: Thrd ",
				Thread.CurrentThread.GetHashCode(),
				" : ",
				s
			}));
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OutToUnmanagedDebugger(string s);

		[SecurityCritical]
		internal static LogicalCallContext PropagateCallContextFromMessageToThread(IMessage msg)
		{
			return CallContext.SetLogicalCallContext((LogicalCallContext)msg.Properties[Message.CallContextKey]);
		}

		[SecurityCritical]
		internal static void PropagateCallContextFromThreadToMessage(IMessage msg)
		{
			LogicalCallContext logicalCallContext = Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext;
			msg.Properties[Message.CallContextKey] = logicalCallContext;
		}

		[SecurityCritical]
		internal static void PropagateCallContextFromThreadToMessage(IMessage msg, LogicalCallContext oldcctx)
		{
			Message.PropagateCallContextFromThreadToMessage(msg);
			CallContext.SetLogicalCallContext(oldcctx);
		}

		internal const int Sync = 0;

		internal const int BeginAsync = 1;

		internal const int EndAsync = 2;

		internal const int Ctor = 4;

		internal const int OneWay = 8;

		internal const int CallMask = 15;

		internal const int FixedArgs = 16;

		internal const int VarArgs = 32;

		private string _MethodName;

		private Type[] _MethodSignature;

		private MethodBase _MethodBase;

		private object _properties;

		private string _URI;

		private string _typeName;

		private Exception _Fault;

		private Identity _ID;

		private ServerIdentity _srvID;

		private ArgMapper _argMapper;

		[SecurityCritical]
		private LogicalCallContext _callContext;

		private IntPtr _frame;

		private IntPtr _methodDesc;

		private IntPtr _metaSigHolder;

		private IntPtr _delegateMD;

		private IntPtr _governingType;

		private int _flags;

		private bool _initDone;

		internal static string CallContextKey = "__CallContext";

		internal static string UriKey = "__Uri";
	}
}
