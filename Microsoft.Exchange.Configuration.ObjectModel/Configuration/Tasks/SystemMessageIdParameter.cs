using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SystemMessageIdParameter : ADIdParameter
	{
		public SystemMessageIdParameter()
		{
		}

		public SystemMessageIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public SystemMessageIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected SystemMessageIdParameter(string identity) : base(identity)
		{
			string[] array = identity.Split(new char[]
			{
				'\\'
			});
			if (array.Length == 1)
			{
				if (base.InternalADObjectId == null)
				{
					throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
				}
				return;
			}
			else if (array.Length == 2)
			{
				CultureInfo cultureInfo;
				if (string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]) || !this.TryGetCulture(array[0], out cultureInfo) || !this.IsValidQuotaMessageType(array[1]))
				{
					throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
				}
				this.relativeDnParts = new string[2];
				this.relativeDnParts[0] = cultureInfo.LCID.ToString(NumberFormatInfo.InvariantInfo);
				this.relativeDnParts[1] = array[1];
				return;
			}
			else
			{
				if (array.Length != 3)
				{
					throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
				}
				CultureInfo cultureInfo2;
				if (string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]) || string.IsNullOrEmpty(array[2]) || !this.TryGetCulture(array[0], out cultureInfo2) || (!array[1].Equals("internal", StringComparison.OrdinalIgnoreCase) && !array[1].Equals("external", StringComparison.OrdinalIgnoreCase)) || !EnhancedStatusCode.IsValid(array[2]))
				{
					throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
				}
				this.relativeDnParts = new string[3];
				this.relativeDnParts[0] = cultureInfo2.LCID.ToString(NumberFormatInfo.InvariantInfo);
				this.relativeDnParts[1] = array[1];
				this.relativeDnParts[2] = array[2];
				return;
			}
		}

		public static SystemMessageIdParameter Parse(string identity)
		{
			return new SystemMessageIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			notFoundReason = null;
			List<T> list = new List<T>();
			try
			{
				if (typeof(T) != typeof(SystemMessage))
				{
					throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
				}
				if (session == null)
				{
					throw new ArgumentNullException("session");
				}
				if (string.IsNullOrEmpty(base.RawIdentity))
				{
					throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
				}
				if (base.InternalADObjectId != null)
				{
					return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
				}
				ADObjectId adobjectId = rootId;
				foreach (string unescapedCommonName in this.relativeDnParts)
				{
					adobjectId = adobjectId.GetChildId(unescapedCommonName);
				}
				if (optionalData != null && optionalData.AdditionalFilter != null)
				{
					throw new NotSupportedException("Supplying Additional Filters without an ADObjectId is not currently supported by this IdParameter.");
				}
				IConfigurable configurable = ((IConfigDataProvider)session).Read<T>(adobjectId);
				if (configurable != null)
				{
					list.Add((T)((object)configurable));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return list;
		}

		private bool TryGetCulture(string culture, out CultureInfo cultureInfo)
		{
			cultureInfo = null;
			try
			{
				cultureInfo = CultureInfo.GetCultureInfo(culture);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}

		private bool IsValidQuotaMessageType(string messageType)
		{
			try
			{
				Enum.Parse(typeof(QuotaMessageType), messageType, true);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}

		private const char CommonNameSeperatorChar = '\\';

		private string[] relativeDnParts;
	}
}
