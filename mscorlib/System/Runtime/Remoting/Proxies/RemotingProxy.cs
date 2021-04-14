using System;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Threading;

namespace System.Runtime.Remoting.Proxies
{
	[SecurityCritical]
	internal class RemotingProxy : RealProxy, IRemotingTypeInfo
	{
		public RemotingProxy(Type serverType) : base(serverType)
		{
		}

		private RemotingProxy()
		{
		}

		internal int CtorThread
		{
			get
			{
				return this._ctorThread;
			}
			set
			{
				this._ctorThread = value;
			}
		}

		internal static IMessage CallProcessMessage(IMessageSink ms, IMessage reqMsg, ArrayWithSize proxySinks, Thread currentThread, Context currentContext, bool bSkippingContextChain)
		{
			if (proxySinks != null)
			{
				DynamicPropertyHolder.NotifyDynamicSinks(reqMsg, proxySinks, true, true, false);
			}
			bool flag = false;
			if (bSkippingContextChain)
			{
				flag = currentContext.NotifyDynamicSinks(reqMsg, true, true, false, true);
				ChannelServices.NotifyProfiler(reqMsg, RemotingProfilerEvent.ClientSend);
			}
			if (ms == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Proxy_NoChannelSink"));
			}
			IMessage message = ms.SyncProcessMessage(reqMsg);
			if (bSkippingContextChain)
			{
				ChannelServices.NotifyProfiler(message, RemotingProfilerEvent.ClientReceive);
				if (flag)
				{
					currentContext.NotifyDynamicSinks(message, true, false, false, true);
				}
			}
			IMethodReturnMessage methodReturnMessage = message as IMethodReturnMessage;
			if (message == null || methodReturnMessage == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadType"));
			}
			if (proxySinks != null)
			{
				DynamicPropertyHolder.NotifyDynamicSinks(message, proxySinks, true, false, false);
			}
			return message;
		}

		[SecurityCritical]
		public override IMessage Invoke(IMessage reqMsg)
		{
			IConstructionCallMessage constructionCallMessage = reqMsg as IConstructionCallMessage;
			if (constructionCallMessage != null)
			{
				return this.InternalActivate(constructionCallMessage);
			}
			if (!base.Initialized)
			{
				if (this.CtorThread != Thread.CurrentThread.GetHashCode())
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_Proxy_InvalidCall"));
				}
				ServerIdentity serverIdentity = this.IdentityObject as ServerIdentity;
				RemotingServices.Wrap((ContextBoundObject)base.UnwrappedServerObject);
			}
			int callType = 0;
			Message message = reqMsg as Message;
			if (message != null)
			{
				callType = message.GetCallType();
			}
			return this.InternalInvoke((IMethodCallMessage)reqMsg, false, callType);
		}

