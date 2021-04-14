using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public sealed class CrossTenantObjectId : ObjectId, ITraceable, IEquatable<CrossTenantObjectId>
	{
		internal CrossTenantObjectId(byte format, Guid externalDirectoryOrganizationId, Guid externalDirectoryObjectId)
		{
			if (externalDirectoryOrganizationId == Guid.Empty)
			{
				throw new ArgumentException("A valid ExternalDirectoryOrganizationId is required to create a link to an object using external directory information.", "externalDirectoryOrganizationId");
			}
			if (externalDirectoryObjectId == Guid.Empty)
			{
				throw new ArgumentException("A valid ExternalDirectoryObjectId is required to create a link to an object using external directory information.", "externalDirectoryObjectId");
			}
			this.format = format;
			this.externalDirectoryOrganizationId = externalDirectoryOrganizationId;
			this.externalDirectoryObjectId = externalDirectoryObjectId;
		}

		public byte Format
		{
			get
			{
				return this.format;
			}
		}

		public Guid ExternalDirectoryObjectId
		{
			get
			{
				return this.externalDirectoryObjectId;
			}
		}

		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return this.externalDirectoryOrganizationId;
			}
		}

		public static CrossTenantObjectId FromExternalDirectoryIds(Guid externalDirectoryOrganizationId, Guid externalDirectoryObjectId)
		{
			return new CrossTenantObjectId(1, externalDirectoryOrganizationId, externalDirectoryObjectId);
		}

		public static CrossTenantObjectId Parse(byte[] input)
		{
			return CrossTenantObjectId.Parse(input, false);
		}

		public static CrossTenantObjectId Parse(byte[] input, bool localizedException)
		{
			int num = 0;
			byte b = input[num++];
			byte b2 = b;
			if (b2 == 1)
			{
				if (input.Length != 33)
				{
					throw new FormatException(DirectoryStrings.InvalidCrossTenantIdFormat(BitConverter.ToString(input)));
				}
				byte[] array = new byte[16];
				Array.Copy(input, num, array, 0, 16);
				num += 16;
				Guid guid = new Guid(array);
				Array.Copy(input, num, array, 0, 16);
				Guid guid2 = new Guid(array);
				return new CrossTenantObjectId(b, guid, guid2);
			}
			else
			{
				string str = BitConverter.ToString(input);
				if (localizedException)
				{
					throw new InvalidCrossTenantIdFormatException(str);
				}
				throw new FormatException(DirectoryStrings.InvalidCrossTenantIdFormat(str));
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CrossTenantObjectId);
		}

		public bool Equals(CrossTenantObjectId other)
		{
			return other != null && this.ExternalDirectoryOrganizationId == other.ExternalDirectoryOrganizationId && this.ExternalDirectoryObjectId == other.ExternalDirectoryObjectId;
		}

		public override byte[] GetBytes()
		{
			byte[] array = new byte[33];
			int num = 0;
			array[num++] = this.format;
			byte[] sourceArray = this.ExternalDirectoryOrganizationId.ToByteArray();
			Array.Copy(sourceArray, 0, array, num, 16);
			num += 16;
			sourceArray = this.ExternalDirectoryObjectId.ToByteArray();
			Array.Copy(sourceArray, 0, array, num, 16);
			return array;
		}

		public override int GetHashCode()
		{
			return (int)this.format ^ this.ExternalDirectoryOrganizationId.GetHashCode() ^ this.ExternalDirectoryObjectId.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}{{{1}}}({2},{3})", new object[]
			{
				CrossTenantObjectId.typeName,
				this.format,
				this.ExternalDirectoryOrganizationId,
				this.ExternalDirectoryObjectId
			});
		}

		public void TraceTo(ITraceBuilder traceBuilder)
		{
			traceBuilder.AddArgument(this.ToString());
		}

		private const int BytesForGuid = 16;

		private const int ExternalDirectoryFormatLength = 33;

		private static readonly string typeName = typeof(CrossTenantObjectId).Name;

		private readonly Guid externalDirectoryObjectId;

		private readonly Guid externalDirectoryOrganizationId;

		private readonly byte format;

		private enum CrossTenantObjectIdFormat : byte
		{
			ExternalDirectory = 1
		}
	}
}
