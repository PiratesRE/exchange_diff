using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class SystemMessage : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SystemMessage.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public string Text
		{
			get
			{
				return (string)this[SystemMessageSchema.Text];
			}
			set
			{
				this[SystemMessageSchema.Text] = value;
			}
		}

		public bool Internal
		{
			get
			{
				return (bool)this[SystemMessageSchema.Internal];
			}
		}

		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)this[SystemMessageSchema.Language];
			}
		}

		public EnhancedStatusCode DsnCode
		{
			get
			{
				return (EnhancedStatusCode)this[SystemMessageSchema.DsnCode];
			}
		}

		public QuotaMessageType? QuotaMessageType
		{
			get
			{
				return (QuotaMessageType?)this[SystemMessageSchema.QuotaMessageType];
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SystemMessage.mostDerivedClass;
			}
		}

		internal static ADObjectId GetDsnCustomizationContainer(ADObjectId orgContainer)
		{
			ADObjectId childId = orgContainer.GetChildId("Transport Settings");
			return childId.GetChildId("Dsn Customization");
		}

		internal static object LanguageGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				string escapedName = ((ADObjectId)propertyBag[ADObjectSchema.Id]).AncestorDN(1).Rdn.EscapedName;
				int culture;
				if (int.TryParse(escapedName, NumberStyles.None, NumberFormatInfo.InvariantInfo, out culture))
				{
					result = CultureInfo.GetCultureInfo(culture);
				}
				else
				{
					escapedName = ((ADObjectId)propertyBag[ADObjectSchema.Id]).AncestorDN(2).Rdn.EscapedName;
					if (int.TryParse(escapedName, NumberStyles.None, NumberFormatInfo.InvariantInfo, out culture))
					{
						result = CultureInfo.GetCultureInfo(culture);
					}
					else
					{
						result = null;
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Language", ex.Message), SystemMessageSchema.Language, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object CodeGetter(IPropertyBag propertyBag)
		{
			string escapedName = ((ADObjectId)propertyBag[ADObjectSchema.Id]).Rdn.EscapedName;
			EnhancedStatusCode result;
			if (EnhancedStatusCode.TryParse(escapedName, out result))
			{
				return result;
			}
			return null;
		}

		internal static object InternalGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Internal", DirectoryStrings.CannotGetDnAtDepth(null, 0)), SystemMessageSchema.Internal, propertyBag[ADObjectSchema.Id]));
				}
				string escapedName = adobjectId.AncestorDN(1).Rdn.EscapedName;
				result = !escapedName.Equals("external", StringComparison.OrdinalIgnoreCase);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Internal", ex.Message), SystemMessageSchema.Internal, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object QuotaMessageTypeGetter(IPropertyBag propertyBag)
		{
			string escapedName = ((ADObjectId)propertyBag[ADObjectSchema.Id]).Rdn.EscapedName;
			EnhancedStatusCode enhancedStatusCode;
			if (EnhancedStatusCode.TryParse(escapedName, out enhancedStatusCode))
			{
				return null;
			}
			try
			{
				return Enum.Parse(typeof(QuotaMessageType), escapedName, true);
			}
			catch (ArgumentException)
			{
			}
			return null;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!ClientCultures.IsCultureSupportedForDsnCustomization(this.Language))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.DsnLanguageNotSupportedForCustomization, SystemMessageSchema.Language, this));
			}
		}

		public const string InternalString = "internal";

		public const string ExternalString = "external";

		private static SystemMessageSchema schema = ObjectSchema.GetInstance<SystemMessageSchema>();

		private static string mostDerivedClass = "msExchDSNMessage";
	}
}
