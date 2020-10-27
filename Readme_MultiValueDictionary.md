## MultiValueDictionary (Part of ```CollectionsUtility```)

---

### Important note

Multi-value dictionary (shorthand MVD) is still in design stage. This is because a few high-impact 
decisions have not been finalized. These are:

#### Decision 1: what should be the primary interface model of MVD?

Choices: 

1. MVD can model a dictionary of collections.
1. MVD can model a collection of key-value pairs (KVP).

Each choice comes with some drawbacks. The unwillingness to accept these drawbacks held back the 
evolution of the MVD implementation.

#### Decision 2: should MVD enforce uniqueness of values (under the same key)?

This is not really a design decision. It is a user decision - different use cases may need 
different capabilities. The real design decision is to survey, research, and support the important 
use cases while keeping code complexity and maintenance reasonable.

#### Decision 3: should the user be allowed to decide what value collections would be used by MVD under each key?

This is related to decision 2.

#### Decision 4: between the different concepts, in what ways can adapters (wrappers) be used to make one look like the other?

#### Decision 5: should the overall implementation optimize for the case where the majority of keys will only have a single value?

Choices:

1. No optimizations. The internal implementation is uniformly applicable whether a key has a single value or multiple values.
  - This also guarantees that the value collection associated with each key can be exposed on the interface.
1. Optimization using two dictionaries, with the first dictionary for single-valued keys and the second for multi-valued keys.
1. Optimization using a sum-type: the associated data for each key can contain either a single value or a collection of values.
  - Expressing the sum-type as an object: it may either be a boxed value, or a collection of values.
  - Expressing the sum-type as a CSharp 7.0 tuple.
  - Expressing the sum-type as a user-defined type that behaves like a CSharp 8.0 record, or a CSharp 7.0 tuple.

#### Decision 6: should the implementation optimize for the cases where the key, the value, or both of them are value-types (structs)?

This is actually a poor (premature) way of thinking about optimization. Instead, the optimization goals should be reframed as:

1. Should the implementation try to minimize the (possibly unnecessary) allocations of transient memory?
1. Should the implementation try to minimize the transient (and possibly redundant) copying of data?
1. Should the implementation try to improve the locality and predictability of data placement and accesses (for better cacheability)?

---

### Interfaces

---

#### IKeyValueCollectionMethods (K, V)

- Requires three methods: Add, Remove, Contains
  - Each method takes two arguments: a `TKey` and a `TValue`.
  - Does not inherit from any other interface.

Rationale for not inheriting from ICollection of KVP

- This is because IEnumerable or ICollection contains properties or methods that would conflict with each other.
- An implementation may either be IEnumerable ( TKey, IEnumerable (TValue) ) or IEnumerable ( KeyValuePair ), but cannot be both at the same time.
- The Count property may report the number of keys, or the total number of key value pairs, but not both.
- To make them unambiguous, interface extension is applied at a different level on the inheritance hierarchy.

---

#### IKeyValueCollection (K, V)

Extends
- IKeyValueCollectionMethods
- ICollection ( KVP )

This interface is basically the union of the two inherited interfaces.

One simplest way to implement this interface is to wrap around a list of KVP.

---

#### IMultiValueDictionary (K, V)

Extends 
- IKeyValueCollection

Assumes that the implementation is organized around key lookup.

Provides
- KeyCount property (in addition to Count, which corresponds to the total KVP)
- Batched add (add-range) operations
  - AddRange ( IEnumerable KVP )
  - AddRange ( K, IEnumerable V )
- Key-only operations
  - ContainsKey ( K )
  - Remove ( K )
- Key-centric operations
  - TryGetAny ( K, out V )
  - TryGetAll ( K ) returns IEnumerable V, never null, count can be zero
  - TryGetCount ( K ) returns the count of V values for K

Does not make the assumptions...
- ... That the implementation contains a single dictionary;
- ... That the associated data for each key is exposed;
- ... That the associated data for each key is an ICollection.

Implementation status
- Nearly fully implemented
  - Removal is not implement
- Not tested

Test plan
- Smoke tests
  - Constructors,
  - Basic operations with 1, 2, 3 unique keys,
  - Verifies use of EqualityComparer.Default when using parameterless constructor or null as argument
  - Verifies use of user-provided IEqualityComparer
  - Verifies correct construction and operation when using types K, V that do not implement IEquatable as long as IEqualityComparer is provided
- For a specific key, transition from one (1) associated values to two (2)
- For a specific key, transition from two (2) values to many
- Removal by (K, V) or by (KVP) *Not implemented, therefore tests not needed*
- Smoke tests for each method

---

#### ICollectionDictionary (K, V, VC)

`VC` is a generic type parameter that is also an ICollection of V.

Extends
- IDictionary (K, VC)
- IKeyValueCollectionMethods

Requires that each key is associated with a specific collection containing values.

Allows replacement of the entire collection.

Allows direct manipulation of the values stored in the value collection, without going through the aggregate root.

The Count property refers to the number of keys, not the total number of KVP.

Provides a method, AsKeyValueCollection, which allows a readout of KVP. 

---

### Concrete classes

```MultiValueDictionary{TKey, TValue}``` 

