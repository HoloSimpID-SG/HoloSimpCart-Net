## Appending Strings

Instead of:

```cs
string strRes = string.Empty;
strRes += "Hello";
strRes += " World!";
return strRes;
```

Do:
```cs
StringBuilder strBuilder = new();
strBuilder.Append("Hello ");
strBuilder.Append(" World!");
return strBuilder.ToString();
```

Think of these two as ***Array*** vs ***Dynamic Array***.

In `string` is like ***Array***, whenever you append it with new string, it has to create a new container, move the contents and remove the old memory. When the two appending `string` is very big, you will run out of memory.

`StringBuilder` is like ***Dynamic Array***, it allows actual appending and much more friendly for memory.

## Upserting `Dictionary`

Instead of:

```cs
if (dict.ContainsKey(key))
    dict[key] += value;
else
    dict.Add(key, value);
```

Do:

```cs
if (dict.TryGetValue(key, out var lastValue))
    dict[key] = lastValue + value;
else
    dict.Add(key, value);
```

The first one, accesses dict three times:

1. `dict.ContainsKey` -> looks if key is inside.
2. `dict[key] +=` -> is broken down to:

```cs
var x = dict[key] + value; // One access call
dict[key] = x;  // Second access call
```

The second one, only do it twice:

1. `dict.TryGetValue` -> looks if key is inside and returns the value where possible
2. `dict[key]` = -> only assignment

## returning `Collection`

Instead of:

```cs
public IList<int> function(params object[])
{
    IList<int> result = new();
    // Process
    return result;
}
// Called with:
List<int> list = function(/*parameter*/);
```

Do (where possible):

```cs
public void function(IList<int> result, params object[])
{
    // Process
}
// Called with:
// where list is pooled
function(list, /*parameter*/);
```

## Iterating over `Collection` with `for` loops

Instead of:

```cs
double[] array;
for (int i = 0; i < array.Length; i++)
{
    // Loop Body
}
```

Do:

```cs
double[] array;
int len = array.Length;
for (int i = 0; i < len; i++)
{
    // Loop Body
}

// Or

foreach (double element in array)
{
    // Loop body
}
```

Loop checks, which in this case the `i < array.Length` will get called on each iteration. While this does allow it to adapt to change in size, if size is known to be fixed, it is better to save it into a variable first or use a `foreach` loop.
