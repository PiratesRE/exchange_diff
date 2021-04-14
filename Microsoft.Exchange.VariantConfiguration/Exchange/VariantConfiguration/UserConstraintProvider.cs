using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.VariantConfiguration.Settings;

namespace Microsoft.Exchange.VariantConfiguration
{
	public abstract class UserConstraintProvider : IConstraintProvider
	{
		protected UserConstraintProvider(string userName, string organization, string locale, string rampId, bool isFirstRelease, bool isPreview, VariantConfigurationUserType userType)
		{
			if (userName == null)
			{
				throw new ArgumentNullException("userName");
			}
			if (organization == null)
			{
				throw new ArgumentNullException("organization");
			}
			if (locale == null)
			{
				throw new ArgumentNullException("locale");
			}
			if (rampId == null)
			{
				throw new ArgumentNullException("rampId");
			}
			this.constraints = ConstraintCollection.CreateGlobal();
			ExTraceGlobals.VariantConfigurationTracer.TraceDebug(0L, "User: {0}; Tenant: {1}; Locale: {2}; rampId: {3}; isFirstRelease: {4};", new object[]
			{
				userName,
				organization,
				locale,
				rampId,
				isFirstRelease
			});
			this.rampId = rampId;
			IOrganizationNameSettings microsoft = VariantConfiguration.InvariantNoFlightingSnapshot.VariantConfig.Microsoft;
			string item = organization.ToLowerInvariant();
			string userName2 = userName;
			if (microsoft.OrgNames.Contains(item))
			{
				organization = "Microsoft";
			}
			else
			{
				userName2 = this.FormatUserNameAndOrganization(userName, organization);
			}
			this.rotationId = ((!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(organization)) ? this.FormatUserNameAndOrganization(userName, organization) : string.Empty);
			bool isDogfood = microsoft.DogfoodOrgNames.Contains(item);
			this.PopulateConstraints(userName2, organization, locale, isDogfood, isFirstRelease, isPreview, userType);
		}

		public static List<CultureInfo> SupportedLocales
		{
			get
			{
				if (UserConstraintProvider.supportedLocales == null)
				{
					UserConstraintProvider.supportedLocales = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));
				}
				return UserConstraintProvider.supportedLocales;
			}
		}

		public ConstraintCollection Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		public string RampId
		{
			get
			{
				return this.rampId;
			}
		}

		public string RotationId
		{
			get
			{
				return this.rotationId;
			}
		}

		private void PopulateConstraints(string userName, string organization, string locale, bool isDogfood, bool isFirstRelease, bool isPreview, VariantConfigurationUserType userType)
		{
			if (!userName.Equals(string.Empty))
			{
				this.constraints.Add(VariantType.User, userName);
			}
			this.constraints.Add(VariantType.Dogfood, isDogfood);
			if (!organization.Equals(string.Empty))
			{
				this.constraints.Add(VariantType.Organization, organization);
			}
			if (!locale.Equals(string.Empty))
			{
				this.constraints.Add(VariantType.Locale, locale);
			}
			if (userType != VariantConfigurationUserType.None)
			{
				this.constraints.Add(VariantType.UserType, userType.ToString());
			}
			this.constraints.Add(VariantType.FirstRelease, isFirstRelease);
			this.constraints.Add(VariantType.Preview, isPreview);
		}

		private string FormatUserNameAndOrganization(string userName, string organization)
		{
			return string.Format("{0}@{1}", userName, organization);
		}

		public const string DefaultLocale = "en-US";

		private const string MicrosoftOrganizationName = "Microsoft";

		private static List<CultureInfo> supportedLocales;

		private readonly ConstraintCollection constraints;

		private readonly string rampId;

		private readonly string rotationId;
	}
}
