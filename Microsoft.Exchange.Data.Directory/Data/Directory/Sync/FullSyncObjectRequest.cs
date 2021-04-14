using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class FullSyncObjectRequest : ConfigurableObject
	{
		public FullSyncObjectRequest(SyncObjectId identity, string serviceInstanceId, FullSyncObjectRequestOptions options, ExDateTime creationTime, FullSyncObjectRequestState state) : this()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (serviceInstanceId == null)
			{
				throw new ArgumentNullException("serviceInstanceId");
			}
			this.SetIdentity(identity);
			this.ServiceInstanceId = serviceInstanceId;
			this.Options = options;
			this.CreationTime = creationTime;
			this.State = state;
		}

		public FullSyncObjectRequest() : base(new SimpleProviderPropertyBag())
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.ResetChangeTracking();
		}

		public string ServiceInstanceId
		{
			get
			{
				return (string)this.propertyBag[FullSyncObjectRequestSchema.ServiceInstanceId];
			}
			set
			{
				this.propertyBag[FullSyncObjectRequestSchema.ServiceInstanceId] = value;
			}
		}

		public FullSyncObjectRequestOptions Options
		{
			get
			{
				return (FullSyncObjectRequestOptions)(this.propertyBag[FullSyncObjectRequestSchema.Options] ?? FullSyncObjectRequestOptions.None);
			}
			set
			{
				this.propertyBag[FullSyncObjectRequestSchema.Options] = value;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return (ExDateTime)this.propertyBag[FullSyncObjectRequestSchema.CreationTime];
			}
			set
			{
				this.propertyBag[FullSyncObjectRequestSchema.CreationTime] = value;
			}
		}

		public FullSyncObjectRequestState State
		{
			get
			{
				return (FullSyncObjectRequestState)(this.propertyBag[FullSyncObjectRequestSchema.State] ?? FullSyncObjectRequestState.New);
			}
			set
			{
				this.propertyBag[FullSyncObjectRequestSchema.State] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return FullSyncObjectRequest.Schema;
			}
		}

		public override string ToString()
		{
			return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}", new object[]
			{
				"#",
				1,
				this.ServiceInstanceId,
				this.Identity,
				Convert.ToInt32(this.Options),
				this.CreationTime.ToFileTimeUtc(),
				Convert.ToInt32(this.State)
			});
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != typeof(FullSyncObjectRequest)) && this.Equals((FullSyncObjectRequest)obj)));
		}

		public override int GetHashCode()
		{
			return this.CreationTime.GetHashCode() ^ ((this.Identity != null) ? this.Identity.ToString().GetHashCode() : 0) ^ this.Options.GetHashCode() ^ ((this.ServiceInstanceId != null) ? this.ServiceInstanceId.GetHashCode() : 0) ^ this.State.GetHashCode();
		}

		internal static FullSyncObjectRequest Parse(string requestBlob)
		{
			if (string.IsNullOrEmpty(requestBlob))
			{
				throw new ArgumentException("requestBlob");
			}
			FullSyncObjectRequest result;
			try
			{
				string[] array = requestBlob.Split(new string[]
				{
					"#"
				}, StringSplitOptions.None);
				if (array.Length != 6)
				{
					throw new FormatException("requestBlob");
				}
				string serviceInstanceId = array[1];
				SyncObjectId identity = SyncObjectId.Parse(array[2]);
				FullSyncObjectRequestOptions options = (FullSyncObjectRequestOptions)int.Parse(array[3]);
				ExDateTime creationTime = ExDateTime.FromFileTimeUtc(long.Parse(array[4]));
				FullSyncObjectRequestState state = (FullSyncObjectRequestState)int.Parse(array[5]);
				result = new FullSyncObjectRequest(identity, serviceInstanceId, options, creationTime, state);
			}
			catch (ArgumentException innerException)
			{
				throw new FormatException("requestBlob", innerException);
			}
			catch (FormatException innerException2)
			{
				throw new FormatException("requestBlob", innerException2);
			}
			catch (OverflowException innerException3)
			{
				throw new FormatException("requestBlob", innerException3);
			}
			return result;
		}

		internal bool Equals(FullSyncObjectRequest request)
		{
			return !object.ReferenceEquals(null, request) && (object.ReferenceEquals(this, request) || (this.CreationTime == request.CreationTime && this.Identity.Equals(request.Identity) && this.Options == request.Options && this.ServiceInstanceId == request.ServiceInstanceId && this.State == request.State));
		}

		internal void SetIdentity(SyncObjectId syncObjectId)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = syncObjectId;
		}

		private const int CurrentVersion = 1;

		private const string Delimiter = "#";

		private static readonly FullSyncObjectRequestSchema Schema = ObjectSchema.GetInstance<FullSyncObjectRequestSchema>();
	}
}
