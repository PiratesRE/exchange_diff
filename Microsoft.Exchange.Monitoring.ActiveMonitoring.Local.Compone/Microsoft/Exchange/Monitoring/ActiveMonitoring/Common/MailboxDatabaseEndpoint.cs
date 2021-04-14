using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class MailboxDatabaseEndpoint : IEndpoint
	{
		public ICollection<MailboxDatabaseInfo> MailboxDatabaseInfoCollectionForBackend
		{
			get
			{
				return this.validMailboxDatabasesForBackend;
			}
		}

		public ICollection<MailboxDatabaseInfo> MailboxDatabaseInfoCollectionForCafe
		{
			get
			{
				return this.validMailboxDatabasesForCafe;
			}
		}

		public ICollection<MailboxDatabaseInfo> UnverifiedMailboxDatabaseInfoCollectionForBackendLiveIdAuthenticationProbe
		{
			get
			{
				return this.unverifiedMailboxDatabasesForBackend;
			}
		}

		public ICollection<MailboxDatabaseInfo> UnverifiedMailboxDatabaseInfoCollectionForCafeLiveIdAuthenticationProbe
		{
			get
			{
				return this.unverifiedMailboxDatabasesForCafe;
			}
		}

		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public bool DetectChange()
		{
			this.changeModulus++;
			if (this.changeModulus >= 5)
			{
				this.changeModulus = 0;
			}
			if (DirectoryAccessor.Instance.Server != null)
			{
				if (this.changeModulus % 5 == 0 && DirectoryAccessor.Instance.Server.IsMailboxServer)
				{
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Detecting local mailbox database endpoint changes", null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpoint.cs", 170);
					if (this.backendDelegate.DetectChange(this.unverifiedMailboxDatabasesForBackend))
					{
						return true;
					}
				}
				if (DirectoryAccessor.Instance.Server.IsCafeServer)
				{
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Detecting remote mailbox database endpoint changes", null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpoint.cs", 182);
					if (this.cafeDelegate.DetectChange(this.unverifiedMailboxDatabasesForCafe))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual void Initialize()
		{
			if (DirectoryAccessor.Instance.Server != null)
			{
				string text = IPGlobalProperties.GetIPGlobalProperties().DomainName.ToLower();
				if (text.Contains("prdmgt01.prod.exchangelabs.com") || text.Contains("sdfmgt02.sdf.exchangelabs.com") || text.Contains("chnmgt03.partner.outlook.cn"))
				{
					return;
				}
				if (DirectoryAccessor.Instance.Server.IsMailboxServer)
				{
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox role installed, initializing local mailbox database list", null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpoint.cs", 215);
					this.backendDelegate.Initialize(this.validMailboxDatabasesForBackend, this.unverifiedMailboxDatabasesForBackend);
				}
				if (DirectoryAccessor.Instance.Server.IsCafeServer)
				{
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Cafe role installed, initializing remote mailbox database list", null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpoint.cs", 224);
					this.cafeDelegate.Initialize(this.validMailboxDatabasesForCafe, this.unverifiedMailboxDatabasesForCafe);
				}
			}
		}

		private const int backendMultiple = 5;

		private List<MailboxDatabaseInfo> validMailboxDatabasesForBackend = new List<MailboxDatabaseInfo>();

		private List<MailboxDatabaseInfo> validMailboxDatabasesForCafe = new List<MailboxDatabaseInfo>();

		private List<MailboxDatabaseInfo> unverifiedMailboxDatabasesForBackend = new List<MailboxDatabaseInfo>();

		private List<MailboxDatabaseInfo> unverifiedMailboxDatabasesForCafe = new List<MailboxDatabaseInfo>();

		private TracingContext traceContext = TracingContext.Default;

		private MailboxDatabaseEndpointDelegate backendDelegate = new BackendMailboxDatabaseEndpointDelegate();

		private MailboxDatabaseEndpointDelegate cafeDelegate = new CafeMailboxDatabaseEndpointDelegate();

		private int changeModulus;
	}
}
