using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class RuleId
	{
		internal RuleId()
		{
		}

		internal RuleId(int ruleIndex, long storeRuleId)
		{
			if (ruleIndex < 0)
			{
				ruleIndex = 0;
			}
			this.ruleIndex = ruleIndex;
			this.storeRuleId = storeRuleId;
		}

		public static RuleId Deserialize(byte[] byteArrayId, int ruleIndex)
		{
			if (byteArrayId == null)
			{
				throw new ArgumentNullException("byteArrayId");
			}
			RuleId ruleId = new RuleId();
			ruleId.Parse(byteArrayId, ruleIndex);
			return ruleId;
		}

		public static RuleId Deserialize(string base64Id, int ruleIndex)
		{
			if (base64Id == null)
			{
				throw new ArgumentNullException(base64Id);
			}
			byte[] byteArrayId = StoreId.Base64ToByteArray(base64Id);
			return RuleId.Deserialize(byteArrayId, ruleIndex);
		}

		public byte[] ToByteArray()
		{
			return BitConverter.GetBytes(this.storeRuleId);
		}

		public string ToBase64String()
		{
			return Convert.ToBase64String(this.ToByteArray());
		}

		public override bool Equals(object id)
		{
			RuleId ruleId = id as RuleId;
			return ruleId != null && this.storeRuleId == ruleId.storeRuleId;
		}

		public override int GetHashCode()
		{
			if (this.hashCode == -1 && this.storeRuleId != 0L)
			{
				this.hashCode = this.storeRuleId.GetHashCode();
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			return this.ToBase64String();
		}

		public long StoreRuleId
		{
			get
			{
				return this.storeRuleId;
			}
		}

		internal int RuleIndex
		{
			get
			{
				return this.ruleIndex;
			}
		}

		internal void Parse(byte[] idBytes, int ruleIndex)
		{
			if (0 > ruleIndex)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "RuleId::Parse. The rule index {0} is negative, hence invalid.", ruleIndex);
				throw new InvalidDataException(ServerStrings.ExRuleIdInvalid);
			}
			if (8 != idBytes.Length)
			{
				ExTraceGlobals.StorageTracer.TraceError<int, int>((long)this.GetHashCode(), "RuleId::Parse. The byte array length {0} does not match the expected length of {1}, hence it is invalid.", idBytes.Length, 8);
				throw new InvalidDataException(ServerStrings.ExRuleIdInvalid);
			}
			this.storeRuleId = BitConverter.ToInt64(idBytes, 0);
			this.ruleIndex = ruleIndex;
		}

		private int hashCode = -1;

		private long storeRuleId;

		private int ruleIndex;
	}
}
