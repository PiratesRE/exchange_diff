using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Configuration.Core.EventLog;

namespace Microsoft.Exchange.Configuration.Core
{
	internal class CrossAppDomainPrimaryObjectBehavior : CrossAppDomainObjectBehavior
	{
		internal CrossAppDomainPrimaryObjectBehavior(string namedPipeName, BehaviorDirection direction, CrossAppDomainPrimaryObjectBehavior.OnMessageReceived onMessagereceived) : base(namedPipeName, direction)
		{
			CrossAppDomainPrimaryObjectBehavior <>4__this = this;
			CoreLogger.ExecuteAndLog("CrossAppDomainPrimaryObjectBehavior.Ctor", true, null, null, delegate()
			{
				if (direction == BehaviorDirection.In)
				{
					<>4__this.onMessageReceived = onMessagereceived;
					<>4__this.serverStream = new NamedPipeServerStream(<>4__this.NamedPipeName, PipeDirection.In, 5);
					<>4__this.receiveMessageThread = new Thread(new ThreadStart(<>4__this.ReceiveMessage));
					<>4__this.receiveMessageThread.Start();
					return;
				}
				<>4__this.serverStream = new NamedPipeServerStream(<>4__this.NamedPipeName, PipeDirection.Out, 5);
				<>4__this.sendMessageThread = new Thread(new ThreadStart(<>4__this.SendMessage));
				<>4__this.sendMessageThread.Start();
			});
		}

		internal override bool IsActive
		{
			get
			{
				if (this.isShutDown || this.serverStream == null)
				{
					return false;
				}
				if (base.Direction == BehaviorDirection.In)
				{
					return this.receiveMessageThread.IsAlive;
				}
				return this.sendMessageThread.IsAlive;
			}
		}

		public void SendMessage(string message)
		{
			CoreLogger.ExecuteAndLog("CrossAppDomainPrimaryObjectBehavior.SendMessage", true, null, null, delegate()
			{
				if (this.IsActive)
				{
					lock (this.sendMessageQueueLocker)
					{
						if (this.IsActive)
						{
							this.sendMessageQueue.Enqueue(message);
						}
					}
				}
			});
		}

		protected override void Dispose(bool isDisposing)
		{
			CoreLogger.ExecuteAndLog("CrossAppDomainPrimaryObjectBehavior.Dispose", false, null, null, delegate()
			{
				try
				{
					if (isDisposing && this.serverStream != null)
					{
						this.isShutDown = true;
						bool flag = false;
						if (this.Direction == BehaviorDirection.In && this.receiveMessageThread.IsAlive)
						{
							using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", this.NamedPipeName, PipeDirection.Out))
							{
								flag = CrossAppDomainObjectBehavior.ConnectClientStream(namedPipeClientStream, 3000, this.NamedPipeName, true);
							}
							if (flag)
							{
								this.receiveMessageThread.Join();
							}
						}
						else if (this.Direction == BehaviorDirection.Out && this.sendMessageThread.IsAlive)
						{
							using (NamedPipeClientStream namedPipeClientStream2 = new NamedPipeClientStream(".", this.NamedPipeName, PipeDirection.In))
							{
								flag = CrossAppDomainObjectBehavior.ConnectClientStream(namedPipeClientStream2, 3000, this.NamedPipeName, true);
							}
							if (flag)
							{
								this.sendMessageThread.Join();
							}
						}
					}
				}
				finally
				{
					if (this.serverStream != null)
					{
						this.serverStream.Dispose();
					}
					this.serverStream = null;
					this.receiveMessageThread = null;
					this.sendMessageThread = null;
					this.<>n__FabricatedMethodb(isDisposing);
				}
			});
		}

		private void ReceiveMessage()
		{
			CoreLogger.ExecuteAndLog("CrossAppDomainPrimaryObjectBehavior.ReceiveMessage", false, null, null, delegate()
			{
				while (!this.isShutDown && this.serverStream != null)
				{
					this.serverStream.WaitForConnection();
					try
					{
						CoreLogger.TraceDebug("Server stream connected.", new object[0]);
						if (!this.isShutDown)
						{
							byte[] array = CrossAppDomainObjectBehavior.LoopReadData((byte[] buffer, int offset, int count) => this.serverStream.Read(buffer, offset, count));
							this.onMessageReceived(array, array.Length);
						}
					}
					finally
					{
						this.serverStream.Disconnect();
					}
				}
			});
		}

		private void SendMessage()
		{
			CoreLogger.ExecuteAndLog("CrossAppDomainPrimaryObjectBehavior.SendMessage", false, null, null, delegate()
			{
				while (!this.isShutDown && this.serverStream != null)
				{
					int num = 0;
					try
					{
						this.serverStream.WaitForConnection();
						try
						{
							if (this.sendMessageQueue.Count != 0)
							{
								lock (this.sendMessageQueueLocker)
								{
									List<string> list = this.sendMessageQueue.ToList<string>();
									if (list != null && list.Count != 0)
									{
										byte[] array = CrossAppDomainObjectBehavior.PackMessages(list);
										this.serverStream.Write(array, 0, array.Length);
										this.sendMessageQueue.Clear();
									}
								}
							}
						}
						finally
						{
							this.serverStream.Disconnect();
						}
					}
					catch (Exception ex)
					{
						if (num >= 4)
						{
							throw;
						}
						ServiceController serviceController = new ServiceController("W3SVC");
						bool flag2 = serviceController.Status == ServiceControllerStatus.Running;
						if (flag2)
						{
							CoreLogger.LogEvent(TaskEventLogConstants.Tuple_CrossAppDomainPrimaryObjectBehaviorException, null, new object[]
							{
								base.NamedPipeName,
								"SendMessage",
								ex.ToString()
							});
						}
						if (this.serverStream != null)
						{
							this.serverStream.Dispose();
						}
						this.serverStream = new NamedPipeServerStream(base.NamedPipeName, PipeDirection.Out, 5);
						num++;
					}
				}
			});
		}

		private const int MaxNumberOfNamedPipeInstances = 5;

		private const int DummyStreamWatingTimeInMilliseconds = 3000;

		private readonly Queue<string> sendMessageQueue = new Queue<string>();

		private readonly object sendMessageQueueLocker = new object();

		private CrossAppDomainPrimaryObjectBehavior.OnMessageReceived onMessageReceived;

		private NamedPipeServerStream serverStream;

		private Thread receiveMessageThread;

		private Thread sendMessageThread;

		private bool isShutDown;

		internal delegate void OnMessageReceived(byte[] message, int receivedSize);
	}
}
