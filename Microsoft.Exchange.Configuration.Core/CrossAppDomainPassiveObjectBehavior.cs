using System;
using System.Collections.Generic;
using System.IO.Pipes;

namespace Microsoft.Exchange.Configuration.Core
{
	internal class CrossAppDomainPassiveObjectBehavior : CrossAppDomainObjectBehavior
	{
		internal CrossAppDomainPassiveObjectBehavior(string namedPipeName, BehaviorDirection direction) : base(namedPipeName, direction)
		{
		}

		internal IEnumerable<string> RecieveMessages()
		{
			return CoreLogger.ExecuteAndLog<IEnumerable<string>>("CrossAppDomainPassiveObjectBehavior.ReceiveMessage", true, null, null, null, delegate()
			{
				IEnumerable<string> result;
				using (NamedPipeClientStream clientStream = new NamedPipeClientStream(".", base.NamedPipeName, PipeDirection.In))
				{
					CrossAppDomainObjectBehavior.ConnectClientStream(clientStream, 1000, base.NamedPipeName, false);
					byte[] array = CrossAppDomainObjectBehavior.LoopReadData((byte[] buffer, int offset, int count) => clientStream.Read(buffer, offset, count));
					if (array == null || array.Length == 0)
					{
						result = null;
					}
					else
					{
						result = CrossAppDomainObjectBehavior.UnpackMessages(array);
					}
				}
				return result;
			});
		}

		internal void SendMessage(byte[] message)
		{
			CoreLogger.ExecuteAndLog("CrossAppDomainPassiveObjectBehavior.ReceiveMessage", true, null, null, delegate()
			{
				using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", this.NamedPipeName, PipeDirection.Out))
				{
					if (CrossAppDomainObjectBehavior.ConnectClientStream(namedPipeClientStream, 1000, this.NamedPipeName, false))
					{
						namedPipeClientStream.Write(message, 0, message.Length);
					}
				}
			});
		}

		private const int DefaultNamedPipeTimeOutInMilliseconds = 1000;
	}
}
