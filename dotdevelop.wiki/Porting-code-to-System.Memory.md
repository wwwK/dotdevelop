With PR https://github.com/mono/monodevelop/pull/5570, we now have support for using the new Span/Memory APIs.

What does this entail? Lots of performance niceties.

This guide will go through common patterns that can be ported over to the new APIs with a mapping for each method and how it should be ported.

# Porting guidelines

To get a `ReadOnlySpan<char>` from a `string`, use `AsSpan()`.

To get a `string` from a `ReadOnlySpan<char>`, use `ToString()`.

We currently do not have stackalloc span support until we use C# 7.3, so use `"mystring".AsSpan()` where you need a local span.

Do not use span1 == span2, unless you know what you want is referential equality! Use `Span.Equals` or `Span.SequenceEquals`

| String API            | ReadOnlySpan API                |
|-----------------------|---------------------------------|
| ==                    | SequenceEquals                  |
| Substring             | Slice                           |
| Trim*                 | Trim*                           |
| Starts/EndsWith       | Starts/EndsWith                 |
| Equals                | Equals                          |
| IndexOfAny(char[], i) | Slice(i).IndexOfAny(char[]) + i |

Note: any `StringComparison` other than `Ordinal` and `OrdinalIgnoreCase` will convert to a string internally, so you won't get any benefit for equality there.

# Common patterns

## Slice manipulation - i.e. Substring+Trim

Normally, a string substring then a trim would allocate two strings.

By using Span APIs, you can prevent one intermediate string allocation:

```csharp
string a = "testcase ";
a = a.Substring(0, "test".Length).Trim(); // one allocation for Substring, one for Trim
// a = "case"
```

Would be with Span APIs:

```csharp
string a = "testcase ";
var span = a.AsSpan();
span = span.Slice(0, "test".Length).Trim();
// The above also be contracted to span.AsSpan(0, "test".Length).Trim()
a = span.ToString(); // just one string allocation
// a = "case"
```

## IndexOfAny(char[], i)

```csharp
char[] chars = new[] { 'a', 'b' };
int start = 1;
int i = "abc".IndexOfAny (chars, start);
if (i == -1)
    Console.WriteLine("Not found");
// i = 1;
```

This is tricker with Span APIs, the correct form is:

```csharp
char[] chars = new[] { 'a', 'b' };
int start = 1;
int i = "abc".AsSpan(start).IndexOfAny(chars);
if (i == -1)
    Console.WriteLine("Not found");
i += start;
// i = 1
```

# Candidates for Spanification

MSBuildProjectService.ToMSBuildPath

# Good reads

* https://msdn.microsoft.com/en-us/magazine/mt814808.aspx
* https://blogs.msdn.microsoft.com/mazhou/2018/03/25/c-7-series-part-10-spant-and-universal-memory-management/
* http://adamsitnik.com/Span/