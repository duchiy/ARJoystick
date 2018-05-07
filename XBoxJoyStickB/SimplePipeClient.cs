using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
namespace SimplePipeClient
{
    class SimplePipeClient
    {
        //       static void Main(string[] args)
        //       {
        //           ReceiveByteAndRespond();
        //       }

        public static string ReceiveByteAndRespond()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("test-pipe"))
            {

                namedPipeClient.Connect();
                namedPipeClient.WriteByte(67);
                int i=0; int ret=0;
                List<Byte> myByte = new List<byte>();
                while ((ret != -1) && (i < 1000))
                {
                    ret= namedPipeClient.ReadByte();
                    if (ret == -1)
                        break;

                    myByte.Add((byte)ret);
                    i++;
                } ;

                ASCIIEncoding encoding = new ASCIIEncoding();
                string command = encoding.GetString(myByte.ToArray());

                return command;

            }
        }
    }
}
