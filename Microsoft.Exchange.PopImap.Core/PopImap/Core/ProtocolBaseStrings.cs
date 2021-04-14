using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.PopImap.Core
{
	internal static class ProtocolBaseStrings
	{
		static ProtocolBaseStrings()
		{
			ProtocolBaseStrings.stringIDs.Add(1139742230U, "UsageText");
			ProtocolBaseStrings.stringIDs.Add(1766818386U, "NotAvailable");
			ProtocolBaseStrings.stringIDs.Add(2966948847U, "ProcessNotResponding");
			ProtocolBaseStrings.stringIDs.Add(527123233U, "InvalidNamesSubject");
			ProtocolBaseStrings.stringIDs.Add(2964959188U, "SystemFromDisplayName");
			ProtocolBaseStrings.stringIDs.Add(1017415836U, "DuplicateFoldersSubject");
		}

		public static string FileNotFound(string fileName)
		{
			return string.Format(ProtocolBaseStrings.ResourceManager.GetString("FileNotFound"), fileName);
		}

		public static string DuplicateFoldersBody(string dupes)
		{
			return string.Format(ProtocolBaseStrings.ResourceManager.GetString("DuplicateFoldersBody"), dupes);
		}

		public static string UsageText
		{
			get
			{
				return ProtocolBaseStrings.ResourceManager.GetString("UsageText");
			}
		}

		public static string NonRenderableMessage(string subject, string displayName, string mailAddress, string sentDate)
		{
			return string.Format(ProtocolBaseStrings.ResourceManager.GetString("NonRenderableMessage"), new object[]
			{
				subject,
				displayName,
				mailAddress,
				sentDate
			});
		}

		public static string InvalidNamesBody(string names)
		{
			return string.Format(ProtocolBaseStrings.ResourceManager.GetString("InvalidNamesBody"), names);
		}

		public static string NotAvailable
		{
			get
			{
				return ProtocolBaseStrings.ResourceManager.GetString("NotAvailable");
			}
		}

		public static string ProcessNotResponding
		{
			get
			{
				return ProtocolBaseStrings.ResourceManager.GetString("ProcessNotResponding");
			}
		}

		public static string NonRenderableSubject(string id)
		{
			return string.Format(ProtocolBaseStrings.ResourceManager.GetString("NonRenderableSubject"), id);
		}

		public static string InvalidNamesSubject
		{
			get
			{
				return ProtocolBaseStrings.ResourceManager.GetString("InvalidNamesSubject");
			}
		}

		public static string SystemFromDisplayName
		{
			get
			{
				return ProtocolBaseStrings.ResourceManager.GetString("SystemFromDisplayName");
			}
		}

		public static string DuplicateFoldersSubject
		{
			get
			{
				return ProtocolBaseStrings.ResourceManager.GetString("DuplicateFoldersSubject");
			}
		}

		public static string GetLocalizedString(ProtocolBaseStrings.IDs key)
		{
			return ProtocolBaseStrings.ResourceManager.GetString(ProtocolBaseStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.PopImap.Core.ProtocolBaseStrings", typeof(ProtocolBaseStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			UsageText = 1139742230U,
			NotAvailable = 1766818386U,
			ProcessNotResponding = 2966948847U,
			InvalidNamesSubject = 527123233U,
			SystemFromDisplayName = 2964959188U,
			DuplicateFoldersSubject = 1017415836U
		}

		private enum ParamIDs
		{
			FileNotFound,
			DuplicateFoldersBody,
			NonRenderableMessage,
			InvalidNamesBody,
			NonRenderableSubject
		}
	}
}
