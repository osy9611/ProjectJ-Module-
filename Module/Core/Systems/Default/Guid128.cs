using System;

namespace Module.Core.Systems
{
    public struct Guid128
    {
        private long m_Id1;
        private long m_Id2;
        private string m_StringId;

        public long Id1
        {
            get
            {
                return m_Id1;
            }

            set
            {
                m_Id1 = value;
                m_StringId = null;
            }
        }

        public long Id2
        {
            get
            {
                return m_Id2;
            }

            set
            {
                m_Id2 = value;
                m_StringId = null;
            }
        }

        public string StringId
        {
            get
            {
                if (m_StringId == null)
                {
                    CreateStringId();
                }

                return m_StringId;
            }
        }

        public Guid128(long id1, long id2)
        {
            m_Id1 = id1;
            m_Id2 = id2;

            m_StringId = null;
        }

        private void CreateStringId()
        {
            byte[] byteId1 = BitConverter.GetBytes(m_Id1);
            byte[] byteId2 = BitConverter.GetBytes(m_Id2);

            //m_GuidValue = BitConverter.ToString(byteId1, 0, byteId1.Length) + BitConverter.ToString(byteId2, 0, byteId2.Length);
            m_StringId = ToString(byteId1, 0, byteId1.Length) + ToString(byteId2, 0, byteId2.Length);
        }

        public override string ToString()
        {
            return StringId;
        }

        public override bool Equals(object o)
        {
            if (((Guid128)o).m_Id1 == m_Id1 && ((Guid128)o).Id2 == m_Id2)
                return true;
            else
                return false;
        }

        public static bool operator ==(Guid128 c1, Guid128 c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(Guid128 c1, Guid128 c2)
        {
            return !c1.Equals(c2);
        }

        static public Guid128 Generate()
        {
            var guid = Guid.NewGuid();

            Guid128 value = new Guid128();

            var bytes = guid.ToByteArray();

            value.m_Id1 = BitConverter.ToInt64(bytes, 0);
            value.m_Id2 = BitConverter.ToInt64(bytes, 8);

            return value;
        }

        private static string ToString(byte[] value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (startIndex < 0 || startIndex >= value.Length && startIndex > 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_StartIndex");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "ArgumentOutOfRange_GenericPositive");
            if (startIndex > value.Length - length)
                throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
            if (length == 0)
                return string.Empty;
            if (length > 715827882)
                throw new ArgumentOutOfRangeException(nameof(length), "ArgumentOutOfRange_LengthTooLarge");

            int length1 = length * 2;
            char[] chArray = new char[length1];
            int num1 = startIndex;

            for (int index = 0; index < length1; index += 2)
            {
                byte num2 = value[num1++];
                chArray[index] = GetHexValue((int)num2 / 16);
                chArray[index + 1] = GetHexValue((int)num2 % 16);
            }

            return new string(chArray, 0, chArray.Length);
        }

        private static char GetHexValue(int i) => i < 10 ? (char)(i + 48) : (char)(i - 10 + 65);
    }
}