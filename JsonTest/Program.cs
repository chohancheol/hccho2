using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 원본 데이터 (문법 교정을 거쳐 올바른 배열 구조 [ ] 로 감싸줍니다)
            string rawJson = "{ \"AAA\", \"BBB\", {\"id\":\"cccc\", \"name\":\"dddd\"}}";
            string validJson = $"[{rawJson.Trim().TrimStart('{').TrimEnd('}')}]";

            Console.WriteLine("=== 클래스 변환 및 파싱 시작 ===");

            try
            {
                // 1. 우선 JSON 배열(JArray)로 변환합니다.
                JArray jsonArray = JArray.Parse(validJson);

                // 2. 우리가 정의한 클래스 객체를 생성합니다.
                CustomDataContainer container = new CustomDataContainer();

                // 3. 배열을 순회하며 자형(Type)에 따라 클래스 속성에 알맞게 채워넣습니다.
                foreach (JToken item in jsonArray)
                {
                    if (item.Type == JTokenType.String)
                    {
                        // 문자열인 경우 TextItems 리스트에 추가
                        container.TextItems.Add(item.ToString());
                    }
                    else if (item.Type == JTokenType.Object)
                    {
                        // 객체 형태인 경우, 정의해둔 UserInfo 클래스로 즉시 역직렬화(ToObject)하여 추가
                        UserInfo userInfo = item.ToObject<UserInfo>();
                        if (userInfo != null)
                        {
                            container.UserItems.Add(userInfo);
                        }
                    }
                }

                // 4. 클래스로 완벽히 변환된 데이터 출력 확인
                Console.WriteLine("\n[1] TextItems 결과 (문자열 리스트):");
                foreach (var text in container.TextItems)
                {
                    Console.WriteLine($" -> {text}");
                }

                Console.WriteLine("\n[2] UserItems 결과 (UserInfo 클래스 리스트):");
                foreach (var user in container.UserItems)
                {
                    Console.WriteLine($" -> ID: {user.Id}, NAME: {user.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"변환 중 오류 발생: {ex.Message}");
            }

            Console.WriteLine("\n===============================");

            string rawJson2 = "[\"AAA\", \"BBB\", {\"id\":\"cccc\", \"name\":\"dddd\"}]";

            JArray jsonArray2 = JArray.Parse(rawJson2);
            string id1= jsonArray2[0].ToString();
            string id2= jsonArray2[1].ToString();

            UserInfo userinfo = jsonArray2[2].ToObject<UserInfo>();

            Console.WriteLine($"id1: {id1}");
            Console.ReadLine();
        }


        // 1. 내부의 ID/Name 전용 객체 클래스
        public class UserInfo
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        // 2. 여러 타입이 섞인 배열을 안전하게 파싱하기 위한 전체 컨테이너 클래스
        public class CustomDataContainer
        {
            // 순수 문자열 데이터만 모아둘 리스트
            public List<string> TextItems { get; set; } = new List<string>();

            // 내부 객체(UserInfo) 데이터만 모아둘 리스트
            public List<UserInfo> UserItems { get; set; } = new List<UserInfo>();
        }
    }
}
