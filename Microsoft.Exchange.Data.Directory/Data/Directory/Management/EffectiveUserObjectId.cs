using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	internal class EffectiveUserObjectId : ObjectId
	{
		internal EffectiveUserObjectId(ADObjectId originalId, ADObjectId effectiveUser)
		{
			this.effectiveUserId = effectiveUser;
			this.identity = originalId;
		}

		public override byte[] GetBytes()
		{
			return this.identity.GetBytes();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.identity.ToString());
			stringBuilder.Append('\\');
			stringBuilder.Append(this.effectiveUserId.Name);
			return stringBuilder.ToString();
		}

		private ADObjectId effectiveUserId;

		private ADObjectId identity;
	}
}
