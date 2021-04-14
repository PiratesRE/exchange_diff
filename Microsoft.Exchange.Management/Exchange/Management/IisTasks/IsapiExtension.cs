using System;
using System.Globalization;

namespace Microsoft.Exchange.Management.IisTasks
{
	internal sealed class IsapiExtension
	{
		public IsapiExtension(string physicalPath, string groupID, string description, bool allow, bool uiDeletable)
		{
			if (physicalPath == null)
			{
				throw new IsapiExtensionMustHavePhysicalPathException();
			}
			this.physicalPath = physicalPath;
			this.groupID = ((groupID != null) ? groupID : "");
			this.description = ((description != null) ? description : "");
			this.allow = allow;
			this.uiDeletable = uiDeletable;
		}

		internal static IsapiExtension Parse(string extensionString)
		{
			bool flag = false;
			string text = null;
			string text2 = null;
			string[] array = extensionString.Split(new char[]
			{
				','
			});
			switch (array.Length)
			{
			case 0:
				return null;
			case 1:
				return null;
			case 2:
				goto IL_5D;
			case 3:
				goto IL_53;
			case 4:
				break;
			default:
				text2 = array[4];
				break;
			}
			text = array[3];
			IL_53:
			flag = IsapiExtension.IntStringToBoolean(array[2]);
			IL_5D:
			string text3 = array[1];
			bool flag2 = IsapiExtension.IntStringToBoolean(array[0]);
			return new IsapiExtension(text3, text, text2, flag2, flag);
		}

		private static bool IntStringToBoolean(string intString)
		{
			int num = 0;
			if (!int.TryParse(intString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out num))
			{
				IisTaskTrace.IisUtilityTracer.TraceError<string>(0L, "One of the ISAPI extension booleans could not be parsed by Int32.TryParse(); using false", intString);
				return false;
			}
			return num != 0;
		}

		internal string ToMetabaseString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", new object[]
			{
				this.allow ? 1 : 0,
				this.physicalPath,
				this.uiDeletable ? 1 : 0,
				this.groupID,
				this.description
			});
		}

		public bool Allow
		{
			get
			{
				return this.allow;
			}
			set
			{
				this.allow = value;
			}
		}

		public string PhysicalPath
		{
			get
			{
				return this.physicalPath;
			}
			set
			{
				this.physicalPath = value;
			}
		}

		public bool UIDeletable
		{
			get
			{
				return this.uiDeletable;
			}
			set
			{
				this.uiDeletable = value;
			}
		}

		public string GroupID
		{
			get
			{
				return this.groupID;
			}
			set
			{
				this.groupID = value;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		private string physicalPath;

		private string groupID;

		private string description;

		private bool allow;

		private bool uiDeletable;
	}
}
