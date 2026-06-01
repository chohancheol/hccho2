using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientTest
{
    internal class Program
    {
        // HttpClient는 프로세스당 하나만 생성하여 재사용하는 것이 권장됩니다.
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== HttpClient 테스트 프로그램 시작 ===");
            Console.WriteLine("※ 주의: 앞서 만든 HttpServer가 실행 중이어야 합니다.\n");

            string baseUrl = "http://localhost:8080";

            // 1. 메인 페이지 경로 (/) 테스트
            await TestGetRequestAsync($"{baseUrl}/");

            // 2. /hello 경로 테스트
            await TestGetRequestAsync($"{baseUrl}/hello");

            // 3. /time 경로 테스트
            await TestGetRequestAsync($"{baseUrl}/time");

            // 4. 존재하지 않는 경로 (404 예외 핸들링) 테스트
            await TestGetRequestAsync($"{baseUrl}/invalid-path");

            Console.WriteLine("\n=== 테스트 종료 ===");
            Console.ReadLine();
        }

        /// <summary>
        /// 지정한 URL로 GET 요청을 보내고 응답 결과를 출력하는 메서드
        /// </summary>
        static async Task TestGetRequestAsync(string url)
        {
            Console.WriteLine($"[요청 전송] GET -> {url}");

            try
            {
                // GetAsync를 통해 비동기로 HTTP 응답을 받아옵니다.
                HttpResponseMessage response = await client.GetAsync(url);

                // 상태 코드 확인 (200 OK 등)
                Console.WriteLine($"[상태 코드] {(int)response.StatusCode} {response.StatusCode}");

                // 응답 Body 내용을 문자열로 읽기
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[응답 내용] {responseBody}");
            }
            catch (HttpRequestException e)
            {
                // 네트워크 연결 실패 등 물리적인 오류 처리
                Console.WriteLine($"[에러 발생] 요청 중 오류가 발생했습니다: {e.Message}");
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
