using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveDelegateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveDelegateRequest : BaseDelegateRequest
	{
		[XmlArrayItem("UserId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public UserId[] UserIds
		{
			get
			{
				return this.userIds;
			}
			set
			{
				this.userIds = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RemoveDelegate(callContext, this);
		}

		public RemoveDelegateRequest() : base(true)
		{
		}

		private UserId[] userIds;
	}
}
