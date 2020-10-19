### ```CollectionsUtility``` (Part of ```LosslessStitcher```)

---

#### UniqueList

```UniqueList{T}``` is a list of unique items.

```UniqueListEx{T}``` is a list of unique items that supports item removal and replacement.

Main article: [Readme_UniqueList.md](Readme_UniqueList.md)

---

#### Histogram

```Histogram<TKey, TValue>``` is a histogram that treats each unique value of ```TKey``` as a histogram bin,
and associate each with an arithmetic accumulator of type ```TValue``` so that their statistical value
(which can be frequency, probability, likelihood, weight) can be accumulated.

The statistical value can be integer-valued (```int```) or real-valued (```double```).

Main article: [Readme_Histogram.md](Readme_Histogram.md)

---
