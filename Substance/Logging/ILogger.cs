namespace Substance.Logging;

public interface ILogger 
{
    void Info(string msg);

    void Warning(string msg);

    void Error(string msg);

    void Debug(string msg);
}