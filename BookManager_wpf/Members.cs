using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager_wpf
{
    public class Members
    {
        public int MemberId { get; set; } // 회원 ID
        public string Name { get; set; } // 이름
        public string MobileNumber { get; set; } // 핸드폰 번호
        public DateTime CreatedDate { get; set; }  // 생성 날짜  

        // ToString 메서드 오버라이딩. 각 속성을 문자열로 변환하여 출력.
        public override string ToString()
        {
            return $"ID: {MemberId}, Name: {Name}, Mobile Number: {MobileNumber}, Created Date: {CreatedDate}";
        }
    }
}
