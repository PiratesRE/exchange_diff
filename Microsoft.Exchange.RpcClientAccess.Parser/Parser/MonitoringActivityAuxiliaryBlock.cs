using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MonitoringActivityAuxiliaryBlock : AuxiliaryBlock
	{
		public MonitoringActivityAuxiliaryBlock(string activityContent) : base(1, AuxiliaryBlockTypes.MonitoringActivity)
		{
			this.activityContent = activityContent;
		}

		internal MonitoringActivityAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.activityContent = reader.ReadString8(MonitoringActivityAuxiliaryBlock.traceEncoding, StringFlags.IncludeNull);
		}

		public string ActivityContent
		{
			get
			{
				return this.activityContent;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteString8(this.activityContent, MonitoringActivityAuxiliaryBlock.traceEncoding, StringFlags.IncludeNull);
		}

		protected override int Truncate(int maxSerializedSize, int currentSize)
		{
			byte[] bytes = MonitoringActivityAuxiliaryBlock.traceEncoding.GetBytes(this.ActivityContent);
			if (currentSize > maxSerializedSize && currentSize - maxSerializedSize < bytes.Length)
			{
				this.activityContent = MonitoringActivityAuxiliaryBlock.traceEncoding.GetString(bytes, 0, maxSerializedSize - (currentSize - bytes.Length));
				return maxSerializedSize;
			}
			return currentSize;
		}

		private static readonly Encoding traceEncoding = Encoding.UTF8;

		private string activityContent;
	}
}
