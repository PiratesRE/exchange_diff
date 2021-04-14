using System;

namespace Microsoft.Exchange.Net
{
	internal class DnsAsyncRequest : LazyAsyncResult
	{
		internal DnsAsyncRequest(string question, DnsRecordType dnsRecordType, ushort queryIdentifier, DnsQueryOptions queryOptions, object workerObject, object callerState, AsyncCallback callback, object previousRequestState) : base(workerObject, callerState, callback)
		{
			this.query = new DnsQuery(dnsRecordType, question);
			this.queryIdentifier = queryIdentifier;
			this.queryOptions = queryOptions;
			int recursionDesired = ((queryOptions & DnsQueryOptions.NoRecursion) != DnsQueryOptions.None) ? 0 : 1;
			this.buffer = DnsNativeMethods.DnsQuestionToBuffer(false, question, dnsRecordType, queryIdentifier, recursionDesired);
			Request request = (Request)workerObject;
			this.clientStatus = new int[request.ClientCount];
			this.previousRequestState = previousRequestState;
		}

		internal bool IsValid
		{
			get
			{
				return this.buffer != null;
			}
		}

		internal byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		internal DnsQuery Query
		{
			get
			{
				return this.query;
			}
		}

		internal ushort QueryIdentifier
		{
			get
			{
				return this.queryIdentifier;
			}
		}

		internal DnsQueryOptions DnsQueryOptions
		{
			get
			{
				return this.queryOptions;
			}
		}

		internal bool UseTcpOnly
		{
			get
			{
				return (this.queryOptions & DnsQueryOptions.UseTcpOnly) != DnsQueryOptions.None;
			}
			set
			{
				if (value)
				{
					this.queryOptions |= DnsQueryOptions.UseTcpOnly;
					return;
				}
				this.queryOptions &= ~DnsQueryOptions.UseTcpOnly;
			}
		}

		internal bool AcceptTruncatedResponse
		{
			get
			{
				return (this.queryOptions & DnsQueryOptions.AcceptTruncatedResponse) != DnsQueryOptions.None;
			}
		}

		internal bool ExceedsDnsErrorLimit
		{
			get
			{
				for (int i = 0; i < this.clientStatus.Length; i++)
				{
					if (this.clientStatus[i] == 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		internal int RetryCount
		{
			get
			{
				return this.retryCount;
			}
			set
			{
				this.retryCount = value;
			}
		}

		public object PreviousRequestState
		{
			get
			{
				return this.previousRequestState;
			}
		}

		internal bool CanQueryClient(int clientId)
		{
			if (clientId < 0 || clientId >= this.clientStatus.Length)
			{
				throw new ArgumentOutOfRangeException("clientId", "Dns client index out of range");
			}
			return this.clientStatus[clientId] == 0;
		}

		internal void SetClientError(int clientId)
		{
			if (clientId < 0 || clientId >= this.clientStatus.Length)
			{
				throw new ArgumentOutOfRangeException("clientId", "Dns client index out of range");
			}
			this.clientStatus[clientId]++;
		}

		public override string ToString()
		{
			return string.Format("id={0}; query={1}; retryCount={2}; Options={3}", new object[]
			{
				this.queryIdentifier,
				this.query,
				this.retryCount,
				this.DnsQueryOptions
			});
		}

		private byte[] buffer;

		private ushort queryIdentifier;

		private DnsQueryOptions queryOptions;

		private readonly DnsQuery query;

		private int[] clientStatus;

		private int retryCount;

		private object previousRequestState;
	}
}
