using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class DiagnosticsContext
	{
		public DiagnosticsLevel DiagnosticsLevel
		{
			get
			{
				return this.diagnosticsLevel;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.diagnosticsLevel != DiagnosticsLevel.None;
			}
		}

		public bool VerboseDiagnostics
		{
			get
			{
				return this.diagnosticsLevel == DiagnosticsLevel.Verbose || this.diagnosticsLevel == DiagnosticsLevel.Etw;
			}
		}

		private int MaxEvents
		{
			get
			{
				if (this.VerboseDiagnostics)
				{
					return Math.Max(8192, ServerCache.Instance.MaxDiagnosticsEvents);
				}
				return ServerCache.Instance.MaxDiagnosticsEvents;
			}
		}

		public DiagnosticsContext(bool suppressIdAllocation, DiagnosticsLevel diagnosticsLevel)
		{
			this.diagnosticsLevel = diagnosticsLevel;
			if (suppressIdAllocation)
			{
				return;
			}
			while (this.requestId == 0)
			{
				this.requestId = Interlocked.Increment(ref DiagnosticsContext.requestIdCounter);
			}
		}

		public void AddProperty(DiagnosticProperty property, object value)
		{
			if (!this.Enabled || value == null)
			{
				return;
			}
			if (this.currentEvent == null)
			{
				this.currentEvent = new List<KeyValuePair<string, object>>(5);
				this.InsertWellknownFields(this.currentEvent);
			}
			this.currentEvent.Add(new KeyValuePair<string, object>(Names<DiagnosticProperty>.Map[(int)property], value));
		}

		public void MergeEvents(string[] events)
		{
			if (!this.Enabled || events == null || events.Length == 0)
			{
				return;
			}
			int num = this.MaxEvents - this.data.Count;
			string text = Names<DiagnosticProperty>.Map[1];
			string text2 = Names<DiagnosticProperty>.Map[0];
			if (num > 1)
			{
				bool flag = false;
				int num2 = Math.Min(events.Length, num - 1);
				for (int i = 0; i < num2; i++)
				{
					if (string.IsNullOrEmpty(events[i]))
					{
						TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Null or empty op-trace was unexpected", new object[0]);
					}
					else
					{
						byte[] bytes = Encoding.UTF8.GetBytes(events[i]);
						object obj = DiagnosticsContext.csvRowBuffer.ResetAndDecode(bytes, new CsvDecoderCallback(DiagnosticsContext.decoder.Decode));
						KeyValuePair<string, object>[] array = obj as KeyValuePair<string, object>[];
						if (array != null && array.Length > 2 && array[0].Value is int && array[1].Value is int)
						{
							if ((!StringComparer.InvariantCulture.Equals(array[0].Key, text2) || !StringComparer.InvariantCulture.Equals(array[1].Key, text)) && !flag)
							{
								TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Reversed Rts and Id, may be from Ex2010 or a new bug. OpTrace: {0}", events[i]);
								flag = true;
							}
							int num3 = this.lastRts + (int)array[0].Value;
							array[0] = new KeyValuePair<string, object>(text2, num3);
							array[1] = new KeyValuePair<string, object>(text, this.requestId);
							this.data.Add(array);
							num--;
						}
						else
						{
							string formatString = string.Format("Malformed OpTrace data, could not be decoded or used. OpTrace: {0}", events[i]);
							TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), formatString, new object[0]);
						}
					}
				}
			}
			if (num == 1)
			{
				this.data.Add(this.GetTruncationEvent());
			}
		}

		public void WriteEvent()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (this.data.Count == this.MaxEvents)
			{
				return;
			}
			if (this.data.Count == this.MaxEvents - 1)
			{
				this.data.Add(this.GetTruncationEvent());
				return;
			}
			this.data.Add(this.currentEvent);
			this.currentEvent = null;
		}

		public List<ICollection<KeyValuePair<string, object>>> Data
		{
			get
			{
				return this.data;
			}
		}

		public string[] Serialize()
		{
			if (!this.Enabled || this.data == null)
			{
				return null;
			}
			string[] array = new string[this.data.Count];
			int num = 0;
			bool flag = false;
			foreach (IEnumerable<KeyValuePair<string, object>> enumerable in this.data)
			{
				array[num] = LogRowFormatter.FormatCollection(enumerable, out flag);
				if (flag)
				{
					byte[] array2 = Utf8Csv.EncodeAndEscape(array[num]);
					if (array2 == null)
					{
						return null;
					}
					array[num] = Encoding.UTF8.GetString(array2);
				}
				num++;
			}
			return array;
		}

		public void LogRpcStart(string server, RpcReason rpcReason)
		{
			this.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[0]);
			this.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[0]);
			this.AddProperty(DiagnosticProperty.Svr, server);
			if (rpcReason != RpcReason.None)
			{
				this.AddProperty(DiagnosticProperty.Data1, Names<RpcReason>.Map[(int)rpcReason]);
			}
			this.WriteEvent();
		}

		public void LogRpcEnd(Exception error, int count)
		{
			this.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[0]);
			this.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[1]);
			this.AddProperty(DiagnosticProperty.Cnt, count);
			if (error != null)
			{
				this.AddProperty(DiagnosticProperty.Err, error.GetType().Name);
			}
			this.WriteEvent();
		}

		private ICollection<KeyValuePair<string, object>> GetTruncationEvent()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>(3);
			this.InsertWellknownFields(list);
			list.Add(DiagnosticsContext.truncationProperty);
			return list;
		}

		private void InsertWellknownFields(List<KeyValuePair<string, object>> eventData)
		{
			if (!this.Enabled)
			{
				return;
			}
			long num = DateTime.UtcNow.ToFileTime() - this.startTime;
			if (num < 0L)
			{
				num = 0L;
			}
			int num2 = (int)num;
			eventData.Add(new KeyValuePair<string, object>(Names<DiagnosticProperty>.Map[0], num2));
			eventData.Add(new KeyValuePair<string, object>(Names<DiagnosticProperty>.Map[1], this.requestId));
			eventData.Add(new KeyValuePair<string, object>(Names<DiagnosticProperty>.Map[2], Environment.CurrentManagedThreadId));
			this.lastRts = num2;
		}

		private static int requestIdCounter;

		private static KeyValuePair<string, object> truncationProperty = new KeyValuePair<string, object>(DiagnosticProperty.Trunc.ToString(), "Trunc");

		private static CsvRowBuffer csvRowBuffer = new CsvRowBuffer(0);

		private static CsvArrayDecoder decoder = new CsvArrayDecoder(typeof(KeyValuePair<string, object>));

		private DiagnosticsLevel diagnosticsLevel;

		private int requestId;

		private long startTime = DateTime.UtcNow.ToFileTime();

		private int lastRts;

		private List<ICollection<KeyValuePair<string, object>>> data = new List<ICollection<KeyValuePair<string, object>>>(30);

		private List<KeyValuePair<string, object>> currentEvent;
	}
}
