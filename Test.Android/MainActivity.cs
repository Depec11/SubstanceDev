using Android.Content.PM;
using Android.Views;
using Substance.Android;
using static Android.Content.PM.ConfigChanges;
using SDLActivity = Org.Libsdl.App.SDLActivity;

namespace Test.Android;

[Activity(
    Label = "@string/app_name", 
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    Exported = true,
    ScreenOrientation = ScreenOrientation.Landscape,
    Theme = "@style/AppTheme",
    ConfigurationChanges =
        ConfigChanges.LayoutDirection | Locale | GrammaticalGender | FontScale | 
        FontWeightAdjustment | ConfigChanges.Orientation | UiMode |
        ScreenLayout | ScreenSize | SmallestScreenSize |
        Keyboard | KeyboardHidden | Navigation
)]
public class MainActivity : SDLActivity
{
    protected override string[]? GetLibraries() => ["SDL3"];

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        RequestWindowFeature(WindowFeatures.NoTitle);
        
        Window?.SetFlags(
            WindowManagerFlags.Fullscreen | 
            WindowManagerFlags.LayoutNoLimits |
            WindowManagerFlags.TranslucentStatus |
            WindowManagerFlags.TranslucentNavigation,
            WindowManagerFlags.Fullscreen |
            WindowManagerFlags.LayoutNoLimits |
            WindowManagerFlags.TranslucentStatus |
            WindowManagerFlags.TranslucentNavigation
        );
        
        base.OnCreate(savedInstanceState);
    }

    protected override void Main()
    {
        base.Main();

        var app = new AndroidApplication();
        var gameLoop = new MainGameLoop();

        app.Initialize();

        app.Exec();

        gameLoop.Dispose();
        app.Dispose();
    }
}