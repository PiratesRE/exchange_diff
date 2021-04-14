using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public class Context
	{
		[SecurityCritical]
		public Context() : this(0)
		{
		}

		[SecurityCritical]
		private Context(int flags)
		{
			this._ctxFlags = flags;
			if ((this._ctxFlags & 1) != 0)
			{
				this._ctxID = 0;
			}
			else
			{
				this._ctxID = Interlocked.Increment(ref Context._ctxIDCounter);
			}
			DomainSpecificRemotingData remotingData = Thread.GetDomain().RemotingData;
			if (remotingData != null)
			{
				IContextProperty[] appDomainContextProperties = remotingData.AppDomainContextProperties;
				if (appDomainContextProperties != null)
				{
					for (int i = 0; i < appDomainContextProperties.Length; i++)
					{
						this.SetProperty(appDomainContextProperties[i]);
					}
				}
			}
			if ((this._ctxFlags & 1) != 0)
			{
				this.Freeze();
			}
			this.SetupInternalContext((this._ctxFlags & 1) == 1);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetupInternalContext(bool bDefault);

		[SecuritySafeCritical]
		~Context()
		{
			if (this._internalContext != IntPtr.Zero && (this._ctxFlags & 1) == 0)
			{
				this.CleanupInternalContext();
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CleanupInternalContext();

		public virtual int ContextID
		{
			[SecurityCritical]
			get
			{
				return this._ctxID;
			}
		}

		internal virtual IntPtr InternalContextID
		{
			get
			{
				return this._internalContext;
			}
		}

		internal virtual AppDomain AppDomain
		{
			get
			{
				return this._appDomain;
			}
		}

		internal bool IsDefaultContext
		{
			get
			{
				return this._ctxID == 0;
			}
		}

		public static Context DefaultContext
		{
			[SecurityCritical]
			get
			{
				return Thread.GetDomain().GetDefaultContext();
			}
		}

		[SecurityCritical]
		internal static Context CreateDefaultContext()
		{
			return new Context(1);
		}

		[SecurityCritical]
		public virtual IContextProperty GetProperty(string name)
		{
			if (this._ctxProps == null || name == null)
			{
				return null;
			}
			IContextProperty result = null;
			for (int i = 0; i < this._numCtxProps; i++)
			{
				if (this._ctxProps[i].Name.Equals(name))
				{
					result = this._ctxProps[i];
					break;
				}
			}
			return result;
		}

		[SecurityCritical]
		public virtual void SetProperty(IContextProperty prop)
		{
			if (prop == null || prop.Name == null)
			{
				throw new ArgumentNullException((prop == null) ? "prop" : "property name");
			}
			if ((this._ctxFlags & 2) != 0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_AddContextFrozen"));
			}
			lock (this)
			{
				Context.CheckPropertyNameClash(prop.Name, this._ctxProps, this._numCtxProps);
				if (this._ctxProps == null || this._numCtxProps == this._ctxProps.Length)
				{
					this._ctxProps = Context.GrowPropertiesArray(this._ctxProps);
				}
				IContextProperty[] ctxProps = this._ctxProps;
				int numCtxProps = this._numCtxProps;
				this._numCtxProps = numCtxProps + 1;
				ctxProps[numCtxProps] = prop;
			}
		}

		[SecurityCritical]
		internal virtual void InternalFreeze()
		{
			this._ctxFlags |= 2;
			for (int i = 0; i < this._numCtxProps; i++)
			{
				this._ctxProps[i].Freeze(this);
			}
		}

		[SecurityCritical]
		public virtual void Freeze()
		{
			lock (this)
			{
				if ((this._ctxFlags & 2) != 0)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ContextAlreadyFrozen"));
				}
				this.InternalFreeze();
			}
		}

		internal virtual void SetThreadPoolAware()
		{
			this._ctxFlags |= 4;
		}

		internal virtual bool IsThreadPoolAware
		{
			get
			{
				return (this._ctxFlags & 4) == 4;
			}
		}

		public virtual IContextProperty[] ContextProperties
		{
			[SecurityCritical]
			get
			{
				if (this._ctxProps == null)
				{
					return null;
				}
				IContextProperty[] result;
				lock (this)
				{
					IContextProperty[] array = new IContextProperty[this._numCtxProps];
					Array.Copy(this._ctxProps, array, this._numCtxProps);
					result = array;
				}
				return result;
			}
		}

		[SecurityCritical]
		internal static void CheckPropertyNameClash(string name, IContextProperty[] props, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (props[i].Name.Equals(name))
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_DuplicatePropertyName"));
				}
			}
		}

		internal static IContextProperty[] GrowPropertiesArray(IContextProperty[] props)
		{
			int num = ((props != null) ? props.Length : 0) + 8;
			IContextProperty[] array = new IContextProperty[num];
			if (props != null)
			{
				Array.Copy(props, array, props.Length);
			}
			return array;
		}

		[SecurityCritical]
		internal virtual IMessageSink GetServerContextChain()
		{
			if (this._serverContextChain == null)
			{
				IMessageSink messageSink = ServerContextTerminatorSink.MessageSink;
				int numCtxProps = this._numCtxProps;
				while (numCtxProps-- > 0)
				{
					object obj = this._ctxProps[numCtxProps];
					IContributeServerContextSink contributeServerContextSink = obj as IContributeServerContextSink;
					if (contributeServerContextSink != null)
					{
						messageSink = contributeServerContextSink.GetServerContextSink(messageSink);
						if (messageSink == null)
						{
							throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
						}
					}
				}
				lock (this)
				{
					if (this._serverContextChain == null)
					{
						this._serverContextChain = messageSink;
					}
				}
			}
			return this._serverContextChain;
		}

		[SecurityCritical]
		internal virtual IMessageSink GetClientContextChain()
		{
			if (this._clientContextChain == null)
			{
				IMessageSink messageSink = ClientContextTerminatorSink.MessageSink;
				for (int i = 0; i < this._numCtxProps; i++)
				{
					object obj = this._ctxProps[i];
					IContributeClientContextSink contributeClientContextSink = obj as IContributeClientContextSink;
					if (contributeClientContextSink != null)
					{
						messageSink = contributeClientContextSink.GetClientContextSink(messageSink);
						if (messageSink == null)
						{
							throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
						}
					}
				}
				lock (this)
				{
					if (this._clientContextChain == null)
					{
						this._clientContextChain = messageSink;
					}
				}
			}
			return this._clientContextChain;
		}

		[SecurityCritical]
		internal virtual IMessageSink CreateServerObjectChain(MarshalByRefObject serverObj)
		{
			IMessageSink messageSink = new ServerObjectTerminatorSink(serverObj);
			int numCtxProps = this._numCtxProps;
			while (numCtxProps-- > 0)
			{
				object obj = this._ctxProps[numCtxProps];
				IContributeObjectSink contributeObjectSink = obj as IContributeObjectSink;
				if (contributeObjectSink != null)
				{
					messageSink = contributeObjectSink.GetObjectSink(serverObj, messageSink);
					if (messageSink == null)
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
					}
				}
			}
			return messageSink;
		}

		[SecurityCritical]
		internal virtual IMessageSink CreateEnvoyChain(MarshalByRefObject objectOrProxy)
		{
			IMessageSink messageSink = EnvoyTerminatorSink.MessageSink;
			for (int i = 0; i < this._numCtxProps; i++)
			{
				object obj = this._ctxProps[i];
				IContributeEnvoySink contributeEnvoySink = obj as IContributeEnvoySink;
				if (contributeEnvoySink != null)
				{
					messageSink = contributeEnvoySink.GetEnvoySink(objectOrProxy, messageSink);
					if (messageSink == null)
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
					}
				}
			}
			return messageSink;
		}

		[SecurityCritical]
		internal IMessage NotifyActivatorProperties(IMessage msg, bool bServerSide)
		{
			IMessage message = null;
			try
			{
				int numCtxProps = this._numCtxProps;
				while (numCtxProps-- != 0)
				{
					object obj = this._ctxProps[numCtxProps];
					IContextPropertyActivator contextPropertyActivator = obj as IContextPropertyActivator;
					if (contextPropertyActivator != null)
					{
						IConstructionCallMessage constructionCallMessage = msg as IConstructionCallMessage;
						if (constructionCallMessage != null)
						{
							if (!bServerSide)
							{
								contextPropertyActivator.CollectFromClientContext(constructionCallMessage);
							}
							else
							{
								contextPropertyActivator.DeliverClientContextToServerContext(constructionCallMessage);
							}
						}
						else if (bServerSide)
						{
							contextPropertyActivator.CollectFromServerContext((IConstructionReturnMessage)msg);
						}
						else
						{
							contextPropertyActivator.DeliverServerContextToClientContext((IConstructionReturnMessage)msg);
						}
					}
				}
			}
			catch (Exception e)
			{
				IMethodCallMessage mcm;
				if (msg is IConstructionCallMessage)
				{
					mcm = (IMethodCallMessage)msg;
				}
				else
				{
					mcm = new ErrorMessage();
				}
				message = new ReturnMessage(e, mcm);
				if (msg != null)
				{
					((ReturnMessage)message).SetLogicalCallContext((LogicalCallContext)msg.Properties[Message.CallContextKey]);
				}
			}
			return message;
		}

		public override string ToString()
		{
			return "ContextID: " + this._ctxID;
		}

		[SecurityCritical]
		public void DoCallBack(CrossContextDelegate deleg)
		{
			if (deleg == null)
			{
				throw new ArgumentNullException("deleg");
			}
			if ((this._ctxFlags & 2) == 0)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_ContextNotFrozenForCallBack"));
			}
			Context currentContext = Thread.CurrentContext;
			if (currentContext == this)
			{
				deleg();
				return;
			}
			currentContext.DoCallBackGeneric(this.InternalContextID, deleg);
			GC.KeepAlive(this);
		}

		[SecurityCritical]
		internal static void DoCallBackFromEE(IntPtr targetCtxID, IntPtr privateData, int targetDomainID)
		{
			if (targetDomainID == 0)
			{
				CallBackHelper @object = new CallBackHelper(privateData, true, targetDomainID);
				CrossContextDelegate deleg = new CrossContextDelegate(@object.Func);
				Thread.CurrentContext.DoCallBackGeneric(targetCtxID, deleg);
				return;
			}
			TransitionCall msg = new TransitionCall(targetCtxID, privateData, targetDomainID);
			Message.PropagateCallContextFromThreadToMessage(msg);
			IMessage message = Thread.CurrentContext.GetClientContextChain().SyncProcessMessage(msg);
			Message.PropagateCallContextFromMessageToThread(message);
			IMethodReturnMessage methodReturnMessage = message as IMethodReturnMessage;
			if (methodReturnMessage != null && methodReturnMessage.Exception != null)
			{
				throw methodReturnMessage.Exception;
			}
		}

		[SecurityCritical]
		internal void DoCallBackGeneric(IntPtr targetCtxID, CrossContextDelegate deleg)
		{
			TransitionCall msg = new TransitionCall(targetCtxID, deleg);
			Message.PropagateCallContextFromThreadToMessage(msg);
			IMessage message = this.GetClientContextChain().SyncProcessMessage(msg);
			if (message != null)
			{
				Message.PropagateCallContextFromMessageToThread(message);
			}
			IMethodReturnMessage methodReturnMessage = message as IMethodReturnMessage;
			if (methodReturnMessage != null && methodReturnMessage.Exception != null)
			{
				throw methodReturnMessage.Exception;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ExecuteCallBackInEE(IntPtr privateData);

		private LocalDataStore MyLocalStore
		{
			get
			{
				if (this._localDataStore == null)
				{
					LocalDataStoreMgr localDataStoreMgr = Context._localDataStoreMgr;
					lock (localDataStoreMgr)
					{
						if (this._localDataStore == null)
						{
							this._localDataStore = Context._localDataStoreMgr.CreateLocalDataStore();
						}
					}
				}
				return this._localDataStore.Store;
			}
		}

		[SecurityCritical]
		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return Context._localDataStoreMgr.AllocateDataSlot();
		}

		[SecurityCritical]
		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			return Context._localDataStoreMgr.AllocateNamedDataSlot(name);
		}

		[SecurityCritical]
		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			return Context._localDataStoreMgr.GetNamedDataSlot(name);
		}

		[SecurityCritical]
		public static void FreeNamedDataSlot(string name)
		{
			Context._localDataStoreMgr.FreeNamedDataSlot(name);
		}

		[SecurityCritical]
		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			Thread.CurrentContext.MyLocalStore.SetData(slot, data);
		}

		[SecurityCritical]
		public static object GetData(LocalDataStoreSlot slot)
		{
			return Thread.CurrentContext.MyLocalStore.GetData(slot);
		}

		private int ReserveSlot()
		{
			if (this._ctxStatics == null)
			{
				this._ctxStatics = new object[8];
				this._ctxStatics[0] = null;
				this._ctxStaticsFreeIndex = 1;
				this._ctxStaticsCurrentBucket = 0;
			}
			if (this._ctxStaticsFreeIndex == 8)
			{
				object[] array = new object[8];
				object[] array2 = this._ctxStatics;
				while (array2[0] != null)
				{
					array2 = (object[])array2[0];
				}
				array2[0] = array;
				this._ctxStaticsFreeIndex = 1;
				this._ctxStaticsCurrentBucket++;
			}
			int ctxStaticsFreeIndex = this._ctxStaticsFreeIndex;
			this._ctxStaticsFreeIndex = ctxStaticsFreeIndex + 1;
			return ctxStaticsFreeIndex | this._ctxStaticsCurrentBucket << 16;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
		public static bool RegisterDynamicProperty(IDynamicProperty prop, ContextBoundObject obj, Context ctx)
		{
			if (prop == null || prop.Name == null || !(prop is IContributeDynamicSink))
			{
				throw new ArgumentNullException("prop");
			}
			if (obj != null && ctx != null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NonNullObjAndCtx"));
			}
			bool result;
			if (obj != null)
			{
				result = IdentityHolder.AddDynamicProperty(obj, prop);
			}
			else
			{
				result = Context.AddDynamicProperty(ctx, prop);
			}
			return result;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
		public static bool UnregisterDynamicProperty(string name, ContextBoundObject obj, Context ctx)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (obj != null && ctx != null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NonNullObjAndCtx"));
			}
			bool result;
			if (obj != null)
			{
				result = IdentityHolder.RemoveDynamicProperty(obj, name);
			}
			else
			{
				result = Context.RemoveDynamicProperty(ctx, name);
			}
			return result;
		}

		[SecurityCritical]
		internal static bool AddDynamicProperty(Context ctx, IDynamicProperty prop)
		{
			if (ctx != null)
			{
				return ctx.AddPerContextDynamicProperty(prop);
			}
			return Context.AddGlobalDynamicProperty(prop);
		}

		[SecurityCritical]
		private bool AddPerContextDynamicProperty(IDynamicProperty prop)
		{
			if (this._dphCtx == null)
			{
				DynamicPropertyHolder dphCtx = new DynamicPropertyHolder();
				lock (this)
				{
					if (this._dphCtx == null)
					{
						this._dphCtx = dphCtx;
					}
				}
			}
			return this._dphCtx.AddDynamicProperty(prop);
		}

		[SecurityCritical]
		private static bool AddGlobalDynamicProperty(IDynamicProperty prop)
		{
			return Context._dphGlobal.AddDynamicProperty(prop);
		}

		[SecurityCritical]
		internal static bool RemoveDynamicProperty(Context ctx, string name)
		{
			if (ctx != null)
			{
				return ctx.RemovePerContextDynamicProperty(name);
			}
			return Context.RemoveGlobalDynamicProperty(name);
		}

		[SecurityCritical]
		private bool RemovePerContextDynamicProperty(string name)
		{
			if (this._dphCtx == null)
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Contexts_NoProperty"), name));
			}
			return this._dphCtx.RemoveDynamicProperty(name);
		}

		[SecurityCritical]
		private static bool RemoveGlobalDynamicProperty(string name)
		{
			return Context._dphGlobal.RemoveDynamicProperty(name);
		}

		internal virtual IDynamicProperty[] PerContextDynamicProperties
		{
			get
			{
				if (this._dphCtx == null)
				{
					return null;
				}
				return this._dphCtx.DynamicProperties;
			}
		}

		internal static ArrayWithSize GlobalDynamicSinks
		{
			[SecurityCritical]
			get
			{
				return Context._dphGlobal.DynamicSinks;
			}
		}

		internal virtual ArrayWithSize DynamicSinks
		{
			[SecurityCritical]
			get
			{
				if (this._dphCtx == null)
				{
					return null;
				}
				return this._dphCtx.DynamicSinks;
			}
		}

		[SecurityCritical]
		internal virtual bool NotifyDynamicSinks(IMessage msg, bool bCliSide, bool bStart, bool bAsync, bool bNotifyGlobals)
		{
			bool result = false;
			if (bNotifyGlobals && Context._dphGlobal.DynamicProperties != null)
			{
				ArrayWithSize globalDynamicSinks = Context.GlobalDynamicSinks;
				if (globalDynamicSinks != null)
				{
					DynamicPropertyHolder.NotifyDynamicSinks(msg, globalDynamicSinks, bCliSide, bStart, bAsync);
					result = true;
				}
			}
			ArrayWithSize dynamicSinks = this.DynamicSinks;
			if (dynamicSinks != null)
			{
				DynamicPropertyHolder.NotifyDynamicSinks(msg, dynamicSinks, bCliSide, bStart, bAsync);
				result = true;
			}
			return result;
		}

		internal const int CTX_DEFAULT_CONTEXT = 1;

		internal const int CTX_FROZEN = 2;

		internal const int CTX_THREADPOOL_AWARE = 4;

		private const int GROW_BY = 8;

		private const int STATICS_BUCKET_SIZE = 8;

		private IContextProperty[] _ctxProps;

		private DynamicPropertyHolder _dphCtx;

		private volatile LocalDataStoreHolder _localDataStore;

		private IMessageSink _serverContextChain;

		private IMessageSink _clientContextChain;

		private AppDomain _appDomain;

		private object[] _ctxStatics;

		private IntPtr _internalContext;

		private int _ctxID;

		private int _ctxFlags;

		private int _numCtxProps;

		private int _ctxStaticsCurrentBucket;

		private int _ctxStaticsFreeIndex;

		private static DynamicPropertyHolder _dphGlobal = new DynamicPropertyHolder();

		private static LocalDataStoreMgr _localDataStoreMgr = new LocalDataStoreMgr();

		private static int _ctxIDCounter = 0;
	}
}
