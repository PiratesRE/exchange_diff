using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class CustomActionRequest : ODataRequest
	{
		public CustomActionRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		protected string ActionName { get; set; }

		protected IDictionary<string, object> Parameters { get; set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			this.ActionName = base.ODataContext.ODataPath.EntitySegment.GetActionName();
			this.Parameters = base.ReadPostBodyAsParameters();
		}
	}
}
