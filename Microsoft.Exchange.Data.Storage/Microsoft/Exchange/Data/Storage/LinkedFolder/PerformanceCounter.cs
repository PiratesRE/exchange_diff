using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PerformanceCounter
	{
		public Dictionary<OperationType, OperationCounter> OperationCounters
		{
			get
			{
				return this.operationCounters;
			}
		}

		public Dictionary<ChangeType, int> ChangeCounters
		{
			get
			{
				return this.changeCounters;
			}
		}

		public int CurrentIoOperations { get; set; }

		private void LazyInitialize(OperationType type)
		{
			if (!this.operationCounters.ContainsKey(type))
			{
				this.operationCounters.Add(type, new OperationCounter(type));
			}
		}

		private void LazyInitialize(ChangeType type)
		{
			if (!this.changeCounters.ContainsKey(type))
			{
				this.changeCounters.Add(type, 0);
			}
		}

		public void Increment(ChangeType type)
		{
			this.LazyInitialize(type);
			Dictionary<ChangeType, int> dictionary;
			(dictionary = this.changeCounters)[type] = dictionary[type] + 1;
		}

		public void Start(OperationType type)
		{
			this.LazyInitialize(type);
			this.operationCounters[type].StopWatch.Reset();
			this.operationCounters[type].StopWatch.Start();
		}

		public void Stop(OperationType type, int ioOperations = 1)
		{
			this.operationCounters[type].StopWatch.Stop();
			this.operationCounters[type].Count++;
			this.operationCounters[type].TotalElapsedTime += this.operationCounters[type].StopWatch.Elapsed;
			if (this.operationCounters[type].StopWatch.Elapsed > this.operationCounters[type].MaximumElapsedTime)
			{
				this.operationCounters[type].MaximumElapsedTime = this.operationCounters[type].StopWatch.Elapsed;
			}
			if (type == OperationType.AddFile || type == OperationType.AddFolder || type == OperationType.DeleteItem || type == OperationType.UpdateFile || type == OperationType.UpdateFolder || type == OperationType.MoveFile)
			{
				this.CurrentIoOperations += ioOperations;
			}
		}

		public string[] GetLogLine()
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<ChangeType, int> keyValuePair in this.changeCounters)
			{
				stringBuilder.AppendFormat("{0}:{1}, ", keyValuePair.Key, keyValuePair.Value);
			}
			if (stringBuilder.Length > 0)
			{
				list.Add(stringBuilder.ToString());
			}
			foreach (KeyValuePair<OperationType, OperationCounter> keyValuePair2 in this.operationCounters)
			{
				list.Add(keyValuePair2.Value.GetLogLine());
			}
			return list.ToArray();
		}

		private readonly Dictionary<OperationType, OperationCounter> operationCounters = new Dictionary<OperationType, OperationCounter>();

		private readonly Dictionary<ChangeType, int> changeCounters = new Dictionary<ChangeType, int>();
	}
}
