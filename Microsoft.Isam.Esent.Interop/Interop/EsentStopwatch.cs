using System;
using System.Diagnostics;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Isam.Esent.Interop
{
	public class EsentStopwatch
	{
		public bool IsRunning { get; private set; }

		public JET_THREADSTATS ThreadStats { get; private set; }

		public TimeSpan Elapsed { get; private set; }

		public static EsentStopwatch StartNew()
		{
			EsentStopwatch esentStopwatch = new EsentStopwatch();
			esentStopwatch.Start();
			return esentStopwatch;
		}

		public override string ToString()
		{
			if (!this.IsRunning)
			{
				return this.Elapsed.ToString();
			}
			return "EsentStopwatch (running)";
		}

		public void Start()
		{
			this.Reset();
			this.stopwatch = Stopwatch.StartNew();
			this.IsRunning = true;
			if (EsentVersion.SupportsVistaFeatures)
			{
				VistaApi.JetGetThreadStats(out this.statsAtStart);
			}
		}

		public void Stop()
		{
			if (this.IsRunning)
			{
				this.IsRunning = false;
				this.stopwatch.Stop();
				this.Elapsed = this.stopwatch.Elapsed;
				if (EsentVersion.SupportsVistaFeatures)
				{
					JET_THREADSTATS jet_THREADSTATS;
					VistaApi.JetGetThreadStats(out jet_THREADSTATS);
					this.ThreadStats = jet_THREADSTATS - this.statsAtStart;
				}
			}
		}

		public void Reset()
		{
			this.stopwatch = null;
			this.ThreadStats = default(JET_THREADSTATS);
			this.Elapsed = TimeSpan.Zero;
			this.IsRunning = false;
		}

		private Stopwatch stopwatch;

		private JET_THREADSTATS statsAtStart;
	}
}
