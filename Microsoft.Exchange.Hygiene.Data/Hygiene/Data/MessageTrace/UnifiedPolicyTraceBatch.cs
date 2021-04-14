using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyTraceBatch : IConfigurable, IEnumerable<UnifiedPolicyTrace>, IEnumerable
	{
		public UnifiedPolicyTraceBatch()
		{
			this.batch = new List<UnifiedPolicyTrace>();
			this.identity = new ConfigObjectId(CombGuidGenerator.NewGuid().ToString());
		}

		public int? PartitionId
		{
			get
			{
				return this.fssPartitionId;
			}
			set
			{
				this.fssPartitionId = value;
			}
		}

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public bool IsValid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int ObjectCount
		{
			get
			{
				return this.batch.Count;
			}
		}

		public int? PersistentStoreCopyId
		{
			get
			{
				return this.fssCopyId;
			}
			set
			{
				this.fssCopyId = value;
			}
		}

		public ValidationError[] Validate()
		{
			throw new NotImplementedException();
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public IEnumerator<UnifiedPolicyTrace> GetEnumerator()
		{
			return this.batch.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(UnifiedPolicyTrace dataTrace)
		{
			if (dataTrace == null)
			{
				throw new ArgumentNullException("dataTrace");
			}
			this.batch.Add(dataTrace);
		}

		public void Add(IEnumerable<UnifiedPolicyTrace> dataObjects)
		{
			if (dataObjects == null)
			{
				throw new ArgumentNullException("dataObjects");
			}
			this.batch.AddRange(dataObjects);
		}

		private int? fssCopyId;

		private int? fssPartitionId;

		private List<UnifiedPolicyTrace> batch;

		private ObjectId identity;
	}
}
