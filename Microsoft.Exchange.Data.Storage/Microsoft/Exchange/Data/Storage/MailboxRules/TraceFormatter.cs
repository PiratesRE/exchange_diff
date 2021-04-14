using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TraceFormatter : DisposableObject
	{
		public TraceFormatter(bool saveTrace)
		{
			if (saveTrace)
			{
				this.traceStream = TemporaryStorage.Create();
				this.traceWriter = new StreamWriter(this.traceStream);
			}
		}

		public int NestedLevel
		{
			get
			{
				this.CheckDisposed(null);
				return this.nestedLevel;
			}
			set
			{
				this.CheckDisposed(null);
				this.nestedLevel = value;
			}
		}

		public bool HasTraceHistory
		{
			get
			{
				this.CheckDisposed(null);
				return this.traceWriter != null;
			}
		}

		public string Format(string message)
		{
			this.CheckDisposed(null);
			return this.InternalFormat(message, new object[0]);
		}

		public string Format(string format, object argument1)
		{
			this.CheckDisposed(null);
			return this.InternalFormat(format, new object[]
			{
				TraceFormatter.GetTraceValue(argument1)
			});
		}

		public string Format(string format, object argument1, object argument2)
		{
			this.CheckDisposed(null);
			return this.InternalFormat(format, new object[]
			{
				TraceFormatter.GetTraceValue(argument1),
				TraceFormatter.GetTraceValue(argument2)
			});
		}

		public string Format(string format, object argument1, object argument2, object argument3)
		{
			this.CheckDisposed(null);
			return this.InternalFormat(format, new object[]
			{
				TraceFormatter.GetTraceValue(argument1),
				TraceFormatter.GetTraceValue(argument2),
				TraceFormatter.GetTraceValue(argument3)
			});
		}

		public string Format(string format, object argument1, object argument2, object argument3, object argument4)
		{
			this.CheckDisposed(null);
			return this.InternalFormat(format, new object[]
			{
				TraceFormatter.GetTraceValue(argument1),
				TraceFormatter.GetTraceValue(argument2),
				TraceFormatter.GetTraceValue(argument3),
				TraceFormatter.GetTraceValue(argument4)
			});
		}

		public void CopyDataTo(Stream targetStream)
		{
			this.CheckDisposed(null);
			if (this.traceWriter == null)
			{
				return;
			}
			this.traceWriter.Flush();
			long position = this.traceStream.Position;
			this.traceStream.Position = 0L;
			byte[] array = new byte[2048];
			for (;;)
			{
				int num = this.traceStream.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				targetStream.Write(array, 0, num);
			}
			this.traceStream.Position = position;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.traceWriter);
				Util.DisposeIfPresent(this.traceStream);
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<TraceFormatter>(this);
		}

		private static string[] GetIndentationArray()
		{
			string[] array = new string[20];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new string(' ', i * 2);
			}
			return array;
		}

		private static object GetTraceValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value.GetType() == typeof(string))
			{
				string text = (string)value;
				if (text.Length > 256)
				{
					StringBuilder stringBuilder = new StringBuilder(text, 0, 256, 259);
					stringBuilder.Append("...");
					return stringBuilder.ToString();
				}
			}
			else if (value.GetType() == typeof(byte[]))
			{
				byte[] array = (byte[])value;
				int num = Math.Min(array.Length, 256);
				StringBuilder stringBuilder2 = new StringBuilder(num * 2 + 2);
				stringBuilder2.Append('[');
				stringBuilder2.Append(HexConverter.ByteArrayToHexString(array, 0, num));
				stringBuilder2.Append(']');
				return stringBuilder2.ToString();
			}
			return value;
		}

		private string InternalFormat(string format, params object[] arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.nestedLevel != 0)
			{
				int num = Math.Min(this.nestedLevel, TraceFormatter.indentationArray.Length - 1);
				stringBuilder.Append(TraceFormatter.indentationArray[num]);
			}
			stringBuilder.AppendFormat(format, arguments);
			string text = stringBuilder.ToString();
			if (this.traceWriter != null)
			{
				this.traceWriter.WriteLine(text);
			}
			return text;
		}

		private const int TraceStringLimit = 256;

		private static readonly string[] indentationArray = TraceFormatter.GetIndentationArray();

		private readonly StreamWriter traceWriter;

		private readonly Stream traceStream;

		private int nestedLevel;
	}
}
