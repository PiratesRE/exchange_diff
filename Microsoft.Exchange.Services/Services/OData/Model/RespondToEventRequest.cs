using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class RespondToEventRequest : EntityActionRequest<Event>
	{
		public RespondToEventRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string Comment { get; protected set; }

		public RespondToEventParameters RespondToEventParameters { get; protected set; }

		public override ODataCommand GetODataCommand()
		{
			return new RespondToEventCommand(this);
		}

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			this.RespondToEventParameters = new RespondToEventParameters();
			this.RespondToEventParameters.SendResponse = true;
			object obj;
			if (base.Parameters.TryGetValue("Comment", out obj))
			{
				this.RespondToEventParameters.Notes = new ItemBody();
				this.RespondToEventParameters.Notes.ContentType = BodyType.Text;
				this.RespondToEventParameters.Notes.Content = (string)obj;
			}
			string actionName = base.ActionName;
			string a;
			if ((a = actionName) != null)
			{
				if (a == "TentativelyAccept")
				{
					this.RespondToEventParameters.Response = ResponseType.TentativelyAccepted;
					return;
				}
				if (a == "Accept")
				{
					this.RespondToEventParameters.Response = ResponseType.Accepted;
					return;
				}
				if (!(a == "Decline"))
				{
					return;
				}
				this.RespondToEventParameters.Response = ResponseType.Declined;
			}
		}
	}
}
