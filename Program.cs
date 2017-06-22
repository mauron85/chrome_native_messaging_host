// https://stackoverflow.com/questions/30880709/c-sharp-native-host-with-chrome-native-messaging
// https://social.msdn.microsoft.com/Forums/en-US/92846ccb-fad3-469a-baf7-bb153ce2d82b/simple-udp-example-code?forum=netfxnetcom

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Console;

namespace chrome_native_messaging_host
{
   class Program
   {
      public static void Main(string[] args)
      {
         Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
         IPAddress address = IPAddress.Parse("127.0.0.1");
         EndPoint endPoint = new IPEndPoint(address, 19700);

         Console.Error.WriteLine("[INFO] Messaging host listening...");

         JObject json;
         while ((json = Read()) != null)
         {
            Console.Error.WriteLine("[DEBUG] msg received: {0}", json.ToString());
            if (json["action"] != null);
            {
                  try 
                  {
                        byte[] sendBuffer = Encoding.ASCII.GetBytes(json.ToString());
                        socket.SendTo(sendBuffer, endPoint);
                        byte[] receiveBuffer = new byte[4096];
                        socket.ReceiveFrom(receiveBuffer, ref endPoint);
                        var response = Encoding.ASCII.GetString(receiveBuffer);
                        Console.Error.WriteLine("[DEBUG]' socket received {0}", response);
                        Write(response);
                  } catch (Exception e)
                  {
                        Console.Error.WriteLine("[ERROR] socket send: {0}", e.ToString());
                  }

            }
         }
      }

      public static JObject Read()
      {
         var stdin = Console.OpenStandardInput();
         var length = 0;

         var lengthBytes = new byte[4];
         stdin.Read(lengthBytes, 0, 4);
         length = BitConverter.ToInt32(lengthBytes, 0);

         var buffer = new char[length];
         using (var reader = new StreamReader(stdin))
         {
            while (reader.Peek() >= 0)
            {
               reader.Read(buffer, 0, buffer.Length);
            }
         }

         return (JObject)JsonConvert.DeserializeObject<JObject>(new string(buffer));
      }

      public static void Write(String jsonString)
      {
         var json = (JObject)JsonConvert.DeserializeObject<JObject>(jsonString);
         var bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString(Formatting.None));

         var stdout = Console.OpenStandardOutput();
         stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
         stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
         stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
         stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
         stdout.Write(bytes, 0, bytes.Length);
         stdout.Flush();
      }
   }
}
