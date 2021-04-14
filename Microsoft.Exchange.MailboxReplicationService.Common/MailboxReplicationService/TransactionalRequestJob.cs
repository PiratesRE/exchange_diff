using System;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class TransactionalRequestJob : RequestJobBase, IDisposable, ISendAsSource
	{
		public TransactionalRequestJob()
		{
			this.moveObject = null;
			this.provider = null;
			this.unknownElements = null;
		}

		internal TransactionalRequestJob(SimpleProviderPropertyBag propertyBag) : base(propertyBag)
		{
			this.moveObject = null;
			this.provider = null;
		}

		internal TransactionalRequestJob(RequestJobXML requestJob) : this((SimpleProviderPropertyBag)requestJob.propertyBag)
		{
			base.CopyNonSchematizedPropertiesFrom(requestJob);
			this.UnknownElements = requestJob.UnknownElements;
		}

		internal MoveObjectInfo<RequestJobXML> MoveObject
		{
			get
			{
				return this.moveObject;
			}
			set
			{
				this.moveObject = value;
			}
		}

		internal RequestJobProvider Provider
		{
			get
			{
				return this.provider;
			}
			set
			{
				this.provider = value;
			}
		}

		internal XmlElement[] UnknownElements
		{
			get
			{
				return this.unknownElements;
			}
			set
			{
				this.unknownElements = value;
			}
		}

		internal new string DomainControllerToUpdate
		{
			get
			{
				return base.DomainControllerToUpdate;
			}
			set
			{
				base.DomainControllerToUpdate = value;
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.DomainControllerUpdate, (value != null) ? new DateTime?(DateTime.UtcNow) : null);
			}
		}

		public Guid SourceGuid
		{
			get
			{
				return base.RequestGuid;
			}
		}

		public SmtpAddress UserEmailAddress
		{
			get
			{
				return base.EmailAddress;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return base.Status != RequestStatus.Failed;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void Dispose(bool disposing)
		{
			if (disposing && this.moveObject != null)
			{
				this.moveObject.Dispose();
				this.moveObject = null;
			}
		}

		internal bool CheckIfUnderlyingMessageHasChanged()
		{
			return this.MoveObject.CheckIfUnderlyingMessageHasChanged();
		}

		internal void Refresh()
		{
			RequestJobXML requestJobXML = this.MoveObject.ReadObject(ReadObjectFlags.Refresh);
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				if (propertyDefinition != SimpleProviderObjectSchema.ObjectState && propertyDefinition != SimpleProviderObjectSchema.ExchangeVersion && propertyDefinition != SimpleProviderObjectSchema.Identity)
				{
					this[propertyDefinition] = requestJobXML[propertyDefinition];
				}
			}
			this.UnknownElements = requestJobXML.UnknownElements;
		}

		[NonSerialized]
		private MoveObjectInfo<RequestJobXML> moveObject;

		[NonSerialized]
		private RequestJobProvider provider;

		[NonSerialized]
		private XmlElement[] unknownElements;
	}
}
