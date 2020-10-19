## UniqueList and related classes (Part of ```CollectionsUtility```)

---

### Concrete classes

```UniqueList{T}``` is a list of unique items.

```UniqueListEx{T}``` is a list of unique items that supports item removal and replacement.

### Interfaces defined

- ```IReadOnlyUniqueList```
- ```IUniqueList```
- ```IUniqueListEx```

### Interfaces implemented

- ```IList```
- ```IReadOnlyList```
- ```ICollection```
- ```IReadOnlyCollection```

---

### Interface hierarchy

<details>
<summary>Click to expand.</summary>

    - **UniqueList** (concrete)
      - UniqueListBase
      - (implied) IUniqueList, IReadOnlyUniqueList, IList, IReadOnlyList, ICollection, IReadOnlyCollection
    - **UniqueListEx** (concrete)
      - UniqueListBase
      - (properties) CanReplace, CanRemove
      - (mutators) Replace, ReplaceAt, Remove, RemoveAt
      - (implied) IUniqueListEx, IUniqueList, IReadOnlyUniqueList, IList, IReadOnlyList, ICollection, IReadOnlyCollection
    - **ReadOnlyUniqueList** (concrete)
      - (implied) IReadOnlyUniqueList, IReadOnlyList, IReadOnlyCollection
    - **UniqueListBase** (abstract base)
      - (implied) IUniqueList, IReadOnlyUniqueList, IList, IReadOnlyList
    - **IReadOnlyUniqueList** (interface)
      - (properties) Count
      - (methods) IndexOf, ItemAt, Contains, CopyTo(Array)
      - (implied) IReadOnlyList, IReadOnlyCollection
    - **IUniqueList** (interface)
      - (properties) Count
      - (accessors) IndexOf, ItemAt, Contains, CopyTo(Array)
      - (mutators) Add, Insert, Clear
      - (implied) IList, ICollection, IReadOnlyList
      - (explicit properties) IsReadOnly
    - **IUniqueListEx** (interface)
      - (properties) CanReplace, CanRemove
      - (mutators, conditionally supported) Replace, ReplaceAt, Remove, RemoveAt
      - (implied) IUniqueList, IReadOnlyUniqueList, IList, IReadOnlyList, ICollection, IReadOnlyCollection

</details>

---

### Basic requirement on item type 

Users should ensure that the item type ```T``` implements ```IEquatable{T}``` and provides 
a meaningful implementation for both ```Equals(T)``` and ```GetHashCode()``` in order to 
correctly detect duplicated items.

---

### Adding items

When each item is added for the first time, it is assigned an index, much like the item index 
on a ```List{T}```.

Adding the same item is an idempotent operation; duplicated additions are ignored.

---

### As an ordered collection

These list-like classes maintain the order of items in which they are added. 

---

### Looking up (by item, or by index)

To look up an item by index, use ```UniqueListBase{T}.ItemAt(int)```.

To look up the index of an item, use ```UniqueListBase{T}.IndexOf(T)```.

---

### Item indexer and reverse indexer

The indexer for looking up an item by index is only available via a cast to ```IList{T}``` 
or ```IReadOnlyList{T}```. It is equivalent to the ```ItemAt(int)``` method.

The indexer for looking up the index for an existing item on the ```UniqueList``` is only available
via a cast to ```???```. It is equivalent to the ```IndexOf(T)``` method.

The reason they are not provided as a natural indexer on the concrete classes themselves is 
because there is a potential ambiguity arising from an instance of ```IUniqueList{int}```.

For situations where the potential ambiguity does not arise, the extension method ```???``` 
can be used to create a lightweight wrapper that enables both indexers.

---

### Features that are available on ```UniqueListEx{T}``` but not ```UniqueList{T}```

Item removal and replacement are provided by ```UniqueListEx{T}``` but not ```UniqueList{T}```.

These features can be enabled or disabled on an instance of ```UniqueListEx{T}``` by setting 
the properties ```CanReplace``` and ```CanRemove``` respectively.

Care must be taken when using these features, because they open up possibility for programming 
errors. 

---

In particular, item removal leave behind a hole in item index assignment, as the index value
for the removed item cannot be reused. This means the collection's index range no longer corresponds
to ```(0 &lt;= index &lt; Count)```.

Moreover, when item replacement is used in a certain way, it can violate a basic assumption 
mentioned previously, that the index associated with an item does not change. Consider the 
following code:

```C#
T old;
T new;
uniqueListEx.Replace(old, new);
uniqueListEx.Add(old);
```

After executing this code, the old item will take on a new index value, whereas the new item
will assume the index value previously assigned to the old item. This can upset certain 
users of the ```UniqueList``` classes.

---

### Item removal (only available on ```UniqueListEx{T}```)

Item removal only affects the item being removed. The index values assigned to other items 
will not be affected. 

The index value associated with that item will not be reused. Moreover, if an item is removed 
and then added again, the original index value associated with it will be reinstated.

---

### Item replacement (only available on ```UniqueListEx{T}```)

Item replacement guarantees that the new item will take over the index value associated with 
the old item.

If the old item was already removed, its index value will be reinstated and reassigned to 
the new item. When this happens, the class will not be able to reinstate the old item to
its original index. If the old item is added again, it will be assigned to a new index value.

---

### Implementation detail

This class internally uses a ```Dictionary{T, int}``` and its default equality comparer to 
determine item equality and to detect duplication.

Users should ensure that the item type ```T``` implements ```IEquatable{T}``` and provides 
a meaningful implementation of both ```Equals(T)``` and ```GetHashCode()``` in order to 
correctly detect duplicated items.

---
