```

BenchmarkDotNet v0.13.7, Windows 11 (10.0.22000.2295/21H2/SunValley)
12th Gen Intel Core i7-1255U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 7.0.203
  [Host]     : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2


```
|  Method |       Mean |     Error |     StdDev | Allocated |
|-------- |-----------:|----------:|-----------:|----------:|
| Method1 |   5.239 ms | 0.1475 ms |  0.4161 ms |   1.21 KB |
| Method2 | 514.396 ms | 9.7247 ms | 14.2543 ms | 117.72 KB |
| Method3 |  10.501 ms | 0.1963 ms |  0.3874 ms |   2.35 KB |
