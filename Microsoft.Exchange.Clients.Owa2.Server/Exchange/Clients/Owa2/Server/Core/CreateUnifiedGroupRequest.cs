using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "CreateUnifiedGroupRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateUnifiedGroupRequest : BaseRequest
	{
		[DataMember(Name = "Name", IsRequired = true)]
		public string Name { get; set; }

		[DataMember(Name = "Alias", IsRequired = true)]
		public string Alias { get; set; }

		[DataMember(Name = "Description", IsRequired = false)]
		public string Description { get; set; }

		[DataMember(Name = "GroupType", IsRequired = true)]
		public ModernGroupObjectType GroupType { get; set; }

		[DataMember(Name = "AutoSubscribeNewGroupMembers", IsRequired = false)]
		public bool AutoSubscribeNewGroupMembers { get; set; }

		[DataMember(Name = "PushToken", IsRequired = false)]
		public string PushToken { get; set; }

		[DataMember(Name = "CultureId", IsRequired = false)]
		public string CultureId { get; set; }

		internal CultureInfo Language { get; private set; }

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override void Validate()
		{
			if (string.IsNullOrWhiteSpace(this.Name))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (string.IsNullOrWhiteSpace(this.Alias) || !CreateUnifiedGroupRequest.ValidAliasRegex.IsMatch(this.Alias))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (this.GroupType != ModernGroupObjectType.Public && this.GroupType != ModernGroupObjectType.Private)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (this.CultureId != null)
			{
				try
				{
					this.Language = CultureInfo.CreateSpecificCulture(this.CultureId);
				}
				catch (CultureNotFoundException innerException)
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(innerException), FaultParty.Sender);
				}
			}
		}

		private static readonly Regex ValidAliasRegex = new Regex("^[A-Za-z0-9-_]+$");
	}
}
