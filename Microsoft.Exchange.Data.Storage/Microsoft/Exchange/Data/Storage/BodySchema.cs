using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BodySchema : Schema
	{
		public new static BodySchema Instance
		{
			get
			{
				if (BodySchema.instance == null)
				{
					BodySchema.instance = new BodySchema();
				}
				return BodySchema.instance;
			}
		}

		internal static readonly StorePropertyDefinition BodyContentBase = InternalSchema.BodyContentBase;

		internal static readonly StorePropertyDefinition BodyContentLocation = InternalSchema.BodyContentLocation;

		public static readonly StorePropertyDefinition Codepage = InternalSchema.Codepage;

		public static readonly StorePropertyDefinition InternetCpid = InternalSchema.InternetCpid;

		internal static readonly StorePropertyDefinition RtfInSync = InternalSchema.RtfInSync;

		internal static readonly StorePropertyDefinition RtfBody = InternalSchema.RtfBody;

		internal static readonly StorePropertyDefinition HtmlBody = InternalSchema.HtmlBody;

		internal static readonly StorePropertyDefinition TextBody = InternalSchema.TextBody;

		private static BodySchema instance = null;
	}
}
