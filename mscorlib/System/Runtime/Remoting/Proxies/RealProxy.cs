using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;

namespace System.Runtime.Remoting.Proxies
{
	[SecurityCritical]
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
	public abstract class RealProxy
	{
		[SecurityCritical]
		protected RealProxy(Type classToProxy) : this(classToProxy, (IntPtr)0, null)
		{
		}

		[SecurityCritical]
		protected RealProxy(Type classToProxy, IntPtr stub, object stubData)
		{
			if (!classToProxy.IsMarshalByRef && !classToProxy.IsInterface)
			{
				throw new ArgumentException(Environment.GetResourceString("Remoting_Proxy_ProxyTypeIsNotMBR"));
			}
			if ((IntPtr)0 == stub)
			{
				stub = RealProxy._defaultStub;
				stubData = RealProxy._defaultStubData;
			}
			this._tp = null;
			if (stubData == null)
			{
				throw new ArgumentNullException("stubdata");
			}
			this._tp = RemotingServices.CreateTransparentProxy(this, classToProxy, stub, stubData);
			RemotingProxy remotingProxy = this as RemotingProxy;
			if (remotingProxy != null)
			{
				this._flags |= RealProxyFlags.RemotingProxy;
			}
		}

		internal bool IsRemotingProxy()
		{
			return (this._flags & RealProxyFlags.RemotingProxy) == RealProxyFlags.RemotingProxy;
		}

		internal bool Initialized
		{
			get
			{
				return (this._flags & RealProxyFlags.Initialized) == RealProxyFlags.Initialized;
			}
			set
			{
				if (value)
				{
					this._flags |= RealProxyFlags.Initialized;
					return;
				}
				this._flags &= ~RealProxyFlags.Initialized;
			}
		}

		[SecurityCritical]
		[ComVisible(true)]
		public IConstructionReturnMessage InitializeServerObject(IConstructionCallMessage ctorMsg)
		{
			IConstructionReturnMessage result = null;
			if (this._serverObject == null)
			{
				Type proxiedType = this.GetProxiedType();
				if (ctorMsg != null && ctorMsg.ActivationType != proxiedType)
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Proxy_BadTypeForActivation"), proxiedType.FullName, ctorMsg.ActivationType));
				}
				this._serverObject = RemotingServices.AllocateUninitializedObject(proxiedType);
				this.SetContextForDefaultStub();
				MarshalByRefObject marshalByRefObject = (MarshalByRefObject)this.GetTransparentProxy();
				IMethodReturnMessage methodReturnMessage = null;
				Exception ex = null;
				if (ctorMsg != null)
				{
					methodReturnMessage = RemotingServices.ExecuteMessage(marshalByRefObject, ctorMsg);
					ex = methodReturnMessage.Exception;
				}
				else
				{
					try
					{
						RemotingServices.CallDefaultCtor(marshalByRefObject);
					}
					catch (Exception ex2)
					{
						ex = ex2;
					}
				}
				if (ex == null)
				{
					object[] array = (methodReturnMessage == null) ? null : methodReturnMessage.OutArgs;
					int outArgsCount = (array == null) ? 0 : array.Length;
					LogicalCallContext callCtx = (methodReturnMessage == null) ? null : methodReturnMessage.LogicalCallContext;
					result = new ConstructorReturnMessage(marshalByRefObject, array, outArgsCount, callCtx, ctorMsg);
					this.SetupIdentity();
					if (this.IsRemotingProxy())
					{
						((RemotingProxy)this).Initialized = true;
					}
				}
				else
				{
					result = new ConstructorReturnMessage(ex, ctorMsg);
				}
			}
			return result;
		}

		[SecurityCritical]
		protected MarshalByRefObject GetUnwrappedServer()
		{
			return this.UnwrappedServerObject;
		}

		[SecurityCritical]
		protected MarshalByRefObject DetachServer()
		{
			object transparentProxy = this.GetTransparentProxy();
			if (transparentProxy != null)
			{
				RemotingServices.ResetInterfaceCache(transparentProxy);
			}
			MarshalByRefObject serverObject = this._serverObject;
			this._serverObject = null;
			serverObject.__ResetServerIdentity();
			return serverObject;
		}

		[SecurityCritical]
		protected void AttachServer(MarshalByRefObject s)
		{
			object transparentProxy = this.GetTransparentProxy();
			if (transparentProxy != null)
			{
				RemotingServices.ResetInterfaceCache(transparentProxy);
			}
			this.AttachServerHelper(s);
		}

		[SecurityCritical]
		private void SetupIdentity()
		{
			if (this._identity == null)
			{
				this._identity = IdentityHolder.FindOrCreateServerIdentity(this._serverObject, null, 0);
				((Identity)this._identity).RaceSetTransparentProxy(this.GetTransparentProxy());
			}
		}

