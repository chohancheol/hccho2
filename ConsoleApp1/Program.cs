using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace httpServerTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. HttpListener 생성 및 접두사(Prefix) 설정
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");

            Console.WriteLine("서버를 시작합니다... (http://localhost:8080/)");
            listener.Start();

            // 2. 무한 루프를 돌며 비동기로 요청을 대기 및 처리
            while (true)
            {
                try
                {
                    // 클라이언트 요청이 들어올 때까지 대기 (비동기)
                    HttpListenerContext context = await listener.GetContextAsync();

                    // 요청 처리를 별도 태스크로 분리하여 멀티스레드 환경 대응
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"오류 발생: {ex.Message}");
                }
            }
        }

        // 3. Path별 요청 처리 및 응답 메서드
        static async Task HandleRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            // 요청 정보 로깅
            string path = request.Url.AbsolutePath;
            Console.WriteLine($"[{request.HttpMethod}] {path} 요청 수신");

            string responseString = "";
            response.ContentType = "text/plain; charset=utf-8";

            // Path(경로)별 분기 처리
            switch (path)
            {
                case "/":
                    response.StatusCode = (int)HttpStatusCode.OK;
                    responseString = "메인 페이지에 오신 것을 환영합니다!";
                    break;

                case "/hello":
                    response.StatusCode = (int)HttpStatusCode.OK;
                    responseString = "안녕하세요! C# HttpListener 서버입니다.";
                    break;

                case "/time":
                    response.StatusCode = (int)HttpStatusCode.OK;
                    responseString = $"현재 서버 시간: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    break;

                default:
                    // 정의되지 않은 경로일 경우 404 Not Found 처리
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    responseString = "404 - 페이지를 찾을 수 없습니다.";
                    break;
            }

            // 4. 응답 스트림 전송
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            using (Stream output = response.OutputStream)
            {
                await output.WriteAsync(buffer, 0, buffer.Length);
            }

            // 스트림을 닫아 응답 전송 완료하기
            response.Close();
        }
    }
}
