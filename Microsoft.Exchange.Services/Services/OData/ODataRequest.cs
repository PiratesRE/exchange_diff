using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ODataRequest
	{
		public ODataRequest(ODataContext odataContext)
		{
			ArgumentValidator.ThrowIfNull("odataContext", odataContext);
			this.ODataContext = odataContext;
		}

		public ODataContext ODataContext { get; private set; }

		public ODataQueryOptions ODataQueryOptions
		{
			get
			{
				return this.ODataContext.ODataQueryOptions;
			}
		}

		public string IfMatchETag { get; protected set; }

		public virtual void LoadFromHttpRequest()
		{
			string text = this.ODataContext.HttpContext.Request.Headers["If-Match"];
			if (!string.IsNullOrEmpty(text))
			{
				if (text.StartsWith("W/") || text.StartsWith("w/"))
				{
					text = text.Substring(2, text.Length - 2);
				}
				text = text.Trim(new char[]
				{
					'"'
				});
				this.IfMatchETag = text;
			}
		}

		public abstract ODataCommand GetODataCommand();

		public virtual void Validate()
		{
		}

		public virtual string GetOperationNameForLogging()
		{
			string name = base.GetType().Name;
			int length = name.IndexOf("Request");
			return string.Format("OData:{0}", name.Substring(0, length));
		}

		public virtual void PerformAdditionalGrantCheck(string[] grantPresented)
		{
		}

		protected TEntity ReadPostBodyAsEntity<TEntity>() where TEntity : Entity
		{
			RequestMessageReader requestMessageReader = new RequestMessageReader(this.ODataContext);
			return (TEntity)((object)requestMessageReader.ReadPostEntity());
		}

		protected IDictionary<string, object> ReadPostBodyAsParameters()
		{
			RequestMessageReader requestMessageReader = new RequestMessageReader(this.ODataContext);
			return requestMessageReader.ReadPostParameters();
		}
	}
}
