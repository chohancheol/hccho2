using JsonTest2.JsonParsingTest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 테스트를 위해 @(이스케이프 스트링) 기호로 기재한 JSON 리터럴
            string jsonInput = @"{
                ""Status"": ""Success"",
                ""ResponseTimeMs"": 120,
                ""ResultCount"": 2,
                ""Users"": [
                    {
                        ""UserId"": ""user_01"",
                        ""UserName"": ""홍길동"",
                        ""IsActive"": true,
                        ""Contact"": {
                            ""Email"": ""gildong@example.com"",
                            ""Phone"": ""010-1234-5678""
                        },
                        ""Roles"": [""Admin"", ""Manager""]
                    },
                    {
                        ""UserId"": ""user_02"",
                        ""UserName"": ""이순신"",
                        ""IsActive"": false,
                        ""Contact"": {
                            ""Email"": ""sunshin@example.com"",
                            ""Phone"": ""010-9876-5432""
                        },
                        ""Roles"": [""User""]
                    }
                ]
            }";

            Console.WriteLine("==================================================");
            Console.WriteLine("       Newtonsoft.Json 역직렬화 가동 테스트");
            Console.WriteLine("==================================================");

            try
            {
                // 핵심 기능: 제네릭 형식 지정을 통한 자동 변환 처리
                ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(jsonInput);

                if (response != null)
                {
                    Console.WriteLine($"[서버 응답 상태] : {response.Status}");
                    Console.WriteLine($"[트랜잭션 속도] : {response.ResponseTimeMs} ms");
                    Console.WriteLine($"[검색 유저 수]   : {response.ResultCount} 명");
                    Console.WriteLine("\n--------------------------------------------------");
                    Console.WriteLine("               유저 상세 리스트 전개");
                    Console.WriteLine("--------------------------------------------------");

                    foreach (var user in response.Users)
                    {
                        Console.WriteLine($"▶ 아이디 : {user.UserId} / 이름 : {user.UserName}");
                        Console.WriteLine($"   계정 활성화 상태 : {(user.IsActive ? "정상 사용중" : "블랙리스트/만료")}");
                        Console.WriteLine($"   이메일 연동 주소 : {user.Contact?.Email}");
                        Console.WriteLine($"   내선 연락처 정보 : {user.Contact?.Phone}");
                        Console.WriteLine($"   부여된 보안 롤   : {string.Join(", ", user.Roles)}");
                        Console.WriteLine("--------------------------------------------------");
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                // 문자열 깨짐, 오탈자, 괄호 쌍 미스매치 시 예외 핸들링
                Console.WriteLine($"[JSON 규격 문법 오류] : {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[런타임 시스템 오류] : {ex.Message}");
            }

            Console.WriteLine("==================================================");
        }
    }
}