		[SecurityCritical]
		private void SetContextForDefaultStub()
		{
			if (this.GetStub() == RealProxy._defaultStub)
			{
				object stubData = RealProxy.GetStubData(this);
				if (stubData is IntPtr && ((IntPtr)stubData).Equals(RealProxy._defaultStubValue))
				{
					RealProxy.SetStubData(this, Thread.CurrentContext.InternalContextID);
				}
			}
		}

		[SecurityCritical]
		internal bool DoContextsMatch()
		{
			bool result = false;
			if (this.GetStub() == RealProxy._defaultStub)
			{
				object stubData = RealProxy.GetStubData(this);
				if (stubData is IntPtr && ((IntPtr)stubData).Equals(Thread.CurrentContext.InternalContextID))
				{
					result = true;
				}
			}
			return result;
		}

		[SecurityCritical]
		internal void AttachServerHelper(MarshalByRefObject s)
		{
			if (s == null || this._serverObject != null)
			{
				throw new ArgumentException(Environment.GetResourceString("ArgumentNull_Generic"), "s");
			}
			this._serverObject = s;
			this.SetupIdentity();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetStub();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStubData(RealProxy rp, object stubData);

		internal void SetSrvInfo(GCHandle srvIdentity, int domainID)
		{
			this._srvIdentity = srvIdentity;
			this._domainID = domainID;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object GetStubData(RealProxy rp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultStub();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetProxiedType();

		public abstract IMessage Invoke(IMessage msg);

		[SecurityCritical]
		public virtual ObjRef CreateObjRef(Type requestedType)
		{
			if (this._identity == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_NoIdentityEntry"));
			}
			return new ObjRef((MarshalByRefObject)this.GetTransparentProxy(), requestedType);
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			object transparentProxy = this.GetTransparentProxy();
			RemotingServices.GetObjectData(transparentProxy, info, context);
		}

		[SecurityCritical]
		private static void HandleReturnMessage(IMessage reqMsg, IMessage retMsg)
		{
			IMethodReturnMessage methodReturnMessage = retMsg as IMethodReturnMessage;
			if (retMsg == null || methodReturnMessage == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadType"));
			}
			Exception exception = methodReturnMessage.Exception;
			if (exception != null)
			{
				throw exception.PrepForRemoting();
			}
			if (!(retMsg is StackBasedReturnMessage))
			{
				if (reqMsg is Message)
				{
					RealProxy.PropagateOutParameters(reqMsg, methodReturnMessage.Args, methodReturnMessage.ReturnValue);
					return;
				}
				if (reqMsg is ConstructorCallMessage)
				{
					RealProxy.PropagateOutParameters(reqMsg, methodReturnMessage.Args, null);
				}
			}
		}

		[SecurityCritical]
		internal static void PropagateOutParameters(IMessage msg, object[] outArgs, object returnValue)
		{
			Message message = msg as Message;
			if (message == null)
			{
				ConstructorCallMessage constructorCallMessage = msg as ConstructorCallMessage;
				if (constructorCallMessage != null)
				{
					message = constructorCallMessage.GetMessage();
				}
			}
			if (message == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Remoting_Proxy_ExpectedOriginalMessage"));
			}
			MethodBase methodBase = message.GetMethodBase();
			RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(methodBase);
			if (outArgs != null && outArgs.Length != 0)
			{
				object[] args = message.Args;
				ParameterInfo[] parameters = reflectionCachedData.Parameters;
				foreach (int num in reflectionCachedData.MarshalRequestArgMap)
				{
					ParameterInfo parameterInfo = parameters[num];
					if (parameterInfo.IsIn && parameterInfo.ParameterType.IsByRef && !parameterInfo.IsOut)
					{
						outArgs[num] = args[num];
					}
				}
				if (reflectionCachedData.NonRefOutArgMap.Length != 0)
				{
					foreach (int num2 in reflectionCachedData.NonRefOutArgMap)
					{
						Array array = args[num2] as Array;
						if (array != null)
						{
							Array.Copy((Array)outArgs[num2], array, array.Length);
						}
					}
				}
				int[] outRefArgMap = reflectionCachedData.OutRefArgMap;
				if (outRefArgMap.Length != 0)
				{
					foreach (int num3 in outRefArgMap)
					{
						RealProxy.ValidateReturnArg(outArgs[num3], parameters[num3].ParameterType);
					}
				}
			}
			int callType = message.GetCallType();
			if ((callType & 15) != 1)
			{
				Type returnType = reflectionCachedData.ReturnType;
				if (returnType != null)
				{
					RealProxy.ValidateReturnArg(returnValue, returnType);
				}
			}
			message.PropagateOutParameters(outArgs, returnValue);
		}

		private static void ValidateReturnArg(object arg, Type paramType)
		{
			if (paramType.IsByRef)
			{
				paramType = paramType.GetElementType();
			}
			if (paramType.IsValueType)
			{
				if (arg == null)
				{
					if (!paramType.IsGenericType || !(paramType.GetGenericTypeDefinition() == typeof(Nullable<>)))
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_Proxy_ReturnValueTypeCannotBeNull"));
					}
				}
				else if (!paramType.IsInstanceOfType(arg))
				{
					throw new InvalidCastException(Environment.GetResourceString("Remoting_Proxy_BadReturnType"));
				}
			}
			else if (arg != null && !paramType.IsInstanceOfType(arg))
			{
				throw new InvalidCastException(Environment.GetResourceString("Remoting_Proxy_BadReturnType"));
			}
		}

		[SecurityCritical]
		internal static IMessage EndInvokeHelper(Message reqMsg, bool bProxyCase)
		{
			AsyncResult asyncResult = reqMsg.GetAsyncResult() as AsyncResult;
			IMessage result = null;
			if (asyncResult == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadAsyncResult"));
			}
			if (asyncResult.AsyncDelegate != reqMsg.GetThisPtr())
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_MismatchedAsyncResult"));
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne(-1, Thread.CurrentContext.IsThreadPoolAware);
			}
			AsyncResult obj = asyncResult;
			lock (obj)
			{
				if (asyncResult.EndInvokeCalled)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EndInvokeCalledMultiple"));
				}
				asyncResult.EndInvokeCalled = true;
				IMethodReturnMessage methodReturnMessage = (IMethodReturnMessage)asyncResult.GetReplyMessage();
				if (!bProxyCase)
				{
					Exception exception = methodReturnMessage.Exception;
					if (exception != null)
					{
						throw exception.PrepForRemoting();
					}
					reqMsg.PropagateOutParameters(methodReturnMessage.Args, methodReturnMessage.ReturnValue);
				}
				else
				{
					result = methodReturnMessage;
				}
				Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext.Merge(methodReturnMessage.LogicalCallContext);
			}
			return result;
		}