		internal virtual IMessage InternalInvoke(IMethodCallMessage reqMcmMsg, bool useDispatchMessage, int callType)
		{
			Message message = reqMcmMsg as Message;
			if (message == null && callType != 0)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Proxy_InvalidCallType"));
			}
			IMessage result = null;
			Thread currentThread = Thread.CurrentThread;
			LogicalCallContext logicalCallContext = currentThread.GetMutableExecutionContext().LogicalCallContext;
			Identity identityObject = this.IdentityObject;
			ServerIdentity serverIdentity = identityObject as ServerIdentity;
			if (serverIdentity != null && identityObject.IsFullyDisconnected())
			{
				throw new ArgumentException(Environment.GetResourceString("Remoting_ServerObjectNotFound", new object[]
				{
					reqMcmMsg.Uri
				}));
			}
			MethodBase methodBase = reqMcmMsg.MethodBase;
			if (RemotingProxy._getTypeMethod == methodBase)
			{
				Type proxiedType = base.GetProxiedType();
				return new ReturnMessage(proxiedType, null, 0, logicalCallContext, reqMcmMsg);
			}
			if (RemotingProxy._getHashCodeMethod == methodBase)
			{
				int hashCode = identityObject.GetHashCode();
				return new ReturnMessage(hashCode, null, 0, logicalCallContext, reqMcmMsg);
			}
			if (identityObject.ChannelSink == null)
			{
				IMessageSink chnlSink = null;
				IMessageSink envoySink = null;
				if (!identityObject.ObjectRef.IsObjRefLite())
				{
					RemotingServices.CreateEnvoyAndChannelSinks(null, identityObject.ObjectRef, out chnlSink, out envoySink);
				}
				else
				{
					RemotingServices.CreateEnvoyAndChannelSinks(identityObject.ObjURI, null, out chnlSink, out envoySink);
				}
				RemotingServices.SetEnvoyAndChannelSinks(identityObject, chnlSink, envoySink);
				if (identityObject.ChannelSink == null)
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_Proxy_NoChannelSink"));
				}
			}
			IInternalMessage internalMessage = (IInternalMessage)reqMcmMsg;
			internalMessage.IdentityObject = identityObject;
			if (serverIdentity != null)
			{
				internalMessage.ServerIdentityObject = serverIdentity;
			}
			else
			{
				internalMessage.SetURI(identityObject.URI);
			}
			switch (callType)
			{
			case 0:
			{
				bool bSkippingContextChain = false;
				Context currentContextInternal = currentThread.GetCurrentContextInternal();
				IMessageSink messageSink = identityObject.EnvoyChain;
				if (currentContextInternal.IsDefaultContext && messageSink is EnvoyTerminatorSink)
				{
					bSkippingContextChain = true;
					messageSink = identityObject.ChannelSink;
				}
				result = RemotingProxy.CallProcessMessage(messageSink, reqMcmMsg, identityObject.ProxySideDynamicSinks, currentThread, currentContextInternal, bSkippingContextChain);
				break;
			}
			case 1:
			case 9:
			{
				logicalCallContext = (LogicalCallContext)logicalCallContext.Clone();
				internalMessage.SetCallContext(logicalCallContext);
				AsyncResult asyncResult = new AsyncResult(message);
				this.InternalInvokeAsync(asyncResult, message, useDispatchMessage, callType);
				result = new ReturnMessage(asyncResult, null, 0, null, message);
				break;
			}
			case 2:
				result = RealProxy.EndInvokeHelper(message, true);
				break;
			case 8:
				logicalCallContext = (LogicalCallContext)logicalCallContext.Clone();
				internalMessage.SetCallContext(logicalCallContext);
				this.InternalInvokeAsync(null, message, useDispatchMessage, callType);
				result = new ReturnMessage(null, null, 0, null, reqMcmMsg);
				break;
			case 10:
				result = new ReturnMessage(null, null, 0, null, reqMcmMsg);
				break;
			}
			return result;
		}

		internal void InternalInvokeAsync(IMessageSink ar, Message reqMsg, bool useDispatchMessage, int callType)
		{
			Identity identityObject = this.IdentityObject;
			ServerIdentity serverIdentity = identityObject as ServerIdentity;
			MethodCall methodCall = new MethodCall(reqMsg);
			IInternalMessage internalMessage = methodCall;
			internalMessage.IdentityObject = identityObject;
			if (serverIdentity != null)
			{
				internalMessage.ServerIdentityObject = serverIdentity;
			}
			if (useDispatchMessage)
			{
				IMessageCtrl messageCtrl = ChannelServices.AsyncDispatchMessage(methodCall, ((callType & 8) != 0) ? null : ar);
			}
			else
			{
				if (identityObject.EnvoyChain == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Remoting_Proxy_InvalidState"));
				}
				IMessageCtrl messageCtrl = identityObject.EnvoyChain.AsyncProcessMessage(methodCall, ((callType & 8) != 0) ? null : ar);
			}
			if ((callType & 1) != 0 && (callType & 8) != 0)
			{
				ar.SyncProcessMessage(null);
			}
		}

		private IConstructionReturnMessage InternalActivate(IConstructionCallMessage ctorMsg)
		{
			this.CtorThread = Thread.CurrentThread.GetHashCode();
			IConstructionReturnMessage result = ActivationServices.Activate(this, ctorMsg);
			base.Initialized = true;
			return result;
		}

		private static void Invoke(object NotUsed, ref MessageData msgData)
		{
			Message message = new Message();
			message.InitFields(msgData);
			object thisPtr = message.GetThisPtr();
			Delegate @delegate;
			if ((@delegate = (thisPtr as Delegate)) == null)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Default"));
			}
			RemotingProxy remotingProxy = (RemotingProxy)RemotingServices.GetRealProxy(@delegate.Target);
			if (remotingProxy != null)
			{
				remotingProxy.InternalInvoke(message, true, message.GetCallType());
				return;
			}
			int callType = message.GetCallType();
			if (callType <= 2)
			{
				if (callType != 1)
				{
					if (callType != 2)
					{
						return;
					}
					RealProxy.EndInvokeHelper(message, false);
					return;
				}
			}
			else if (callType != 9)
			{
				return;
			}
			message.Properties[Message.CallContextKey] = Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext.Clone();
			AsyncResult asyncResult = new AsyncResult(message);
			AgileAsyncWorkerItem state = new AgileAsyncWorkerItem(message, ((callType & 8) != 0) ? null : asyncResult, @delegate.Target);
			ThreadPool.QueueUserWorkItem(new WaitCallback(AgileAsyncWorkerItem.ThreadPoolCallBack), state);
			if ((callType & 8) != 0)
			{
				asyncResult.SyncProcessMessage(null);
			}
			message.PropagateOutParameters(null, asyncResult);
		}

		internal ConstructorCallMessage ConstructorMessage
		{
			get
			{
				return this._ccm;
			}
			set
			{
				this._ccm = value;
			}
		}

		public string TypeName
		{
			[SecurityCritical]
			get
			{
				return base.GetProxiedType().FullName;
			}
			[SecurityCritical]
			set
			{
				throw new NotSupportedException();
			}
		}

		[SecurityCritical]
		public override IntPtr GetCOMIUnknown(bool fIsBeingMarshalled)
		{
			IntPtr result = IntPtr.Zero;
			object transparentProxy = this.GetTransparentProxy();
			bool flag = RemotingServices.IsObjectOutOfProcess(transparentProxy);
			if (flag)
			{
				if (fIsBeingMarshalled)
				{
					result = MarshalByRefObject.GetComIUnknown((MarshalByRefObject)transparentProxy);
				}
				else
				{
					result = MarshalByRefObject.GetComIUnknown((MarshalByRefObject)transparentProxy);
				}
			}
			else
			{
				bool flag2 = RemotingServices.IsObjectOutOfAppDomain(transparentProxy);
				if (flag2)
				{
					result = ((MarshalByRefObject)transparentProxy).GetComIUnknown(fIsBeingMarshalled);
				}
				else
				{
					result = MarshalByRefObject.GetComIUnknown((MarshalByRefObject)transparentProxy);
				}
			}
			return result;
		}

		[SecurityCritical]
		public override void SetCOMIUnknown(IntPtr i)
		{
		}

		[SecurityCritical]
		public bool CanCastTo(Type castType, object o)
		{
			if (castType == null)
			{
				throw new ArgumentNullException("castType");
			}
			RuntimeType runtimeType = castType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"));
			}
			bool flag = false;
			if (runtimeType == RemotingProxy.s_typeofObject || runtimeType == RemotingProxy.s_typeofMarshalByRefObject)
			{
				return true;
			}
			ObjRef objectRef = this.IdentityObject.ObjectRef;
			if (objectRef != null)
			{
				object transparentProxy = this.GetTransparentProxy();
				IRemotingTypeInfo typeInfo = objectRef.TypeInfo;
				if (typeInfo != null)
				{
					flag = typeInfo.CanCastTo(runtimeType, transparentProxy);
					if (!flag && typeInfo.GetType() == typeof(TypeInfo) && objectRef.IsWellKnown())
					{
						flag = this.CanCastToWK(runtimeType);
					}
				}
				else if (objectRef.IsObjRefLite())
				{
					flag = MarshalByRefObject.CanCastToXmlTypeHelper(runtimeType, (MarshalByRefObject)o);
				}
			}
			else
			{
				flag = this.CanCastToWK(runtimeType);
			}
			return flag;
		}

		private bool CanCastToWK(Type castType)
		{
			bool result = false;
			if (castType.IsClass)
			{
				result = base.GetProxiedType().IsAssignableFrom(castType);
			}
			else if (!(this.IdentityObject is ServerIdentity))
			{
				result = true;
			}
			return result;
		}

		private static MethodInfo _getTypeMethod = typeof(object).GetMethod("GetType");

		private static MethodInfo _getHashCodeMethod = typeof(object).GetMethod("GetHashCode");

		private static RuntimeType s_typeofObject = (RuntimeType)typeof(object);

		private static RuntimeType s_typeofMarshalByRefObject = (RuntimeType)typeof(MarshalByRefObject);

		private ConstructorCallMessage _ccm;

		private int _ctorThread;
	}
}
