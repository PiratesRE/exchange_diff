using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class MergedCookie : BaseCookie
	{
		public MergedCookie()
		{
		}

		public MergedCookie(string contextId, string serviceInstance, byte[] data) : base(serviceInstance, data)
		{
			if (contextId == null)
			{
				throw new ArgumentNullException("contextId");
			}
			this.ContextId = new ADObjectId(Guid.Parse(contextId));
		}

		public ADObjectId ContextId
		{
			get
			{
				return this[MergedCookieSchema.ContextIdProperty] as ADObjectId;
			}
			set
			{
				this[MergedCookieSchema.ContextIdProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MergedCookie.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MergedCookie.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly MergedCookieSchema schema = ObjectSchema.GetInstance<MergedCookieSchema>();

		private static string mostDerivedClass = "MergedCookie";
	}
}
