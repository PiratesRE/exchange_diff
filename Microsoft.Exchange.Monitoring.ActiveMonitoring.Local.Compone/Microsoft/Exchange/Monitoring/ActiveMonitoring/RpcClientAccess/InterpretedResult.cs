using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	public class InterpretedResult
	{
		public ProbeResult RawResult { get; internal set; }

		public string ActivityContext
		{
			get
			{
				return this.RawResult.StateAttribute15;
			}
			set
			{
				this.RawResult.StateAttribute15 = value;
			}
		}

		public string UserLegacyDN
		{
			get
			{
				return this.RawResult.StateAttribute21;
			}
			set
			{
				this.RawResult.StateAttribute21 = value;
			}
		}

		public string RequestUrl
		{
			get
			{
				return this.RawResult.StateAttribute23;
			}
			set
			{
				this.RawResult.StateAttribute23 = value;
			}
		}

		public string AuthType
		{
			get
			{
				return this.RawResult.StateAttribute24;
			}
			set
			{
				this.RawResult.StateAttribute24 = value;
			}
		}

		public string RespondingHttpServer
		{
			get
			{
				return this.RawResult.StateAttribute3;
			}
			set
			{
				this.RawResult.StateAttribute3 = value;
			}
		}

		public string RespondingRpcProxyServer
		{
			get
			{
				return this.RawResult.StateAttribute4;
			}
			set
			{
				this.RawResult.StateAttribute4 = value;
			}
		}

		public string MonitoringAccount
		{
			get
			{
				return this.RawResult.StateAttribute13;
			}
			set
			{
				this.RawResult.StateAttribute13 = value;
			}
		}

		public string OutlookSessionCookie
		{
			get
			{
				return this.RawResult.StateAttribute5;
			}
			set
			{
				this.RawResult.StateAttribute5 = value;
			}
		}

		public TimeSpan TotalLatency
		{
			get
			{
				return TimeSpan.FromMilliseconds(this.RawResult.SampleValue);
			}
			set
			{
				this.RawResult.SampleValue = value.TotalMilliseconds;
			}
		}

		public string FirstFailedTaskName
		{
			get
			{
				return this.RawResult.StateAttribute22;
			}
			set
			{
				this.RawResult.StateAttribute22 = value;
			}
		}

		public string ExecutionOutline
		{
			get
			{
				return this.RawResult.StateAttribute25;
			}
			set
			{
				this.RawResult.StateAttribute25 = value;
			}
		}

		public string RootCause
		{
			get
			{
				return this.RawResult.StateAttribute2;
			}
		}

		public string OspUrl
		{
			get
			{
				return this.RawResult.StateAttribute14;
			}
			set
			{
				this.RawResult.StateAttribute14 = value;
			}
		}

		public virtual string VerboseLog
		{
			get
			{
				return this.RawResult.ExecutionContext;
			}
			set
			{
				this.RawResult.ExecutionContext = value;
			}
		}

		public string ErrorDetails
		{
			get
			{
				return this.RawResult.FailureContext;
			}
			set
			{
				this.RawResult.FailureContext = value;
			}
		}

		public TimeSpan InitialLatency
		{
			get
			{
				return TimeSpan.FromMilliseconds(this.RawResult.StateAttribute18);
			}
			set
			{
				this.RawResult.StateAttribute18 = value.TotalMilliseconds;
			}
		}

		public string InitialException
		{
			get
			{
				return this.RawResult.StateAttribute12;
			}
			set
			{
				this.RawResult.StateAttribute12 = value;
			}
		}

		public void SetRootCause(string rootCause, FailingComponent failingComponent)
		{
			this.RawResult.FailureCategory = (int)failingComponent;
			this.RawResult.StateAttribute1 = failingComponent.ToString();
			this.RawResult.StateAttribute2 = rootCause;
			this.RawResult.StateAttribute11 = FailingComponent.Momt.ToString();
		}

		protected internal virtual void OnBeforeSerialize()
		{
		}

		public const string UnknownRPCProxyServer = "Unknown";
	}
}
