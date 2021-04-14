using System;
using System.Linq;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Send")]
	[AllowedOAuthGrant("Mail.Write")]
	internal class CreateMessageRequest : CreateEntityRequest<Message>
	{
		public CreateMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string ParentFolderId { get; protected set; }

		public MessageDisposition MessageDisposition { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.EntitySegment is NavigationPropertySegment)
			{
				if (base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment && base.ODataContext.ODataPath.ParentOfEntitySegment.EdmType.Equals(Folder.EdmEntityType))
				{
					this.ParentFolderId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
				}
				else if (base.ODataContext.ODataPath.ParentOfEntitySegment is NavigationPropertySegment)
				{
					this.ParentFolderId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetPropertyName();
				}
			}
			MessageDisposition? queryEnumValue = base.ODataContext.QueryString.GetQueryEnumValue("MessageDisposition");
			if (queryEnumValue == null)
			{
				if (string.IsNullOrEmpty(this.ParentFolderId))
				{
					queryEnumValue = new MessageDisposition?(MessageDisposition.SendOnly);
				}
				else
				{
					queryEnumValue = new MessageDisposition?(MessageDisposition.SaveOnly);
				}
			}
			this.MessageDisposition = queryEnumValue.Value;
		}

		public override void PerformAdditionalGrantCheck(string[] grantPresented)
		{
			base.PerformAdditionalGrantCheck(grantPresented);
			if (this.MessageDisposition == MessageDisposition.SendOnly && grantPresented.Contains("Mail.Send"))
			{
				return;
			}
			if (this.MessageDisposition == MessageDisposition.SaveOnly && grantPresented.Contains("Mail.Write"))
			{
				return;
			}
			if (this.MessageDisposition == MessageDisposition.SendAndSaveCopy && grantPresented.Contains("Mail.Send") && grantPresented.Contains("Mail.Write"))
			{
				return;
			}
			throw new ODataAuthorizationException(new InvalidOAuthTokenException(OAuthErrors.NotEnoughGrantPresented, null, null));
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateMessageCommand(this);
		}

		public const string MessageDispositionParameter = "MessageDisposition";
	}
}
