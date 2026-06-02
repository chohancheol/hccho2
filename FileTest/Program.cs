using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTest
{
    //https://zetcode.com/csharp

    internal class Program
    {
        static void Main(string[] args)
        {
            // ReadLine
            var path = "words.txt";

            var lines = File.ReadLines(path);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }


            string[] lines2 = File.ReadAllLines(path);

            foreach (var line2 in lines2)
            {
                Console.WriteLine(line2);
            }

            string readText = File.ReadAllText(path);
            Console.WriteLine(readText);

            string targetDirectory = @"C:\TestFolder";

            try
            {
                if (Directory.Exists(targetDirectory))
                {
                    DirectoryInfo di = new DirectoryInfo(targetDirectory);

                    // 하위 디렉터리를 포함하여 모든 파일 정보 객체를 가져옵니다.
                    FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);

                    Console.WriteLine("{0,-20} \t {1,-10} \t {2}", "파일 이름", "크기(Bytes)", "수정된 날짜");
                    Console.WriteLine(new string('-', 70));

                    foreach (FileInfo file in files)
                    {
                        // file 객체의 프로퍼티를 활용해 상세 정보 출력
                        Console.WriteLine("{0,-20} \t {1,-10:N0} \t {2:yyyy-MM-dd HH:mm:ss}",
                            file.Name,
                            file.Length,
                            file.LastWriteTime);
                    }
                }
                else
                {
                    Console.WriteLine("디렉터리가 존재하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }
    }
}
