``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|      Method |     N |         Mean |        Error |        StdDev |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------ |------ |-------------:|-------------:|--------------:|---------:|---------:|---------:|----------:|
|        **List** |    **10** |     **129.7 ns** |     **1.990 ns** |      **1.862 ns** |   **0.2134** |        **-** |        **-** |     **336 B** |
| GrowthArray |    10 |     142.5 ns |     1.746 ns |      1.548 ns |   0.1779 |        - |        - |     280 B |
|        **List** |   **250** |   **1,057.5 ns** |    **20.738 ns** |     **21.297 ns** |   **2.7122** |        **-** |        **-** |    **4272 B** |
| GrowthArray |   250 |   2,235.7 ns |    12.115 ns |     10.740 ns |   1.5144 |        - |        - |    2384 B |
|        **List** |  **5000** |  **21,855.9 ns** |   **325.262 ns** |    **304.250 ns** |  **83.3130** |        **-** |        **-** |  **131368 B** |
| GrowthArray |  5000 |  40,337.3 ns |   399.101 ns |    353.792 ns |  41.6260 |        - |        - |   66144 B |
|        **List** | **49152** | **389,233.3 ns** | **4,419.481 ns** |  **4,133.985 ns** | **285.6445** | **285.6445** | **285.6445** | **1048944 B** |
| GrowthArray | 49152 | 448,520.2 ns | 9,589.232 ns | 16,794.766 ns | 124.5117 | 124.5117 | 124.5117 |  524968 B |
