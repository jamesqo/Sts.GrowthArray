``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|      Method |     N |         Mean |         Error |        StdDev |       Median |
|------------ |------ |-------------:|--------------:|--------------:|-------------:|
|        **List** |    **10** |     **127.8 ns** |      **1.922 ns** |      **1.605 ns** |     **127.6 ns** |
| GrowthArray |    10 |     138.7 ns |      1.744 ns |      1.632 ns |     138.8 ns |
|        **List** |   **250** |   **1,085.3 ns** |     **26.613 ns** |     **75.929 ns** |   **1,058.0 ns** |
| GrowthArray |   250 |   2,255.3 ns |     44.470 ns |     59.366 ns |   2,232.4 ns |
|        **List** |  **5000** |  **23,933.5 ns** |    **774.668 ns** |  **2,133.661 ns** |  **23,184.6 ns** |
| GrowthArray |  5000 |  40,788.9 ns |    775.484 ns |    761.629 ns |  40,696.9 ns |
|        **List** | **49152** | **410,341.9 ns** | **10,511.812 ns** | **29,476.248 ns** | **404,218.7 ns** |
| GrowthArray | 49152 | 458,069.1 ns | 10,220.751 ns | 29,325.252 ns | 446,726.6 ns |
