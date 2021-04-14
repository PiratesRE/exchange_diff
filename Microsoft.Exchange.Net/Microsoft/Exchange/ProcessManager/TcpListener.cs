using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.ProcessManager
{
	internal sealed class TcpListener : ITcpListener
	{
		public int MaxConnectionRate
		{
			set
			{
				this.ipConnectionTable.ConnectionRateLimit = value;
			}
		}

		public bool ProcessStopping
		{
			get
			{
				return this.processStopping;
			}
			set
			{
				this.processStopping = value;
			}
		}

		public IPEndPoint[] CurrentBindings
		{
			get
			{
				return this.bindings;
			}
		}

		public TcpListener(TcpListener.HandleFailure failureHandler, TcpListener.HandleConnection connectionHandler, IPEndPoint[] bindings, Trace tracer, ExEventLog eventLogger, int maxConnectionRate, bool exclusiveAddressUse = false, bool disableHandleInheritance = false) : this(failureHandler, connectionHandler, bindings, tracer, new ExEventLogWrapper(eventLogger), maxConnectionRate, exclusiveAddressUse, disableHandleInheritance)
		{
		}

		public TcpListener(TcpListener.HandleFailure failureHandler, TcpListener.HandleConnection connectionHandler, IPEndPoint[] bindings, ITracer tracer, IExEventLog eventLogger, int maxConnectionRate, bool exclusiveAddressUse = false, bool disableHandleInheritance = false)
		{
			this.failureHandler = failureHandler;
			this.connectionHandler = connectionHandler;
			this.bindings = bindings;
			this.tracer = tracer;
			this.eventLogger = eventLogger;
			this.ipConnectionTable = new IPConnectionTable(maxConnectionRate);
			this.exclusiveAddressUse = exclusiveAddressUse;
			this.disableHandleInheritance = disableHandleInheritance;
		}

		public static void CreatePersistentTcpPortReservation(ushort startPort, ushort numberOfPorts)
		{
			UIntPtr uintPtr;
			ulong num = TcpListener.CreatePersistentTcpPortReservation(startPort, numberOfPorts, out uintPtr);
			if (num == 32UL)
			{
				ExTraceGlobals.NetworkTracer.TraceDebug<ushort, ushort>(0L, "CreatePersistentTcpPortReservation failed with ERROR_SHARING_VIOLATION when reserving ports {0}-{1}, most likely because we've already reserved them.", startPort, numberOfPorts);
				return;
			}
			if (num != 0UL)
			{
				throw new InvalidOperationException("CreatePersistentTcpPortReservation returned status " + num);
			}
		}

		public bool IsListening()
		{
			return this.listening;
		}

		public void SetBindings(IPEndPoint[] newBindings, bool invokeDelegateOnFailure)
		{
			bool flag = true;
			bool addressAlreadyInUseFailure = false;
			lock (this)
			{
				if (this.listening)
				{
					flag = this.SetupBindings(newBindings, out addressAlreadyInUseFailure);
				}
				this.bindings = newBindings;
			}
			if (!flag && invokeDelegateOnFailure)
			{
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ServiceStoppingOnSocketOpenError, null, new object[0]);
				this.failureHandler(addressAlreadyInUseFailure);
			}
		}

		public void StartListening(bool invokeDelegateOnFailure)
		{
			bool addressAlreadyInUseFailure;
			bool flag2;
			lock (this)
			{
				if (this.listening)
				{
					return;
				}
				this.listening = true;
				if (this.bindings == null)
				{
					return;
				}
				flag2 = this.SetupBindings(this.bindings, out addressAlreadyInUseFailure);
			}
			if (!flag2 && invokeDelegateOnFailure)
			{
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ServiceStoppingOnSocketOpenError, null, new object[0]);
				this.failureHandler(addressAlreadyInUseFailure);
			}
		}

		public void StopListening()
		{
			lock (this)
			{
				if (this.listening)
				{
					this.listening = false;
					if (this.socketBindingList != null)
					{
						foreach (TcpListener.SocketIPEndPointPair socketIPEndPointPair in this.socketBindingList)
						{
							if (socketIPEndPointPair.ListenSocket != null)
							{
								socketIPEndPointPair.ListenSocket.Close();
								socketIPEndPointPair.ListenSocket = null;
							}
						}
					}
				}
			}
		}

		public void Shutdown()
		{
			this.ipConnectionTable.Close();
		}

		private bool SetupBindings(IPEndPoint[] configBindings, out bool addressAlreadyInUseFailure)
		{
			addressAlreadyInUseFailure = false;
			bool flag = false;
			List<TcpListener.SocketIPEndPointPair> newSocketBindingList;
			lock (this)
			{
				List<IPEndPoint> list;
				List<int> list2;
				List<int> list3;
				this.FindIPAnyAndEliminateDuplicateBindings(configBindings, out list, out list2, out list3);
				List<IPEndPoint> newBindings;
				List<TcpListener.SocketIPEndPointPair> bindingsToRemove;
				List<int> list4;
				List<int> list5;
				TcpListener.GroupBindings(this.socketBindingList, list, list2, list3, out newSocketBindingList, out newBindings, out bindingsToRemove, out list4, out list5);
				this.CloseOldBindings(bindingsToRemove);
				if (Socket.OSSupportsIPv4)
				{
					bool flag3;
					if (!this.StartIPANYBindings(list2, list4, newSocketBindingList, IPAddress.Any, out flag3))
					{
						flag = true;
					}
					addressAlreadyInUseFailure = (addressAlreadyInUseFailure || flag3);
				}
				if (Socket.OSSupportsIPv6)
				{
					bool flag4;
					if (!this.StartIPANYBindings(list3, list5, newSocketBindingList, IPAddress.IPv6Any, out flag4))
					{
						flag = true;
					}
					addressAlreadyInUseFailure = (addressAlreadyInUseFailure || flag4);
				}
				bool flag5;
				if (!this.StartIPBindings(newBindings, list4, list5, newSocketBindingList, out flag5))
				{
					flag = true;
				}
				addressAlreadyInUseFailure = (addressAlreadyInUseFailure || flag5);
			}
			this.socketBindingList = newSocketBindingList;
			return !flag;
		}

		private bool StartIPANYBindings(List<int> allIpAnyPortsNeeded, List<int> ipAnyPortsAlreadyOpened, List<TcpListener.SocketIPEndPointPair> newSocketBindingList, IPAddress bindAddress, out bool addressAlreadyInUseFailure)
		{
			addressAlreadyInUseFailure = false;
			bool result = true;
			if (!bindAddress.Equals(IPAddress.Any) && !bindAddress.Equals(IPAddress.IPv6Any))
			{
				throw new ArgumentException("Only IPAddress.Any or IPAddress.IPv6Any allowed as arguments");
			}
			foreach (int num in allIpAnyPortsNeeded)
			{
				if (!ipAnyPortsAlreadyOpened.Contains(num))
				{
					IPEndPoint ipendPoint = new IPEndPoint(bindAddress, num);
					TcpListener.SocketIPEndPointPair socketIPEndPointPair = new TcpListener.SocketIPEndPointPair(ipendPoint);
					socketIPEndPointPair.ListenSocket = this.CreateListenSocket(ipendPoint);
					if (socketIPEndPointPair.ListenSocket == null)
					{
						result = false;
					}
					else
					{
						bool flag;
						if (this.StartListeningSocket(socketIPEndPointPair.ListenSocket, ipendPoint, out flag))
						{
							newSocketBindingList.Add(socketIPEndPointPair);
							ipAnyPortsAlreadyOpened.Add(num);
						}
						else
						{
							result = false;
							this.CloseListenSocket(socketIPEndPointPair.ListenSocket);
							socketIPEndPointPair.ListenSocket = null;
						}
						addressAlreadyInUseFailure = (addressAlreadyInUseFailure || flag);
					}
				}
			}
			return result;
		}

		private bool StartIPBindings(List<IPEndPoint> newBindings, List<int> openedIPv4AnyPort, List<int> openedIPv6AnyPort, List<TcpListener.SocketIPEndPointPair> newSocketBindingList, out bool addressAlreadyInUseFailure)
		{
			addressAlreadyInUseFailure = false;
			bool result = true;
			foreach (IPEndPoint ipendPoint in newBindings)
			{
				if ((ipendPoint.AddressFamily != AddressFamily.InterNetwork || !openedIPv4AnyPort.Contains(ipendPoint.Port)) && (ipendPoint.AddressFamily != AddressFamily.InterNetworkV6 || !openedIPv6AnyPort.Contains(ipendPoint.Port)))
				{
					TcpListener.SocketIPEndPointPair socketIPEndPointPair = new TcpListener.SocketIPEndPointPair(ipendPoint);
					socketIPEndPointPair.ListenSocket = this.CreateListenSocket(ipendPoint);
					if (socketIPEndPointPair.ListenSocket == null)
					{
						result = false;
					}
					else
					{
						bool flag;
						if (this.StartListeningSocket(socketIPEndPointPair.ListenSocket, ipendPoint, out flag))
						{
							newSocketBindingList.Add(socketIPEndPointPair);
						}
						else
						{
							result = false;
							this.CloseListenSocket(socketIPEndPointPair.ListenSocket);
							socketIPEndPointPair.ListenSocket = null;
						}
						addressAlreadyInUseFailure = (addressAlreadyInUseFailure || flag);
					}
				}
			}
			return result;
		}

		internal static List<IPEndPoint> GroupBindings(List<TcpListener.SocketIPEndPointPair> sourceSocketBindingList, List<IPEndPoint> bindings, List<int> allIPv4AnyPortInNewConfig, List<int> allIPv6AnyPortInNewConfig, out List<TcpListener.SocketIPEndPointPair> newSocketBindingList, out List<IPEndPoint> newBindings, out List<TcpListener.SocketIPEndPointPair> oldBindings, out List<int> currentIPv4AnyPortToKeep, out List<int> currentIPv6AnyPortToKeep)
		{
			newBindings = new List<IPEndPoint>();
			newSocketBindingList = new List<TcpListener.SocketIPEndPointPair>();
			oldBindings = new List<TcpListener.SocketIPEndPointPair>();
			currentIPv4AnyPortToKeep = new List<int>();
			currentIPv6AnyPortToKeep = new List<int>();
			foreach (IPEndPoint ipendPoint in bindings)
			{
				if (!ipendPoint.Address.Equals(IPAddress.Any) && !ipendPoint.Address.Equals(IPAddress.IPv6Any))
				{
					newBindings.Add(ipendPoint);
				}
			}
			if (sourceSocketBindingList != null)
			{
				foreach (TcpListener.SocketIPEndPointPair socketIPEndPointPair in sourceSocketBindingList)
				{
					bool flag = false;
					foreach (IPEndPoint obj in bindings)
					{
						if (socketIPEndPointPair.Binding.Equals(obj))
						{
							flag = true;
							break;
						}
					}
					if (AddressFamily.InterNetwork.Equals(socketIPEndPointPair.Binding.Address.AddressFamily) && !socketIPEndPointPair.Binding.Address.Equals(IPAddress.Any) && allIPv4AnyPortInNewConfig.Contains(socketIPEndPointPair.Binding.Port))
					{
						flag = false;
					}
					else if (AddressFamily.InterNetworkV6.Equals(socketIPEndPointPair.Binding.Address.AddressFamily) && !socketIPEndPointPair.Binding.Address.Equals(IPAddress.IPv6Any) && allIPv6AnyPortInNewConfig.Contains(socketIPEndPointPair.Binding.Port))
					{
						flag = false;
					}
					if (socketIPEndPointPair.ListenSocket != null)
					{
						if (!flag)
						{
							TcpListener.SocketIPEndPointPair socketIPEndPointPair2 = new TcpListener.SocketIPEndPointPair(socketIPEndPointPair.Binding);
							socketIPEndPointPair2.ListenSocket = socketIPEndPointPair.ListenSocket;
							oldBindings.Add(socketIPEndPointPair2);
							socketIPEndPointPair.ListenSocket = null;
						}
						else
						{
							newBindings.Remove(socketIPEndPointPair.Binding);
							newSocketBindingList.Add(socketIPEndPointPair);
							if (socketIPEndPointPair.Binding.Address.Equals(IPAddress.Any))
							{
								currentIPv4AnyPortToKeep.Add(socketIPEndPointPair.Binding.Port);
							}
							else if (socketIPEndPointPair.Binding.Address.Equals(IPAddress.IPv6Any))
							{
								currentIPv6AnyPortToKeep.Add(socketIPEndPointPair.Binding.Port);
							}
						}
					}
				}
			}
			return newBindings;
		}

		private void CloseOldBindings(List<TcpListener.SocketIPEndPointPair> bindingsToRemove)
		{
			if (bindingsToRemove == null)
			{
				return;
			}
			foreach (TcpListener.SocketIPEndPointPair socketIPEndPointPair in bindingsToRemove)
			{
				if (socketIPEndPointPair.ListenSocket != null)
				{
					this.CloseListenSocket(socketIPEndPointPair.ListenSocket);
					socketIPEndPointPair.ListenSocket = null;
				}
			}
		}

		private void FindIPAnyAndEliminateDuplicateBindings(IPEndPoint[] newBindings, out List<IPEndPoint> configBindings, out List<int> ipv4AnyPorts, out List<int> ipv6AnyPorts)
		{
			configBindings = new List<IPEndPoint>();
			ipv4AnyPorts = new List<int>();
			ipv6AnyPorts = new List<int>();
			if (newBindings != null)
			{
				for (int i = 0; i < newBindings.Length; i++)
				{
					bool flag = false;
					for (int j = 0; j < i; j++)
					{
						if (newBindings[j].Equals(newBindings[i]))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						if (newBindings[i].Address.Equals(IPAddress.Any))
						{
							ipv4AnyPorts.Add(newBindings[i].Port);
						}
						else if (newBindings[i].Address.Equals(IPAddress.IPv6Any))
						{
							ipv6AnyPorts.Add(newBindings[i].Port);
						}
						configBindings.Add(newBindings[i]);
					}
				}
			}
		}

		private void CloseListenSocket(Socket socket)
		{
			try
			{
				socket.Close();
			}
			catch (SocketException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetHandleInformation(IntPtr hObject, TcpListener.HANDLE_FLAGS dwMask, TcpListener.HANDLE_FLAGS dwFlags);

		[DllImport("iphlpapi.dll")]
		private static extern ulong CreatePersistentTcpPortReservation([In] ushort startPort, [In] ushort numberOfPorts, out UIntPtr token);

		private Socket CreateListenSocket(IPEndPoint endPoint)
		{
			bool flag = endPoint.Address.AddressFamily == AddressFamily.InterNetwork;
			bool flag2 = endPoint.Address.AddressFamily == AddressFamily.InterNetworkV6;
			if (!flag && !flag2)
			{
				throw new ArgumentException(string.Format("Invalid AddressFamily <{0}> was specified.", endPoint.Address.AddressFamily), "endPoint");
			}
			Socket socket;
			try
			{
				this.tracer.TraceDebug<IPEndPoint>(0L, "Creating a new socket: {0}", endPoint);
				socket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				if (this.disableHandleInheritance && !TcpListener.SetHandleInformation(socket.Handle, TcpListener.HANDLE_FLAGS.HANDLE_FLAG_INHERIT, (TcpListener.HANDLE_FLAGS)0))
				{
					throw new SocketException(Marshal.GetLastWin32Error());
				}
			}
			catch (SocketException ex)
			{
				if (Array.IndexOf<SocketError>(TcpListener.socketErrorsHandleOnSocketCreate, ex.SocketErrorCode) == -1)
				{
					throw;
				}
				this.tracer.TraceDebug<IPEndPoint, SocketError>(0L, "A binding has been configured but the requested network features are not enabled or fully functioning. (binding={0}, socket error={1})", endPoint, ex.SocketErrorCode);
				return null;
			}
			return socket;
		}

		private bool StartListeningSocket(Socket listenSocket, IPEndPoint endPoint, out bool addressAlreadyInUseFailure)
		{
			addressAlreadyInUseFailure = false;
			SocketException ex = null;
			for (int i = 0; i < 60; i++)
			{
				try
				{
					listenSocket.ExclusiveAddressUse = this.exclusiveAddressUse;
					listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, !this.exclusiveAddressUse);
					listenSocket.Bind(endPoint);
					listenSocket.Listen(128);
					listenSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), listenSocket);
					this.tracer.TraceDebug<IPEndPoint>(0L, "Successfully started listening at binding: {0}", endPoint);
					return true;
				}
				catch (SocketException ex2)
				{
					this.tracer.TraceDebug<SocketError>(0L, "Socket exception (error={0})", ex2.SocketErrorCode);
					ex = ex2;
					if (ex2.SocketErrorCode != SocketError.AccessDenied)
					{
						this.tracer.TraceError<int, IPEndPoint>(0L, "Unhandled socket error (error={0}) while starting to listen: {1}", ex2.ErrorCode, endPoint);
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_SocketListenError, null, new object[]
						{
							ex2.ErrorCode,
							endPoint
						});
						break;
					}
					this.tracer.TraceDebug(0L, "Failed to open listen socket (error={0}): {1}, retry={2}, maxretry={3}", new object[]
					{
						ex2.SocketErrorCode,
						endPoint,
						i,
						60
					});
					Thread.Sleep(1000);
					if (this.processStopping)
					{
						break;
					}
				}
			}
			if (ex != null)
			{
				SocketError socketErrorCode = ex.SocketErrorCode;
				if (socketErrorCode != SocketError.AccessDenied)
				{
					if (socketErrorCode == SocketError.AddressAlreadyInUse)
					{
						addressAlreadyInUseFailure = true;
						this.tracer.TraceError<IPEndPoint>(0L, "Address already in use: {0}", endPoint);
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_AddressInUse, null, new object[]
						{
							endPoint
						});
					}
				}
				else
				{
					this.tracer.TraceError<IPEndPoint>(0L, "Socket Access Denied: {0}", endPoint);
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_SocketAccessDenied, null, new object[]
					{
						endPoint
					});
				}
			}
			return false;
		}

		private bool ReplaceSocket(Socket socket, out bool addressAlreadyInUseFailure)
		{
			addressAlreadyInUseFailure = false;
			lock (this)
			{
				if (this.socketBindingList == null || socket == null)
				{
					return true;
				}
				foreach (TcpListener.SocketIPEndPointPair socketIPEndPointPair in this.socketBindingList)
				{
					if (socketIPEndPointPair.ListenSocket == socket)
					{
						IPEndPoint endPoint = (IPEndPoint)socket.LocalEndPoint;
						this.CloseListenSocket(socket);
						socketIPEndPointPair.ListenSocket = this.CreateListenSocket(endPoint);
						if (socketIPEndPointPair.ListenSocket == null)
						{
							return false;
						}
						if (this.listening && !this.StartListeningSocket(socketIPEndPointPair.ListenSocket, endPoint, out addressAlreadyInUseFailure))
						{
							return false;
						}
						break;
					}
				}
			}
			return true;
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			Socket socket2 = null;
			try
			{
				socket2 = socket.EndAccept(ar);
				IPAddress address = ((IPEndPoint)socket2.RemoteEndPoint).Address;
				if (!this.ipConnectionTable.CanAcceptConnection(address))
				{
					this.tracer.TraceDebug<IPAddress, int>(0L, "The connection rate from {0} has exceeded the configured limit {1}.", address, this.ipConnectionTable.ConnectionRateLimit);
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_IPConnectionRateExceeded, address.ToString(), new object[]
					{
						address,
						this.ipConnectionTable.ConnectionRateLimit
					});
					socket2.Disconnect(false);
					socket2.Close();
					socket2 = null;
				}
			}
			catch (SocketException ex)
			{
				this.tracer.TraceError<SocketError, int>(0L, "Exception calling EndAccept. (SocketException: {0}, ErrorCode: {1})", ex.SocketErrorCode, ex.ErrorCode);
				if (Array.IndexOf<SocketError>(TcpListener.socketErrorsIgnoreOnEndAccept, ex.SocketErrorCode) == -1)
				{
					throw;
				}
			}
			catch (ObjectDisposedException)
			{
				this.tracer.TraceError(0L, "Exception calling EndAccept. (ObjectDisposedException)");
			}
			if (socket2 != null)
			{
				this.connectionHandler(socket2);
			}
			try
			{
				socket.BeginAccept(new AsyncCallback(this.AcceptCallback), socket);
			}
			catch (SocketException ex2)
			{
				this.tracer.TraceError<int>(0L, "Exception calling BeginAccept. (SocketException ErrorCode: {0})", ex2.ErrorCode);
				bool addressAlreadyInUseFailure = false;
				bool flag = true;
				lock (this)
				{
					if (this.listening)
					{
						flag = this.ReplaceSocket(socket, out addressAlreadyInUseFailure);
					}
				}
				if (!flag)
				{
					this.failureHandler(addressAlreadyInUseFailure);
				}
			}
			catch (ObjectDisposedException)
			{
				this.tracer.TraceError(0L, "Exception calling BeginAccept. (ObjectDisposedException)");
			}
		}

		private const int ListenQueueLength = 128;

		private const long TraceId = 0L;

		private static readonly SocketError[] socketErrorsIgnoreOnEndAccept = new SocketError[]
		{
			SocketError.ConnectionReset,
			SocketError.NetworkDown,
			SocketError.TooManyOpenSockets,
			SocketError.NoBufferSpaceAvailable,
			SocketError.TimedOut,
			SocketError.InvalidArgument,
			SocketError.NetworkUnreachable
		};

		private static readonly SocketError[] socketErrorsHandleOnSocketCreate = new SocketError[]
		{
			SocketError.ProtocolFamilyNotSupported,
			SocketError.ProtocolNotSupported,
			SocketError.AddressFamilyNotSupported
		};

		private IPEndPoint[] bindings;

		private List<TcpListener.SocketIPEndPointPair> socketBindingList;

		private bool listening;

		private readonly ITracer tracer;

		private readonly IExEventLog eventLogger;

		private readonly IPConnectionTable ipConnectionTable;

		private bool processStopping;

		private readonly TcpListener.HandleConnection connectionHandler;

		private readonly TcpListener.HandleFailure failureHandler;

		private readonly bool exclusiveAddressUse;

		private readonly bool disableHandleInheritance;

		public delegate bool HandleConnection(Socket connection);

		public delegate void HandleFailure(bool addressAlreadyInUseFailure);

		[Flags]
		internal enum HANDLE_FLAGS
		{
			HANDLE_FLAG_INHERIT = 1,
			HANDLE_FLAG_PROTECT_FROM_CLOSE = 2
		}

		internal class SocketIPEndPointPair
		{
			internal SocketIPEndPointPair(IPEndPoint endpoint)
			{
				this.binding = endpoint;
			}

			internal IPEndPoint Binding
			{
				get
				{
					return this.binding;
				}
			}

			internal Socket ListenSocket
			{
				get
				{
					return this.listenSocket;
				}
				set
				{
					this.listenSocket = value;
				}
			}

			private IPEndPoint binding;

			private Socket listenSocket;
		}
	}
}
