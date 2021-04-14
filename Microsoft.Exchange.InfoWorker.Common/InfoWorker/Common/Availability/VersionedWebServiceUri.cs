using System;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class VersionedWebServiceUri
	{
		public static VersionedWebServiceUri Create(WebServiceUri webServiceUri, int versionBucket)
		{
			VersionedWebServiceUri versionedWebServiceUri = new VersionedWebServiceUri();
			versionedWebServiceUri.Set(webServiceUri, versionBucket);
			return versionedWebServiceUri;
		}

		public void Set(WebServiceUri webServiceUri, int versionBucket)
		{
			if (webServiceUri == null)
			{
				throw new ArgumentNullException("webServiceUri");
			}
			if (!LocalizedString.Empty.Equals(webServiceUri.AutodiscoverFailedExceptionString))
			{
				this.uriByVersionBuckets[versionBucket] = webServiceUri;
				return;
			}
			for (int i = versionBucket; i > -1; i--)
			{
				if (i == versionBucket)
				{
					this.uriByVersionBuckets[i] = webServiceUri;
				}
				else
				{
					WebServiceUri webServiceUri2 = this.uriByVersionBuckets[i];
					if (webServiceUri2 == null)
					{
						this.uriByVersionBuckets[i] = webServiceUri;
					}
					else if (!LocalizedString.Empty.Equals(webServiceUri2.AutodiscoverFailedExceptionString))
					{
						this.uriByVersionBuckets[i] = webServiceUri;
					}
				}
			}
		}

		public WebServiceUri Get(int versionBucket)
		{
			return this.uriByVersionBuckets[versionBucket];
		}

		public void Clear(int versionBucket)
		{
			this.uriByVersionBuckets[versionBucket] = null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			for (int i = 0; i < this.uriByVersionBuckets.Length; i++)
			{
				WebServiceUri webServiceUri = this.uriByVersionBuckets[i];
				if (webServiceUri != null)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						i,
						":",
						webServiceUri.ToString(),
						";"
					}));
				}
			}
			return stringBuilder.ToString();
		}

		private WebServiceUri[] uriByVersionBuckets = new WebServiceUri[4];
	}
}
