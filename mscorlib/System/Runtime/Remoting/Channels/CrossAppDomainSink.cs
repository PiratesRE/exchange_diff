using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace System.Runtime.Remoting.Channels
{
	internal class CrossAppDomainSink : InternalSink, IMessageSink
	{
		internal CrossAppDomainSink(CrossAppDomainData xadData)
		{
			this._xadData = xadData;
		}

		internal static void GrowArrays(int oldSize)
		{
			if (CrossAppDomainSink._sinks == null)
			{
				CrossAppDomainSink._sinks = new CrossAppDomainSink[8];
				CrossAppDomainSink._sinkKeys = new int[8];
				return;
			}
			CrossAppDomainSink[] array = new CrossAppDomainSink[CrossAppDomainSink._sinks.Length + 8];
			int[] array2 = new int[CrossAppDomainSink._sinkKeys.Length + 8];
			Array.Copy(CrossAppDomainSink._sinks, array, CrossAppDomainSink._sinks.Length);
			Array.Copy(CrossAppDomainSink._sinkKeys, array2, CrossAppDomainSink._sinkKeys.Length);
			CrossAppDomainSink._sinks = array;
			CrossAppDomainSink._sinkKeys = array2;
		}

		internal static CrossAppDomainSink FindOrCreateSink(CrossAppDomainData xadData)
		{
			object obj = CrossAppDomainSink.staticSyncObject;
			CrossAppDomainSink result;
			lock (obj)
			{
				int domainID = xadData.DomainID;
				if (CrossAppDomainSink._sinks == null)
				{
					CrossAppDomainSink.GrowArrays(0);
				}
				int num = 0;
				while (CrossAppDomainSink._sinks[num] != null)
				{
					if (CrossAppDomainSink._sinkKeys[num] == domainID)
					{
						return CrossAppDomainSink._sinks[num];
					}
					num++;
					if (num == CrossAppDomainSink._sinks.Length)
					{
						CrossAppDomainSink.GrowArrays(num);
						break;
					}
				}
				CrossAppDomainSink._sinks[num] = new CrossAppDomainSink(xadData);
				CrossAppDomainSink._sinkKeys[num] = domainID;
				result = CrossAppDomainSink._sinks[num];
			}
			return result;
		}

		internal static void DomainUnloaded(int domainID)
		{
			object obj = CrossAppDomainSink.staticSyncObject;
			lock (obj)
			{
				if (CrossAppDomainSink._sinks != null)
				{
					int num = 0;
					int num2 = -1;
					while (CrossAppDomainSink._sinks[num] != null)
					{
						if (CrossAppDomainSink._sinkKeys[num] == domainID)
						{
							num2 = num;
						}
						num++;
						if (num == CrossAppDomainSink._sinks.Length)
						{
							break;
						}
					}
					if (num2 != -1)
					{
						CrossAppDomainSink._sinkKeys[num2] = CrossAppDomainSink._sinkKeys[num - 1];
						CrossAppDomainSink._sinks[num2] = CrossAppDomainSink._sinks[num - 1];
						CrossAppDomainSink._sinkKeys[num - 1] = 0;
						CrossAppDomainSink._sinks[num - 1] = null;
					}
				}
			}
		}

		[SecurityCritical]
		internal static byte[] DoDispatch(byte[] reqStmBuff, SmuggledMethodCallMessage smuggledMcm, out SmuggledMethodReturnMessage smuggledMrm)
		{
			IMessage msg;
			if (smuggledMcm != null)
			{
				ArrayList deserializedArgs = smuggledMcm.FixupForNewAppDomain();
				msg = new MethodCall(smuggledMcm, deserializedArgs);
			}
			else
			{
				MemoryStream stm = new MemoryStream(reqStmBuff);
				msg = CrossAppDomainSerializer.DeserializeMessage(stm);
			}
			LogicalCallContext logicalCallContext = Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext;
			logicalCallContext.SetData("__xADCall", true);
			IMessage message = ChannelServices.SyncDispatchMessage(msg);
			logicalCallContext.FreeNamedDataSlot("__xADCall");
			smuggledMrm = SmuggledMethodReturnMessage.SmuggleIfPossible(message);
			if (smuggledMrm != null)
			{
				return null;
			}
			if (message != null)
			{
				LogicalCallContext logicalCallContext2 = (LogicalCallContext)message.Properties[Message.CallContextKey];
				if (logicalCallContext2 != null && logicalCallContext2.Principal != null)
				{
					logicalCallContext2.Principal = null;
				}
				return CrossAppDomainSerializer.SerializeMessage(message).GetBuffer();
			}
			return null;
		}

		[SecurityCritical]
		internal static object DoTransitionDispatchCallback(object[] args)
		{
			byte[] reqStmBuff = (byte[])args[0];
			SmuggledMethodCallMessage smuggledMcm = (SmuggledMethodCallMessage)args[1];
			SmuggledMethodReturnMessage smuggledMethodReturnMessage = null;
			byte[] result = null;
			try
			{
				result = CrossAppDomainSink.DoDispatch(reqStmBuff, smuggledMcm, out smuggledMethodReturnMessage);
			}
			catch (Exception e)
			{
				IMessage msg = new ReturnMessage(e, new ErrorMessage());
				result = CrossAppDomainSerializer.SerializeMessage(msg).GetBuffer();
			}
			args[2] = smuggledMethodReturnMessage;
			return result;
		}

		[SecurityCritical]
		internal byte[] DoTransitionDispatch(byte[] reqStmBuff, SmuggledMethodCallMessage smuggledMcm, out SmuggledMethodReturnMessage smuggledMrm)
		{
			object[] array = new object[3];
			array[0] = reqStmBuff;
			array[1] = smuggledMcm;
			object[] array2 = array;
			byte[] result = (byte[])Thread.CurrentThread.InternalCrossContextCallback(null, this._xadData.ContextID, this._xadData.DomainID, CrossAppDomainSink.s_xctxDel, array2);
			smuggledMrm = (SmuggledMethodReturnMessage)array2[2];
			return result;
		}

		[SecurityCritical]
		public virtual IMessage SyncProcessMessage(IMessage reqMsg)
		{
			IMessage message = InternalSink.ValidateMessage(reqMsg);
			if (message != null)
			{
				return message;
			}
			IPrincipal principal = null;
			IMessage message2 = null;
			try
			{
				IMethodCallMessage methodCallMessage = reqMsg as IMethodCallMessage;
				if (methodCallMessage != null)
				{
					LogicalCallContext logicalCallContext = methodCallMessage.LogicalCallContext;
					if (logicalCallContext != null)
					{
						principal = logicalCallContext.RemovePrincipalIfNotSerializable();
					}
				}
				MemoryStream memoryStream = null;
				SmuggledMethodCallMessage smuggledMethodCallMessage = SmuggledMethodCallMessage.SmuggleIfPossible(reqMsg);
				if (smuggledMethodCallMessage == null)
				{
					memoryStream = CrossAppDomainSerializer.SerializeMessage(reqMsg);
				}
				LogicalCallContext logicalCallContext2 = CallContext.SetLogicalCallContext(null);
				byte[] array = null;
				SmuggledMethodReturnMessage smuggledMethodReturnMessage;
				try
				{
					if (smuggledMethodCallMessage != null)
					{
						array = this.DoTransitionDispatch(null, smuggledMethodCallMessage, out smuggledMethodReturnMessage);
					}
					else
					{
						array = this.DoTransitionDispatch(memoryStream.GetBuffer(), null, out smuggledMethodReturnMessage);
					}
				}
				finally
				{
					CallContext.SetLogicalCallContext(logicalCallContext2);
				}
				if (smuggledMethodReturnMessage != null)
				{
					ArrayList deserializedArgs = smuggledMethodReturnMessage.FixupForNewAppDomain();
					message2 = new MethodResponse((IMethodCallMessage)reqMsg, smuggledMethodReturnMessage, deserializedArgs);
				}
				else if (array != null)
				{
					MemoryStream stm = new MemoryStream(array);
					message2 = CrossAppDomainSerializer.DeserializeMessage(stm, reqMsg as IMethodCallMessage);
				}
			}
			catch (Exception e)
			{
				try
				{
					message2 = new ReturnMessage(e, reqMsg as IMethodCallMessage);
				}
				catch (Exception)
				{
				}
			}
			if (principal != null)
			{
				IMethodReturnMessage methodReturnMessage = message2 as IMethodReturnMessage;
				if (methodReturnMessage != null)
				{
					LogicalCallContext logicalCallContext3 = methodReturnMessage.LogicalCallContext;
					logicalCallContext3.Principal = principal;
				}
			}
			return message2;
		}

		[SecurityCritical]
		public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
		{
			ADAsyncWorkItem @object = new ADAsyncWorkItem(reqMsg, this, replySink);
			WaitCallback callBack = new WaitCallback(@object.FinishAsyncWork);
			ThreadPool.QueueUserWorkItem(callBack);
			return null;
		}

		public IMessageSink NextSink
		{
			[SecurityCritical]
			get
			{
				return null;
			}
		}

		internal const int GROW_BY = 8;

		internal static volatile int[] _sinkKeys;

		internal static volatile CrossAppDomainSink[] _sinks;

		internal const string LCC_DATA_KEY = "__xADCall";

		private static object staticSyncObject = new object();

		private static InternalCrossContextDelegate s_xctxDel = new InternalCrossContextDelegate(CrossAppDomainSink.DoTransitionDispatchCallback);

		internal CrossAppDomainData _xadData;
	}
}
