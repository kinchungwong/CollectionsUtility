using System;
using NUnit.Framework;
using Autofac;

namespace CollectionsUtility.Tests
{
    using CollectionsUtility.Specialized;
    using CollectionsUtility.Specialized.Internals;

    public class Histogram_AutofacTests
    {
        public struct DummyStruct
            : IEquatable<DummyStruct>
        {
            public bool Equals(DummyStruct other)
            {
                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj is DummyStruct other)
                {
                    return Equals(other);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class DummyClass
            : IEquatable<DummyClass>
        {
            public DummyClass()
            { 
            }

            public DummyClass(DummyClass other)
            {
            }

            public bool Equals(DummyClass other)
            {
                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj is DummyClass other)
                {
                    return Equals(other);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public void RegisterHistArith(ContainerBuilder builder)
        {
            builder.Register(c => new HistArith_Int32()).As<IHistArith<int>>();
            builder.Register(c => new HistArith_Float64()).As<IHistArith<double>>();
        }

        public void RegisterGenericHistogram(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Histogram<,>)).As(typeof(IHistogram<,>));
        }

        public void RegisterHistogram(ContainerBuilder builder)
        {
            builder.Register(c => new HistArith_Int32()).As<IHistArith<int>>();
            builder.Register(c => new HistArith_Float64()).As<IHistArith<double>>();
            builder.RegisterGeneric(typeof(Histogram<,>)).As(typeof(IHistogram<,>));
        }

        public IContainer CreateContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterHistogram(builder);
            return builder.Build();
        }

        public IHistogram<TKey, TValue> CreateHistogram<TKey, TValue>(IContainer container)
            where TValue : struct
        {
            return container.Resolve<IHistogram<TKey, TValue>>();
        }

        [Test]
        public void RegistrationTest_IHistArith()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterHistArith(builder);
            Assert.Pass();
        }

        [Test]
        public void RegistrationTest_OpenGeneric()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterGenericHistogram(builder);
            Assert.Pass();
        }

        [Test]
        public void RegistrationTest_Both()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterHistArith(builder);
            RegisterGenericHistogram(builder);
            Assert.Pass();
        }

        [Test]
        public void BuildTest()
        {
            var container = CreateContainer();
            Assert.That(container, Is.Not.Null);
        }

        [Test]
        public void ResolveTest_DummyStruct_int()
        {
            var hist = CreateHistogram<DummyStruct, int>(CreateContainer());
            Assert.That(hist, Is.Not.Null);
            hist.Add(new DummyStruct());
            hist.Add(new DummyStruct(), 3);
        }

        [Test]
        public void ResolveTest_DummyStruct_double()
        {
            var hist = CreateHistogram<DummyStruct, double>(CreateContainer());
            Assert.That(hist, Is.Not.Null);
            hist.Add(new DummyStruct());
            hist.Add(new DummyStruct(), Math.PI);
        }

        [Test]
        public void ResolveTest_DummyClass_int()
        {
            var hist = CreateHistogram<DummyClass, int>(CreateContainer());
            Assert.That(hist, Is.Not.Null);
            hist.Add(new DummyClass());
            hist.Add(new DummyClass(), 3);
        }

        [Test]
        public void ResolveTest_DummyClass_double()
        {
            var hist = CreateHistogram<DummyClass, double>(CreateContainer());
            Assert.That(hist, Is.Not.Null);
            hist.Add(new DummyClass());
            hist.Add(new DummyClass(), Math.PI);
        }
    }
}
