using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SimpleContext : DisposeTrackableBase
	{
		public SimpleContext(string applicationName) : this(applicationName, new AnchorConfig(applicationName))
		{
		}

		internal SimpleContext(string applicationName, AnchorConfig config)
		{
			this.Initialize(applicationName, this.CreateLogger(applicationName, config), config);
		}

		protected SimpleContext()
		{
		}

		public string ApplicationName
		{
			get
			{
				return this.applicationName;
			}
			protected set
			{
				this.applicationName = SimpleContext.RemoveWhiteSpace.Replace(value, string.Empty);
			}
		}

		public AnchorConfig Config { get; private set; }

		public ILogger Logger { get; private set; }

		public override void SuppressDisposeTracker()
		{
			if (this.Logger != null)
			{
				this.Logger.SuppressDisposeTracker();
			}
			base.SuppressDisposeTracker();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SimpleContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Logger != null)
			{
				this.Logger.Dispose();
				this.Logger = null;
			}
		}

		protected void Initialize(string applicationName, ILogger logger, AnchorConfig config)
		{
			AnchorUtil.ThrowOnNullOrEmptyArgument(applicationName, "applicationName");
			this.ApplicationName = applicationName;
			this.Logger = logger;
			this.Config = config;
		}

		protected virtual ILogger CreateLogger(string applicationName, AnchorConfig config)
		{
			return new AnchorLogger(applicationName, config);
		}

		private static readonly Regex RemoveWhiteSpace = new Regex("\\s+");

		private string applicationName;
	}
}
