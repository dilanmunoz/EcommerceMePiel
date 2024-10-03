using System.Net;
using System.Runtime.InteropServices;
using System.Security;

namespace EcommerceMePiel.Datos
{
    public class NetworkConnection : IDisposable
    {
        private string _networkPath;

        public NetworkConnection(string networkPath, string username, SecureString password)
        {
            _networkPath = networkPath;

            // Convertir SecureString a string para usar en la conexión
            IntPtr passwordPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
            try
            {
                string passwordString = Marshal.PtrToStringUni(passwordPtr);

                var netResource = new NetResource
                {
                    ResourceType = ResourceType.Disk,
                    RemoteName = _networkPath
                };

                // Conectar a la red
                WNetAddConnection2(netResource, passwordString, username, 0);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passwordPtr);
            }
        }

        public void Dispose()
        {
            // Desconectar la unidad
            WNetCancelConnection2(_networkPath, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceType ResourceType;
            public string ResourceName;
            public string Provider;
            public string RemoteName;
            public string LocalName;
            public int Flags;
        }

        public enum ResourceType
        {
            Disk = 1,
            Print = 2,
            Reserved = 8,
            Unknown = 0xFFFF
        }
    }

}
