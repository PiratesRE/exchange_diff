using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class FailedMSOSyncObjectPresentationObject : ConfigurableObject
	{
		public FailedMSOSyncObjectPresentationObject(FailedMSOSyncObject divergence) : base(new SimpleProviderPropertyBag())
		{
			if (divergence == null)
			{
				throw new ArgumentNullException("divergence");
			}
			this.propertyBag = divergence.propertyBag;
			this.divergence = divergence;
		}

		public FailedMSOSyncObjectPresentationObject() : base(new SimpleProviderPropertyBag())
		{
		}

		public SyncObjectId ObjectId
		{
			get
			{
				return (SyncObjectId)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.SyncObjectId];
			}
		}

		public DateTime? DivergenceTimestamp
		{
			get
			{
				return (DateTime?)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.DivergenceTimestamp];
			}
		}

		public int DivergenceCount
		{
			get
			{
				return (int)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.DivergenceCount];
			}
		}

		public bool IsTemporary
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsTemporary];
			}
		}

		public bool IsIncrementalOnly
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsIncrementalOnly];
			}
		}

		public bool IsLinkRelated
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsLinkRelated];
			}
		}

		[Parameter]
		public bool IsIgnoredInHaltCondition
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsIgnoredInHaltCondition];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsIgnoredInHaltCondition] = value;
			}
		}

		public bool IsTenantWideDivergence
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsTenantWideDivergence];
			}
		}

		public bool IsValidationDivergence
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsValidationDivergence];
			}
		}

		[Parameter]
		public bool IsRetriable
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsRetriable];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.IsRetriable] = value;
			}
		}

		public string BuildNumber
		{
			get
			{
				return this.divergence.BuildNumber;
			}
		}

		public string TargetBuildNumber
		{
			get
			{
				return this.divergence.TargetBuildNumber;
			}
		}

		public string CmdletName
		{
			get
			{
				return this.divergence.CmdletName;
			}
		}

		public string CmdletParameters
		{
			get
			{
				return this.divergence.CmdletParameters;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.divergence.ErrorMessage;
			}
		}

		public string ErrorSymbolicName
		{
			get
			{
				return this.divergence.ErrorSymbolicName;
			}
		}

		public string ErrorStringId
		{
			get
			{
				return this.divergence.ErrorStringId;
			}
		}

		public string ErrorCategory
		{
			get
			{
				return this.divergence.ErrorCategory;
			}
		}

		public string StreamName
		{
			get
			{
				return this.divergence.StreamName;
			}
		}

		public string MinDivergenceRetryDatetime
		{
			get
			{
				return this.divergence.MinDivergenceRetryDatetime;
			}
		}

		public MultiValuedProperty<string> Errors
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.Errors];
			}
		}

		[Parameter]
		public string Comment
		{
			get
			{
				return (string)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.Comment];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.Comment] = value;
			}
		}

		public string ServiceInstanceId
		{
			get
			{
				return this.divergence.ServiceInstanceId;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new CompoundSyncObjectId(this.ObjectId, new ServiceInstanceId(this.ServiceInstanceId));
			}
		}

		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.ContextId];
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.ObjectId];
			}
		}

		public DirectoryObjectClass ExternalDirectoryObjectClass
		{
			get
			{
				return (DirectoryObjectClass)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.ExternalDirectoryObjectClass];
			}
		}

		public DateTime? WhenChanged
		{
			get
			{
				return (DateTime?)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.WhenChanged];
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this.propertyBag[FailedMSOSyncObjectPresentationObjectSchema.WhenChangedUTC];
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return FailedMSOSyncObjectPresentationObject.SchemaObject;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private readonly FailedMSOSyncObject divergence;

		private static readonly FailedMSOSyncObjectPresentationObjectSchema SchemaObject = ObjectSchema.GetInstance<FailedMSOSyncObjectPresentationObjectSchema>();
	}
}
