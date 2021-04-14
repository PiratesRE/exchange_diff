using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync
{
	[Serializable]
	public class SyncData : ConfigurableObject
	{
		internal SyncData(byte[] cookie, object response) : this()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New SyncData");
			this.response = response;
			this.Data = SyncObject.SerializeResponse(response, !SyncConfiguration.SkipSchemaValidation);
			this.Identity = ((cookie != null) ? Convert.ToBase64String(cookie) : null);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncData cookie {0}", this.Identity);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncData data {0}", this.Data);
		}

		internal SyncData() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public new string Identity
		{
			get
			{
				return (string)this.propertyBag[SyncDataSchema.Identity];
			}
			private set
			{
				this.propertyBag[SyncDataSchema.Identity] = value;
			}
		}

		public string Data
		{
			get
			{
				return (string)this.propertyBag[SyncDataSchema.Data];
			}
			private set
			{
				this.propertyBag[SyncDataSchema.Data] = value;
			}
		}

		internal object Response
		{
			get
			{
				return this.response;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<SyncDataSchema>();
			}
		}

		[NonSerialized]
		private readonly object response;
	}
}
