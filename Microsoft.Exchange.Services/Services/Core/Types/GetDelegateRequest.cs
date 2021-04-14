using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetDelegateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetDelegateRequest : BaseDelegateRequest
	{
		public GetDelegateRequest() : base(false)
		{
		}

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

		[XmlAttribute("IncludePermissions")]
		public bool IncludePermissions
		{
			get
			{
				return this.includePermissions;
			}
			set
			{
				this.includePermissions = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetDelegate(callContext, this);
		}

		private UserId[] userIds;

		private bool includePermissions;
	}
}
