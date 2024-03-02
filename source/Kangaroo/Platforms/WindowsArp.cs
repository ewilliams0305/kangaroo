namespace Kangaroo.Platforms
{
    internal class WindowsArp
    {
        [System.Runtime.InteropServices.DllImport("iphlpapi.dll", EntryPoint = "SendARP")]
        internal static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref uint physicalAddrLen);

    }
}
