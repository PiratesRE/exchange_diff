using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ServiceIssueCache : IDiagnosable
	{
		protected ServiceIssueCache()
		{
			this.issueCache = new Dictionary<string, ServiceIssue>();
			this.cacheLock = new object();
			this.scanTimer = new System.Timers.Timer();
			this.scanTimer.Elapsed += this.RunScansEventHandler;
			this.isScanning = false;
			this.invokeScan = false;
			this.lastScanDuration = TimeSpan.Zero;
			this.lastScanStartTime = DateTime.UtcNow;
		}

		public virtual bool ScanningIsEnabled
		{
			get
			{
				bool enabled;
				lock (this.cacheLock)
				{
					enabled = this.scanTimer.Enabled;
				}
				return enabled;
			}
		}

		public Exception LastScanError { get; protected set; }

		protected virtual string ComponentName
		{
			get
			{
				return "IssueCache";
			}
		}

		protected abstract TimeSpan FullScanFrequency { get; }

		protected abstract int IssueLimit { get; }

		public void EnableScanning()
		{
			lock (this.cacheLock)
			{
				if (!this.scanTimer.Enabled)
				{
					this.scanTimer.Interval = this.FullScanFrequency.TotalMilliseconds;
					this.scanTimer.Start();
					this.InvokeScan();
				}
			}
		}

		public void DisableScanning()
		{
			lock (this.cacheLock)
			{
				this.scanTimer.Stop();
			}
		}

		public void InvokeScan()
		{
			lock (this.cacheLock)
			{
				this.invokeScan = true;
			}
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				this.RunScans();
			});
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return this.ComponentName;
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.ComponentName);
			SICDiagnosticArgument arguments = this.CreateDiagnosticArgumentParser();
			try
			{
				arguments.Initialize(parameters);
			}
			catch (DiagnosticArgumentException ex)
			{
				xelement.Add(new XElement("Error", "Encountered exception: " + ex.Message));
				xelement.Add(new XElement("Help", "Supported arguments: " + arguments.GetSupportedArguments()));
				return xelement;
			}
			List<ServiceIssue> list = null;
			if (arguments.HasArgument("invokescan"))
			{
				this.InvokeScan();
			}
			lock (this.cacheLock)
			{
				xelement.Add(new object[]
				{
					new XElement("ScanFrequency", this.FullScanFrequency.ToString()),
					new XElement("IssueLimit", this.IssueLimit),
					new XElement("IsScanning", this.isScanning),
					new XElement("IsEnabled", this.ScanningIsEnabled),
					new XElement("LastScanStartTime", this.lastScanStartTime),
					new XElement("LastScanDuration", this.lastScanDuration.TotalMilliseconds),
					new XElement("NumberOfIssues", this.issueCache.Count)
				});
				if (this.LastScanError != null)
				{
					xelement.Add(new object[]
					{
						new XElement("LastScanErrorName", this.LastScanError.GetType().Name),
						new XElement("LastScanErrorMessage", this.LastScanError.Message)
					});
				}
				if (arguments.HasArgument("issue"))
				{
					list = new List<ServiceIssue>(this.issueCache.Values);
				}
			}
			if (list != null)
			{
				int argumentOrDefault = arguments.GetArgumentOrDefault<int>("maxsize", list.Count);
				XElement xelement2 = new XElement("ServiceIssues");
				using (IEnumerator<ServiceIssue> enumerator = list.Take(argumentOrDefault).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ServiceIssue issue = enumerator.Current;
						xelement2.Add(arguments.RunDiagnosticOperation(() => issue.GetDiagnosticInfo(arguments)));
					}
				}
				xelement.Add(xelement2);
			}
			if (arguments.ArgumentCount == 0)
			{
				xelement.Add(new XElement("Help", "Supported arguments: " + arguments.GetSupportedArguments()));
			}
			return xelement;
		}

		protected virtual SICDiagnosticArgument CreateDiagnosticArgumentParser()
		{
			return new SICDiagnosticArgument();
		}

		protected abstract ICollection<ServiceIssue> RunFullIssueScan();

		private void UpdateCache(ICollection<ServiceIssue> issues)
		{
			Dictionary<string, ServiceIssue> dictionary = new Dictionary<string, ServiceIssue>();
			if (issues != null)
			{
				foreach (ServiceIssue serviceIssue in issues)
				{
					if (dictionary.Count >= this.IssueLimit)
					{
						break;
					}
					if (this.issueCache.ContainsKey(serviceIssue.IdentifierString))
					{
						serviceIssue.DeriveFromIssue(this.issueCache[serviceIssue.IdentifierString]);
					}
					dictionary[serviceIssue.IdentifierString] = serviceIssue;
				}
			}
			this.issueCache = dictionary;
		}

		private void RunScansEventHandler(object source, ElapsedEventArgs e)
		{
			this.RunScans();
		}

		private void RunScans()
		{
			lock (this.cacheLock)
			{
				if ((!this.ScanningIsEnabled && !this.invokeScan) || this.isScanning)
				{
					return;
				}
				this.isScanning = true;
				this.lastScanStartTime = DateTime.UtcNow;
				this.LastScanError = null;
			}
			ICollection<ServiceIssue> issues = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				issues = this.RunFullIssueScan();
			}
			catch (LocalizedException lastScanError)
			{
				this.LastScanError = lastScanError;
			}
			stopwatch.Stop();
			lock (this.cacheLock)
			{
				this.UpdateCache(issues);
				this.lastScanDuration = stopwatch.Elapsed;
				this.isScanning = false;
				this.invokeScan = false;
			}
		}

		private const string DiagnosticsComponentName = "IssueCache";

		private object cacheLock;

		private Dictionary<string, ServiceIssue> issueCache;

		private bool isScanning;

		private bool invokeScan;

		private TimeSpan lastScanDuration;

		private DateTime lastScanStartTime;

		private System.Timers.Timer scanTimer;
	}
}
