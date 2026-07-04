using Android.Content.PM;
using Android.Views;
using static Android.Content.PM.ConfigChanges;
using Application = Substance.Application;
using SDLActivity = Org.Libsdl.App.SDLActivity;

namespace Test.Android;

[Activity(
    Label = "@string/app_name", 
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    Exported = true,
    ScreenOrientation = ScreenOrientation.Landscape,
    Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
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

        var app = new Application();

        app.Exec();
    }
}