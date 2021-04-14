using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class ServiceModel
	{
		public ServiceModel()
		{
			this.EdmModel = this.BuildModel();
		}

		public IEdmModel EdmModel { get; private set; }

		public virtual ODataVersion ProtocolVersion
		{
			get
			{
				return 0;
			}
		}

		protected abstract EdmModel BuildModel();

		public static readonly LazyMember<ServiceModel> Version1Model = new LazyMember<ServiceModel>(() => new Version1Model());
	}
}
