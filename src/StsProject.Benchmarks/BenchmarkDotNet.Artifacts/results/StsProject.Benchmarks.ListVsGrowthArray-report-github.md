``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|      Method |  N |     Mean |     Error |     StdDev |   Median |
|------------ |--- |---------:|----------:|-----------:|---------:|
|        **List** | **10** | **122.4 ns** |  **2.430 ns** |  **2.4956 ns** | **121.4 ns** |
| GrowthArray | 10 | 131.7 ns |  1.088 ns |  0.9086 ns | 131.7 ns |
|        **List** | **25** | **195.8 ns** |  **2.145 ns** |  **1.6743 ns** | **195.7 ns** |
| GrowthArray | 25 | 310.2 ns |  3.721 ns |  3.2989 ns | 309.9 ns |
|        **List** | **50** | **333.5 ns** | **10.524 ns** | **30.6977 ns** | **322.6 ns** |
| GrowthArray | 50 | 567.8 ns | 16.608 ns | 48.4474 ns | 548.1 ns |