		[SecurityCritical]
		public virtual IntPtr GetCOMIUnknown(bool fIsMarshalled)
		{
			return MarshalByRefObject.GetComIUnknown((MarshalByRefObject)this.GetTransparentProxy());
		}

		public virtual void SetCOMIUnknown(IntPtr i)
		{
		}

		public virtual IntPtr SupportsInterface(ref Guid iid)
		{
			return IntPtr.Zero;
		}

		public virtual object GetTransparentProxy()
		{
			return this._tp;
		}

		internal MarshalByRefObject UnwrappedServerObject
		{
			get
			{
				return this._serverObject;
			}
		}

		internal virtual Identity IdentityObject
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return (Identity)this._identity;
			}
			set
			{
				this._identity = value;
			}
		}

		[SecurityCritical]
		private void PrivateInvoke(ref MessageData msgData, int type)
		{
			IMessage message = null;
			IMessage message2 = null;
			int num = -1;
			RemotingProxy remotingProxy = null;
			if (1 == type)
			{
				Message message3 = new Message();
				message3.InitFields(msgData);
				message = message3;
				num = message3.GetCallType();
			}
			else if (2 == type)
			{
				num = 0;
				remotingProxy = (this as RemotingProxy);
				bool flag = false;
				ConstructorCallMessage constructorCallMessage;
				if (!this.IsRemotingProxy())
				{
					constructorCallMessage = new ConstructorCallMessage(null, null, null, (RuntimeType)this.GetProxiedType());
				}
				else
				{
					constructorCallMessage = remotingProxy.ConstructorMessage;
					Identity identityObject = remotingProxy.IdentityObject;
					if (identityObject != null)
					{
						flag = identityObject.IsWellKnown();
					}
				}
				if (constructorCallMessage == null || flag)
				{
					constructorCallMessage = new ConstructorCallMessage(null, null, null, (RuntimeType)this.GetProxiedType());
					constructorCallMessage.SetFrame(msgData);
					message = constructorCallMessage;
					if (flag)
					{
						remotingProxy.ConstructorMessage = null;
						if (constructorCallMessage.ArgCount != 0)
						{
							throw new RemotingException(Environment.GetResourceString("Remoting_Activation_WellKnownCTOR"));
						}
					}
					message2 = new ConstructorReturnMessage((MarshalByRefObject)this.GetTransparentProxy(), null, 0, null, constructorCallMessage);
				}
				else
				{
					constructorCallMessage.SetFrame(msgData);
					message = constructorCallMessage;
				}
			}
			ChannelServices.IncrementRemoteCalls();
			if (!this.IsRemotingProxy() && (num & 2) == 2)
			{
				Message reqMsg = message as Message;
				message2 = RealProxy.EndInvokeHelper(reqMsg, true);
			}
			if (message2 == null)
			{
				Thread currentThread = Thread.CurrentThread;
				LogicalCallContext logicalCallContext = currentThread.GetMutableExecutionContext().LogicalCallContext;
				this.SetCallContextInMessage(message, num, logicalCallContext);
				logicalCallContext.PropagateOutgoingHeadersToMessage(message);
				message2 = this.Invoke(message);
				this.ReturnCallContextToThread(currentThread, message2, num, logicalCallContext);
				Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext.PropagateIncomingHeadersToCallContext(message2);
			}
			if (!this.IsRemotingProxy() && (num & 1) == 1)
			{
				Message message4 = message as Message;
				AsyncResult asyncResult = new AsyncResult(message4);
				asyncResult.SyncProcessMessage(message2);
				message2 = new ReturnMessage(asyncResult, null, 0, null, message4);
			}
			RealProxy.HandleReturnMessage(message, message2);
			if (2 == type)
			{
				IConstructionReturnMessage constructionReturnMessage = message2 as IConstructionReturnMessage;
				if (constructionReturnMessage == null)
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_Proxy_BadReturnTypeForActivation"));
				}
				ConstructorReturnMessage constructorReturnMessage = constructionReturnMessage as ConstructorReturnMessage;
				MarshalByRefObject marshalByRefObject;
				if (constructorReturnMessage != null)
				{
					marshalByRefObject = (MarshalByRefObject)constructorReturnMessage.GetObject();
					if (marshalByRefObject == null)
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_Activation_NullReturnValue"));
					}
				}
				else
				{
					marshalByRefObject = (MarshalByRefObject)RemotingServices.InternalUnmarshal((ObjRef)constructionReturnMessage.ReturnValue, this.GetTransparentProxy(), true);
					if (marshalByRefObject == null)
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_Activation_NullFromInternalUnmarshal"));
					}
				}
				if (marshalByRefObject != (MarshalByRefObject)this.GetTransparentProxy())
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_Activation_InconsistentState"));
				}
				if (this.IsRemotingProxy())
				{
					remotingProxy.ConstructorMessage = null;
				}
			}
		}

		private void SetCallContextInMessage(IMessage reqMsg, int msgFlags, LogicalCallContext cctx)
		{
			Message message = reqMsg as Message;
			if (msgFlags == 0)
			{
				if (message != null)
				{
					message.SetLogicalCallContext(cctx);
					return;
				}
				((ConstructorCallMessage)reqMsg).SetLogicalCallContext(cctx);
			}
		}

		[SecurityCritical]
		private void ReturnCallContextToThread(Thread currentThread, IMessage retMsg, int msgFlags, LogicalCallContext currCtx)
		{
			if (msgFlags == 0)
			{
				if (retMsg == null)
				{
					return;
				}
				IMethodReturnMessage methodReturnMessage = retMsg as IMethodReturnMessage;
				if (methodReturnMessage == null)
				{
					return;
				}
				LogicalCallContext logicalCallContext = methodReturnMessage.LogicalCallContext;
				if (logicalCallContext == null)
				{
					currentThread.GetMutableExecutionContext().LogicalCallContext = currCtx;
					return;
				}
				if (!(methodReturnMessage is StackBasedReturnMessage))
				{
					ExecutionContext mutableExecutionContext = currentThread.GetMutableExecutionContext();
					LogicalCallContext logicalCallContext2 = mutableExecutionContext.LogicalCallContext;
					mutableExecutionContext.LogicalCallContext = logicalCallContext;
					if (logicalCallContext2 != logicalCallContext)
					{
						IPrincipal principal = logicalCallContext2.Principal;
						if (principal != null)
						{
							logicalCallContext.Principal = principal;
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal virtual void Wrap()
		{
			ServerIdentity serverIdentity = this._identity as ServerIdentity;
			if (serverIdentity != null && this is RemotingProxy)
			{
				RealProxy.SetStubData(this, serverIdentity.ServerContext.InternalContextID);
			}
		}

		protected RealProxy()
		{
		}

		private object _tp;

		private object _identity;

		private MarshalByRefObject _serverObject;

		private RealProxyFlags _flags;

		internal GCHandle _srvIdentity;

		internal int _optFlags;

		internal int _domainID;

		private static IntPtr _defaultStub = RealProxy.GetDefaultStub();

		private static IntPtr _defaultStubValue = new IntPtr(-1);

		private static object _defaultStubData = RealProxy._defaultStubValue;
	}
}
