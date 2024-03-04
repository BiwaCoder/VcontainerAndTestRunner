using UnityEngine;
using VContainer;

// 先頭に[Logger]を付けてログ出力するクラス
public sealed class Logger
{
    public void Log(string message) => Debug.Log("[Logger] " + message);
}

// 足し算するだけのクラス
public sealed class Calculator
{
    public int Add(int a, int b) => a + b;
}

// LoggerとCalculatorに依存するクラス
public sealed class HogeClass : System.IDisposable
{
    private readonly Logger logger;
    private readonly Calculator calculator;

    [Inject]
    public HogeClass(Logger logger, Calculator calculator)
    {
        this.logger = logger;
        this.calculator = calculator;
    }

    public void LoggerTest()
    {
        logger.Log("LoggerTest");
    }

    public int CalculatorTest(int a, int b)
    {
        int result = calculator.Add(a, b);
        logger.Log($"{a} + {b} = {result}");
        return result;
    }
    
    public void Dispose()
    {
        Debug.Log("Dispose");
    }
}