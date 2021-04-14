using System;
using Microsoft.Ceres.External.ContentApi;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class FastDocument : IFastDocument
	{
		internal FastDocument(IDiagnosticsSession diagnosticsSession, string contextId, DocumentOperation operation)
		{
			this.diagnosticsSession = diagnosticsSession;
			this.document = new Document(contextId, FastDocument.GetFastOperation(operation));
			this.correlationId = Guid.NewGuid();
			this.SetGuid("CorrelationId", this.correlationId);
		}

		public int AttemptCount
		{
			get
			{
				return this.attemptCount;
			}
			set
			{
				this.SetInteger(FastIndexSystemSchema.AttemptCount.Name, value);
				this.attemptCount = value;
			}
		}

		public Document Document
		{
			get
			{
				return this.document;
			}
		}

		public string CompositeItemId
		{
			get
			{
				return this.compositeItemId;
			}
			set
			{
				this.SetString(FastIndexSystemSchema.ItemId.Name, value);
				this.compositeItemId = value;
			}
		}

		public string ContextId
		{
			get
			{
				return this.document.DocumentId;
			}
		}

		public Guid CorrelationId
		{
			get
			{
				return this.correlationId;
			}
		}

		public long DocumentId
		{
			get
			{
				return this.documentId;
			}
			set
			{
				this.SetLong(FastIndexSystemSchema.DocumentId.Name, value);
				this.documentId = value;
			}
		}

		public int ErrorCode
		{
			get
			{
				if (this.errorCode == null)
				{
					return 0;
				}
				return this.errorCode.Value;
			}
			set
			{
				this.errorCode = new int?(value);
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.SetString("errormessage", value);
				this.errorMessage = value;
			}
		}

		public int FeedingVersion
		{
			get
			{
				return this.feedingVersion;
			}
			set
			{
				this.SetInteger(FastIndexSystemSchema.Version.Name, value);
				this.feedingVersion = value;
			}
		}

		public string FlowOperation
		{
			get
			{
				return this.flowOperation;
			}
			set
			{
				this.SetString("ExchangeCtsFlowOperation", value);
				this.flowOperation = value;
			}
		}

		public string FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.SetString(FastIndexSystemSchema.FolderId.Name, value);
				this.folderId = value;
			}
		}

		public long IndexId
		{
			get
			{
				return this.indexId;
			}
			set
			{
				this.SetLong("indexid", value);
				this.indexId = value;
			}
		}

		public string IndexSystemName
		{
			get
			{
				return this.indexSystemName;
			}
			set
			{
				this.SetString("indexsystemname", value);
				this.indexSystemName = value;
			}
		}

		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
			set
			{
				this.SetString("instancename", value);
				this.instanceName = value;
			}
		}

		public bool IsLocalMdb
		{
			get
			{
				return this.isLocalMdb;
			}
			set
			{
				this.SetBool("islocalmdb", value);
				this.isLocalMdb = value;
			}
		}

		public bool IsMoveDestination
		{
			get
			{
				return this.isMoveDestination;
			}
			set
			{
				this.SetBool("ismovedestination", value);
				this.isMoveDestination = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
			set
			{
				this.SetString(FastIndexSystemSchema.MailboxGuid.Name, value.ToString());
				this.mailboxGuid = value;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				this.SetInteger("port", value);
				this.port = value;
			}
		}

		public int MessageFlags
		{
			get
			{
				return this.messageFlags;
			}
			set
			{
				this.SetInteger("messageflags", value);
				this.messageFlags = value;
			}
		}

		public Guid TenantId
		{
			get
			{
				return this.tenantId;
			}
			set
			{
				this.SetGuid("tenantid", value);
				this.tenantId = value;
			}
		}

		public bool Tracked { get; set; }

		public string TransportContextId
		{
			get
			{
				return this.transportContextId;
			}
			set
			{
				this.SetString("contextid", value);
				this.transportContextId = value;
			}
		}

		public long Watermark
		{
			get
			{
				return this.watermark;
			}
			set
			{
				this.SetLong(FastIndexSystemSchema.Watermark.Name, value);
				this.watermark = value;
			}
		}

		public void PrepareForSubmit()
		{
			this.SetDateTime("SubmitTime", DateTime.UtcNow);
			if (this.errorCode != null)
			{
				this.SetInteger(FastIndexSystemSchema.ErrorCode.Name, this.ErrorCode);
			}
		}

		private static Operation GetFastOperation(DocumentOperation operation)
		{
			Operation result;
			switch (operation)
			{
			case DocumentOperation.Insert:
				result = Operation.Insert;
				break;
			case DocumentOperation.Update:
			case DocumentOperation.Move:
				result = Operation.Update;
				break;
			case DocumentOperation.Delete:
				result = Operation.Delete;
				break;
			case DocumentOperation.DeleteSelection:
				result = Operation.DeleteSelection;
				break;
			default:
				throw new NotSupportedException("This document operation is not supported");
			}
			return result;
		}

		private void SetBool(string name, bool value)
		{
			this.diagnosticsSession.TraceDebug<string, string, bool>("Document {0}, {1}={2}", this.ContextId, name, value);
			this.document.SetBool(name, new bool?(value));
		}

		private void SetLong(string name, long value)
		{
			this.diagnosticsSession.TraceDebug<string, string, long>("Document {0}, {1}={2}", this.ContextId, name, value);
			this.document.SetLong(name, new long?(value));
		}

		private void SetInteger(string name, int value)
		{
			this.diagnosticsSession.TraceDebug<string, string, int>("Document {0}, {1}={2}", this.ContextId, name, value);
			this.document.SetInteger(name, new int?(value));
		}

		private void SetString(string name, string value)
		{
			this.diagnosticsSession.TraceDebug<string, string, string>("Document {0}, {1}={2}", this.ContextId, name, value);
			this.document.SetString(name, value);
		}

		private void SetGuid(string name, Guid value)
		{
			this.diagnosticsSession.TraceDebug<string, string, Guid>("Document {0}, {1}={2}", this.ContextId, name, value);
			this.document.SetGuid(name, value);
		}

		private void SetDateTime(string name, DateTime value)
		{
			this.diagnosticsSession.TraceDebug<string, string, DateTime>("Document {0}, {1}={2}", this.ContextId, name, value);
			this.document.SetDateTime(name, new DateTime?(value));
		}

		private readonly Document document;

		private readonly Guid correlationId;

		private readonly IDiagnosticsSession diagnosticsSession;

		private int attemptCount;

		private string compositeItemId;

		private long documentId;

		private int? errorCode;

		private string errorMessage;

		private int feedingVersion;

		private string flowOperation;

		private string folderId;

		private long indexId;

		private string indexSystemName;

		private string instanceName;

		private bool isLocalMdb;

		private bool isMoveDestination;

		private Guid mailboxGuid;

		private int port;

		private int messageFlags;

		private Guid tenantId;

		private string transportContextId;

		private long watermark;
	}
}
