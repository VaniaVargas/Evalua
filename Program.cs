using System;
using System.IO;

namespace Evalua
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Lenguaje a = new Lenguaje();

                a.Programa();
                /*while(!a.FinArchivo())
                {
                    a.NextToken();
                }*/
                a.cerrar();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message);
            }
        }
    }
}
