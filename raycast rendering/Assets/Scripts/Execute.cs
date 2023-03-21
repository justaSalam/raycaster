//sneaky...
using System.Diagnostics;
using UnityEngine;

public class Execute : MonoBehaviour
{
    string[] randomMessage = new string[]
    {
        "hi..","you shouldnt continue playing..", "its getting late..", "why are you still here", "confused?", "link"
    };

    private void Start()
    {
        ExecuteCommand("msg %username% " + randomMessage[Random.Range(0, randomMessage.Length)]);
    }
    public static void ExecuteCommand(string command)
    {
        UnityEngine.Debug.Log(command);
        ProcessStartInfo processInfo;
        Process process;

        processInfo = new ProcessStartInfo("cmd.exe", "/c" + command);

        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processInfo.CreateNoWindow = true;
        
        process = Process.Start(processInfo);
        process.WaitForExit();
    }
}
