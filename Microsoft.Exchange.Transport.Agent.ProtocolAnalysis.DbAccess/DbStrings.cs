using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	public static class DbStrings
	{
		static DbStrings()
		{
			DbStrings.stringIDs.Add(3885054659U, "DatabaseInstanceName");
			DbStrings.stringIDs.Add(777731237U, "InvalidTraceObject");
			DbStrings.stringIDs.Add(1694836300U, "DetachRefCountFailed");
			DbStrings.stringIDs.Add(2935401978U, "AttachRefCountFailed");
			DbStrings.stringIDs.Add(1207509604U, "PaRecordNotLoaded");
		}

		public static LocalizedString DatabaseInstanceName
		{
			get
			{
				return new LocalizedString("DatabaseInstanceName", "Ex8E136F", false, true, DbStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTraceObject
		{
			get
			{
				return new LocalizedString("InvalidTraceObject", "Ex102DA4", false, true, DbStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DetachRefCountFailed
		{
			get
			{
				return new LocalizedString("DetachRefCountFailed", "Ex00CB9A", false, true, DbStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachRefCountFailed
		{
			get
			{
				return new LocalizedString("AttachRefCountFailed", "ExF5B4CD", false, true, DbStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseAttachFailed(string databaseName)
		{
			return new LocalizedString("DatabaseAttachFailed", "", false, false, DbStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString PaRecordNotLoaded
		{
			get
			{
				return new LocalizedString("PaRecordNotLoaded", "Ex813A29", false, true, DbStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataBaseInUse(string databaseName)
		{
			return new LocalizedString("DataBaseInUse", "", false, false, DbStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString GetLocalizedString(DbStrings.IDs key)
		{
			return new LocalizedString(DbStrings.stringIDs[(uint)key], DbStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(5);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess.DbStrings", typeof(DbStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DatabaseInstanceName = 3885054659U,
			InvalidTraceObject = 777731237U,
			DetachRefCountFailed = 1694836300U,
			AttachRefCountFailed = 2935401978U,
			PaRecordNotLoaded = 1207509604U
		}

		private enum ParamIDs
		{
			DatabaseAttachFailed,
			DataBaseInUse
		}
	}
}
