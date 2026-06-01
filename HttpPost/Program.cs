using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    // 1. 주고받을 데이터 구조(DTO) 정의
    public class RequestData
    {
        public string TagName { get; set; }
        public int TagValue { get; set; }
    }

    public class ResponseData
    {
        public string Result { get; set; }
        public string Message { get; set; }
    }

    class Program
    {
        private static readonly string ServerUrl = "http://localhost:8080/";
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== HTTP POST 통신 데모 시작 ===");

            // 2. 서버를 백그라운드 태스크로 먼저 실행
            Task serverTask = Task.Run(() => StartServerAsync());

            // 서버가 구동될 시간을 잠시 대기 (1초)
            await Task.Delay(1000);

            // 3. 클라이언트가 POST 요청 실행
            await StartClientAsync();

            Console.WriteLine("\n아무 키나 누르면 프로그램이 종료됩니다...");
            Console.ReadKey();
        }

        // ==================== [ SERVER SIDE ] ====================
        static async Task StartServerAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(ServerUrl);
            listener.Start();
            Console.WriteLine("[SERVER] 서버가 시작되었습니다. 요청을 대기합니다...");

            while (true)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleServerRequestAsync(context));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER ERROR] {ex.Message}");
                    break;
                }
            }
        }

        static async Task HandleServerRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            Console.WriteLine($"[SERVER] [{request.HttpMethod}] 요청이 들어왔습니다.");

            // POST 메서드 검증
            if (request.HttpMethod.ToUpper() == "POST")
            {
                try
                {
                    // 4. 클라이언트가 보낸 Body(JSON) 스트림 읽기
                    string requestBody = "";
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    Console.WriteLine($"[SERVER] 수신된 원본 JSON: {requestBody}");

                    // 5. JSON 문자열을 C# 객체로 역직렬화
                    RequestData receivedData = JsonConvert.DeserializeObject<RequestData>(requestBody);
                    Console.WriteLine($"[SERVER] 파싱 결과 -> TagName: {receivedData.TagName}, TagValue: {receivedData.TagValue}");

                    // 6. 응답 데이터 생성 및 직렬화
                    ResponseData resObj = new ResponseData
                    {
                        Result = "OK",
                        Message = $"태그 [{receivedData.TagName}] 데이터가 성공적으로 저장되었습니다."
                    };
                    string jsonResponse = JsonConvert.SerializeObject(resObj);

                    // 7. 응답 전송
                    byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
                    response.ContentType = "application/json; charset=utf-8";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentLength64 = buffer.Length;

                    using (Stream output = response.OutputStream)
                    {
                        await output.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER ERROR] 요청 처리 중 오류: {ex.Message}");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                // POST가 아닌 메서드로 요청이 오면 405 Method Not Allowed 반환
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            }

            response.Close();
        }

        // ==================== [ CLIENT SIDE ] ====================
        static async Task StartClientAsync()
        {
            Console.WriteLine("[CLIENT] 서버로 POST 요청을 보낼 준비를 합니다.");

            // 1. 전송할 데이터 객체 생성
            RequestData myData = new RequestData
            {
                TagName = "A-1",
                TagValue = 29
            };

            // 2. 객체를 JSON 문자열로 변환
            string jsonString = JsonConvert.SerializeObject(myData);

            // 3. HttpContent 구성 (미디어 타입을 application/json으로 설정)
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                Console.WriteLine("[CLIENT] POST 요청 전송 중...");
                // 4. POST 요청 실행
                HttpResponseMessage response = await client.PostAsync(ServerUrl, content);

                // 5. 응답 결과 확인
                Console.WriteLine($"[CLIENT] 서버 응답 상태 코드: {(int)response.StatusCode} {response.StatusCode}");

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[CLIENT] 서버로부터 받은 응답 Body: {responseBody}");

                // 6. 받은 응답 JSON을 다시 객체화하여 사용
                ResponseData resultObj = JsonConvert.DeserializeObject<ResponseData>(responseBody);
                Console.WriteLine($"[CLIENT] 최종 분석 결과 내용: {resultObj.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT ERROR] 요청 실패: {ex.Message}");
            }
        }
    }
}
