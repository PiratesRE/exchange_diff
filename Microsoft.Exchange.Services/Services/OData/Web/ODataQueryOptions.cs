using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class ODataQueryOptions
	{
		public ODataQueryOptions(HttpContext httpContext, ODataUriParser uriParser)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			ArgumentValidator.ThrowIfNull("uriParser", uriParser);
			this.httpContext = httpContext;
			this.oDataUriParser = uriParser;
			this.Populate();
		}

		private ODataQueryOptions()
		{
		}

		public int? Top { get; private set; }

		public int? Skip { get; private set; }

		public string[] Select { get; private set; }

		public FilterClause Filter { get; private set; }

		public OrderByClause OrderBy { get; private set; }

		public bool InlineCount { get; private set; }

		public bool Expands(string propertyName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("propertyName", propertyName);
			return this.rawValues != null && this.rawValues.Expand != null && this.rawValues.Expand.Contains(propertyName);
		}

		private void Populate()
		{
			this.rawValues = new ODataRawQueryOptions(this.httpContext.Request.QueryString);
			if (!string.IsNullOrEmpty(this.rawValues.Top))
			{
				int value;
				if (!int.TryParse(this.rawValues.Top, out value))
				{
					throw new InvalidUrlQueryException(string.Format("$top - '{0}'", this.rawValues.Top));
				}
				this.Top = new int?(value);
			}
			if (!string.IsNullOrEmpty(this.rawValues.Skip))
			{
				int value2;
				if (!int.TryParse(this.rawValues.Skip, out value2))
				{
					throw new InvalidUrlQueryException(string.Format("$skip - '{0}'", this.rawValues.Skip));
				}
				this.Skip = new int?(value2);
			}
			if (!string.IsNullOrEmpty(this.rawValues.Select))
			{
				this.Select = this.rawValues.Select.Split(new char[]
				{
					','
				});
			}
			if (!string.IsNullOrEmpty(this.rawValues.Filter))
			{
				try
				{
					this.Filter = this.oDataUriParser.ParseFilter();
				}
				catch (ODataException internalException)
				{
					throw new InvalidUrlQueryException("$filter", internalException);
				}
			}
			if (!string.IsNullOrEmpty(this.rawValues.OrderBy))
			{
				try
				{
					this.OrderBy = this.oDataUriParser.ParseOrderBy();
				}
				catch (ODataException internalException2)
				{
					throw new InvalidUrlQueryException("$orderby", internalException2);
				}
			}
			if (!string.IsNullOrEmpty(this.rawValues.InlineCount))
			{
				string inlineCount;
				if ((inlineCount = this.rawValues.InlineCount) != null)
				{
					if (inlineCount == "allpages")
					{
						this.InlineCount = true;
						return;
					}
					if (inlineCount == "none")
					{
						this.InlineCount = false;
						return;
					}
				}
				throw new InvalidUrlQueryException(string.Format("$inlinecount - '{0}'", this.rawValues.InlineCount));
			}
		}

		public static readonly ODataQueryOptions Empty = new ODataQueryOptions();

		private HttpContext httpContext;

		private ODataUriParser oDataUriParser;

		private ODataRawQueryOptions rawValues;
	}
}
