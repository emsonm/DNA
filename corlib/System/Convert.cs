using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace System
{
    class Convert
    {
        #region bool
        public static bool ToBoolean(bool value)
        {
            return value;
        }

        public static bool ToBoolean(byte value)
        {
            return (value != 0);
        }

        public static bool ToBoolean(char value)
        {
            throw new InvalidCastException();
//            return ((IConvertible)value).ToBoolean(null);
        }

        //public static bool ToBoolean(decimal value)
        //{
        //    return ((int)(double)value != 0);
        //}

        public static bool ToBoolean(double value)
        {
            return (value != 0.0);
        }

        public static bool ToBoolean(short value)
        {
            return (value != 0);
        }

        public static bool ToBoolean(int value)
        {
            return (value != 0);
        }

        public static bool ToBoolean(long value)
        {
            return (value != 0L);
        }

        public static bool ToBoolean(object value)
        {
            Console.WriteLine("TOBOOLEAN!!!");
            if (value != null && value is IConvertible)
            {
                return ((IConvertible)value).ToBoolean(null);
            }
            return false;
        }

     //   [CLSCompliant(false)]
        public static bool ToBoolean(sbyte value)
        {
            return (value != 0);
        }

        public static bool ToBoolean(float value)
        {
            return (value != 0f);
        }

        public static bool ToBoolean(string value)
        {
            if (value == null)
            {
                return false;
            }
            return bool.Parse(value);
        }

       // [CLSCompliant(false)]
        public static bool ToBoolean(ushort value)
        {
            return (value != 0);
        }

      //  [CLSCompliant(false)]
        public static bool ToBoolean(uint value)
        {
            return (value != 0);
        }

      //  [CLSCompliant(false)]
        public static bool ToBoolean(ulong value)
        {
            return (value != 0L);
        }

        public static bool ToBoolean(object value, IFormatProvider provider)
        {
            return ((value != null) && ((IConvertible)value).ToBoolean(provider));
        }

        public static bool ToBoolean(string value, IFormatProvider provider)
        {
            if (value == null)
            {
                return false;
            }
            return bool.Parse(value);
        }
        #endregion
        #region byte

        public static byte ToByte(bool value)
        {
            if (!value)
            {
                return 0;
            }
            return 1;
        }

        public static byte ToByte(byte value)
        {
            return value;
        }

        public static byte ToByte(char value)
        {
            if (value > '\x00ff')
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        public static byte ToByte(decimal value)
        {
            throw new NotImplementedException();
         //   return decimal.ToByte(decimal.Round(value, 0));
        }

        public static byte ToByte(double value)
        {
            return ToByte(ToInt32(value));
        }

        public static byte ToByte(short value)
        {
            if ((value < 0) || (value > 0xff))
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        public static byte ToByte(int value)
        {
            if ((value < 0) || (value > 0xff))
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        public static byte ToByte(long value)
        {
            if ((value < 0L) || (value > 0xffL))
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        [CLSCompliant(false)]
        public static byte ToByte(sbyte value)
        {
            if (value < 0)
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        public static byte ToByte(float value)
        {
            return ToByte((double)value);
        }

        public static byte ToByte(string value)
        {
            if (value == null)
            {
                return 0;
            }
            return byte.Parse(value);
        }

        [CLSCompliant(false)]
        public static byte ToByte(ushort value)
        {
            if (value > 0xff)
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        [CLSCompliant(false)]
        public static byte ToByte(uint value)
        {
            if (value > 0xff)
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        [CLSCompliant(false)]
        public static byte ToByte(ulong value)
        {
            if (value > 0xffL)
            {
                throw new OverflowException();
            }
            return (byte)value;
        }

        public static byte ToByte(object value, IFormatProvider provider)
        {
            if (value != null)
            {
                return ((IConvertible)value).ToByte(provider);
            }
            return 0;
        }

        //public static byte ToByte(string value, IFormatProvider provider)
        //{
        //    if (value == null)
        //    {
        //        return 0;
        //    }
        //    return byte.Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        //}

        //public static byte ToByte(string value, int fromBase)
        //{
        //    if (((fromBase != 2) && (fromBase != 8)) && ((fromBase != 10) && (fromBase != 0x10)))
        //    {
        //        throw new ArgumentException();
        //    }
        //    int num = ParseNumbers.StringToInt(value, fromBase, 0x1200);
        //    if ((num < 0) || (num > 0xff))
        //    {
        //        throw new OverflowException();
        //    }
        //    return (byte)num;
        //}


        #endregion
        #region double
        public static double ToDouble(bool value)
        {
            return (value ? ((double)1) : ((double)0));
        }

        public static double ToDouble(byte value)
        {
            return (double)value;
        }

        public static double ToDouble(char value)
        {
            throw new InvalidCastException();
         //   return ((IConvertible)value).ToDouble(null);
        }

        //public static double ToDouble(decimal value)
        //{
        //    return (double)value;
        //}

        public static double ToDouble(double value)
        {
            return value;
        }

        public static double ToDouble(short value)
        {
            return (double)value;
        }

        public static double ToDouble(int value)
        {
            return (double)value;
        }

        public static double ToDouble(long value)
        {
            return (double)value;
        }

        public static double ToDouble(object value)
        {
            if (value != null)
            {
                return ((IConvertible)value).ToDouble(null);
            }
            return 0.0;
        }

        [CLSCompliant(false)]
        public static double ToDouble(sbyte value)
        {
            return (double)value;
        }

        public static double ToDouble(float value)
        {
            return (double)value;
        }

        public static double ToDouble(string value)
        {
            if (value == null)
            {
                return 0.0;
            }
            return double.Parse(value);
        }

        [CLSCompliant(false)]
        public static double ToDouble(ushort value)
        {
            return (double)value;
        }

        [CLSCompliant(false)]
        public static double ToDouble(uint value)
        {
            return (double)value;
        }

        [CLSCompliant(false)]
        public static double ToDouble(ulong value)
        {
            return (double)value;
        }

        public static double ToDouble(string value, IFormatProvider isop)
        {
            if (value == null)
            {
                return 0.0;
            }
            return double.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.GetInstance(isop));
        }

        #endregion
        #region  int32

        public static int ToInt32(bool value)
        {
            if (!value)
            {
                return 0;
            }
            return 1;
        }

        public static int ToInt32(byte value)
        {
            return value;
        }

        public static int ToInt32(char value)
        {
            return value;
        }

        //public static int ToInt32(decimal value)
        //{
        //    return decimal.ToInt32(decimal.Round(value, 0));
        //}

        public static int ToInt32(double value)
        {
            if (value >= 0.0)
            {
                if (value < 2147483647.5)
                {
                    int num = (int)value;
                    double num2 = value - num;
                    if ((num2 > 0.5) || ((num2 == 0.5) && ((num & 1) != 0)))
                    {
                        num++;
                    }
                    return num;
                }
            }
            else if (value >= -2147483648.5)
            {
                int num3 = (int)value;
                double num4 = value - num3;
                if ((num4 < -0.5) || ((num4 == -0.5) && ((num3 & 1) != 0)))
                {
                    num3--;
                }
                return num3;
            }
            throw new OverflowException();
        }

        public static int ToInt32(short value)
        {
            return value;
        }

        public static int ToInt32(int value)
        {
            return value;
        }

        public static int ToInt32(long value)
        {
            if ((value < -2147483648L) || (value > 0x7fffffffL))
            {
                throw new OverflowException();
            }
            return (int)value;
        }

        public static int ToInt32(object value)
        {
            if (value != null)
            {
                return ((IConvertible)value).ToInt32(null);
            }
            return 0;
        }

        [CLSCompliant(false)]
        public static int ToInt32(sbyte value)
        {
            return value;
        }

        public static int ToInt32(float value)
        {
            return ToInt32((double)value);
        }

        public static int ToInt32(string value)
        {
            if (value == null)
            {
                return 0;
            }
            return int.Parse(value);
        }

        [CLSCompliant(false)]
        public static int ToInt32(ushort value)
        {
            return value;
        }

        [CLSCompliant(false)]
        public static int ToInt32(uint value)
        {
            if (value > 0x7fffffff)
            {
                throw new OverflowException();
            }
            return (int)value;
        }

        [CLSCompliant(false)]
        public static int ToInt32(ulong value)
        {
            if (value > 0x7fffffffL)
            {
                throw new OverflowException();
            }
            return (int)value;
        }

        public static int ToInt32(string value, IFormatProvider provider)
        {
            if (value == null)
            {
                return 0;
            }
            return int.Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        //public static int ToInt32(string value, int fromBase)
        //{
        //    if (((fromBase != 2) && (fromBase != 8)) && ((fromBase != 10) && (fromBase != 0x10)))
        //    {
        //        throw new ArgumentException();
        //    }
        //    return ParseNumbers.StringToInt(value, fromBase, 0x1000);
        //}

        #endregion
    }

   // [CLSCompliant(false)]
    public interface IConvertible
    {
        TypeCode GetTypeCode();
        bool ToBoolean(IFormatProvider provider);
        byte ToByte(IFormatProvider provider);
        char ToChar(IFormatProvider provider);
        DateTime ToDateTime(IFormatProvider provider);
        decimal ToDecimal(IFormatProvider provider);
        double ToDouble(IFormatProvider provider);
        short ToInt16(IFormatProvider provider);
        int ToInt32(IFormatProvider provider);
        long ToInt64(IFormatProvider provider);
        sbyte ToSByte(IFormatProvider provider);
        float ToSingle(IFormatProvider provider);
        string ToString(IFormatProvider provider);
        object ToType(Type conversionType, IFormatProvider provider);
        ushort ToUInt16(IFormatProvider provider);
        uint ToUInt32(IFormatProvider provider);
        ulong ToUInt64(IFormatProvider provider);
    }

 

 

 

}
