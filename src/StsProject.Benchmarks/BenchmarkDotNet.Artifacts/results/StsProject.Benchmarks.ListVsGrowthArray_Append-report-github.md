``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|      Method |  N |      Mean |     Error |     StdDev |    Median |  Gen 0 | Allocated |
|------------ |--- |----------:|----------:|-----------:|----------:|-------:|----------:|
|        **List** | **10** | **121.79 ns** |  **3.003 ns** |  **2.6622 ns** | **120.81 ns** | **0.2134** |     **336 B** |
| GrowthArray | 10 |  77.08 ns |  1.058 ns |  0.9901 ns |  76.87 ns | 0.1830 |     288 B |
|        **List** | **25** | **370.01 ns** |  **7.422 ns** | **19.0266 ns** | **376.60 ns** | **0.3915** |     **616 B** |
| GrowthArray | 25 | 238.01 ns |  4.385 ns |  8.3426 ns | 239.87 ns | 0.2797 |     440 B |
|        **List** | **50** | **567.49 ns** | **11.387 ns** | **29.7976 ns** | **574.63 ns** | **0.7315** |    **1152 B** |
| GrowthArray | 50 | 371.61 ns |  8.240 ns | 23.9067 ns | 373.44 ns | 0.4573 |     720 B |
