## Histogram and related classes (Part of ```CollectionsUtility```)

---

### Concrete classes

```Histogram{TKey, TValue}``` is a histogram that treats each unique value of ```TKey``` as a histogram bin,
and associate each with an arithmetic accumulator of type ```TValue``` so that their statistical value
(which can be frequency, probability, likelihood, weight) can be accumulated.

The statistical value can be integer-valued (```int```) or real-valued (```double```).

---

### Supported accumulator (value) types

- ```int```
- ```double```

---

### Basic requirement on the key type

Users should ensure that the key type ```TKey``` implements ```IEquatable{TKey}``` and provides 
a meaningful implementation for both ```Equals(TKey)``` and ```GetHashCode()```.

---

### Instantiation

When not using dependency injection, ```HistogramFactory{KeyType, ValueType}.Create()``` creates an instance
of histogram with the specified key type and accumulator type.

When using dependency injection such as AutoFac, use the following code example:

    void RegisterHistogram(Autofac.ContainerBuilder builder)
    {
        builder.Register(c => new HistArith_Int32()).As<IHistArith<int>>();
        builder.Register(c => new HistArith_Float64()).As<IHistArith<double>>();
        builder.RegisterGeneric(typeof(Histogram<,>)).As(typeof(IHistogram<,>));
    }

    IHistogram<TKey, TValue> CreateHistogram<TKey, TValue>(Autofac.IContainer container)
        where TValue : struct
    {
        return container.Resolve<IHistogram<TKey, TValue>>();
    }

---

### Adding items and values to the histogram

Items can be added to the histogram with or without a statistical value specified.

- ```Add(TKey key)```
- ```Add(TKey key, TValue value)```

When the same item is added again, its statistical value accumulates.

When an item is added to the histogram without a statistical value specified, the current value of 
the ```DefaultIncrement``` property is used.

On a newly created histogram, this value is set to one ```(1.0)```. This value can be changed at 
any time. To restore this property to the original value, use the following code:

    hist.DefaultIncrement = hist.HistArith.UnitValue;

Care should be taken when changing this property on histogram instances that are shared between 
different owners. Changes to the value may disrupt other users of the instance unless the original 
value is restored before handing over the instance to the other users.

---

### Implementation detail

The histogram class internally uses a ```UniqueList{TKey}``` to generate a unique integer for
each histogram key.

Users should ensure that the key type ```TKey``` implements ```IEquatable{TKey}``` and provides 
a meaningful implementation for both ```Equals(TKey)``` and ```GetHashCode()```.

In order to perform arithmetic operations on a generic type, the histogram class relies on
an interface ```IHistArith{TValue}``` for these operations. This adds some complexity to the 
class instantiation. Refer to the Instantiation section for code recommendation.

---
