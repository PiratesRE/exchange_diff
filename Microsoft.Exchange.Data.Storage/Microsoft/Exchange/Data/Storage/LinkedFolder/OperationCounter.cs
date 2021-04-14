using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OperationCounter
	{
		public Stopwatch StopWatch { get; private set; }

		public OperationType Name { get; private set; }

		public int Count { get; set; }

		public TimeSpan TotalElapsedTime { get; set; }

		public TimeSpan MaximumElapsedTime { get; set; }

		public TimeSpan AveragedElapsedTime
		{
			get
			{
				if (this.Count <= 0)
				{
					return TimeSpan.FromMilliseconds(0.0);
				}
				return TimeSpan.FromMilliseconds((double)((int)this.TotalElapsedTime.TotalMilliseconds / this.Count));
			}
		}

		public string GetLogLine()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Name:{0}, Count:{1}, TotalElaspedTime:{2}, AverageElapsedTime:{3}, MaxElapsedTime:{4} ", new object[]
			{
				this.Name,
				this.Count,
				this.TotalElapsedTime,
				this.AveragedElapsedTime,
				this.MaximumElapsedTime
			});
			return stringBuilder.ToString();
		}

		public OperationCounter(OperationType name)
		{
			this.Name = name;
			this.StopWatch = new Stopwatch();
		}
	}
}
