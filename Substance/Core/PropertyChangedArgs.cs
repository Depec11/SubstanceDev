namespace Substance.Core;

public record class PropertyChangedArgs<T>(T OldValue, T NewValue);