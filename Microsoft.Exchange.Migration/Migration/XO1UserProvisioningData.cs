using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class XO1UserProvisioningData : ProvisioningData
	{
		internal XO1UserProvisioningData()
		{
			base.Action = ProvisioningAction.CreateNew;
			base.ProvisioningType = ProvisioningType.XO1User;
		}

		public string FirstName
		{
			get
			{
				return (string)base["FirstName"];
			}
			private set
			{
				base["FirstName"] = value;
			}
		}

		public string LastName
		{
			get
			{
				return (string)base["LastName"];
			}
			private set
			{
				base["LastName"] = value;
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				return (ExTimeZone)base["TimeZone"];
			}
			private set
			{
				base["TimeZone"] = value;
			}
		}

		public int LocaleId
		{
			get
			{
				return (int)base["LocaleId"];
			}
			private set
			{
				base["LocaleId"] = value;
			}
		}

		public string[] EmailAddresses
		{
			get
			{
				return (string[])base["EmailAddresses"];
			}
			private set
			{
				base["EmailAddresses"] = value;
			}
		}

		public string Database
		{
			get
			{
				return (string)base["Database"];
			}
			private set
			{
				base["Database"] = value;
			}
		}

		public bool MakeExoSecondary
		{
			get
			{
				return (bool)base["MakeExoSecondary"];
			}
			private set
			{
				base["MakeExoSecondary"] = value;
			}
		}

		public static XO1UserProvisioningData Create(string identity, long puid, ExTimeZone timeZone, int localeId, string database, string firstName, string lastName, string[] aliases)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(identity, "identity");
			MigrationUtil.AssertOrThrow(puid != 0L, "invalid puid", new object[0]);
			MigrationUtil.ThrowOnNullArgument(timeZone, "timeZone");
			MigrationUtil.ThrowOnNullOrEmptyArgument(database, "database");
			XO1UserProvisioningData xo1UserProvisioningData = new XO1UserProvisioningData();
			xo1UserProvisioningData.Identity = new NetID(puid).ToString();
			xo1UserProvisioningData.TimeZone = timeZone;
			xo1UserProvisioningData.LocaleId = localeId;
			xo1UserProvisioningData.Database = database;
			xo1UserProvisioningData.MakeExoSecondary = true;
			if (!string.IsNullOrEmpty(firstName))
			{
				xo1UserProvisioningData.FirstName = firstName;
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				xo1UserProvisioningData.LastName = lastName;
			}
			List<string> list = new List<string>();
			if (aliases != null)
			{
				list.AddRange(aliases);
			}
			list.Add(identity);
			if (list.Count > 0)
			{
				xo1UserProvisioningData.EmailAddresses = list.ToArray();
			}
			return xo1UserProvisioningData;
		}

		public const string FirstNameParameterName = "FirstName";

		public const string LastNameParameterName = "LastName";

		public const string TimeZoneParameterName = "TimeZone";

		public const string LocaleIdParameterName = "LocaleId";

		public const string EmailAddressesParameterName = "EmailAddresses";

		public const string DatabaseParameterName = "Database";

		public const string MakeExoSecondaryParameterName = "MakeExoSecondary";
	}
}
