using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class BaseMailboxSearchParameters : SetObjectProperties
	{
		[DataMember]
		public string Name
		{
			get
			{
				return (string)base[SearchObjectBaseSchema.Name];
			}
			set
			{
				base[SearchObjectBaseSchema.Name] = value;
			}
		}

		[DataMember]
		public string Recipients
		{
			get
			{
				return base[SearchObjectSchema.Recipients].StringArrayJoin(",");
			}
			set
			{
				base[SearchObjectSchema.Recipients] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public bool SearchDumpster
		{
			get
			{
				return (bool)(base[SearchObjectSchema.SearchDumpster] ?? false);
			}
			set
			{
				base[SearchObjectSchema.SearchDumpster] = value;
			}
		}

		[DataMember]
		public string SearchQuery
		{
			get
			{
				return (string)base[SearchObjectSchema.SearchQuery];
			}
			set
			{
				base[SearchObjectSchema.SearchQuery] = value;
			}
		}

		[DataMember]
		public bool IncludeUnsearchableItems
		{
			get
			{
				return (bool)(base[SearchObjectSchema.IncludeUnsearchableItems] ?? false);
			}
			set
			{
				base[SearchObjectSchema.IncludeUnsearchableItems] = value;
			}
		}

		[DataMember]
		public string Senders
		{
			get
			{
				return base[SearchObjectSchema.Senders].StringArrayJoin(",");
			}
			set
			{
				base[SearchObjectSchema.Senders] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public bool SendMeEmailOnComplete
		{
			get
			{
				return base[SearchObjectSchema.StatusMailRecipients] != null;
			}
			set
			{
				base[SearchObjectSchema.StatusMailRecipients] = (value ? RbacPrincipal.Current.ExecutingUserId : null);
			}
		}

		[DataMember]
		public Identity TargetMailbox
		{
			get
			{
				return Identity.FromIdParameter(base[SearchObjectSchema.TargetMailbox]);
			}
			set
			{
				base[SearchObjectSchema.TargetMailbox] = value.ToIdParameter();
			}
		}

		[DataMember]
		public bool EnableFullLogging
		{
			get
			{
				return LoggingLevel.Full.Equals(base[SearchObjectSchema.LogLevel]);
			}
			set
			{
				base[SearchObjectSchema.LogLevel] = (value ? LoggingLevel.Full : LoggingLevel.Basic);
			}
		}

		[DataMember]
		public bool EstimateOnly
		{
			get
			{
				return (bool)(base[SearchObjectSchema.EstimateOnly] ?? false);
			}
			set
			{
				base[SearchObjectSchema.EstimateOnly] = value;
			}
		}

		[DataMember]
		public bool ExcludeDuplicateMessages
		{
			get
			{
				return (bool)(base[SearchObjectSchema.ExcludeDuplicateMessages] ?? true);
			}
			set
			{
				base[SearchObjectSchema.ExcludeDuplicateMessages] = value;
			}
		}
	}
}
