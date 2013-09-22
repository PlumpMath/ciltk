﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class CilTests
    {
        struct TestStruct
        {
            public static int Static;

            public int A;
            public int B;
        }

        class TestClass
        {
            public static int Static;

            public int A;
            public int B;

            public TestClass()
            {
            }

            public TestClass(int a, int b)
            {
                A = a;
                B = b;
            }

            public override bool Equals(object obj)
            {
                var other = obj as TestClass;
                if (other == null)
                {
                    return false;
                }

                return A == other.A && B == other.B;
            }
        }

        class TestDerivedClass : TestClass
        {
            public int C;

            public TestDerivedClass()
            {
            }

            public TestDerivedClass(int a, int b, int c) : base(a, b)
            {
                C = c;
            }

            public override bool Equals(object obj)
            {
                var other = obj as TestDerivedClass;
                if (other == null)
                {
                    return false;
                }

                return base.Equals(other) && C == other.C;
            }
        }

        public void JustReturn()
        {
            Silk.Cil.Ret();
        }

        [TestMethod]
        public void TestReturn()
        {
            JustReturn();
        }

        public int ReturnOne()
        {
            Silk.Cil.Ldc_I4(1);
            Silk.Cil.Ret();

            return 0;
        }

        [TestMethod]
        public void TestLdcReturn()
        {
            int i = ReturnOne();
            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void TestAdd()
        {
            int i = 1;
            int j = 2;

            Silk.Cil.Ldloc(0);
            Silk.Cil.Ldloc(1);
            Silk.Cil.Add();
            Silk.Cil.Stloc(0);

            Silk.Cil.KeepAlive(j);

            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void TestLoadStore()
        {
            int i = 0;

            Silk.Cil.Ldc_I4(1);
            Silk.Cil.Stloc(0);

            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void TestLabel()
        {
            int i = 0;

            Silk.Cil.Label("a");

            i = 1;

            Silk.Cil.Label("b");

            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void TestBranch()
        {
            int i = 0;

            Silk.Cil.Br("branch");
            i = 1;
            Silk.Cil.Label("branch");

            Assert.AreEqual(0, i);
        }

        [TestMethod]
        public void TestBranchEqual()
        {
            int i = 0;
            int j = 1;

            Silk.Cil.Ldloc(0);
            Silk.Cil.Ldloc(1);
            Silk.Cil.Beq("equal");
            i = j;
            Silk.Cil.Label("equal");

            Assert.AreEqual(i, j);
        }

        [TestMethod]
        public void TestSizeof()
        {
            int size = 0;

            Silk.Cil.Sizeof<Int32>();
            Silk.Cil.Stloc(0);

            Assert.AreEqual(4, size);
        }

        public int GenericSizeof<T>()
        {
            Silk.Cil.Sizeof<T>();
            Silk.Cil.Ret();

            return 0;
        }

        [TestMethod]
        public void TestGenericSizeof()
        {
            Assert.AreEqual(8, GenericSizeof<TestStruct>());
        }

        public T ReadWrite<T>(T value)
        {
            T result = default(T);

            Silk.Cil.Ldloca(0);
            Silk.Cil.Ldarga(0);
            Silk.Cil.Sizeof<T>();
            Silk.Cil.Cpblk();

            return result;
        }

        [TestMethod]
        public void TestReadWriteGenerics()
        {
            TestStruct test = new TestStruct()
            {
                A = 1,
                B = 2
            };

            TestStruct result = ReadWrite<TestStruct>(test);

            Assert.AreEqual(test.A, result.A);
            Assert.AreEqual(test.B, result.B);
        }

        [TestMethod]
        public void TestLdStobj()
        {
            TestStruct test1 = new TestStruct()
            {
                A = 1,
                B = 2,
            };

            TestStruct test2 = new TestStruct();

            Silk.Cil.Ldloca(1);
            Silk.Cil.Ldloca(0);
            Silk.Cil.Ldobj<TestStruct>();
            Silk.Cil.Stobj<TestStruct>();

            Assert.AreEqual(test1.A, test2.A);
            Assert.AreEqual(test1.B, test2.B);
        }

        [TestMethod]
        public void TestCpobj()
        {
            TestStruct test1 = new TestStruct()
            {
                A = 1,
                B = 2,
            };

            TestStruct test2 = new TestStruct();

            Silk.Cil.Ldloca(1);
            Silk.Cil.Ldloca(0);
            Silk.Cil.Cpobj<TestStruct>();

            Assert.AreEqual(test1.A, test2.A);
            Assert.AreEqual(test1.B, test2.B);
        }

        [TestMethod]
        public void TestInitobj()
        {
            TestStruct test = new TestStruct()
            {
                A = 1,
                B = 2,
            };

            Silk.Cil.Ldloca(0);
            Silk.Cil.Initobj<TestStruct>();

            Assert.AreEqual(test.A, 0);
            Assert.AreEqual(test.B, 0);
        }

        [TestMethod]
        public void TestLoadLocal()
        {
            int a = 4;
            int b = 4;
            int c = 8;

            Silk.Cil.Load(a);
            Silk.Cil.Load(b);
            Silk.Cil.Add();
            Silk.Cil.Stloc(0);

            Assert.AreEqual(a, c);
        }

        [TestMethod]
        public void TestLoadStructField()
        {
            var a = new TestStruct() { A = 1, B = 2 };
            int b = 1;

            Silk.Cil.Load(a.A);
            Silk.Cil.Load(a.B);
            Silk.Cil.Add();
            Silk.Cil.Stloc(1);

            Assert.AreEqual(b, 3);
        }

        [TestMethod]
        public void TestLoadClassField()
        {
            var a = new TestClass() { A = 1, B = 2 };
            int b = 1;

            Silk.Cil.Load(a.A);
            Silk.Cil.Load(a.B);
            Silk.Cil.Add();
            Silk.Cil.Stloc(1);

            Assert.AreEqual(b, 3);
        }

        [TestMethod]
        public void TestStoreInt()
        {
            int a = 4;
            int b;

            Silk.Cil.Load(a);
            Silk.Cil.Store(out b);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestStoreStruct()
        {
            TestStruct a = new TestStruct() { A = 1, B = 2 };
            TestStruct b;

            Silk.Cil.Load(a);
            Silk.Cil.Store(out b);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestStoreClass()
        {
            TestClass a = new TestClass() { A = 1, B = 2 };
            TestClass b;

            Silk.Cil.Load(a);
            Silk.Cil.Store(out b);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestStoreStructField()
        {
            TestStruct a = new TestStruct() { A = 1, B = 2 };
            TestStruct b = new TestStruct() { A = 1, B = 3 };

            Silk.Cil.Load(a.B);
            Silk.Cil.Store(out b.B);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestStoreClassField()
        {
            TestClass a = new TestClass() { A = 1, B = 2 };
            TestClass b = new TestClass() { A = 1, B = 3 };

            Silk.Cil.Load(a.B);
            Silk.Cil.Store(out b.B);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestStoreStaticStructField()
        {
            int a = 1;

            Silk.Cil.Load(a);
            Silk.Cil.Store(out TestStruct.Static);

            Assert.AreEqual(a, TestStruct.Static);
        }

        [TestMethod]
        public void TestStoreStaticClassField()
        {
            int a = 1;

            Silk.Cil.Load(a);
            Silk.Cil.Store(out TestClass.Static);

            Assert.AreEqual(a, TestClass.Static);
        }
        
        [TestMethod]
        public void TestNewarr()
        {
            int[] a;

            Silk.Cil.Ldc_I4(5);
            Silk.Cil.Newarr<int>();
            Silk.Cil.Store(out a);

            Assert.IsNotNull(a);
            Assert.AreEqual(a.Length, 5);
        }

        [TestMethod]
        public void TestStelem()
        {
            byte[] a = new byte[1];
            short[] b = new short[1];
            int[] c = new int[1];
            long[] d = new long[1];
            float[] e = new float[1];
            double[] f = new double[1];

            Silk.Cil.Load(a);
            Silk.Cil.Load(0);
            Silk.Cil.Load(0xFF);
            Silk.Cil.Conv_I1();
            Silk.Cil.Stelem_I1();
            Assert.AreEqual(a[0], 0xFF, "Stelem_I1 failed");

            Silk.Cil.Load(b);
            Silk.Cil.Load(0);
            Silk.Cil.Load(0xFF);
            Silk.Cil.Conv_I2();
            Silk.Cil.Stelem_I2();
            Assert.AreEqual(a[0], 0xFF, "Stelem_I2 failed");

            Silk.Cil.Load(c);
            Silk.Cil.Load(0);
            Silk.Cil.Load(0xFF);
            Silk.Cil.Conv_I4();
            Silk.Cil.Stelem_I4();
            Assert.AreEqual(a[0], 0xFF, "Stelem_I4 failed");

            Silk.Cil.Load(d);
            Silk.Cil.Load(0);
            Silk.Cil.Load(0xFF);
            Silk.Cil.Conv_I8();
            Silk.Cil.Stelem_I8();
            Assert.AreEqual(a[0], 0xFF, "Stelem_I8 failed");

            Silk.Cil.Load(e);
            Silk.Cil.Load(0);
            Silk.Cil.Load(0xFF);
            Silk.Cil.Conv_R4();
            Silk.Cil.Stelem_R4();
            Assert.AreEqual(a[0], 0xFF, "Stelem_R4 failed");

            Silk.Cil.Load(f);
            Silk.Cil.Load(0);
            Silk.Cil.Load(0xFF);
            Silk.Cil.Conv_R8();
            Silk.Cil.Stelem_R8();
            Assert.AreEqual(a[0], 0xFF, "Stelem_R8 failed");
        }

        [TestMethod]
        public void TestStelemT()
        {
            TestStruct[] a = new TestStruct[1];
            TestStruct b = new TestStruct() { A = 1, B = 2 };

            Silk.Cil.Load(a);
            Silk.Cil.Load(0);
            Silk.Cil.Load(b);
            Silk.Cil.Stelem<TestStruct>();

            Assert.AreEqual(a[0], b);
        }

        [TestMethod]
        public void TestStelemRef()
        {
            TestDerivedClass[] a = new TestDerivedClass[1];
            TestClass b = new TestDerivedClass(1, 2, 3);            

            Silk.Cil.Load(a);
            Silk.Cil.Load(0);
            Silk.Cil.Load(b);
            Silk.Cil.Stelem_Ref();

            Assert.AreEqual(a[0].A, 1);
            Assert.AreEqual(a[0].B, 2);
        }

        [TestMethod]
        public void TestBox()
        {
            TestStruct a = new TestStruct() { A = 1, B = 2 };
            object b;
            TestStruct c;

            Silk.Cil.Load(a);
            Silk.Cil.Box<TestStruct>();
            Silk.Cil.Store(out b);

            c = (TestStruct)b;

            Assert.AreEqual(a, c);
        }

        [TestMethod]
        public void TestUnbox()
        {
            TestStruct a = new TestStruct() { A = 1, B = 2 };
            object b = a;
            TestStruct c;

            Silk.Cil.Load(b);
            Silk.Cil.Unbox<TestStruct>();
            Silk.Cil.Ldobj<TestStruct>();
            Silk.Cil.Store(out c);

            Assert.AreEqual(a, c);
        }

        [TestMethod]
        public void TestUnboxAnyStruct()
        {
            TestStruct a = new TestStruct() { A = 1, B = 2 };
            object b = a;
            TestStruct c;

            Silk.Cil.Load(b);
            Silk.Cil.Unbox_Any<TestStruct>();
            Silk.Cil.Store(out c);

            Assert.AreEqual(a, c);
        }

        [TestMethod]
        public void TestUnboxAnyClass()
        {
            TestClass a = new TestClass(1, 2);
            object b = a;
            TestClass c;

            Silk.Cil.Load(b);
            Silk.Cil.Unbox_Any<TestClass>();
            Silk.Cil.Store(out c);

            Assert.AreSame(a, c);
        }
    }
}
