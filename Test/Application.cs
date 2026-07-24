using Substance;

namespace Test;

public class Application(WindowOptions? options = null) : Substance.Application(options)
{
    protected override void OnCreatedOverride()
    {
        base.OnCreatedOverride();
    }
}
