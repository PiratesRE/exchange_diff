using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTraceBatch : IConfigurable, IEnumerable<MessageTrace>, IEnumerable
	{
		public MessageTraceBatch()
		{
			this.batch = new List<MessageTrace>();
			this.identity = new ConfigObjectId(Guid.NewGuid().ToString());
		}

		public MessageTraceBatch(Guid tenantId) : this()
		{
			if (tenantId.Equals(Guid.Empty))
			{
				throw new ArgumentException("tenantId == Guid.Empty");
			}
			this.organizationalUnitRoot = new Guid?(tenantId);
		}

		public int? PartionId
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

		public int MessageCount
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

		public Guid? OrganizationalUnitRoot
		{
			get
			{
				return this.organizationalUnitRoot;
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

		public IEnumerator<MessageTrace> GetEnumerator()
		{
			return this.batch.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(MessageTrace messageTrace)
		{
			if (this.OrganizationalUnitRoot != null && !messageTrace.OrganizationalUnitRoot.Equals(this.OrganizationalUnitRoot))
			{
				throw new ArgumentException("Unable to add a message to the batch because it is for a different tenant.", "messageTrace");
			}
			this.batch.Add(messageTrace);
		}

		public void Add(IEnumerable<MessageTrace> messageTraces)
		{
			if (this.OrganizationalUnitRoot != null)
			{
				MessageTrace messageTrace2 = messageTraces.FirstOrDefault((MessageTrace messageTrace) => !messageTrace.OrganizationalUnitRoot.Equals(this.OrganizationalUnitRoot));
				if (messageTrace2 != null)
				{
					string message = string.Format("Unable to add a message with ExMessageId {1} to the batch because it is for a different tenant {0}", messageTrace2.OrganizationalUnitRoot, messageTrace2.ExMessageId);
					throw new ArgumentException(message, "messageTraces");
				}
			}
			this.batch.AddRange(messageTraces);
		}

		private readonly Guid? organizationalUnitRoot;

		private int? fssCopyId;

		private int? fssPartitionId;

		private List<MessageTrace> batch;

		private ObjectId identity;
	}
}
