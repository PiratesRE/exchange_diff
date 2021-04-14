using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class EdgeSubscription : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return EdgeSubscription.schema;
			}
		}

		public ADObjectId Site
		{
			get
			{
				if (this.server != null)
				{
					return this.server.ServerSite;
				}
				return null;
			}
		}

		public string Domain
		{
			get
			{
				if (this.server != null)
				{
					return this.server.Domain;
				}
				return null;
			}
		}

		public EdgeSubscription()
		{
		}

		public EdgeSubscription(Server dataObject) : base(dataObject)
		{
			this.server = dataObject;
		}

		private static EdgeSubscriptionSchema schema = ObjectSchema.GetInstance<EdgeSubscriptionSchema>();

		private Server server;
	}
}
