using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DetailsTemplateIdParameter : ADIdParameter
	{
		public DetailsTemplateIdParameter(string rawString) : base(rawString)
		{
			if (base.InternalADObjectId != null && base.InternalADObjectId.Rdn != null)
			{
				return;
			}
			string[] array = rawString.Split(new char[]
			{
				'\\'
			});
			if (array.Length > 2)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(rawString), "Identity");
			}
			if (!array[0].Equals("*") && !Culture.TryGetCulture(array[0], out this.language))
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(rawString), "Identity");
			}
			if (array.Length == 2)
			{
				this.type = DetailsTemplate.TranslateTemplateNameToID(array[1]);
				if (this.type == null && !array[1].Equals("*"))
				{
					throw new ArgumentException(Strings.ErrorInvalidIdentity(rawString), "Identity");
				}
			}
		}

		public DetailsTemplateIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DetailsTemplateIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public DetailsTemplateIdParameter()
		{
		}

		public static DetailsTemplateIdParameter Parse(string rawString)
		{
			return new DetailsTemplateIdParameter(rawString);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			IEnumerable<T> enumerable = null;
			notFoundReason = null;
			if (typeof(T) != typeof(DetailsTemplate))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			try
			{
				if (base.InternalADObjectId != null)
				{
					return base.GetADObjectIdObjects<T>(base.InternalADObjectId, rootId, subTreeSession, optionalData);
				}
				ADObjectId childId = ((IConfigurationSession)session).GetOrgContainerId().GetChildId("Addressing").GetChildId("Display-Templates");
				QueryFilter filter = null;
				if (this.language != null)
				{
					childId = childId.GetChildId(this.language.LCID.ToString("X"));
				}
				if (this.type != null)
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.type);
				}
				enumerable = base.PerformPrimarySearch<T>(filter, childId, session, true, optionalData);
				EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(enumerable);
				if (wrapper.HasElements())
				{
					return wrapper;
				}
				enumerable = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return enumerable;
		}

		private Culture language;

		private string type;
	}
}
