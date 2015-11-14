/******************************************************************************/
/*                                                                            */
/*   Program: MyStructStreamConsoleApp                                        */
/*                                                                            */
/*                                                                            */
/*   11.10.2013 1.0.*.* uhwgmxorg Start                                       */
/*                                                                            */
/******************************************************************************/
//#define WORKING_CORRECT
using System;
using System.Runtime.InteropServices;

namespace MyStructStreamConsoleApp
{
#if WORKING_CORRECT
    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
    public struct TypesI
    {
        [FieldOffset(0)]
        public byte   b1;
        [FieldOffset(1)]
        public ushort us;
        [FieldOffset(3)]
        public byte   b2;
        [FieldOffset(4)]
        public uint   ui;
        [FieldOffset(8)]
        public short  s1;
        [FieldOffset(10)]
        public short  s2;
        [FieldOffset(12)]
        public int    i;
    }
#else
    public struct TypesI
    {
        public byte   b1;
        public ushort us;
        public byte   b2;
        public uint   ui;
        public short  s1;
        public short  s2;
        public int    i;
    }
#endif

    class Program
    {
        static readonly string FILE_NAME = "MyStructData.bin";
        static Random _random;

        /// <summary>
        /// RandomDouble
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="deci"></param>
        /// <returns></returns>
        public static double RandomDouble(double min, double max, int deci)
        {
            double d;            
            d = _random.NextDouble() * (max - min) + min;
            return Math.Round(d, deci);
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] Convert<T>(T item)
            where T : struct
        {
            int size = Marshal.SizeOf(item);
            byte[] data = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(item, ptr, true);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);

            return data;
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Convert<T>(byte[] data)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(data, 0, ptr, size);

            var result = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return result;
        }

        static void Main(string[] args)
        {
            _random = new Random();

            ProcessTypeI();

            Console.WriteLine("\nEnd.");
        }

        /// <summary>
        /// ProcessTypeI
        /// </summary>
        static void ProcessTypeI()
        {
            ConsoleKeyInfo KeyInfo;
            char Key;
            do
            {
                Console.Clear();
                string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Console.WriteLine("Program MyStructStreamConsoleApp C# Version " + Version + "\n");

                TypesI TI1;
                TypesI TI2;

                TI1.b1 = (byte)RandomDouble(0, 255, 0);                   // byte
                TI1.us = (ushort)RandomDouble(0, 65535, 0);               // ushort
                TI1.b2 = (byte)RandomDouble(0, 255, 0);                   // byte
                TI1.ui = (uint)RandomDouble(0, 4294967295, 0);            // uint
                TI1.s1 = (short)RandomDouble(-32767, 32767, 0);           // short
                TI1.s2 = (short)RandomDouble(-32767, 32767, 0);           // short
                TI1.i  = (int)RandomDouble(-2147483647, 2147483647, 0);   // short

                Console.WriteLine(String.Format("byte   b1 = {0}", TI1.b1));
                Console.WriteLine(String.Format("ushort us = {0}", TI1.us));
                Console.WriteLine(String.Format("byte   b2 = {0}", TI1.b2));
                Console.WriteLine(String.Format("uint   ui = {0}", TI1.ui));
                Console.WriteLine(String.Format("short  s1 = {0}", TI1.s1));
                Console.WriteLine(String.Format("short  s2 = {0}", TI1.s2));
                Console.WriteLine(String.Format("int    i  = {0}", TI1.i));

                byte[] bstream1 = new byte[Marshal.SizeOf(typeof(TypesI))];

                var TTypes = Convert<TypesI>(TI1);
                System.Buffer.BlockCopy(TTypes, 0, bstream1, 0, TTypes.Length);

                // Write data to file
                using (System.IO.Stream dest = System.IO.File.Create(FILE_NAME))
                {
                    dest.Write(bstream1, 0, TTypes.Length);
                }
                Console.WriteLine(String.Format("\nthe size should be 16 size is: {0}", TTypes.Length));
                Console.WriteLine(BitConverter.ToString(bstream1)+"\n");


                byte[] bstream2 = new byte[Marshal.SizeOf(typeof(TypesI))];
                System.Buffer.BlockCopy(bstream1, 0, bstream2, 0, TTypes.Length);
                TI2 = Convert<TypesI>(bstream2);

                Console.WriteLine(String.Format("byte   b1 = {0}", TI2.b1));
                Console.WriteLine(String.Format("ushort us = {0}", TI2.us));
                Console.WriteLine(String.Format("byte   b2 = {0}", TI2.b2));
                Console.WriteLine(String.Format("uint   ui = {0}", TI2.ui));
                Console.WriteLine(String.Format("short  s1 = {0}", TI2.s1));
                Console.WriteLine(String.Format("short  s2 = {0}", TI2.s2));
                Console.WriteLine(String.Format("int    i  = {0}", TI2.i));

                Console.WriteLine("\npress Return to continue q to quit");
                KeyInfo = Console.ReadKey();
                Key = KeyInfo.KeyChar;
            }
            while (Key != 'q');
        }
    }
}
