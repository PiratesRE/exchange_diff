using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class CalendarModuleViewState : ModuleViewState
	{
		public CalendarModuleViewState(StoreObjectId folderId, string folderType, ExDateTime dateTime) : base(NavigationModule.Calendar, folderId, folderType)
		{
			this.dateTime = dateTime;
		}

		public ExDateTime DateTime
		{
			get
			{
				return this.dateTime;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = base.ToPreFormActionResponse();
			preFormActionResponse.AddParameter("yr", this.dateTime.Year.ToString());
			preFormActionResponse.AddParameter("mn", this.dateTime.Month.ToString());
			preFormActionResponse.AddParameter("dy", this.dateTime.Day.ToString());
			return preFormActionResponse;
		}

		private ExDateTime dateTime;
	}
}
