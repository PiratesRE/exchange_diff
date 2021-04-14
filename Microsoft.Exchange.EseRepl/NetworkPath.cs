using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.EseRepl
{
	internal class NetworkPath
	{
		public DagNetRoute[] AlternateRoutes { get; set; }

		public bool IgnoreMutualAuth { get; set; }

		public bool UseNullSpn { get; set; }

		public bool UseSocketStream { get; set; }

		public ISimpleBufferPool SocketStreamBufferPool { get; set; }

		public IPool<SocketStreamAsyncArgs> SocketStreamAsyncArgPool { get; set; }

		public SocketStream.ISocketStreamPerfCounters SocketStreamPerfCounters { get; set; }

		internal NetworkPath(string targetNodeName, IPAddress targetAddr, int targetPort, IPAddress sourceAddr)
		{
			this.m_targetNodeName = targetNodeName;
			this.m_targetEndPoint = new IPEndPoint(targetAddr, targetPort);
			if (sourceAddr != null)
			{
				this.m_sourceEndPoint = new IPEndPoint(sourceAddr, 0);
			}
			NetworkPath.Tracer.TraceDebug(0L, "NetworkPath for {0} mapped to {1}:{2} from {3}", new object[]
			{
				targetNodeName,
				targetAddr,
				targetPort,
				(sourceAddr == null) ? "default" : sourceAddr.ToString()
			});
		}

		public string TargetNodeName
		{
			get
			{
				return this.m_targetNodeName;
			}
		}

		public IPEndPoint TargetEndPoint
		{
			get
			{
				return this.m_targetEndPoint;
			}
		}

		public IPEndPoint SourceEndPoint
		{
			get
			{
				return this.m_sourceEndPoint;
			}
		}

		public bool CrossSubnet
		{
			get
			{
				return this.m_isCrossSubnet;
			}
			set
			{
				this.m_isCrossSubnet = value;
			}
		}

		public bool Compress
		{
			get
			{
				return this.m_compression;
			}
			set
			{
				this.m_compression = value;
			}
		}

		public bool Encrypt
		{
			get
			{
				return this.m_encryption;
			}
			set
			{
				this.m_encryption = value;
			}
		}

		public bool NetworkChoiceIsMandatory
		{
			get
			{
				return this.m_networkChoiceIsMandatory;
			}
			set
			{
				this.m_networkChoiceIsMandatory = value;
			}
		}

		public NetworkPath.ConnectionPurpose Purpose
		{
			get
			{
				return this.m_connectionPurpose;
			}
			set
			{
				this.m_connectionPurpose = value;
			}
		}

		internal string NetworkName
		{
			get
			{
				return this.m_networkName;
			}
			set
			{
				this.m_networkName = value;
			}
		}

		public bool HasSourceEndpoint()
		{
			return this.m_sourceEndPoint != null;
		}

		public void ApplyNetworkPolicy(DagNetConfig dagConfig)
		{
			this.Encrypt = false;
			switch (dagConfig.NetworkEncryption)
			{
			case NetworkOption.Disabled:
				this.Encrypt = false;
				break;
			case NetworkOption.Enabled:
				this.Encrypt = true;
				break;
			case NetworkOption.InterSubnetOnly:
				this.Encrypt = this.CrossSubnet;
				break;
			case NetworkOption.SeedOnly:
				this.Encrypt = (this.Purpose == NetworkPath.ConnectionPurpose.Seeding);
				break;
			}
			this.Compress = false;
			if (!Parameters.CurrentValues.LogShipCompressionDisable)
			{
				switch (dagConfig.NetworkCompression)
				{
				case NetworkOption.Disabled:
					this.Compress = false;
					return;
				case NetworkOption.Enabled:
					this.Compress = true;
					return;
				case NetworkOption.InterSubnetOnly:
					this.Compress = this.CrossSubnet;
					return;
				case NetworkOption.SeedOnly:
					this.Compress = (this.Purpose == NetworkPath.ConnectionPurpose.Seeding);
					break;
				default:
					return;
				}
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.NetPathTracer;

		private IPEndPoint m_targetEndPoint;

		private IPEndPoint m_sourceEndPoint;

		private readonly string m_targetNodeName;

		private bool m_isCrossSubnet = true;

		private bool m_encryption;

		private bool m_compression;

		private string m_networkName;

		private bool m_networkChoiceIsMandatory;

		private NetworkPath.ConnectionPurpose m_connectionPurpose;

		public enum ConnectionPurpose
		{
			General,
			TestHealth,
			Seeding,
			LogCopy
		}
	}
}
