using System;
using System.IO;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs
{
	public sealed class OwaClientLogOutputStream : FileChunkOutputStream
	{
		public OwaClientLogOutputStream(OutputStream stream) : this(stream, Environment.MachineName + "_OWAClientLogOutputStream", Path.Combine(new JobConfiguration("default").DiagnosticsRootDirectory, "CosmosLog"), OwaClientLogOutputStream.DefaultFields)
		{
		}

		public OwaClientLogOutputStream(OutputStream stream, string name, string outputDirectory, string[] fields) : base(name, outputDirectory, fields)
		{
			this.defaultOutputStream = stream;
		}

		public override OutputStream OpenOutputStream(string analyzerName, string outputFormatName, string streamName)
		{
			if (!string.Equals(streamName, "OWARawUserLatencyData", StringComparison.OrdinalIgnoreCase))
			{
				return ((FileChunkOutputStream)this.defaultOutputStream).OpenOutputStream(analyzerName, outputFormatName, streamName);
			}
			return new OwaClientLogOutputStream.WrappedOwaClientLogOutputStream(this, analyzerName, outputFormatName);
		}

		protected override void InternalWriteHeaderLine(string format, params object[] args)
		{
		}

		protected override void InternalWriteLine(string format, params object[] args)
		{
			base.WriteRawLine(format, args);
		}

		private const string OwaRawUserDataOutputFile = "OWARawUserLatencyData";

		private static readonly string[] DefaultFields = new string[]
		{
			"DateTime",
			"MailboxGuid",
			"Country_ISP",
			"TenantGuid",
			"Country_State",
			"ConnectTime"
		};

		private readonly OutputStream defaultOutputStream;

		private class WrappedOwaClientLogOutputStream : OutputStream
		{
			public WrappedOwaClientLogOutputStream(OwaClientLogOutputStream stream, string analyzerName, string outputFormat) : base(analyzerName)
			{
				this.stream = stream;
			}

			protected override void InternalDispose(bool disposing)
			{
			}

			protected override void InternalWriteHeaderLine(string format, params object[] args)
			{
			}

			protected override void InternalWriteLine(string format, params object[] args)
			{
				this.stream.WriteRawLine(format, args);
			}

			private readonly OwaClientLogOutputStream stream;
		}
	}
}
