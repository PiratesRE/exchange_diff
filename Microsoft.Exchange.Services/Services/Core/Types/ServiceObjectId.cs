using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class ServiceObjectId
	{
		internal BaseServerIdInfo GetServerInfo(bool isHierarchicalOperation)
		{
			if (this.serverInfo == null)
			{
				this.InitServerInfo(isHierarchicalOperation);
			}
			return this.serverInfo;
		}

		internal void SetServerInfo(BaseServerIdInfo serverInfo)
		{
			this.serverInfo = serverInfo;
		}

		internal abstract BasicTypes BasicType { get; }

		public ServiceObjectId()
		{
		}

		public virtual string GetId()
		{
			throw new InvalidOperationException();
		}

		public virtual string GetChangeKey()
		{
			throw new InvalidOperationException();
		}

		protected virtual void InitServerInfo(bool isHierarchicalOperation)
		{
			CallContext callContext = CallContext.Current;
			IdHeaderInformation header = IdConverter.ConvertFromConcatenatedId(this.GetId(), this.BasicType, null, false);
			this.serverInfo = IdConverter.ServerInfoFromIdHeaderInformation(callContext, header, isHierarchicalOperation);
		}

		private BaseServerIdInfo serverInfo;
	}
}
