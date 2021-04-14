using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	[Serializable]
	public class ComplianceJobId : ObjectId
	{
		public ComplianceJobId()
		{
			this.complianceJobId = Guid.NewGuid();
		}

		public ComplianceJobId(ComplianceJobId id)
		{
			this.complianceJobId = id.complianceJobId;
		}

		public ComplianceJobId(Guid guid)
		{
			this.complianceJobId = guid;
		}

		public ComplianceJobId(byte[] bytes) : this(new Guid(bytes))
		{
		}

		public Guid Guid
		{
			get
			{
				return this.complianceJobId;
			}
		}

		public override byte[] GetBytes()
		{
			return this.complianceJobId.ToByteArray();
		}

		public override string ToString()
		{
			return this.complianceJobId.ToString();
		}

		private readonly Guid complianceJobId;
	}
}
