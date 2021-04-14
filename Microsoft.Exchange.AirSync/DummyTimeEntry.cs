using System;

namespace Microsoft.Exchange.AirSync
{
	internal class DummyTimeEntry : ITimeEntry, IDisposable
	{
		public TimeId TimeId
		{
			get
			{
				return TimeId.HandlerBeginProcessRequest;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public TimeSpan ElapsedInclusive
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		public TimeSpan ElapsedExclusive
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		public void AddChild(ITimeEntry child)
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public static readonly DummyTimeEntry Singleton = new DummyTimeEntry();
	}
}
