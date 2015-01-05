using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

namespace socketSever
{
    class Program
    {
        static void Main(string[] args)
        {
            String connString = ConfigurationManager.ConnectionStrings["socketdb"].ConnectionString;
            

            TcpListener serverSocket = new TcpListener(8888);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            requestCount = 0;




            while ((true))
            {
               
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> Data from client - "  + dataFromClient);
                  



                     string insert;
                     insert = "Insert into data(msg)VALUES(@msg)";
                     SqlConnection conn = new SqlConnection(connString);
                     SqlCommand comm = new SqlCommand(insert, conn);

                     // comm.Parameters.AddWithValue("@Id", added);
                     comm.Parameters.AddWithValue("@msg", dataFromClient);

                     try
                     {
                         conn.Open();
                         comm.ExecuteNonQuery();
                         //    added=added + 1;
                     }
                     catch (Exception err)
                     {
                         Console.WriteLine(err);
                     }
                     finally
                     {
                         conn.Close();

                     }


                    string serverResponse = "SERVER RESPONSE: Last Message from client ==> "+ dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }

    }
}
