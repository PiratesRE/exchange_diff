using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class RequestIndexEntryObjectId : ObjectId
	{
		public RequestIndexEntryObjectId(Guid requestGuid, MRSRequestType requestType, OrganizationId organizationId, RequestIndexId indexId, RequestBase owner = null)
		{
			this.RequestGuid = requestGuid;
			this.RequestType = requestType;
			this.OrganizationId = organizationId;
			this.IndexId = indexId;
			this.RequestObject = owner;
		}

		public RequestIndexEntryObjectId(Guid requestGuid, Guid targetExchangeGuid, MRSRequestType requestType, OrganizationId organizationId, RequestIndexId indexId, RequestBase owner = null) : this(requestGuid, requestType, organizationId, indexId, owner)
		{
			this.TargetExchangeGuid = targetExchangeGuid;
		}

		public Guid RequestGuid { get; private set; }

		public MRSRequestType RequestType { get; private set; }

		public RequestIndexId IndexId { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public RequestBase RequestObject { get; private set; }

		public Guid TargetExchangeGuid { get; private set; }

		public override byte[] GetBytes()
		{
			return this.RequestGuid.ToByteArray();
		}

		public override string ToString()
		{
			if (this.RequestObject != null)
			{
				return this.RequestObject.ToString();
			}
			if (this.OrganizationId != null && this.OrganizationId.OrganizationalUnit != null)
			{
				return string.Format("{0}\\{1}", this.OrganizationId.OrganizationalUnit.Name, this.RequestGuid.ToString());
			}
			return string.Format("{0}", this.RequestGuid);
		}

		public override bool Equals(object obj)
		{
			RequestIndexEntryObjectId requestIndexEntryObjectId = obj as RequestIndexEntryObjectId;
			if (requestIndexEntryObjectId != null)
			{
				bool flag = (this.IndexId != null) ? this.IndexId.Equals(requestIndexEntryObjectId.IndexId) : (requestIndexEntryObjectId.IndexId == null);
				return this.RequestGuid == requestIndexEntryObjectId.RequestGuid && this.RequestType == requestIndexEntryObjectId.RequestType && this.OrganizationId == requestIndexEntryObjectId.OrganizationId && flag && this.TargetExchangeGuid == requestIndexEntryObjectId.TargetExchangeGuid;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
