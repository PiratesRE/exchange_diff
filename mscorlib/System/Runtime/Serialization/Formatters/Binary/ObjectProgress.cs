using System;
using System.Diagnostics;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class ObjectProgress
	{
		internal ObjectProgress()
		{
		}

		[Conditional("SER_LOGGING")]
		private void Counter()
		{
			lock (this)
			{
				this.opRecordId = ObjectProgress.opRecordIdCount++;
				if (ObjectProgress.opRecordIdCount > 1000)
				{
					ObjectProgress.opRecordIdCount = 1;
				}
			}
		}

		internal void Init()
		{
			this.isInitial = false;
			this.count = 0;
			this.expectedType = BinaryTypeEnum.ObjectUrt;
			this.expectedTypeInformation = null;
			this.name = null;
			this.objectTypeEnum = InternalObjectTypeE.Empty;
			this.memberTypeEnum = InternalMemberTypeE.Empty;
			this.memberValueEnum = InternalMemberValueE.Empty;
			this.dtType = null;
			this.numItems = 0;
			this.nullCount = 0;
			this.typeInformation = null;
			this.memberLength = 0;
			this.binaryTypeEnumA = null;
			this.typeInformationA = null;
			this.memberNames = null;
			this.memberTypes = null;
			this.pr.Init();
		}

		internal void ArrayCountIncrement(int value)
		{
			this.count += value;
		}

		internal bool GetNext(out BinaryTypeEnum outBinaryTypeEnum, out object outTypeInformation)
		{
			outBinaryTypeEnum = BinaryTypeEnum.Primitive;
			outTypeInformation = null;
			if (this.objectTypeEnum == InternalObjectTypeE.Array)
			{
				if (this.count == this.numItems)
				{
					return false;
				}
				outBinaryTypeEnum = this.binaryTypeEnum;
				outTypeInformation = this.typeInformation;
				if (this.count == 0)
				{
					this.isInitial = false;
				}
				this.count++;
				return true;
			}
			else
			{
				if (this.count == this.memberLength && !this.isInitial)
				{
					return false;
				}
				outBinaryTypeEnum = this.binaryTypeEnumA[this.count];
				outTypeInformation = this.typeInformationA[this.count];
				if (this.count == 0)
				{
					this.isInitial = false;
				}
				this.name = this.memberNames[this.count];
				Type[] array = this.memberTypes;
				this.dtType = this.memberTypes[this.count];
				this.count++;
				return true;
			}
		}

		internal static int opRecordIdCount = 1;

		internal int opRecordId;

		internal bool isInitial;

		internal int count;

		internal BinaryTypeEnum expectedType = BinaryTypeEnum.ObjectUrt;

		internal object expectedTypeInformation;

		internal string name;

		internal InternalObjectTypeE objectTypeEnum;

		internal InternalMemberTypeE memberTypeEnum;

		internal InternalMemberValueE memberValueEnum;

		internal Type dtType;

		internal int numItems;

		internal BinaryTypeEnum binaryTypeEnum;

		internal object typeInformation;

		internal int nullCount;

		internal int memberLength;

		internal BinaryTypeEnum[] binaryTypeEnumA;

		internal object[] typeInformationA;

		internal string[] memberNames;

		internal Type[] memberTypes;

		internal ParseRecord pr = new ParseRecord();
	}
}
