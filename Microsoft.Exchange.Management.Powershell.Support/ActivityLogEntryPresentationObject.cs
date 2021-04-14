using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class ActivityLogEntryPresentationObject : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ActivityLogEntryPresentationObject.Schema;
			}
		}

		internal ActivityLogEntryPresentationObject(Activity activity) : this()
		{
			this.ClientId = activity.ClientId.ToString();
			this.ActivityId = Enum.GetName(typeof(ActivityId), activity.Id);
			this.TimeStamp = activity.TimeStamp;
			object itemId = activity.ItemId;
			this.CustomProperties = activity.CustomPropertiesString;
			if (itemId != null)
			{
				this.EntryId = ((StoreObjectId)itemId).ToBase64String();
			}
			base.ResetChangeTracking();
		}

		public ActivityLogEntryPresentationObject() : base(new SimpleProviderPropertyBag())
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
		}

		[Parameter(Mandatory = true)]
		public string ClientId
		{
			get
			{
				return (string)this[ActivityLogEntryPresentationObjectSchema.ClientId];
			}
			set
			{
				this[ActivityLogEntryPresentationObjectSchema.ClientId] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ActivityId
		{
			get
			{
				return (string)this[ActivityLogEntryPresentationObjectSchema.ActivityId];
			}
			set
			{
				this[ActivityLogEntryPresentationObjectSchema.ActivityId] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ExDateTime TimeStamp
		{
			get
			{
				return (ExDateTime)this[ActivityLogEntryPresentationObjectSchema.TimeStamp];
			}
			set
			{
				this[ActivityLogEntryPresentationObjectSchema.TimeStamp] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string EntryId
		{
			get
			{
				return (string)this[ActivityLogEntryPresentationObjectSchema.EntryId];
			}
			set
			{
				this[ActivityLogEntryPresentationObjectSchema.EntryId] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string CustomProperties
		{
			get
			{
				return (string)this[ActivityLogEntryPresentationObjectSchema.CustomProperties];
			}
			set
			{
				this[ActivityLogEntryPresentationObjectSchema.CustomProperties] = value;
			}
		}

		private static readonly ActivityLogEntryPresentationObjectSchema Schema = ObjectSchema.GetInstance<ActivityLogEntryPresentationObjectSchema>();
	}
}
