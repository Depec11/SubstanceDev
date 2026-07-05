namespace Substance.Core;

public record class PropertyChangedArgs<T>(T Value, T OldValue);