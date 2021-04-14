using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsCertStore
	{
		public ApnsCertStore(X509Store store)
		{
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			this.Store = store;
		}

		public virtual X509Certificate2Collection Certificates
		{
			get
			{
				return this.Store.Certificates;
			}
		}

		public StoreLocation Location
		{
			get
			{
				return this.Store.Location;
			}
		}

		public string Name
		{
			get
			{
				return this.Store.Name;
			}
		}

		private X509Store Store { get; set; }

		public virtual void Open(OpenFlags flags)
		{
			this.Store.Open(flags);
		}

		public virtual void Close()
		{
			this.Store.Close();
		}
	}
}
