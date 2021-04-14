using System;
using System.Collections;
using System.Runtime.Remoting.Activation;
using System.Runtime.Serialization;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	internal class MessageSurrogate : ISerializationSurrogate
	{
		[SecurityCritical]
		internal MessageSurrogate(RemotingSurrogateSelector ss)
		{
			this._ss = ss;
		}

		[SecurityCritical]
		public virtual void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			bool flag = false;
			bool flag2 = false;
			IMethodMessage methodMessage = obj as IMethodMessage;
			if (methodMessage != null)
			{
				IDictionaryEnumerator enumerator = methodMessage.Properties.GetEnumerator();
				if (methodMessage is IMethodCallMessage)
				{
					if (obj is IConstructionCallMessage)
					{
						flag2 = true;
					}
					info.SetType(flag2 ? MessageSurrogate._constructionCallType : MessageSurrogate._methodCallType);
				}
				else
				{
					IMethodReturnMessage methodReturnMessage = methodMessage as IMethodReturnMessage;
					if (methodReturnMessage == null)
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_InvalidMsg"));
					}
					flag = true;
					info.SetType((obj is IConstructionReturnMessage) ? MessageSurrogate._constructionResponseType : MessageSurrogate._methodResponseType);
					if (((IMethodReturnMessage)methodMessage).Exception != null)
					{
						info.AddValue("__fault", ((IMethodReturnMessage)methodMessage).Exception, MessageSurrogate._exceptionType);
					}
				}
				while (enumerator.MoveNext())
				{
					if (obj != this._ss.GetRootObject() || this._ss.Filter == null || !this._ss.Filter((string)enumerator.Key, enumerator.Value))
					{
						if (enumerator.Value != null)
						{
							string text = enumerator.Key.ToString();
							if (text.Equals("__CallContext"))
							{
								LogicalCallContext logicalCallContext = (LogicalCallContext)enumerator.Value;
								if (logicalCallContext.HasInfo)
								{
									info.AddValue(text, logicalCallContext);
								}
								else
								{
									info.AddValue(text, logicalCallContext.RemotingData.LogicalCallID);
								}
							}
							else if (text.Equals("__MethodSignature"))
							{
								if (flag2 || RemotingServices.IsMethodOverloaded(methodMessage))
								{
									info.AddValue(text, enumerator.Value);
								}
							}
							else
							{
								flag = flag;
								info.AddValue(text, enumerator.Value);
							}
						}
						else
						{
							info.AddValue(enumerator.Key.ToString(), enumerator.Value, MessageSurrogate._objectType);
						}
					}
				}
				return;
			}
			throw new RemotingException(Environment.GetResourceString("Remoting_InvalidMsg"));
		}

		[SecurityCritical]
		public virtual object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_PopulateData"));
		}

		private static Type _constructionCallType = typeof(ConstructionCall);

		private static Type _methodCallType = typeof(MethodCall);

		private static Type _constructionResponseType = typeof(ConstructionResponse);

		private static Type _methodResponseType = typeof(MethodResponse);

		private static Type _exceptionType = typeof(Exception);

		private static Type _objectType = typeof(object);

		[SecurityCritical]
		private RemotingSurrogateSelector _ss;
	}
}
