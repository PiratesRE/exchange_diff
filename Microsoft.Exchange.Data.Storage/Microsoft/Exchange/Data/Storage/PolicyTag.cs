using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyTag
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public EnhancedTimeSpan TimeSpanForRetention
		{
			get
			{
				return this.timeSpanForRetention;
			}
			set
			{
				this.timeSpanForRetention = value;
			}
		}

		public Guid PolicyGuid
		{
			get
			{
				return this.policyGuid;
			}
			set
			{
				this.policyGuid = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
			set
			{
				this.isVisible = value;
			}
		}

		public bool OptedInto
		{
			get
			{
				return this.optedInto;
			}
			set
			{
				this.optedInto = value;
			}
		}

		public bool IsArchive
		{
			get
			{
				return this.isArchive;
			}
			set
			{
				this.isArchive = value;
			}
		}

		public ElcFolderType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ElcFolderType>(value, "ElcFolderType");
				this.type = value;
			}
		}

		public RetentionActionType RetentionAction
		{
			get
			{
				return this.retentionAction;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<RetentionActionType>(value, "RetentionActionType");
				this.retentionAction = value;
			}
		}

		private string name;

		private string description;

		private EnhancedTimeSpan timeSpanForRetention;

		private Guid policyGuid = Guid.Empty;

		private bool isVisible;

		private bool optedInto;

		private bool isArchive;

		private ElcFolderType type;

		private RetentionActionType retentionAction;
	}
}
