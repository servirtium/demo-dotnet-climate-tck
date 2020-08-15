using System.IO;

namespace Servirtium.Demo
{
    public static class TestDirectories
    {
        static TestDirectories()
        {
            Directory.CreateDirectory(RECORDING_OUTPUT_DIRECTORY);
            Directory.CreateDirectory(PROXY_RECORDING_OUTPUT_DIRECTORY);
        }
        
        internal static readonly string RECORDING_OUTPUT_DIRECTORY = "test_recording_output";
        internal static readonly string PROXY_RECORDING_OUTPUT_DIRECTORY = "proxy_test_recording_output";
        internal static readonly string PREGENERATED_PLAYBACKS_DIRECTORY = @"..\..\..\test_playbacks".Replace('\\', System.IO.Path.DirectorySeparatorChar);
        
    }
}