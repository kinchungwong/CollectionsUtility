## MultiValueDictionary (Part of ```CollectionsUtility```)

---

### Important note

Multi-value dictionary (shorthand MVD) is still in design stage. This is because a few high-impact 
decisions have not been finalized. These are:

#### Decision 1: what should be the conceptual model of MVD?

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

---

### Concrete classes

```MultiValueDictionary{TKey, TValue}``` 

