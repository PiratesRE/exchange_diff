using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.ProcessManager
{
	internal sealed class ControlObject : WorkerControlObject
	{
		public ControlObject(PipeStream readPipeStream, ControlObject.TransportWorker transportWorkerArg) : base(readPipeStream, transportWorkerArg, ExTraceGlobals.ProcessManagerTracer)
		{
			this.transportWorker = transportWorkerArg;
		}

		public bool SeenActivateCommand
		{
			get
			{
				return this.seenActivateCommand;
			}
		}

		protected override bool ProcessCommand(char command, int size, Stream data)
		{
			if (command <= 'F')
			{
				if (command == 'A')
				{
					this.seenActivateCommand = true;
					this.transportWorker.Activate();
					return true;
				}
				if (command == 'F')
				{
					this.transportWorker.HandleLogFlush();
					return true;
				}
			}
			else
			{
				switch (command)
				{
				case 'L':
					this.transportWorker.ClearConfigCache();
					return true;
				case 'M':
					this.transportWorker.HandleMemoryPressure();
					return true;
				case 'N':
				{
					if (size < 1)
					{
						this.socketCreationErrors += 1L;
						ExTraceGlobals.ProcessManagerTracer.TraceError<int, long>(0L, "A unexpected new connection message with size {0} received. Connection error count = {1}", size, this.socketCreationErrors);
						return true;
					}
					Socket socket = null;
					SocketInformation socketInformation;
					try
					{
						socketInformation = (SocketInformation)ControlObject.socketInfoFormatter.Deserialize(data);
					}
					catch (SerializationException arg)
					{
						this.socketCreationErrors += 1L;
						ExTraceGlobals.ProcessManagerTracer.TraceError<SerializationException, long>(0L, "ControlObject.CommandReceived: SerializationException: {0}. Connection error count = {1}", arg, this.socketCreationErrors);
						return true;
					}
					try
					{
						socket = new Socket(socketInformation);
					}
					catch (SocketException arg2)
					{
						this.socketCreationErrors += 1L;
						ExTraceGlobals.ProcessManagerTracer.TraceError<SocketException, long>(0L, "ControlObject.CommandReceived: SocketException: {0}. Connection error count = {1}", arg2, this.socketCreationErrors);
						return true;
					}
					if (socket != null)
					{
						this.transportWorker.HandleConnection(socket);
						return true;
					}
					return true;
				}
				case 'O':
				case 'P':
					break;
				case 'Q':
					this.transportWorker.HandleBlockedSubmissionQueue();
					return true;
				default:
					switch (command)
					{
					case 'U':
						this.transportWorker.ConfigUpdate();
						return true;
					case 'W':
						this.transportWorker.HandleForceCrash();
						return true;
					}
					break;
				}
			}
			return false;
		}

		private readonly ControlObject.TransportWorker transportWorker;

		private static IFormatter socketInfoFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);

		private bool seenActivateCommand;

		private long socketCreationErrors;

		public interface TransportWorker : IWorkerProcess
		{
			void Activate();

			void ConfigUpdate();

			void HandleMemoryPressure();

			void HandleLogFlush();

			void HandleBlockedSubmissionQueue();

			void ClearConfigCache();

			void HandleForceCrash();

			void HandleConnection(Socket clientConnection);
		}

		private static class ServiceToWorkerCommands
		{
			public const char NewConnection = 'N';

			public const char Activate = 'A';

			public const char ConfigUpdate = 'U';

			public const char HandleMemoryPressure = 'M';

			public const char ClearConfigCache = 'L';

			public const char HandleLogFlush = 'F';

			public const char HandleBlockedSubmissionQueue = 'Q';

			public const char HandleForceCrash = 'W';
		}
	}
}
