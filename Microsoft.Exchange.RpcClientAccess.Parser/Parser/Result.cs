using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class Result
	{
		protected Result(RopId ropId)
		{
			this.ropId = ropId;
		}

		protected Result(Reader reader)
		{
			this.ropId = (RopId)reader.ReadByte();
		}

		public RopId RopId
		{
			get
			{
				return this.ropId;
			}
		}

		public Encoding String8Encoding
		{
			get
			{
				return this.string8Encoding;
			}
			set
			{
				this.string8Encoding = value;
			}
		}

		internal virtual void Serialize(Writer writer)
		{
			if (this.string8Encoding == null)
			{
				throw new InvalidOperationException("No encoding was set on this result");
			}
			writer.WriteByte((byte)this.ropId);
		}

		public override bool Equals(object obj)
		{
			string message = "Results don't support Equals.  To verify equality of Results in test code, use test EqualityComparer.";
			throw new NotSupportedException(message);
		}

		public override int GetHashCode()
		{
			string message = "Results don't support GetHashCode. Use test EqualityComparer.";
			throw new NotSupportedException(message);
		}

		private readonly RopId ropId;

		private Encoding string8Encoding;
	}
}
