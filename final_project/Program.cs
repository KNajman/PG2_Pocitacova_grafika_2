using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;


namespace PG2
{
    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1200, 720),
                Title = "Solar system",
                // This is needed to run on macos and debug
                Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                Window.ShowHWinfo();
                window.Run();
            }
        }
    }
}