using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbConnectResponse : MapiHttpOperationResponse
	{
		public EmsmdbConnectResponse(uint returnCode, uint maxPollingInterval, uint retryCount, uint retryDelay, string dnPrefix, string displayName, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.maxPollingInterval = maxPollingInterval;
			this.retryCount = retryCount;
			this.retryDelay = retryDelay;
			this.dnPrefix = dnPrefix;
			this.displayName = displayName;
		}

		public EmsmdbConnectResponse(Reader reader) : base(reader)
		{
			this.maxPollingInterval = reader.ReadUInt32();
			this.retryCount = reader.ReadUInt32();
			this.retryDelay = reader.ReadUInt32();
			this.dnPrefix = reader.ReadAsciiString(StringFlags.IncludeNull);
			this.displayName = reader.ReadUnicodeString(StringFlags.IncludeNull);
			base.ParseAuxiliaryBuffer(reader);
		}

		public uint MaxPollingInterval
		{
			get
			{
				return this.maxPollingInterval;
			}
		}

		public uint RetryCount
		{
			get
			{
				return this.retryCount;
			}
		}

		public uint RetryDelay
		{
			get
			{
				return this.retryDelay;
			}
		}

		public string DnPrefix
		{
			get
			{
				return this.dnPrefix;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.maxPollingInterval);
			writer.WriteUInt32(this.retryCount);
			writer.WriteUInt32(this.retryDelay);
			writer.WriteAsciiString(this.dnPrefix, StringFlags.IncludeNull);
			writer.WriteUnicodeString(this.displayName, StringFlags.IncludeNull);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint maxPollingInterval;

		private readonly uint retryCount;

		private readonly uint retryDelay;

		private readonly string dnPrefix;

		private readonly string displayName;
	}
}
