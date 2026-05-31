using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest2
{
    namespace JsonParsingTest
    {
        // 1. 가장 하위 단계의 연락처 오브젝트 매핑 클래스
        public class ContactInfo
        {
            [JsonProperty("Email")]
            public string Email { get; set; }

            [JsonProperty("Phone")]
            public string Phone { get; set; }
        }

        // 2. 개별 사용자 엔티티를 담는 클래스
        public class UserItem
        {
            [JsonProperty("UserId")]
            public string UserId { get; set; }

            [JsonProperty("UserName")]
            public string UserName { get; set; }

            [JsonProperty("IsActive")]
            public bool IsActive { get; set; }

            // 내부에 정의한 또 다른 객체 클래스 배치
            [JsonProperty("Contact")]
            public ContactInfo Contact { get; set; }

            // 문자열 배열 구조 매핑
            [JsonProperty("Roles")]
            public List<string> Roles { get; set; }
        }

        // 3. 최상위 루트 데이터를 담는 컨테이너 응답 클래스
        public class ApiResponse
        {
            [JsonProperty("Status")]
            public string Status { get; set; }

            [JsonProperty("ResponseTimeMs")]
            public int ResponseTimeMs { get; set; }

            [JsonProperty("ResultCount")]
            public int ResultCount { get; set; }

            // 사용자 데이터가 배열 형태이므로 List<T> 구조로 선언
            [JsonProperty("Users")]
            public List<UserItem> Users { get; set; }
        }
    }
}
